namespace Assurity.AgentPortal.Managers.Integration
{
    using Assurity.AgentPortal.Contracts.Integration;

    public interface IIPipelineManager
    {
        IPipelineSsoInfo? GetAgentInfoForIPipelineForHomeOfficeUser();

        string GetBrowserPostSamlSignature();

        Task<string?> GetBrowserPostSamlSignature(
            string agentId,
            CancellationToken cancellationToken = default);
    }
}
