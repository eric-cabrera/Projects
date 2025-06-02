namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Contains policy requirements. Can join to PRQRM and PMEDR.
    /// TODO (SDF) - This table might only be used for policies with multiple insureds. Not sure.
    ///     Do we need to set a [Key] field? In SQL, the only key for this table is
    ///     the RQRM_KEY0 (PK, binary(30), not null) field, but it's not used when joining tables.
    ///     Currently specifying .HasNoKey() in the DataStoreContext.cs OnModelCreating.
    /// </summary>
    [Table("PRQRMTBL")]
    public class PRQRMTBL
    {
        /// <summary>
        /// Code representing a New York policy (02) or other states (01).
        /// </summary>
        [Required]
        [StringLength(2)]
        [Unicode(false)]
        public string COMPANY_CODE { get; set; }

        /// <summary>
        /// The Name ID used to relate the requirement to an insured.
        /// </summary>
        public int NAME_ID { get; set; }

        /// <summary>
        /// Policy number tied to the record.
        /// </summary>
        [Required]
        [StringLength(12)]
        public string POLICY_NUMBER { get; set; }

        /// <summary>
        /// Requirement Sequence, used to join with PRQRM for a particular Requirement.
        /// </summary>
        public short REQ_SEQUENCE { get; set; }

        /// <summary>
        /// The Sequence of the Requirement, used to denote order.
        /// </summary>
        public short SEQ { get; set; }

        /// <summary>
        /// The description code for the Requirement.
        /// Can be used to correlate with the PMEDR.REQ_NUMBER to get a friendly description.
        /// </summary>
        public short UND_DESC_CODE { get; set; }

        /// <summary>
        /// Underwriting Requirement Date.
        /// Stored as integer. Date format: YYYYMMDD.
        /// </summary>
        public int UND_REQ_DATE { get; set; }

        /// <summary>
        /// Underwriting Obtain Date.
        /// Stored as integer. Date format: YYYYMMDD.
        /// </summary>
        public int UND_OBTAIN_DATE { get; set; }

        /// <summary>
        /// A single character flag indicating whether or not the Requirement has been met.
        /// Values may be: "Y" or "N".
        /// </summary>
        [Required]
        [StringLength(1)]
        [Unicode(false)]
        public string UND_REQ_MET { get; set; }

        /// <summary>
        /// Underwriting comments.
        /// </summary>
        [Required]
        [StringLength(18)]
        public string UND_COMMENTS { get; set; }

        /// <summary>
        /// A note sequence, used to find a specific Requirement Comment. Not fully understood. Based on existing usage.
        /// </summary>
        public int UND_REQ_NOTE_SEQ { get; set; }
    }
}