namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// An entity representing the SysACAgentMarketCode table in the GlobalData database.
    /// </summary>
    [Table("SysACAgentMarketCodes")]

    public class SysACAgentMarketCodes
    {
        [StringLength(44)]
        [Unicode(false)]
        public string FOLDERID { get; set; } = null!;

        [StringLength(10)]
        [Unicode(false)]
        public string MARKETCODE { get; set; } = null!;

        [StringLength(2)]
        [Unicode(false)]
        public string CONTRACTLEVEL { get; set; } = null!;

        [StringLength(12)]
        [Unicode(false)]
        public string? UPLINEAGENTID { get; set; }

        [StringLength(10)]
        [Unicode(false)]
        public string? UPLINEMARKETCODE { get; set; }

        [StringLength(2)]
        [Unicode(false)]
        public string? UPLINECONTRACTLEVEL { get; set; }

        public int PENDINGRPTDISABLED { get; set; }
    }
}
