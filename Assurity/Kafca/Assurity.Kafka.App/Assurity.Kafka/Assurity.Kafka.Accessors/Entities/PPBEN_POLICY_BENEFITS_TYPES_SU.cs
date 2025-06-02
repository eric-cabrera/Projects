namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Provides the data points necessary to calculate the amount for a particular
    /// UniversalLife base (BF) policy benefit.
    /// </summary>
    [Table("PPBEN_POLICY_BENEFITS_TYPES_SU")]
    public partial class PPBEN_POLICY_BENEFITS_TYPES_SU
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
    }
}