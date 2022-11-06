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
            var tokenService = new TokenService(Configuration);
            var tokenPair = new TokenPair
            {
                AccessToken = tokenService.BuildAccessToken(Configuration["JWT:Key"],
                    Configuration["JWT:Issuer"], user),
                RefreshToken = tokenService.BuildRefreshToken(Configuration["JWT:Key"],
                    Configuration["JWT:Issuer"], user),
                ExpiredInAccessToken = int.Parse(Configuration["JWT:AccessTokenLifeTime"]),
                ExpiredInRefreshToken = int.Parse(Configuration["JWT:RefreshTokenLifeTime"]),
                CreationDateTime = DateTime.Now
            };
            return tokenPair;
        }

        public int GetUserIdFromRefreshToken(string refreshToken)
        {
            var tokenService = new TokenService(Configuration);
            if (!tokenService.IsTokenValid(Configuration["JWT:Key"], Configuration["JWT:Issuer"], refreshToken))
                throw new AuthenticationException();
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(refreshToken);
            var id = jsonToken?.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
            if (id == null)
            {
                throw new AuthenticationException();
            }

            return int.Parse(id);
        }
        
        public int GetUserIdFromAccessToken(string accessToken)
        {
            var tokenService = new TokenService(Configuration);
            if (!tokenService.IsTokenValid(Configuration["JWT:Key"], Configuration["JWT:Issuer"], accessToken))
                throw new AuthenticationException();
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(accessToken);
            var email = jsonToken?.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
            if (email == null)
            {
                throw new AuthenticationException();
            }

            return GetByEmail(email).Id;
        }

        public TokenPair RefreshPairTokens(string refreshToken)
        {
            var tokenService = new TokenService(Configuration);
            var user = GetById(GetUserIdFromRefreshToken(refreshToken));
            user = GetByEmail(user.Email);
            var tokenPair = new TokenPair
            {
                AccessToken = tokenService.BuildAccessToken(Configuration["JWT:Key"], Configuration["JWT:Issuer"], user),
                RefreshToken = tokenService.BuildRefreshToken(Configuration["JWT:Key"], Configuration["JWT:Issuer"], user),
                ExpiredInAccessToken = int.Parse(Configuration["JWT:AccessTokenLifeTime"]),
                ExpiredInRefreshToken = int.Parse(Configuration["JWT:RefreshTokenLifeTime"]),
                CreationDateTime = DateTime.Now
            };
            return tokenPair;
        }

        private User GetByEmail(string email)
        {
            return Context.Users.FirstOrDefault(u => (u.Email == email));
        }

        public User GetCurrentUserInfo(string accessToken)
        {
            return Context.Users.FirstOrDefault(u => u.Id == GetUserIdFromAccessToken(accessToken));
        }
    }
}