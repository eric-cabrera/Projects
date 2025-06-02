namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("PPOLM_POLICY_BENEFIT_MISC")]
    public class PPOLM_POLICY_BENEFIT_MISC
    {
        [Key]
        public long POLM_ID { get; set; }

        /// <summary>
        /// Code representing a New York policy (02) or other states (01).
        /// </summary>
        [Required]
        [StringLength(2)]
        [Unicode(false)]
        public string COMPANY_CODE { get; set; }

        /// <summary>
        /// Policy number tied to the record.
        /// </summary>
        [Required]
        [StringLength(12)]
        public string POLICY_NUMBER { get; set; }

        [Required]
        [StringLength(2)]
        [Unicode(false)]
        public string CANCEL_REASON { get; set; }

        [Required]
        [StringLength(60)]
        public string CANCEL_DESC { get; set; }

        [Required]
        public short SEQ { get; set; }
    }
}
