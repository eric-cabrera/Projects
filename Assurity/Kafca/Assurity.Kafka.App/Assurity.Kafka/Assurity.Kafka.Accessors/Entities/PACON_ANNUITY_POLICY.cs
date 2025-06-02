namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.EntityFrameworkCore;

    public class PACON_ANNUITY_POLICY
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

        [Required]
        [StringLength(1)]
        [Unicode(false)]
        public string STATUS_CODE { get; set; }

        [Required]
        [StringLength(2)]
        [Unicode(false)]
        public string STATUS_REASON { get; set; }

        public int STATUS_DATE { get; set; }

        public int ISSUE_DATE { get; set; }

        [Required]
        [StringLength(1)]
        [Unicode(false)]
        public string TAX_QUALIFICATION { get; set; }
    }
}
