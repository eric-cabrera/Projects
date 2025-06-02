namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Provides the data points necessary to calculate the amount for a particular
    /// non-UniversalLife base (BA) or Other Rider (OR) policy benefit.
    /// </summary>
    [Table("PPBEN_POLICY_BENEFITS_TYPES_BA_OR")]
    public partial class PPBEN_POLICY_BENEFITS_TYPES_BA_OR
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
        public string DIVIDEND { get; set; }
    }
}