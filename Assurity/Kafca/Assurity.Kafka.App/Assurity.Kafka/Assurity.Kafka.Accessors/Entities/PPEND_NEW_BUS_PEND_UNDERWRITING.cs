namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("PPEND_NEW_BUS_PEND_UNDERWRITING")]
    public class PPEND_NEW_BUS_PEND_UNDERWRITING
    {
        [Required]
        public long PEND_ID { get; set; }

        public short IDX { get; set; }

        [Required]
        [StringLength(1)]
        [Unicode(false)]
        public string UND_FLAG { get; set; }

        public int UND_DATE { get; set; }

        public int UND_O_DATE { get; set; }

        public short UND_CODE { get; set; }

        [StringLength(18)]
        public string COMMENTS { get; set; }

        public int NOTE_SEQ { get; set; }
    }
}