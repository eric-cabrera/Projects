namespace Assurity.AgentPortal.Accessors.CommissionsAndDebt;

using Assurity.AgentPortal.Accessors.DTOs;
using Assurity.AgentPortal.Contracts.Enums;
using Assurity.Commissions.Internal.Contracts.AgentStatementOptions;
using Assurity.Commissions.Internal.Contracts.Cycle;
using Assurity.Commissions.Internal.Contracts.PolicyDetails;
using Assurity.Commissions.Internal.Contracts.Summary;
using Assurity.Commissions.Internal.Contracts.WritingAgent;

public interface ICommissionsApiAccessor
{
    /// <summary>
    /// Returns Commissions Cycle data from the Internal Commissions API.
    /// </summary>
    /// <param name="agentIds">Agent Ids to be passed to the Commissions API.</param>
    /// <param name="cycleBeginDate">Cycle begin date to filter results.</param>
    /// <param name="cycleEndDate">Optional paramter to filter by a date range.</param>
    /// <param name="writingAgents">Optional list of writing agents to further filter the results.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CycleCommissionsResponse?> GetCommissionsCycle(
        List<string> agentIds,
        DateTimeOffset? cycleBeginDate,
        DateTimeOffset? cycleEndDate,
        List<string>? writingAgents,
        CancellationToken cancellationToken);

    /// <summary>
    /// Returns policy details commission data.
    /// </summary>
    /// <param name="filteredAgentIds">Agent Ids to be passed to the Commissions API.</param>
    /// <param name="policyDetailsParameters">Additional parameters to filter and sort results.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PolicyDetailsResponse?> GetPolicyDetails(
        List<string> filteredAgentIds,
        CommissionParameters policyDetailsParameters,
        CancellationToken cancellationToken);

    /// <summary>
    /// Returns Commissions Summary data for a collection of agents.
    /// </summary>
    /// <param name="agentIds">List of agent Ids to filter the results.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<SummaryCommissionsResponse?> GetCommissionsSummary(List<string> agentIds, CancellationToken cancellationToken);

    /// <summary>
    /// Returns Commission Data grouped by Writing Agent.
    /// </summary>
    /// <param name="agentIds">List of agent ids to filter the results.</param>
    /// <param name="parameters">Additional parameters to filter and sort results.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<WritingAgentsResponse?> GetWritingAgentDetails(
        List<string> agentIds,
        CommissionParameters parameters,
        CancellationToken cancellationToken);

    /// <summary>
    /// Returns a file stream of the requested Commissions report.
    /// </summary>
    /// <param name="agentId">The Agent Id to run the report on.</param>
    /// <param name="cycleDate">The "as of" date for the report.</param>
    /// <param name="statementType">The type of report to run.</param>
    /// <param name="sessionId">The unique session id.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Stream?> GetAgentStatement(
        string agentId,
        DateTime cycleDate,
        AgentStatementType statementType,
        string sessionId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Returns a list of agents and cycle dates to be used for agent statement reports.  Format of cycle date is yyyyMMdd.
    /// </summary>
    /// <param name="agentIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AgentStatementOptionsResponse?> GetAgentStatementOptions(
        List<string> agentIds,
        CancellationToken cancellationToken);
}
