namespace Assurity.AgentPortal.Managers.Integration
{
    using Assurity.AgentPortal.Accessors.Agent;
    using Assurity.AgentPortal.Contracts.Integration;
    using Assurity.AgentPortal.Engines.Integration;
    using Assurity.AgentPortal.Utilities.Integration;
    using AutoMapper;

    public class IPipelineManager : IIPipelineManager
    {
        public IPipelineManager(
            IAgentApiAccessor agentApiAccessor,
            IIPipelineEngine iPipelineEngine,
            IMapper mapper)
        {
            AgentApiAccessor = agentApiAccessor;
            IPipelineEngine = iPipelineEngine;
            Mapper = mapper;
        }

        private IAgentApiAccessor AgentApiAccessor { get; }

        private IIPipelineEngine IPipelineEngine { get; }

        private IMapper Mapper { get; }

        public async Task<string?> GetBrowserPostSamlSignature(
            string agentId,
            CancellationToken cancellationToken = default)
        {
            var agentInformation = await AgentApiAccessor.GetAgentInformation(agentId, cancellationToken);
            if (agentInformation == null)
            {
                return null;
            }

            var mappedAgentInfo = Mapper.Map<IPipelineSsoInfo>(agentInformation);
            mappedAgentInfo.AgentNumber = agentId;
            mappedAgentInfo.AIAgency = "0";
            mappedAgentInfo.ShowAgency = "0";
            mappedAgentInfo.Initial_DisconnectedLogonID = null;
            mappedAgentInfo.Initial_DisconnectedPassword = null;

            return IPipelineEngine.GetBrowserPostSamlSignature(mappedAgentInfo);
        }

        public string GetBrowserPostSamlSignature()
        {
            var agentInfo = GetAgentInfoForIPipelineForHomeOfficeUser();
            agentInfo.AIAgency = "0";
            agentInfo.ShowAgency = "0";
            agentInfo.Initial_DisconnectedLogonID = null;
            agentInfo.Initial_DisconnectedPassword = null;

            return IPipelineEngine.GetBrowserPostSamlSignature(agentInfo);
        }

        public IPipelineSsoInfo? GetAgentInfoForIPipelineForHomeOfficeUser()
        {
            var agentInfo = new IPipelineSsoInfo
            {
                AgentNumber = "HO9999",
                FirstName = "Assurity",
                LastName = "Life",
                Agentname = "Assurity Life",
                Email = "HelpDesk@assurity.com",
                Phone = "0000000000",
                Fax = "0000000000",
                Address1 = "PO Box 82533",
                City = "LINCOLN",
                State = "NE",
                ZipCode = "68501"
            };

            return agentInfo;
        }
    }
}