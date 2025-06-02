namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// This is just really the PRELA, PNAME, PNALK, and PADDR entities stiched together to read the results from the payors query
    /// </summary>
    [Table("PPOLCEntityQuery")]
    public class PPOLCEntityQuery
    {
        [Required]
        [StringLength(2)]
        public string CompanyCode { get; set; }

        /// <summary>
        /// Policy number tied to the record.
        /// </summary>
        [Key]
        [Required]
        [StringLength(12)]
        public string PolicyNumber { get; set; }

        /// <summary>
        /// Product Code for policy (i.e. GH1715CI, IL1901, etc).
        /// </summary>
        [Required]
        [StringLength(10)]
        public string ProductCode { get; set; }

        public string ProductCategory { get; set; }

        public string ProductDescription { get; set; }

        /// <summary>
        /// "T", "A" or "P", for Terminated, Active or Pending policy.
        /// </summary>
        [Required]
        [StringLength(1)]
        public string PolicyStatus { get; set; }

        [Required]
        [StringLength(2)]
        public string PolicyStatusReason { get; set; }

        public int IssueDate { get; set; }

        [Required]
        [StringLength(2)]
        public string IssueState { get; set; }

        [Required]
        [StringLength(2)]
        public string ResidentState { get; set; }

        public short BillingMode { get; set; }

        [Required]
        [StringLength(3)]
        public string BillingForm { get; set; }

        public int PaidToDate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ModePremium { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal AnnualPremium { get; set; }

        public int BillingDay { get; set; }

        public decimal FaceAmount { get; set; }

        public string SubmitDate { get; set; }
    }
}