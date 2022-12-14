using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.DataAccess.Database
{
    [Table("refresh_sessions")]
    public class RefreshSession
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {get;set;}

        [Column("user_id")]
        public int UserId {get; set;}

        [Column("refresh_token")]
        public Guid RefreshToken {get; set;}

        [Column("expires_in")]
        public Int64 ExpiresIn {get; set;}

        [DataType(DataType.DateTime)]
        [Column("creation_date")]
        public DateTimeOffset CreationDate { get; set; }
    }
}