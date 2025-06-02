namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Provides the data points necessary to calculate the amount for a particular
    /// UniversalLife base (BF) policy benefit.
    /// </summary>
    [Table("PPBEN_POLICY_BENEFITS_TYPES_SL")]
    public partial class PPBEN_POLICY_BENEFITS_TYPES_SL
    {
        [Key]
        [Required]
        public long PBEN_ID { get; set; }

        public decimal NUMBER_OF_UNITS { get; set; }

        public decimal VALUE_PER_UNIT { get; set; }

        public decimal ANN_PREM_PER_UNIT { get; set; }
    }
}