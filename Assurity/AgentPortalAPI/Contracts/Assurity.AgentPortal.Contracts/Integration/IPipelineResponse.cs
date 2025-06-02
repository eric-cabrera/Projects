namespace Assurity.AgentPortal.Contracts.Integration
{
    public class IPipelineResponse
    {
        public string SAMLResponse { get; set; }

        public string IPipelineConnectionString { get; set; }

        public string IPipelineTargetString { get; set; }
    }
}
