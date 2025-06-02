namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Contains the address record.
    /// </summary>
    [Table("PADDR")]
    public class PADDR
    {
        [Key]
        public int ADDRESS_ID { get; set; }

        [StringLength(35)]
        public string ADDR_LINE_1 { get; set; }

        [StringLength(35)]
        public string ADDR_LINE_2 { get; set; }

        [StringLength(35)]
        public string ADDR_LINE_3 { get; set; }

        [StringLength(24)]
        public string CITY { get; set; }

        [StringLength(2)]
        [Unicode(false)]
        public string STATE { get; set; }

        [StringLength(5)]
        public string ZIP { get; set; }

        [StringLength(2)]
        [Unicode(false)]
        public string ZIP_EXTENSION { get; set; }

        [StringLength(4)]
        [Unicode(false)]
        public string BOX_NUMBER { get; set; }

        [StringLength(30)]
        public string COUNTRY { get; set; }
    }
}