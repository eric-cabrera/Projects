namespace Assurity.AgentPortal.Managers.UserData;

using Assurity.AgentPortal.Accessors.Agent;
using Assurity.AgentPortal.Accessors.ProductionCredit;
using Assurity.AgentPortal.Contracts.AgentContracts;
using Assurity.AgentPortal.Contracts.UserData;
using Assurity.AgentPortal.Utilities.Emails;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;

public class UserDataManager : IUserDataManager
{
    public UserDataManager(
        IProductionCreditApiAccessor productionCreditApiAccessor,
        IAgentApiAccessor agentApiAccessor,
        IEmailUtility emailUtility,
        IMemoryCache memoryCache,
        IMapper mapper)
    {
        ProductionCreditApiAccessor = productionCreditApiAccessor;
        AgentApiAccessor = agentApiAccessor;
        EmailUtility = emailUtility;
        MemoryCache = memoryCache;
        Mapper = mapper;
    }

    private IProductionCreditApiAccessor ProductionCreditApiAccessor { get; }

    private IAgentApiAccessor AgentApiAccessor { get; }

    private IMemoryCache MemoryCache { get; }

    private IEmailUtility EmailUtility { get; }

    private IMapper Mapper { get; }

    public async Task<AccessLevel?> GetAgentAccess(string agentId, CancellationToken cancellationToken = default)
    {
        if (MemoryCache.TryGetValue(agentId, out AccessLevel accessLevel))
        {
            return accessLevel;
        }

        var agentAccessResponse = await AgentApiAccessor.GetAgentAccess(agentId, cancellationToken);
        if (agentAccessResponse != null)
        {
            var mappedAccessLevel = Mapper.Map<AccessLevel>(agentAccessResponse?.AccessLevel);

            MemoryCache.Set(
            agentId,
            mappedAccessLevel,
            TimeSpan.FromMinutes(15));

            return mappedAccessLevel;
        }

        return null;
    }

    public async Task<HashSet<Market>> GetBusinessTypesByAgentId(string agentId, CancellationToken cancellationToken)
    {
        var marketCodes = await ProductionCreditApiAccessor.GetProductionCreditMarketcodes(agentId, cancellationToken);

        return GetBusinessTypesFromMarketCodes(marketCodes);
    }

    public async Task SendEmailNotifications(string originalEmail, string newEmail)
    {
        var originalEmailMessage = EmailUtility.CreateMFAEmailChangeEmail(originalEmail, true);
        var newEmailMessage = EmailUtility.CreateMFAEmailChangeEmail(newEmail, false);

        var originalEmailTask = EmailUtility.SendEmail(originalEmailMessage);
        var newEmailTask = EmailUtility.SendEmail(newEmailMessage);

        await Task.WhenAll([newEmailTask, originalEmailTask]);
    }

    public async Task<List<string>?> GetAdditionalAgentIds(string agentId, CancellationToken cancellationToken)
    {
        return await AgentApiAccessor.GetAdditionalAgentIds(agentId, cancellationToken);
    }

    private static HashSet<Market> GetBusinessTypesFromMarketCodes(List<string>? marketCodes)
    {
        var worksitePrefix = "ws";
        var markets = new HashSet<Market>();

        if (marketCodes?.Any(marketCode => marketCode.StartsWith(worksitePrefix, StringComparison.InvariantCultureIgnoreCase)) ?? false)
        {
            markets.Add(Market.Worksite);
        }

        if (marketCodes?.Any(marketCode => !marketCode.StartsWith(worksitePrefix, StringComparison.InvariantCultureIgnoreCase)) ?? false)
        {
            markets.Add(Market.Individual);
        }

        return markets;
    }
}
