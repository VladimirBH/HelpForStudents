using WebApi.DataAccess.Contracts;
using WebApi.DataAccess.Database;
using WebApi.DataAccess.Database;
using WebApi.DataAccess.Contracts;
using WebApi.Classes;
using WebApi.Services;

namespace WebApi.DataAccess.Repositories
{
    public class RefreshSessionRepository: GenericRepository<RefreshSession>, IRefreshSessionRepository
    {
        public RefreshSessionRepository(HelpForStudentsContext context, IConfiguration configuration)
            : base(context, configuration)
        {

        }  
        public RefreshSession GetByRefreshToken(string refreshToken)
        {
            return Context.RefreshSessions.FirstOrDefault(rs => rs.RefreshToken == Guid.Parse(refreshToken));
        }
    }
}