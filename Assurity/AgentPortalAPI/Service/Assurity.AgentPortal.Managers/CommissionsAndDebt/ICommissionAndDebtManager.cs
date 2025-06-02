namespace Assurity.AgentPortal.Managers.CommissionsAndDebt;

using Assurity.AgentPortal.Contracts.CommissionsDebt;
using Assurity.AgentPortal.Contracts.CommissionsDebt.Request;
using Assurity.AgentPortal.Contracts.Enums;
using Assurity.AgentPortal.Contracts.Shared;

public interface ICommissionAndDebtManager
{
    Task<CommissionResponse?> GetCommissionAndSummaryData(
        string agentId,
        PolicyDetailsParameters parameters,
        CancellationToken cancellationToken = default);

    Task<FileResponse?> GetPolicyDetailsExcel(
        string agentId,
        PolicyDetailsParameters parameters,
        CancellationToken cancellationToken = default);

    Task<WritingAgentDetailsResponse?> GetCommissionDataByWritingAgent(
        string agentId,
        WritingAgentParameters parameters,
        CancellationToken cancellationToken = default);

    Task<FileResponse?> GetWritingAgentDetailsExcel(
        string agentId,
        WritingAgentParameters parameters,
        CancellationToken cancellationToken);

    Task<DebtResponse> GetUnsecuredAdvances(
        string agentId,
        UnsecuredAdvanceParameters parameters,
        CancellationToken cancellationToken = default);

    Task<FileResponse?> GetUnsecuredAdvancesExcel(
        string agentId,
        UnsecuredAdvanceParameters parameters,
        CancellationToken cancellationToken = default);

    Task<DebtResponse> GetSecuredAdvances(
        string agentId,
        SecuredAdvanceParameters parameters,
        CancellationToken cancellationToken = default);

    Task<FileResponse?> GetSecuredAdvancesExcel(
        string agentId,
        SecuredAdvanceParameters parameters,
        CancellationToken cancellationToken = default);

    Task<Stream?> GetAgentStatement(
        string agentId,
        string sessionId,
        string requestAgentId,
        DateTime cycleDate,
        AgentStatementType agentStatementType,
        CancellationToken cancellationToken = default);

    Task<AgentStatementOptions?> GetAgentStatementOptions(string agentId, CancellationToken cancellationToken);
}
