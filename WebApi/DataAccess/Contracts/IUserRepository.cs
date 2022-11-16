using WebApi.DataAccess.Database;
using WebApi.Classes;
namespace WebApi.DataAccess.Contracts
{
    public interface IUserRepository : IGenericRepository<User>
    {
        TokenPair Authorization(AuthorizationData dataAuth);
        TokenPair RefreshPairTokens(string refreshToken);
        User GetCurrentUserInfo(string refreshToken);
        int GetUserIdFromRefreshToken(string refreshToken);
        Task SubmitEmail(string email);
        bool CheckCodeFromEmail(string email, string code);
        User GetByEmail(string email);
        void CheckForConfirmationCode(string email);
    }
}