namespace Assurity.AgentPortal.Managers.Alerts;

using Assurity.AgentPortal.Accessors.Alerts;
using ClientContracts = Assurity.AgentPortal.Contracts.Alerts;
using DBEntities = Assurity.AgentPortal.Accessors.AssureLink.Entities;

public class AlertsManager : IAlertsManager
{
    public AlertsManager(IAlertsAccessor alertsAccessor)
    {
        AlertsAccessor = alertsAccessor;
    }

    private IAlertsAccessor AlertsAccessor { get; }

    public async Task<List<ClientContracts.DistributionList>?> GetDistributionEmailsByAgentId(string agentId, CancellationToken cancellationToken)
    {
        var distributionEmails = await AlertsAccessor.GetDistributionListsByAgentId(agentId, cancellationToken);
        if (distributionEmails != null)
        {
            return distributionEmails.Select(x => new ClientContracts.DistributionList
            {
                Id = x.Id,
                Email = x.Email,
            }).ToList();
        }

        return null;
    }

    public async Task AddDistributionEmail(string agentId, string email, CancellationToken cancellationToken)
    {
        var distributionList = new DBEntities.DistributionList
        {
            AgentId = agentId,
            Email = email
        };

        await AlertsAccessor.AddDistributionList(distributionList, cancellationToken);
    }

    public async Task DeleteDistributionEmail(int distributionEmailId, string agentId, CancellationToken cancellationToken)
    {
        await AlertsAccessor.DeleteDistributionList(distributionEmailId, agentId, cancellationToken);
    }

    public async Task<ClientContracts.AlertPreferences?> GetAlertPreferencesByAgentId(string agentId, CancellationToken cancellationToken)
    {
        var distributionMaster = await AlertsAccessor.GetDistributionMasterByAgentId(agentId, cancellationToken);
        if (distributionMaster != null)
        {
            return new ClientContracts.AlertPreferences
            {
                DisableAll = distributionMaster.DisableAll,
                SelfAdd = distributionMaster.SelfAdd,
                SelfMet = distributionMaster.SelfMet,
                SelfOutstanding = distributionMaster.SelfOutstanding,
                HierarchyAdd = distributionMaster.HierarchyAdd,
                HierarchyMet = distributionMaster.HierarchyMet,
                HierarchyOutstanding = distributionMaster.HierarchyOutstanding
            };
        }

        return new ClientContracts.AlertPreferences
        {
            DisableAll = false,
            SelfAdd = false,
            SelfMet = false,
            SelfOutstanding = false,
            HierarchyAdd = false,
            HierarchyMet = false,
            HierarchyOutstanding = false
        };
    }

    public async Task AddOrUpdateAlertPreferences(string agentId, ClientContracts.AlertPreferences alertPreferences, CancellationToken cancellationToken)
    {
        var distributionMasterToUpdate = ConvertAlertPreferencesToDistributionMaster(agentId, alertPreferences);
        await AlertsAccessor.UpsertDistributionMaster(agentId, distributionMasterToUpdate, cancellationToken);
    }

    public async Task DeleteAlertPreferences(string agentId, CancellationToken cancellationToken)
    {
        var distributionMaster = await AlertsAccessor.GetDistributionMasterByAgentId(agentId, cancellationToken);
        if (distributionMaster != null)
        {
            await AlertsAccessor.DeleteDistributionMaster(distributionMaster, cancellationToken);
        }
        else
        {
            throw new KeyNotFoundException($"Unable to delete distribution master with the agent id {agentId}; it does not exist.");
        }
    }

    private static DBEntities.DistributionMaster ConvertAlertPreferencesToDistributionMaster(string agentId, ClientContracts.AlertPreferences alertPreferences)
    {
        if (alertPreferences == null ||
            !alertPreferences.DisableAll.HasValue ||
            !alertPreferences.SelfAdd.HasValue ||
            !alertPreferences.SelfMet.HasValue ||
            !alertPreferences.SelfOutstanding.HasValue ||
            !alertPreferences.HierarchyAdd.HasValue ||
            !alertPreferences.HierarchyMet.HasValue ||
            !alertPreferences.HierarchyOutstanding.HasValue)
        {
            throw new Exception("Alert preferences and all of its parameters are required.");
        }

        return new DBEntities.DistributionMaster
        {
            AgentId = agentId,
            DisableAll = alertPreferences.DisableAll.Value,
            SelfAdd = alertPreferences.SelfAdd.Value,
            SelfMet = alertPreferences.SelfMet.Value,
            SelfOutstanding = alertPreferences.SelfOutstanding.Value,
            HierarchyAdd = alertPreferences.HierarchyAdd.Value,
            HierarchyMet = alertPreferences.HierarchyMet.Value,
            HierarchyOutstanding = alertPreferences.HierarchyOutstanding.Value
        };
    }
}
