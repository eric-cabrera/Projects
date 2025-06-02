namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("PBDRV")]
    public class PBDRV
    {
        [Required]
        [StringLength(15)]
        public string DESCRIPTION { get; set; }

        [Required]
        [StringLength(1)]
        [Unicode(false)]
        public string STATUS_CODE { get; set; }

        public int BATCH_START_DATE { get; set; }

        public int BATCH_STOP_DATE { get; set; }
    }
}