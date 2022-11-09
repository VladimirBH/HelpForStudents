using WebApi.DataAccess.Database;

namespace WebApi.DataAccess.Contracts
{
    public interface ISubjectRepository : IGenericRepository<Subject>
    {
        void BuySubject(int id);
    }
}