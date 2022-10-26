using WebApi.DataAccess.Contracts;
using WebApi.DataAccess.Database;

namespace WebApi.DataAccess.Repositories
{
    public class FormulaRepository : GenericRepository<Formula>, IFormulaRepository
    {
        public FormulaRepository(HelpForStudentsContext context, IConfiguration configuration)
            : base(context, configuration)
        {

        }
    }
}