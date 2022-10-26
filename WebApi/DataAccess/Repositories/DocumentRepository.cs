using WebApi.DataAccess.Contracts;
using WebApi.DataAccess.Database;

namespace WebApi.DataAccess.Repositories
{
    public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
    {
        public DocumentRepository(HelpForStudentsContext context, IConfiguration configuration)
            : base(context, configuration)
        {
            
        }
    }
}