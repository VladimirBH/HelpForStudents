using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Database
{
    public class Documents : Entity
    {
        [Column("path")]
        public string Path {get; set;}

        [Column("price")]
        public decimal Price {get; set;}
    }
}