using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using WebApi.DataAccess.Contracts;

namespace WebApi.DataAccess.Database
{
    public class Entity : IEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [DataType(DataType.DateTime)]
        [Column("creation_date")]
        public DateTimeOffset CreationDate { get; set; }

        [DataType(DataType.DateTime)]
        [Column("updated_date")]
        public DateTimeOffset? UpdatedDate { get; set; }
    }
}