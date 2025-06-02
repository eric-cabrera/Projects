namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Contains the address record.
    /// </summary>
    [Table("PKDEF_KEY_DEFINITION")]
    public class PKDEF_KEY_DEFINITION
    {
        [Key]
        public long KDEF_ID { get; set; }

        [StringLength(10)]
        public string IDENT { get; set; }

        public short DESC_NUM { get; set; }

        public short KEY_NUM { get; set; }

        [StringLength(20)]
        public string KDEF_DESC { get; set; }
    }
}
