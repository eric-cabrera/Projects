namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Contains the Product Coverages record.
    /// </summary>
    [Table("PCOVR_PRODUCT_COVERAGES")]
    public class PCOVR_PRODUCT_COVERAGES
    {
        [Key]
        [Required]
        public long COVR_ID { get; set; }

        [Required]
        [StringLength(10)]
        public string COVERAGE_ID { get; set; }

        [Required]
        [StringLength(40)]
        public string DESCRIPTION { get; set; }

        [Required]
        [StringLength(2)]
        [Unicode(false)]
        public string COMPANY_CODE { get; set; }
    }
}
