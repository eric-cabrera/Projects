namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Contains records of the Group.
    /// </summary>
    [Table("PGRUP_GROUP_MASTER")]
    public class PGRUP_GROUP_MASTER
    {
        [Key]
        [Required]
        [StringLength(10)]
        public string GROUP_NUMBER { get; set; }

        /// <summary>
        /// Code representing a New York policy (02) or other states (01).
        /// </summary>
        [Required]
        [StringLength(2)]
        [Unicode(false)]
        public string COMPANY_CODE { get; set; }

        /// <summary>
        /// Id to tie record to names (PNAME) table.
        /// </summary>
        public int NAME_ID { get; set; }

        [Required]
        [Unicode(false)]
        public char STATUS_CODE { get; set; }

        /// <summary>
        /// Name record.
        /// </summary>
        /// <remarks>
        /// There are instances in Dev where a NAME_ID exists but
        /// PNAME contains no record.
        /// Otherwise, this is not expected to be null.
        /// </remarks>
        public virtual PNAME? PNAME { get; set; }
    }
}