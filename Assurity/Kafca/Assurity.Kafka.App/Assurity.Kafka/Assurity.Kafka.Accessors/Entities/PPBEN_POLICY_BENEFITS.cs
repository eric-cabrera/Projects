namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Representation of a benefit for a given policy.
    /// </summary>
    [Table("PPBEN_POLICY_BENEFITS")]
    public class PPBEN_POLICY_BENEFITS
    {
        [Key]
        public long PBEN_ID { get; set; }

        /// <summary>
        /// Policy number tied to the record.
        /// </summary>
        [Required]
        [StringLength(12)]
        public string POLICY_NUMBER { get; set; }

        /// <summary>
        /// Code representing a New York policy (02) or other states (01).
        /// </summary>
        [Required]
        [StringLength(2)]
        [Unicode(false)]
        public string COMPANY_CODE { get; set; }

        /// <summary>
        /// Sequence number to control the order of the benefits in LifePro.
        /// Among other things, this comes into play when one benefit is dependent on another benefit.
        /// The base benefit is always 1.
        /// </summary>
        public short BENEFIT_SEQ { get; set; }

        [Required]
        [StringLength(2)]
        [Unicode(false)]
        public string BENEFIT_TYPE { get; set; }

        [Required]
        [StringLength(1)]
        [Unicode(false)]
        public string STATUS_CODE { get; set; }

        [Required]
        [StringLength(2)]
        [Unicode(false)]
        public string STATUS_REASON { get; set; }

        [Required]
        [StringLength(10)]
        public string PLAN_CODE { get; set; }

        [Required]
        public int STATUS_DATE { get; set; }

        public virtual PCOVR_PRODUCT_COVERAGES PCOVR_PRODUCT_COVERAGES { get; set; }

        public virtual PPBEN_POLICY_BENEFITS_TYPES_BA_OR? PPBEN_POLICY_BENEFITS_TYPES_BA_OR { get; set; }

        public virtual PPBEN_POLICY_BENEFITS_TYPES_BF? PPBEN_POLICY_BENEFITS_TYPES_BF { get; set; }

        public virtual PPBEN_POLICY_BENEFITS_TYPES_SU? PPBEN_POLICY_BENEFITS_TYPES_SU { get; set; }

        public virtual PPBEN_POLICY_BENEFITS_TYPES_SL? PPBEN_POLICY_BENEFITS_TYPES_SL { get; set; }

        public virtual PPBEN_POLICY_BENEFITS_TYPES_SP? PPBEN_POLICY_BENEFITS_TYPES_SP { get; set; }

        public virtual ICollection<PMUIN_MULTIPLE_INSUREDS> PMUIN_MULTIPLE_INSUREDs { get; set; }

        public virtual PCEXP_COVERAGE_EXPANSION PCEXP_COVERAGE_EXPANSION { get; set; }
    }
}