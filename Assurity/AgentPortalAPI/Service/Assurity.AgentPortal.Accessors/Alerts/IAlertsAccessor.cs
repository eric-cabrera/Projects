namespace Assurity.AgentPortal.Accessors.Alerts;

using Assurity.AgentPortal.Accessors.AssureLink.Entities;

public interface IAlertsAccessor
{
    Task<List<DistributionList>?> GetDistributionListsByAgentId(string agentId, CancellationToken cancellationToken);

    Task AddDistributionList(DistributionList distributionList, CancellationToken cancellationToken);

    Task DeleteDistributionList(int distributionListId, string agentId, CancellationToken cancellationToken);

    Task<DistributionMaster?> GetDistributionMasterByAgentId(string agentId, CancellationToken cancellationToken);

    Task DeleteDistributionMaster(DistributionMaster distributionMasterToRemove, CancellationToken cancellationToken);

    Task UpsertDistributionMaster(string agentId, DistributionMaster newDistributionMaster, CancellationToken cancellationToken);
}
