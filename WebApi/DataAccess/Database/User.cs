using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.DataAccess.Database
{
    [Table("users")]
    public class User : Entity
    {
        [Column("name")]
        public string Name {get; set;}

        [Column("surname")]
        public string Surname {get; set;}

        [Column("email")]
        public string Email {get; set;}

        [Column("password")]
        public string Password {get; set;}
    }
}