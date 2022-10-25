namespace WebApi.Database
{
    public class Entity : IEntity
    {
        public int Id { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
    }
}