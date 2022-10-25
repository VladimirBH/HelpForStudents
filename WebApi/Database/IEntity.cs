using NodaTime;

namespace WebApi.Database
{
    public interface IEntity
    {
        int Id { get; set; }
        DateTimeOffset CreationDate { get; set; }
        DateTimeOffset? UpdatedDate { get; set; }
    }
}