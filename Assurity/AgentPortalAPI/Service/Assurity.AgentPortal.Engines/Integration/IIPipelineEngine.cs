namespace Assurity.AgentPortal.Engines.Integration
{
    using Assurity.AgentPortal.Contracts.Integration;

    public interface IIPipelineEngine
    {
        string GetBrowserPostSamlSignature(
            IPipelineSsoInfo agentInfo);
    }
}
