namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("SysNBRequirements")]

    public class SysNBRequirements
    {
        [StringLength(12)]
        [Unicode(false)]
        public string POLICYNUMBER { get; set; } = null!;

        public int REQSEQ { get; set; }

        public int IX { get; set; }

        [StringLength(20)]
        [Unicode(false)]
        public string REQTYPE { get; set; } = null!;

        [StringLength(2000)]
        [Unicode(false)]
        public string? REQNOTE { get; set; }
    }
}