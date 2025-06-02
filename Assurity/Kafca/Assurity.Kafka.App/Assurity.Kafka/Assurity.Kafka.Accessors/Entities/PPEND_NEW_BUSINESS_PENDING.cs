namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("PPEND_NEW_BUSINESS_PENDING")]
    public class PPEND_NEW_BUSINESS_PENDING
    {
        [Key]
        [Required]
        public long PEND_ID { get; set; }

        /// <summary>
        /// Code representing a New York policy (02) or other states (01).
        /// </summary>
        [Required]
        [StringLength(2)]
        [Unicode(false)]
        public string COMPANY_CODE { get; set; }

        [Required]
        [StringLength(12)]
        public string POLICY_NUMBER { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal FACE_AMOUNT { get; set; }

        public int REQUIREMENT_DATE { get; set; }

        public int LAST_CHANGE_DATE { get; set; }

        public int UND_NAME_ID { get; set; }

        [Required]
        [StringLength(2)]
        [Unicode(false)]
        public string REDEF_TYPE { get; set; }

        public short REQ_SEQUENCE { get; set; }
    }
}