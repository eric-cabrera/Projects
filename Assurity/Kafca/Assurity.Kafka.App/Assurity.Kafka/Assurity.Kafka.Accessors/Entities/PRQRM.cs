namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Contains policy requirement data.
    /// TODO (SDF) - Do we need to set a [Key] field? In SQL, the only key for this table is
    ///     the RQRM_KEY0 (PK, binary(26), not null) field, but it's not used when joining
    ///     tables. Currently specifying .HasNoKey() in the DataStoreContext.cs OnModelCreating.
    /// </summary>
    [Table("PRQRM")]
    public class PRQRM
    {
        /// <summary>
        /// Code representing a New York policy (02) or other states (01).
        /// </summary>
        [Required]
        [StringLength(2)]
        [Unicode(false)]
        public string COMPANY_CODE { get; set; }

        /// <summary>
        /// Last Change Date for the Requirement.
        /// Stored as integer. Date format: YYYYMMDD.
        /// </summary>
        public int LAST_CHG_DATE { get; set; }

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
        /// Requirement Sequence, used to join with PRQRMTBL for a particular Requirement.
        /// </summary>
        public short REQ_SEQUENCE { get; set; }
    }
}