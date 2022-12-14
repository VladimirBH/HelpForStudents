using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.DataAccess.Database
{
    [Table("themes")]
    public class Theme : Entity
    {
        [Column("name")]
        public string Name {get; set;}

        [Column("type_theme")]
        public ThemeTypeEnum TypeTheme {get; set;}
    }

    public enum ThemeTypeEnum
    {
        diploma,
        course
    }
}