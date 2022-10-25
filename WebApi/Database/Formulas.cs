using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebApi.Database
{
    public class Formulas : Entity
    {
        [Column("formula_text")]
        public string FormulaText {get; set;}
    }
}