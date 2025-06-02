namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// An entity representing the SysZ9Process table in the GlobalData database.
    /// </summary>
    [Table("SysZ9Process")]

    public class SysZ9Process
    {
        [Precision(18, 0)]
        public decimal RECORDID { get; set; }

        [StringLength(12)]
        [Unicode(false)]
        public string? AGENTID { get; set; }

        [StringLength(10)]
        [Unicode(false)]
        public string? AGENTMARKETCODE { get; set; }

        [StringLength(2)]
        [Unicode(false)]
        public string? AGENTLEVEL { get; set; }

        [StringLength(44)]
        [Unicode(false)]
        public string? NBFOLDEROBJID { get; set; }
    }
}
