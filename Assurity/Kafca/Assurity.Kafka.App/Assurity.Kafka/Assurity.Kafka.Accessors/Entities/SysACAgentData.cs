namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// An entity representing the SysACAgentData table in the GlobalData database.
    /// </smmary>
    [Table("SysACAgentData")]

    public class SysACAgentData
    {
        [StringLength(44)]
        [Unicode(false)]
        public string FOLDERID { get; set; } = null!;

        [StringLength(12)]
        [Unicode(false)]
        public string? AGENTID { get; set; }

        [StringLength(20)]
        [Unicode(false)]
        public string FIRSTNAME { get; set; }

        [StringLength(10)]
        [Unicode(false)]
        public string MIDDLENAME { get; set; }

        [StringLength(20)]
        [Unicode(false)]
        public string LASTNAME { get; set; }

        [StringLength(70)]
        [Unicode(false)]
        public string BUSINESSNAME { get; set; }
    }
}
