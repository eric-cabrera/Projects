namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("PCOMC_COMMISSION_CONTROL")]
    public class PCOMC_COMMISSION_CONTROL
    {
        [Key]
        [Required]
        public long COMC_ID { get; set; }

        [Required]
        [StringLength(12)]
        public string POLICY_NUMBER { get; set; }

        [Required]
        [Unicode(false)]
        [StringLength(2)]
        public string COMPANY_CODE { get; set; }

        [Required]
        [StringLength(1)]
        [Unicode(false)]
        public string RECORD_TYPE { get; set; }

        [Required]
        public int ISSUE_DATE { get; set; }

        public virtual PCOMC_COMMISSION_CONTROL_TYPE_S? PCOMC_COMMISSION_CONTROL_TYPE_S { get; set; }
    }
}
