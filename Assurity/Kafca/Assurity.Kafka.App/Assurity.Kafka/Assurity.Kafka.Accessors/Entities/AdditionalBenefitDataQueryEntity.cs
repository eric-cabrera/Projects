namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// This is the entity for the first of 3 queries to get all of the needed benefit information with the repsective extended keys
    /// </summary>
    [Table("AdditionalBenefitDataQueryEntity")]
    public class AdditionalBenefitDataQueryEntity
    {
        [Required]
        [StringLength(12)]
        public string PolicyNumber { get; set; }

        public long BenefitId { get; set; }

        [Key]
        public int NameId { get; set; }

        public string InsuredRelationhipToPrimary { get; set; }

        public string KD_DEF_SEGT_ID { get; set; }

        public string KD_BEN_EXTEND_KEYS { get; set; }
    }
}