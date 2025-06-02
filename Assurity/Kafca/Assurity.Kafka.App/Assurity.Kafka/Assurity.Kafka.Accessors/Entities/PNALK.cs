namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Contains all name records of the insured, policy holder, etc.
    /// </summary>
    [Table("PNALK")]
    public class PNALK
    {
        public int NAME_ID { get; set; }

        [StringLength(3)]
        [Unicode(false)]
        public string ADDRESS_CODE { get; set; }

        public int ADDRESS_ID { get; set; }

        public int CANCEL_DATE { get; set; }

        [StringLength(10)]
        public string TELE_NUM { get; set; }

        public virtual PADDR PADDR { get; set; }
    }
}