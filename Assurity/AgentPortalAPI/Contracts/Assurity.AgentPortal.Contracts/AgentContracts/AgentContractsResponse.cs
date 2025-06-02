namespace Assurity.AgentPortal.Contracts.AgentContracts
{
    public class AgentContractsResponse
    {
        public Dictionary<string, Dictionary<string, List<string>>>? AgentContracts { get; set; } = new();
    }
}
