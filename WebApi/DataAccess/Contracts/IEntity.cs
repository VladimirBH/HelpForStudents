using NodaTime;

namespace WebApi.DataAccess.Contracts
{
    public interface IEntity
    {
        int Id { get; set; }
        DateTimeOffset CreationDate { get; set; }
        DateTimeOffset? UpdateDate { get; set; }
    }
}