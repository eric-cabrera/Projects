namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("PCEXP_COVERAGE_EXPANSION_UWCLS_DETAILS")]
    public class PCEXP_COVERAGE_EXPANSION_UWCLS_DETAILS
    {
        [Required]
        public long CEXP_ID { get; set; }

        [Required]
        public short IDX { get; set; }

        [Required]
        [StringLength(25)]
        public string UWCLS_DESC { get; set; }

        [Required]
        [StringLength(1)]
        [Unicode(false)]
        public string UWCLS_CODE { get; set; }
    }
}
