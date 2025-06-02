namespace Assurity.AgentPortal.Managers.Integration
{
    using Assurity.AgentPortal.Accessors.Agent;
    using Assurity.AgentPortal.Accessors.IllustrationPro;
    using Assurity.AgentPortal.Accessors.Integration;
    using Assurity.AgentPortal.Contracts.Integration;
    using Assurity.AgentPortal.Engines.Integration;
    using Assurity.AgentPortal.Utilities.Integration;
    using AutoMapper;

    public class IllustrationProManager : IIllustrationProManager
    {
        public IllustrationProManager(
            IAgentApiAccessor agentApiAccessor,
            IIllustrationProEngine illustrationProEngine,
            IIllustrationProApiAccessor illustrationProApiAccessor,
            IMapper mapper)
        {
            AgentApiAccessor = agentApiAccessor;
            IllustrationProEngine = illustrationProEngine;
            IllustrationProApiAccessor = illustrationProApiAccessor;
            Mapper = mapper;
        }

        private IAgentApiAccessor AgentApiAccessor { get; }

        private IIllustrationProEngine IllustrationProEngine { get; }

        private IIllustrationProApiAccessor IllustrationProApiAccessor { get; }

        private IMapper Mapper { get; }

        public string GetCredentialsForIllustrationProForHomeOfficeUser(string userName)
        {
            var agentInfo = new IllustrationSsoInfo
            {
                AGENCYID = "1",
                FIRSTNAME = "Assurity Life",
                LASTNAME = "Insurance Co.",
                AGENCYNAME = "Assurity Life Insurance Co.",
                ADDRESS1 = "PO Box 82533",
                CITY = "Lincoln",
                STATE = "NE",
                ZIP = "68501-2533",
                PHONE = "800-869-0355",
                ISBANK = "0"
            };

            return XmlHelper.SerializeIllustrationProXml(agentInfo, "HO", userName).InnerXml;
        }

        public async Task<string?> GetCredentialsForIllustrationPro(
            string agentId,
            string userName,
            CancellationToken cancellationToken = default)
        {
            var agentInformation = await AgentApiAccessor.GetAgentInformation(agentId, cancellationToken);
            if (agentInformation == null)
            {
                return null;
            }

            var mappedAgentInfo = Mapper.Map<IllustrationSsoInfo>(agentInformation);
            mappedAgentInfo.AGENCYID = agentId;
            mappedAgentInfo.ISBANK = "0";

            var marketCodes = await AgentApiAccessor.GetAgentMarketCodes(agentId, true, cancellationToken);
            if (marketCodes == null)
            {
                return null;
            }

            var distributionChannel = IllustrationProEngine.GetFiservDistributionChannelForIllustrationPro(marketCodes);

            return XmlHelper.SerializeIllustrationProXml(mappedAgentInfo, distributionChannel, userName).InnerXml;
        }

        public async Task<string?> GetIllustrationProAccountId(string credentials)
        {
            return await IllustrationProApiAccessor.GetAccountId(credentials);
        }
    }
}