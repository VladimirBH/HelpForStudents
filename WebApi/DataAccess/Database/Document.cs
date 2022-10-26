using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.DataAccess.Database
{
    [Table("documents")]
    public class Document : Entity
    {
        [Column("path")]
        public string Path {get; set;}

        [Column("price")]
        public decimal Price {get; set;}
    }
}