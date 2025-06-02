namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("PACTG")]
    public class PACTG
    {
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

        public short BENEFIT_SEQ { get; set; }

        public int DATE_ADDED { get; set; }

        public int TIME_ADDED { get; set; }

        public short RECORD_SEQUENCE { get; set; }

        public short LIFEPRO_ID { get; set; }

        public int EFFECTIVE_DATE { get; set; }

        public short DEBIT_CODE { get; set; }

        public short CREDIT_CODE { get; set; }

        [Unicode(false)]
        [StringLength(1)]
        public string REVERSAL_CODE { get; set; }
    }
}
