using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.DataAccess.Database
{
    [Table("formulas")]
    public class Formula : Entity
    {
        [Column("formula_text")]
        public string FormulaText {get; set;}
    }
}