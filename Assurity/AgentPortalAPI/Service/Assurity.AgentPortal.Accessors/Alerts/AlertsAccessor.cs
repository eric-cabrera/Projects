namespace Assurity.AgentPortal.Accessors.Alerts;

using System.Threading;
using Assurity.AgentPortal.Accessors.AssureLink.Context;
using Assurity.AgentPortal.Accessors.AssureLink.Entities;
using Microsoft.EntityFrameworkCore;

public class AlertsAccessor : IAlertsAccessor
{
    public AlertsAccessor(IDbContextFactory<AssureLinkContext> assureLinkContextFactory)
    {
        AssureLinkContext = assureLinkContextFactory;
    }

    private IDbContextFactory<AssureLinkContext> AssureLinkContext { get; }

    public async Task<List<DistributionList>?> GetDistributionListsByAgentId(string agentId, CancellationToken cancellationToken)
    {
        using var assureLinkContext = await AssureLinkContext.CreateDbContextAsync(cancellationToken);

        return await assureLinkContext.DistributionLists
            .Where(distributionList => distributionList.AgentId == agentId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddDistributionList(DistributionList distributionList, CancellationToken cancellationToken)
    {
        using var assureLinkContext = await AssureLinkContext.CreateDbContextAsync(cancellationToken);
        var existingDistributionList = await GetDistributionListByAgentIdAndEmail(distributionList.AgentId, distributionList.Email, assureLinkContext, cancellationToken);
        if (existingDistributionList == null)
        {
            await assureLinkContext.DistributionLists.AddAsync(distributionList, cancellationToken);
            await assureLinkContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            throw new InvalidOperationException($"Email {distributionList.Email} already exists on for agentId {distributionList.AgentId}");
        }
    }

    public async Task DeleteDistributionList(int distributionListId, string agentId, CancellationToken cancellationToken)
    {
        using var assureLinkContext = await AssureLinkContext.CreateDbContextAsync(cancellationToken);
        var distributionListToRemove = await GetDistributionListById(distributionListId, agentId, assureLinkContext, cancellationToken);
        if (distributionListToRemove != null)
        {
            assureLinkContext.DistributionLists.Remove(distributionListToRemove);
            await assureLinkContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            throw new KeyNotFoundException($"Unable to delete distribution list with the id {distributionListId}; it does not exist.");
        }
    }

    public async Task<DistributionMaster?> GetDistributionMasterByAgentId(string agentId, CancellationToken cancellationToken)
    {
        using var assureLinkContext = await AssureLinkContext.CreateDbContextAsync(cancellationToken);

        return await assureLinkContext.DistributionMasters
            .Where(distributionList => distributionList.AgentId == agentId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task DeleteDistributionMaster(DistributionMaster distributionMasterToRemove, CancellationToken cancellationToken)
    {
        using var assureLinkContext = await AssureLinkContext.CreateDbContextAsync(cancellationToken);
        assureLinkContext.DistributionMasters.Remove(distributionMasterToRemove);
        await assureLinkContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpsertDistributionMaster(string agentId, DistributionMaster newDistributionMaster, CancellationToken cancellationToken)
    {
        using (var assureLinkContext = await AssureLinkContext.CreateDbContextAsync(cancellationToken))
        {
            var distributionMasterEntity = await assureLinkContext.DistributionMasters
                .Where(distributionList => distributionList.AgentId == agentId)
                .FirstOrDefaultAsync(cancellationToken);

            if (distributionMasterEntity != null)
            {
                distributionMasterEntity.DisableAll = newDistributionMaster.DisableAll;
                distributionMasterEntity.SelfAdd = newDistributionMaster.SelfAdd;
                distributionMasterEntity.SelfMet = newDistributionMaster.SelfMet;
                distributionMasterEntity.SelfOutstanding = newDistributionMaster.SelfOutstanding;
                distributionMasterEntity.HierarchyAdd = newDistributionMaster.HierarchyAdd;
                distributionMasterEntity.HierarchyMet = newDistributionMaster.HierarchyMet;
                distributionMasterEntity.HierarchyOutstanding = newDistributionMaster.HierarchyOutstanding;

                await assureLinkContext.SaveChangesAsync(cancellationToken);
            }
            else
            {
                await assureLinkContext.DistributionMasters.AddAsync(newDistributionMaster, cancellationToken);
                await assureLinkContext.SaveChangesAsync(cancellationToken);
            }
        }
    }

    private async Task<DistributionList?> GetDistributionListById(int distributionListId, string agentId, AssureLinkContext assureLinkContext, CancellationToken cancellationToken)
    {
        return await assureLinkContext.DistributionLists
            .Where(distributionList => distributionList.Id == distributionListId && distributionList.AgentId == agentId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<DistributionList?> GetDistributionListByAgentIdAndEmail(string agentId, string email, AssureLinkContext assureLinkContext, CancellationToken cancellationToken)
    {
        return await assureLinkContext.DistributionLists
            .Where(distributionList =>
                distributionList.AgentId == agentId
                && distributionList.Email == email)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
