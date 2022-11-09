using WebApi.DataAccess.Contracts;
using WebApi.DataAccess.Database;

namespace WebApi.DataAccess.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(HelpForStudentsContext context, IConfiguration configuration)
            : base(context, configuration)
        {

        }


    }
}