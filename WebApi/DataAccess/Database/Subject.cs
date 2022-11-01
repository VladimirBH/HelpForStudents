using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.DataAccess.Database
{
    [Table("subjects")]
    public class Subject : Entity
    {
        [Column("theme_id")]
        public int ThemeId {get; set;}

        [Column("subject_type")]
        public PaymentSubjectsEnum SubjectType {get; set;}

        [Column("formula_text")]
        public string FormulaText {get; set;}

        [Column("document_path")]
        public string DocumentPath {get; set;}

        [Column("price")]
        public decimal Price {get; set;}
    }

    public enum PaymentSubjectsEnum
    {
        document,
        subject
    }
}

