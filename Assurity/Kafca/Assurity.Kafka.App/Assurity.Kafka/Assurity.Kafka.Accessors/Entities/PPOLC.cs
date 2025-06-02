namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// This is the policy master table and holds information on policies.
    /// </summary>
    [Table("PPOLC")]
    public class PPOLC
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

        /// <summary>
        /// Group number tied to the record.
        /// </summary>
        [Required]
        [StringLength(10)]
        public string GROUP_NUMBER { get; set; }

        /// <summary>
        /// Line Of Business ("A", "G", "H", "I", "L", "S", "U").
        /// </summary>
        [Required]
        [StringLength(1)]
        [Unicode(false)]
        public string LINE_OF_BUSINESS { get; set; }

        /// <summary>
        /// Product Code for policy (i.e. GH1715CI, IL1901, etc).
        /// </summary>
        [Required]
        [StringLength(10)]
        public string PRODUCT_CODE { get; set; }

        /// <summary>
        /// "T", "A" or "P", for Terminated, Active or Pending policy.
        /// </summary>
        [Required]
        [StringLength(1)]
        [Unicode(false)]
        public string CONTRACT_CODE { get; set; }

        [Required]
        [StringLength(2)]
        [Unicode(false)]
        public string CONTRACT_REASON { get; set; }

        [Required]
        public int CONTRACT_DATE { get; set; }

        public int BILLING_DATE { get; set; }

        public int ISSUE_DATE { get; set; }

        [Required]
        [StringLength(2)]
        [Unicode(false)]
        public string ISSUE_STATE { get; set; }

        [Required]
        [StringLength(2)]
        [Unicode(false)]
        public string RES_STATE { get; set; }

        /// <summary>
        /// "A", "H" or "S", for Active, HoldBilling or Suspended Billing Status.
        /// </summary>
        [Required]
        [StringLength(1)]
        [Unicode(false)]
        public string BILLING_CODE { get; set; }

        public short BILLING_MODE { get; set; }

        [Required]
        [StringLength(3)]
        [Unicode(false)]
        public string BILLING_FORM { get; set; }

        /// <summary>
        /// Billing Status Reason
        /// ("PC", "SP", "VB", "VN", "WD", "AJ", "BE", "DP", "EP", "ET", "NF", "PU", "RU", "ST", "WP").
        /// </summary>
        [Required]
        [StringLength(2)]
        [Unicode(false)]
        public string BILLING_REASON { get; set; }

        public int PAID_TO_DATE { get; set; }

        public short POLICY_BILL_DAY { get; set; }

        [Column(TypeName = "decimal(9,2)")]
        public decimal MODE_PREMIUM { get; set; }

        [Column(TypeName = "decimal(9,2)")]
        public decimal ANNUAL_PREMIUM { get; set; }

        public int APPLICATION_DATE { get; set; }

        public int APP_RECEIVED_DATE { get; set; }

        [Required]
        [StringLength(1)]
        [Unicode(false)]
        public string POLC_SPECIAL_MODE { get; set; }

        /// <summary>
        /// Payment Status Reason
        /// ("NM", "PR", "PS", "WD", "DP", "IS", "PC", "PW", "RS", "WP").
        /// </summary>
        [Required]
        [StringLength(2)]
        [Unicode(false)]
        public string PAYMENT_REASON { get; set; }

        /// <summary>
        /// Tax Qualification Status
        /// ("0, "1", "2", "3", "4", "5", "6", "7", "8", "9", "S", "C", "R", "Q", "O", "N", "Y", "T", "G", "A", "K", "V").
        /// </summary>
        [Required]
        [StringLength(1)]
        [Unicode(false)]
        public string TAX_QUALIFY_CODE { get; set; }

        public virtual PACON_ANNUITY_POLICY? PACON_ANNUITY_POLICY { get; set; }

        public virtual PGRUP_GROUP_MASTER? PGRUP_GROUP_MASTER { get; set; }

        public virtual PPEND_NEW_BUSINESS_PENDING? PPEND_NEW_BUSINESS_PENDING { get; set; }
    }
}