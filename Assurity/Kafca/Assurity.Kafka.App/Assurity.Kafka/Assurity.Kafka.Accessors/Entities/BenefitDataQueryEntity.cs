namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// This is the entity for the first of 3 queries to get all of the needed benefit information with the repsective extended keys
    /// </summary>
    [Table("BenefitDataQueryEntity")]
    public class BenefitDataQueryEntity
    {
        [Required]
        [StringLength(12)]
        [Key]
        public string PolicyNumber { get; set; }

        public string CoverageType { get; set; }

        public long BenefitId { get; set; }

        public string PlanCode { get; set; }

        public string BenefitCategory { get; set; }

        public string BenefitDescription { get; set; }

        public string BenefitStatus { get; set; }

        public string BenefitStatusReason { get; set; }

        public decimal BenefitAmount { get; set; }
    }
}