namespace Assurity.AgentPortal.Accessors;

using Assurity.AgentPortal.Contracts;

public interface IUserDataAccessor
{
    Task<List<Market>?> GetProductionCreditBusinessTypes(string accessToken, string? agentId);

    Task<MarketCodeInformationResponse?> GetMarketCodeInformation(string accessToken, string agentId, string marketCode);

    Task<AdditionalAgentIds> GetAdditionalAgentIds(string accessToken, string? agentId);
}