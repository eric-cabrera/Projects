namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Contains records of the relationships a policy has: insured(s), agent, owner, group, etc.
    /// </summary>
    [Table("PRELA_RELATIONSHIP_MASTER")]
    public class PRELA_RELATIONSHIP_MASTER
    {
        /// <summary>
        /// Id to tie record to names (PNAME) table.
        /// </summary>
        public int NAME_ID { get; set; }

        [Required]
        [StringLength(2)]
        public string RELATE_CODE { get; set; }

        [Required]
        [StringLength(20)]
        public string IDENTIFYING_ALPHA { get; set; }

        public short BENEFIT_SEQ_NUMBER { get; set; }

        public virtual PNAME? PNAME { get; set; }
    }
}