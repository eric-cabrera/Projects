namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("PHIER_AGENT_HIERARCHY")]
    public class PHIER_AGENT_HIERARCHY
    {
        [StringLength(2)]
        [Unicode(false)]
        public string COMPANY_CODE { get; set; }

        [StringLength(12)]
        public string AGENT_NUM { get; set; }

        [StringLength(10)]
        public string MARKET_CODE { get; set; }

        [StringLength(2)]
        [Unicode(false)]
        public string AGENT_LEVEL { get; set; }

        public int STOP_DATE { get; set; }

        public int START_DATE { get; set; }

        [StringLength(12)]
        public string HIERARCHY_AGENT { get; set; }

        [StringLength(10)]
        public string HIER_MARKET_CODE { get; set; }

        [StringLength(2)]
        [Unicode(false)]
        public string HIER_AGENT_LEVEL { get; set; }
    }
}
