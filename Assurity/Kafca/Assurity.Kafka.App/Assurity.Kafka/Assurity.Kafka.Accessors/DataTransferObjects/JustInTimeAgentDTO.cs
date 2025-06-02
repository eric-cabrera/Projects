namespace Assurity.Kafka.Accessors.DataTransferObjects
{
    public class JustInTimeAgentDTO
    {
        public string AgentId { get; set; } = null!;

        public string? MarketCode { get; set; }

        public string? Level { get; set; }

        public string? UplineAgentId { get; set; }

        public string? UplineMarketCode { get; set; }

        public string? UplineLevel { get; set; }
    }
}
