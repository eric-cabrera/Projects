namespace Assurity.AgentPortal.Managers.Integration
{
    using Assurity.AgentPortal.Accessors.Agent;
    using Assurity.AgentPortal.Accessors.Integration;
    using Assurity.AgentPortal.Accessors.LifePortraits;
    using Assurity.AgentPortal.Contracts.Integration;
    using Assurity.AgentPortal.Utilities.Integration;
    using AutoMapper;

    public class LifePortraitsManager : ILifePortraitsManager
    {
        public LifePortraitsManager(
            IAgentApiAccessor agentApiAccessor,
            IIntegrationAccessor dataStoreAccessor,
            ILifePortraitsApiAccessor lifePortraitsApiAccessor,
            IMapper mapper)
        {
            AgentApiAccessor = agentApiAccessor;
            DataStoreAccessor = dataStoreAccessor;
            LifePortraitsApiAccessor = lifePortraitsApiAccessor;
            Mapper = mapper;
        }

        private IAgentApiAccessor AgentApiAccessor { get; }

        private IIntegrationAccessor DataStoreAccessor { get; }

        private ILifePortraitsApiAccessor LifePortraitsApiAccessor { get; }

        private IMapper Mapper { get; }

        public async Task<string> GetCredentialsForLifePortraitsForHomeOfficeUser(string userName)
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
                PHONE = "1-800-869-0355",
                ISBANK = "0"
            };

            var ssoUserId = await DataStoreAccessor.GetLifePortraitsSSOUserId(userName);
            if (ssoUserId == 0)
            {
                ssoUserId = await DataStoreAccessor.CreateSSOUserId(userName);
            }

            return XmlHelper.SerializeLifePortraitsXml(agentInfo, "HO", ssoUserId.ToString()).InnerXml;
        }

        public async Task<string?> GetCredentialsForLifePortraits(
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

            var distributionChannel = string.Empty;
            var marketCodes = await AgentApiAccessor.GetAgentMarketCodes(agentId, true, cancellationToken);
            if (marketCodes == null)
            {
                return null;
            }

            if (!(marketCodes?.Any() ?? false))
            {
                distributionChannel = "DEFAULT";
            }
            else
            {
                distributionChannel = await DataStoreAccessor.GetFiservDistributionChannelForLifePortraits(marketCodes);
            }

            var ssoUserId = await DataStoreAccessor.GetLifePortraitsSSOUserId(userName);
            if (ssoUserId == 0)
            {
                ssoUserId = await DataStoreAccessor.CreateSSOUserId(userName);
            }

            return XmlHelper.SerializeLifePortraitsXml(mappedAgentInfo, distributionChannel, ssoUserId.ToString()).InnerXml;
        }

        public async Task<string?> GetLifePortraitsURL(
            string credentials,
            CancellationToken cancellationToken = default)
        {
            return await LifePortraitsApiAccessor.GetURL(credentials, cancellationToken);
        }
    }
}
