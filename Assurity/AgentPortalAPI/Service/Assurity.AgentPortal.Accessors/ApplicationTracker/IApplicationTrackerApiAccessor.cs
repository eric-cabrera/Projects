namespace Assurity.AgentPortal.Accessors.ApplicationTracker;

using Assurity.AgentPortal.Contracts.CaseManagement;
using Assurity.ApplicationTracker.Contracts;

public interface IApplicationTrackerApiAccessor
{
    Task<PagedEvents?> GetCases(string agentId, CaseManagementParameters parameters, CancellationToken cancellationToken = default);

    Task<EventFilterOptions?> GetFilterOptions(string agentId, CancellationToken cancellationToken = default);
}