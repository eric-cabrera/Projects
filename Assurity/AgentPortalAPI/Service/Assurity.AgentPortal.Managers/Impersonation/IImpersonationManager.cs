namespace Assurity.AgentPortal.Managers.Impersonation;

using Assurity.AgentPortal.Contracts.Impersonation;

public interface IImpersonationManager
{
    Task<List<ImpersonationRecord>> SearchAgents(string searchTerm, CancellationToken cancellationToken);

    Task<ImpersonationRecord> ImpersonateAgent(string homeOfficeId, string homeOfficeEmail, string impersonationId);

    Task<List<ImpersonationRecord>> GetRecentImpersonations(string homeOfficeId, CancellationToken cancellationToken);
}
