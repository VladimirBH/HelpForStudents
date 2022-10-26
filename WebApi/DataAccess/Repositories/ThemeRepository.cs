using WebApi.DataAccess.Contracts;
using WebApi.DataAccess.Database;

namespace WebApi.DataAccess.Repositories
{
    public class ThemeRepository : GenericRepository<Theme>, IThemeRepository
    {
        public ThemeRepository(HelpForStudentsContext context, IConfiguration configuration)
            : base(context, configuration)
        {
            
        }
    }
}