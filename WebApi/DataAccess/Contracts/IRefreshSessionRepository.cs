using WebApi.DataAccess.Database;

namespace WebApi.DataAccess.Contracts
{
    public interface IRefreshSessionRepository: IGenericRepository<RefreshSession>
    {
        RefreshSession GetByRefreshToken(string refreshToken);
    }
}