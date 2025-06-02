namespace Assurity.PolicyInfo.Contracts.V1
{
    public class Agent
    {
        public string AgentId { get; set; }

        public bool IsWritingAgent { get; set; }

        public bool IsServicingAgent { get; set; }

        public bool IsJustInTimeAgent { get; set; }

        public string Level { get; set; }

        public string MarketCode { get; set; }

        public Participant Participant { get; set; }
    }
}