using WebApi.DataAccess.Database;
using WebApi.DataAccess.Contracts;

namespace WebApi.DataAccess.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(HelpForStudentsContext context, IConfiguration configuration) : base(context, configuration)
        {
        }
    }
}