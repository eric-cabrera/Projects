namespace Assurity.Kafka.Accessors.Entities
{
    public class AgentHierarchyChange
    {
        public string AgentId { get; set; }

        public string AgentLevel { get; set; }

        public string? BeforeAgentId { get; set; }

        public ChangeType ChangeType { get; set; }

        public string CompanyCode { get; set; }

        public int Id { get; set; }

        public string MarketCode { get; set; }

        public int StartDate { get; set; }

        public int StopDate { get; set; }

        public int SystemDataLoadId { get; set; }

        public string UplineAgentId { get; set; }

        public string UplineAgentLevel { get; set; }

        public string UplineMarketCode { get; set; }
    }
}