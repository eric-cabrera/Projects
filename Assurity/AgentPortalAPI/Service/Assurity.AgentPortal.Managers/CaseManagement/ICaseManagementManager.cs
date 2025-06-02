namespace Assurity.AgentPortal.Managers.CaseManagement;

using Assurity.AgentPortal.Contracts.CaseManagement;

public interface ICaseManagementManager
{
    Task<CaseManagementResponse?> GetCases(string agentId, CaseManagementParameters parameters, CancellationToken cancellationToken = default);

    Task<CaseManagementFilters?> GetFilterOptions(string agentId, CancellationToken cancellationToken = default);

    Task<bool> ResendEmail(string envelopeId, CancellationToken cancellationToken = default);
}
