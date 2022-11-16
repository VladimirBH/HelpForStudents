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
        public UserRepository(HelpForStudentsContext context, IConfiguration configuration, ICacheService cacheService, ICiaccoRandom randomer) : base(context, configuration)
        {
            _iCacheService = cacheService;
            _iRandomer = randomer;
        }

        public TokenPair Authorization(AuthorizationData dataAuth)
        {
            var user = GetByEmail(dataAuth.Email);
            if (user == null) throw new AuthenticationException();
            if (!BCrypt.Net.BCrypt.Verify(dataAuth.Password, user.Password)) throw new AuthenticationException();
            var tokenPair = new TokenPair
            {
                AccessToken = TokenService.BuildAccessToken(
                    user: user, 
                    key: Configuration["JWT:Key"],
                    issuer: Configuration["JWT:Issuer"],
                    audience: Configuration["JWT:Audience"],
                    accessTokenLifeTime: int.Parse(Configuration["JWT:AccessTokenLifeTime"])),
                RefreshToken = TokenService.BuildRefreshToken(
                    user: user, 
                    key: Configuration["JWT:Key"],
                    issuer: Configuration["JWT:Issuer"],
                    audience: Configuration["JWT:Audience"],
                    refreshTokenLifeTime: int.Parse(Configuration["JWT:RefreshTokenLifeTime"])),
                ExpiredInAccessToken = int.Parse(Configuration["JWT:AccessTokenLifeTime"]),
                ExpiredInRefreshToken = int.Parse(Configuration["JWT:RefreshTokenLifeTime"]),
                CreationDateTime = DateTime.Now
            };
            return tokenPair;
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
            var user = GetById(GetUserIdFromRefreshToken(refreshToken));
            user = GetByEmail(user.Email);
            var tokenPair = new TokenPair
            {
                AccessToken = TokenService.BuildAccessToken(
                    user: user, 
                    key: Configuration["JWT:Key"],
                    issuer: Configuration["JWT:Issuer"],
                    audience: Configuration["JWT:Audience"],
                    accessTokenLifeTime: int.Parse(Configuration["JWT:AccessTokenLifeTime"])),
                RefreshToken = TokenService.BuildRefreshToken(
                    user: user, 
                    key: Configuration["JWT:Key"],
                    issuer: Configuration["JWT:Issuer"],
                    audience: Configuration["JWT:Audience"],
                    refreshTokenLifeTime: int.Parse(Configuration["JWT:RefreshTokenLifeTime"])),
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

        public async Task SubmitEmail(string email)
        {
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
                message: "<h3> Благодарим за регистрацию на нашем сайте. Для окончания регистрации необходимо подтвердить почту.<br> " +
                        "Введите код подтверждения, указанный ниже.</h3><br> <h1>" + strCode + "</h1>");
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
    }
}