namespace Assurity.AgentPortal.Accessors.Integration;

public interface IIntegrationAccessor
{
    Task<string> GetFiservDistributionChannelForLifePortraits(List<string> marketCodes);

    Task<List<string>> GetReverseHierarchyMarketCodes();

    Task<int> GetLifePortraitsSSOUserId(string username);

    Task<int> CreateSSOUserId(string username);

    Task<List<string>> GetNewYorkMarketCodes();
}
