using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;
using WebApi.DataAccess.Database;
using WebApi.DataAccess.Contracts;
using WebApi.Classes;
using WebApi.Services;
namespace WebApi.DataAccess.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly ICacheService _iCacheService;
        private readonly ICiaccoRandom _iRandomer;
        private readonly IRefreshSessionRepository _iRefreshSessionRepository;
        public UserRepository(
            HelpForStudentsContext context, 
            IConfiguration configuration, 
            IRefreshSessionRepository refreshSessionRepository, 
            ICacheService cacheService, ICiaccoRandom randomer) : base(context, configuration)
        {
            _iCacheService = cacheService;
            _iRandomer = randomer;
            _iRefreshSessionRepository = refreshSessionRepository;
        }

        public UserToken Authorization(AuthorizationData dataAuth)
        {
            var user = GetByEmail(dataAuth.Email);
            if (user == null) throw new AuthenticationException();
            if (!BCrypt.Net.BCrypt.Verify(dataAuth.Password, user.Password)) throw new AuthenticationException();
            Guid refreshToken = TokenService.BuildRefreshToken();
            _iRefreshSessionRepository.Add(new RefreshSession
            {
                UserId = user.Id,
                RefreshToken = refreshToken,
                ExpiresIn = Int64.Parse(Configuration["JWT:RefreshTokenLifeTime"])
            });
            _iRefreshSessionRepository.SaveChanges();
            var userToken = new UserToken
            {
                AccessToken = TokenService.BuildAccessToken(
                    user: user, 
                    key: Configuration["JWT:Key"],
                    issuer: Configuration["JWT:Issuer"],
                    audience: Configuration["JWT:Audience"],
                    accessTokenLifeTime: int.Parse(Configuration["JWT:AccessTokenLifeTime"])),
                RefreshToken = refreshToken.ToString(),
                ExpiredInAccessToken = int.Parse(Configuration["JWT:AccessTokenLifeTime"]),
                ExpiredInRefreshToken = int.Parse(Configuration["JWT:RefreshTokenLifeTime"]),
                CreationDateTime = DateTime.Now,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed
            };
            return userToken;
        }

        public int GetUserIdFromRefreshToken(string refreshToken)
        {
            if (!TokenService.IsTokenValid(
                token: refreshToken,
                key: Configuration["JWT:Key"],
                issuer: Configuration["JWT:Issuer"],
                audience: Configuration["JWT:Audience"]))
                throw new AuthenticationException();
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(refreshToken);
            var id = jsonToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
            if (id == null)
            {
                throw new AuthenticationException();
            }

            return int.Parse(id);
        }
        
        public int GetUserIdFromAccessToken(string accessToken)
        {
            if (!TokenService.IsTokenValid(
                token: accessToken,
                key: Configuration["JWT:Key"],
                issuer: Configuration["JWT:Issuer"],
                audience: Configuration["JWT:Audience"]))
                throw new AuthenticationException();
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(accessToken);
            var email = jsonToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
            if (email == null)
            {
                throw new AuthenticationException();
            }

            return GetByEmail(email).Id;
        }

        public TokenPair RefreshPairTokens(string refreshToken)
        {
            RefreshSession refreshSession = _iRefreshSessionRepository.GetByRefreshToken(refreshToken);
            if(refreshSession == null) 
            {
                throw new AuthenticationException();
            }
            _iRefreshSessionRepository.Remove(refreshSession);
            var user = GetById(refreshSession.UserId);
            Guid newRefreshToken = TokenService.BuildRefreshToken();
            _iRefreshSessionRepository.Add(new RefreshSession
            {
                UserId = user.Id,
                RefreshToken = newRefreshToken,
                ExpiresIn = Int64.Parse(Configuration["JWT:RefreshTokenLifeTime"])
            });  
            var tokenPair = new TokenPair
            {
                AccessToken = TokenService.BuildAccessToken(
                    user: user, 
                    key: Configuration["JWT:Key"],
                    issuer: Configuration["JWT:Issuer"],
                    audience: Configuration["JWT:Audience"],
                    accessTokenLifeTime: int.Parse(Configuration["JWT:AccessTokenLifeTime"])),
                RefreshToken = refreshToken.ToString(),
                ExpiredInAccessToken = int.Parse(Configuration["JWT:AccessTokenLifeTime"]),
                ExpiredInRefreshToken = int.Parse(Configuration["JWT:RefreshTokenLifeTime"]),
                CreationDateTime = DateTime.Now
            };
            return tokenPair;
        }

        public User GetCurrentUserInfo(string accessToken)
        {
            return Context.Users.FirstOrDefault(u => u.Id == GetUserIdFromAccessToken(accessToken));
        }

        public async Task<string> SubmitEmailAsync(string email, string message)
        {
         User user = GetByEmail(email);
            if(user == null)
            {
                return "User doesn't exist";
            }
            if(user.EmailConfirmed)
            {
                return "Email has already confirmed";
            }
            Random rand = new Random();
            int seed = rand.Next(0, 999999);
            int min = 0;
            int max = 9999999;

            _iRandomer.SetSeed(seed);
            int confirmCode = _iRandomer.GetRand(min, max);
            string strCode = confirmCode.ToString().PadLeft(max.ToString().Length - confirmCode.ToString().Length, '0');

            _iCacheService.SetCodeForConfirmationEmail(key: email, code: strCode);
            await EmailService.SendEmailAsync(
                emailFrom: Configuration["EmailData:Email"], 
                password: Configuration["EmailData:Password"], 
                emailTo: email, 
                subject: "Подтверждение адреса электронной почты", 
                message: message + $"<br><h1> {strCode} </h1>");

            return "Success";
        }

        public bool CheckCodeFromEmail(string email, string code)
        {
            if(_iCacheService.GetCodeForConfirmationEmail(email) == code)
            {
                 _iCacheService.DeleteFromCache(key: email);
                 return true;
            }
            return false; 
        }

        public User GetByEmail(string email)
        {
            return Context.Users.FirstOrDefault(u => (u.Email == email));
        }

        public void CheckForConfirmationCode(string email)
        {
            if(_iCacheService.GetCodeForConfirmationEmail(email) != string.Empty)
            {
                _iCacheService.DeleteFromCache(email);
            }
        }

        public bool CheckEmailForDuplication(string email)
        {
            return GetByEmail(email) != null;
        }
    }
}