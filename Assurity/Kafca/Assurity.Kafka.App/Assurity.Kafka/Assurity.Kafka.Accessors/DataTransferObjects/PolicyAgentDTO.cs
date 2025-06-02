namespace Assurity.Kafka.Accessors.DataTransferObjects
{
    public record PolicyAgentDTO
    {
        public string AgentNumber { get; set; }

        public decimal CommissionPercent { get; set; }

        public string Level { get; set; }

        public string MarketCode { get; set; }

        public NameDTO Name { get; set; }

        public string ServiceAgentIndicator { get; set; }
    }
}
