namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("PMUIN_MULTIPLE_INSUREDS")]
    public class PMUIN_MULTIPLE_INSUREDS
    {
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

        public int NAME_ID { get; set; }

        [StringLength(2)]
        [Unicode(false)]
        public string RELATIONSHIP_CODE { get; set; }

        [StringLength(11)]
        public string MULT_RELATE { get; set; }

        public short BENEFIT_SEQ { get; set; }

        [StringLength(10)]
        public string KD_DEF_SEGT_ID { get; set; }

        [StringLength(50)]
        public string KD_BEN_EXTEND_KEYS { get; set; }

        [StringLength(1)]
        [Unicode(false)]
        public string UWCLS { get; set; }

        public int START_DATE { get; set; }

        public int STOP_DATE { get; set; }
    }
}
