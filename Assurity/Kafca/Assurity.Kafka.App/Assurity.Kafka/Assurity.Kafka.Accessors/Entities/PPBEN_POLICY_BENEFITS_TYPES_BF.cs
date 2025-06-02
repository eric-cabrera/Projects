namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Provides the data points necessary to calculate the amount for a particular
    /// UniversalLife base (BF) policy benefit.
    /// </summary>
    [Table("PPBEN_POLICY_BENEFITS_TYPES_BF")]
    public partial class PPBEN_POLICY_BENEFITS_TYPES_BF
    {
        [Key]
        [Required]
        public long PBEN_ID { get; set; }

        [Precision(18, 5)]
        public decimal NUMBER_OF_UNITS { get; set; }

        [Precision(18, 2)]
        public decimal VALUE_PER_UNIT { get; set; }

        [Precision(18, 5)]
        public decimal ANN_PREM_PER_UNIT { get; set; }

        [StringLength(1)]
        [Unicode(false)]
        public string BF_DB_OPTION { get; set; }

        public int BF_DATE_NEGATIVE { get; set; }

        [Precision(18, 2)]
        public decimal BF_CURRENT_DB { get; set; }
    }
}