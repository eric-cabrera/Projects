namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Contains records of the agent.
    /// </summary>
    [Table("PAGNT_AGENT_MASTER")]
    public class PAGNT_AGENT_MASTER
    {
        [Key]
        [Required]
        public long AGNT_ID { get; set; }

        /// <summary>
        /// Code representing a New York policy (02) or other states (01).
        /// </summary>
        [Required]
        [Unicode(false)]
        [StringLength(2)]
        public string COMPANY_CODE { get; set; }

        /// <summary>
        /// Id to tie record to names (PNAME) table.
        /// </summary>
        [Required]
        public int NAME_ID { get; set; }

        [Required]
        [StringLength(12)]
        public string AGENT_NUMBER { get; set; }

        public virtual PNAME? PNAME { get; set; }
    }
}