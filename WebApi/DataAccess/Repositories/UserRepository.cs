using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using WebApi.DataAccess.Database;
using WebApi.DataAccess.Contracts;
using WebApi;
using WebApi.Classes;
using WebApi.Services;
namespace WebApi.DataAccess.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(HelpForStudentsContext context, IConfiguration configuration) : base(context, configuration)
        {
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

        public async Task<bool> SubmitEmail(string email)
        {
            await EmailService.SendEmailAsync(
                emailFrom: "vovapresent@yandex.ru", 
                password: "07112002vladburB", 
                emailTo: email, 
                subject: "Тест", 
                message: "Чекай почту");
            return true;
        }

        private User GetByEmail(string email)
        {
            return Context.Users.FirstOrDefault(u => (u.Email == email));
        }


    }
}