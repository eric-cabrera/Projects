namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PCEXP_COVERAGE_EXPANSION")]
    public class PCEXP_COVERAGE_EXPANSION
    {
        [Key]
        [Required]
        public long CEXP_ID { get; set; }

        [Required]
        [StringLength(10)]
        public string COVERAGE_ID { get; set; }

        public virtual ICollection<PCEXP_COVERAGE_EXPANSION_UWCLS_DETAILS> PCEXP_COVERAGE_EXPANSION_UWCLS_DETAILS { get; set; }
    }
}
