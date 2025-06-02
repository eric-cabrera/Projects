namespace Assurity.AgentPortal.Accessors.Integration;

using Assurity.AgentPortal.Accessors.DataStore.Context;
using Microsoft.EntityFrameworkCore;

public class IntegrationAccessor : IIntegrationAccessor
{
    public IntegrationAccessor(IDbContextFactory<DataStoreContext> dataStoreContextFactory)
    {
        DataStoreContextFactory = dataStoreContextFactory;
    }

    private IDbContextFactory<DataStoreContext> DataStoreContextFactory { get; }

    public async Task<string> GetFiservDistributionChannelForLifePortraits(List<string> marketCodes)
    {
        using var dataStoreContext = await DataStoreContextFactory.CreateDbContextAsync();

        var distChannel = await dataStoreContext.FiservDistributionChannel
            .Where(distributionChannel => marketCodes.Contains(distributionChannel.MarketCodes))
            .Select(distributionChannel => distributionChannel.AccessCode)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(distChannel))
        {
            distChannel = string.Empty;
        }

        return distChannel;
    }

    public async Task<List<string>> GetNewYorkMarketCodes()
    {
        using var dataStoreContext = await DataStoreContextFactory.CreateDbContextAsync();

        return await dataStoreContext.PhierAgentHierarchies
            .Where(pah => pah.CompanyCode == "02")
            .Select(pah => pah.MarketCode)
            .Distinct()
            .ToListAsync();
    }

    public async Task<int> GetLifePortraitsSSOUserId(string username)
    {
        using var dataStoreContext = await DataStoreContextFactory.CreateDbContextAsync();

        return await dataStoreContext.LifePortraits
            .Where(lifeportrait => lifeportrait.LoweredUserName == username)
            .Select(lifeportrait => lifeportrait.SSOUserId)
            .FirstOrDefaultAsync();
    }

    public async Task<int> CreateSSOUserId(string username)
    {
        using var dataStoreContext = await DataStoreContextFactory.CreateDbContextAsync();
        const int ssoUserIDThreshold = 60000;

        var ssoUserID = (from a in dataStoreContext.LifePortraits
                         select a.SSOUserId).DefaultIfEmpty().Max();

        if (ssoUserID < ssoUserIDThreshold)
        {
            // The max ssoUserID is less than the threshold, so set the new ssoUserID to the threshold number
            ssoUserID = ssoUserIDThreshold;
        }
        else
        {
            // The max ssoUserID is at or above the threshold number, so simply increment the max by 1 for the new ssoUserID
            ssoUserID++;
        }

        // Add the new SSOUserID for the identityKey to the table
        dataStoreContext.LifePortraits.Add(new DataStore.Entities.LifePortraits
        {
            SSOUserId = ssoUserID,
            UserName = username,
            LoweredUserName = username.ToLower()
        });

        dataStoreContext.SaveChanges();

        return ssoUserID;
    }

    // NOTE: we are making sure that the stop date of a market code doesn't equal
    // 20991231 because it was used for converted contracts so that Contracting could code
    // an agent for servicing without having to reactivate that number. Since this date doesn't
    // actually represent a contract stop date, we have to hardcode around it.
    public async Task<List<string>> GetReverseHierarchyMarketCodes()
    {
        using var dataStoreContext = await DataStoreContextFactory.CreateDbContextAsync();

        var dateInt = (DateTime.Now.Year * 10000) + (DateTime.Now.Month * 100) + DateTime.Now.Day;

        return await dataStoreContext.PhierAgentHierarchies
            .Where(pah =>
                pah.HierAgentLevel != string.Empty
                && pah.StopDate > dateInt
                && string.Compare(pah.AgentLevel, pah.HierAgentLevel) == 1
                && pah.StopDate != 20991231)
            .Select(pah => pah.MarketCode)
            .Distinct()
            .ToListAsync();
    }
}
