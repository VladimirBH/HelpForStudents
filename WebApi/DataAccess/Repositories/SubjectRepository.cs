using WebApi.DataAccess.Contracts;
using WebApi.DataAccess.Database;

namespace WebApi.DataAccess.Repositories
{
    public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
    {
        public SubjectRepository(HelpForStudentsContext context, IConfiguration configuration)
            : base(context, configuration)
        {
            
        }
    }
}