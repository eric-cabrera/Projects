namespace Assurity.AgentPortal.Contracts.ProductionCredit.Response.Worksite
{
    using Assurity.AgentPortal.Contracts.ProductionCredit.FileExport;

    public class ProductionCreditWorksiteViewTypeResponse
    {
        public List<ProductionCreditWorksiteProductExport> GroupProducts { get; set; } = new();

        public List<ProductionCreditWorksiteGroupExport> GroupDetails { get; set; } = new();

        public List<ProductionCreditWorksiteAgentExport> GroupedAgents { get; set; } = new();

        public List<ProductionCreditWorksiteDownlineWritingAgentsExport> DownlineWritingAgents { get; set; } = new();
    }
}
