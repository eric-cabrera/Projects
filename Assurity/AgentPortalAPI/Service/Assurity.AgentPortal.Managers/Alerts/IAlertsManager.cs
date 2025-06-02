namespace Assurity.AgentPortal.Managers.Alerts;

using Assurity.AgentPortal.Contracts.Alerts;

public interface IAlertsManager
{
    Task<List<DistributionList>?> GetDistributionEmailsByAgentId(string agentId, CancellationToken cancellationToken);

    Task AddDistributionEmail(string agentId, string email, CancellationToken cancellationToken);

    Task DeleteDistributionEmail(int distributionEmailId, string agentId, CancellationToken cancellationToken);

    Task<AlertPreferences?> GetAlertPreferencesByAgentId(string agentId, CancellationToken cancellationToken);

    Task AddOrUpdateAlertPreferences(string agentId, AlertPreferences distributionMaster, CancellationToken cancellationToken);

    Task DeleteAlertPreferences(string agentId, CancellationToken cancellationToken);
}