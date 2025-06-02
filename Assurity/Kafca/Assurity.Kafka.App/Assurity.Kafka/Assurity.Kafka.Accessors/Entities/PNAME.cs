namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Contains the name record.
    /// </summary>
    [Table("PNAME")]
    public class PNAME
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NAME_ID { get; set; }

        [StringLength(1)]
        [Unicode(false)]
        public string NAME_FORMAT_CODE { get; set; }

        [StringLength(30)]
        public string INDIVIDUAL_PREFIX { get; set; }

        [StringLength(40)]
        public string INDIVIDUAL_LAST { get; set; }

        [StringLength(20)]
        public string INDIVIDUAL_FIRST { get; set; }

        [StringLength(10)]
        public string INDIVIDUAL_MIDDLE { get; set; }

        [StringLength(6)]
        public string INDIVIDUAL_SUFFIX { get; set; }

        [StringLength(70)]
        public string NAME_BUSINESS { get; set; }

        [StringLength(256)]
        public string BUSINESS_EMAIL_ADR { get; set; }

        [StringLength(256)]
        public string PERSONAL_EMAIL_ADR { get; set; }

        public int DATE_OF_BIRTH { get; set; }

        [StringLength(1)]
        [Unicode(false)]
        public string SEX_CODE { get; set; }

        public virtual ICollection<PNALK>? PNALKs { get; set; }
    }
}