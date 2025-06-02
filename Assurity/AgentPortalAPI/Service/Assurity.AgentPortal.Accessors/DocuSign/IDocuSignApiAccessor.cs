namespace Assurity.AgentPortal.Accessors.ApplicationTracker;

using Assurity.AgentPortal.Contracts.CaseManagement;
using Assurity.ApplicationTracker.Contracts;

public interface IDocuSignApiAccessor
{
    Task<bool> ResendEmail(string agentId, CancellationToken cancellationToken = default);
}