namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("PCOMC_COMMISSION_CONTROL_TYPE_S")]
    public class PCOMC_COMMISSION_CONTROL_TYPE_S
    {
        [Required]
        public long COMC_ID { get; set; }

        [Required]
        public short I { get; set; }

        [Required]
        [StringLength(12)]
        public string AGENT { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal COMM_PCNT { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal PROD_PCNT { get; set; }

        [Required]
        [Unicode(false)]
        [StringLength(1)]
        public string SERVICE_AGENT_IND { get; set; }

        [Required]
        [StringLength(10)]
        public string MARKET_CODE { get; set; }

        [Required]
        [Unicode(false)]
        [StringLength(2)]
        public string AGENT_LEVEL { get; set; }

        public virtual ICollection<PAGNT_AGENT_MASTER> PAGNT_AGENT_MASTERs { get; set; }
    }
}
