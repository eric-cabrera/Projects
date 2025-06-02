namespace Assurity.AgentPortal.Accessors.CommissionsAndDebt;

using Assurity.AgentPortal.Accessors.DTOs;
using Assurity.Commissions.Debt.Contracts.Advances;

public interface IDebtApiAccessor
{
    Task<AgentDetailsResponse?> GetUnsecuredAdvances(
        string agentId,
        DebtParameters parameters,
        CancellationToken cancellationToken = default);

    Task<List<Agent>> GetAllUnsecuredAdvances(
        string agentId,
        DebtParameters parameters,
        CancellationToken cancellationToken = default);

    Task<AgentDetailsResponse?> GetSecuredAdvances(
        string agentId,
        DebtParameters parameters,
        CancellationToken cancellationToken = default);

    Task<List<Agent>> GetAllSecuredAdvances(
        string agentId,
        DebtParameters parameters,
        CancellationToken cancellationToken = default);
}
