namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// This is the entity for the first of 3 queries to get all of the needed benefit information with the repsective extended keys
    /// </summary>
    [Table("AgentQueryEntity")]
    public class AgentQueryEntity
    {
        [Required]
        [StringLength(12)]
        [Key]
        public string PolicyNumber { get; set; }

        public string IsServicingAgent { get; set; }

        public string AgentId { get; set; }

        public string MarketCode { get; set; }

        public string Level { get; set; }
    }
}