using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using WebApi.Database;
namespace WebApi
{
    public class Themes : Entity
    {
        [Column("name")]
        public string Name {get; set;}
    }
}