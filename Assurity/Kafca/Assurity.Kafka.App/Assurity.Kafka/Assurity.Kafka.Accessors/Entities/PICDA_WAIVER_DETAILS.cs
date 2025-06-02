namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("PICDA_WAIVER_DETAILS")]
    public class PICDA_WAIVER_DETAILS
    {
        [Required]
        [StringLength(2)]
        [Unicode(false)]
        public string TYPE_CODE { get; set; }

        [Required]
        [StringLength(20)]
        public string KEY_DATA { get; set; }

        [Required]
        public short RECORD_SEQUENCE { get; set; }

        [Required]
        [StringLength(60)]
        public string DESCRIPTION { get; set; }
    }
}
