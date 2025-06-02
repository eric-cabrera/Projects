namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Requirement lookup table (contains description).
    /// TODO (SDF) - Do we need to set a [Key] field? In SQL, the only key for this table is
    ///     the MEDR_KEY0 (PK, binary(5), not null) field, but it's not used when joining
    ///     tables. Currently specifying .HasNoKey() in the DataStoreContext.cs OnModelCreating.
    /// </summary>
    [Table("PMEDR")]
    public class PMEDR
    {
        /// <summary>
        /// Record Type.
        /// TODO: SDF - Come up with a better summary description. I saw this field used in some
        ///       queries within AgentCorre SQL code that specified that it had to be "R".
        /// Possible values: "C", "R", "T".
        /// </summary>
        [Required]
        [StringLength(1)]
        [Unicode(false)]
        public string RECORD_TYPE { get; set; }

        /// <summary>
        /// Friendly description for the Requirement.
        /// </summary>
        [Required]
        [StringLength(45)]
        public string REQ_DESCRIPTION { get; set; }

        /// <summary>
        /// Shorthand name for the Requirement, often abbreviated to fit within the nchar(10) field.
        /// </summary>
        [Required]
        [StringLength(10)]
        public string REQ_NAME { get; set; }

        /// <summary>
        /// The description code for the Requirement.
        /// Can be used to correlate with the PRQRMTBL.UND_DESC_CODE.
        /// </summary>
        public short REQ_NUMBER { get; set; }
    }
}