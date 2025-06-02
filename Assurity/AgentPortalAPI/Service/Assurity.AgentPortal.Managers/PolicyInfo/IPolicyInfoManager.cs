namespace Assurity.AgentPortal.Managers.PolicyInfo;

using Assurity.AgentPortal.Contracts.PolicyInfo;
using Assurity.AgentPortal.Contracts.Shared;
using Assurity.PolicyInfo.Contracts.V1.Enums;

public interface IPolicyInfoManager
{
    /// <summary>
    ///     Finds and returns a list of policy summaries by AgentId and a Policy Status.
    /// </summary>
    /// <param name="agentId">Agent to be found.</param>
    /// <param name="status">Specific Policy Status to search in policies.</param>
    /// <param name="queryString">Query string parameters for customizing Policy Information API response.</param>
    /// <returns>A list of policy summaries by assigned AgentId and Policy Status.</returns>
    Task<PolicySummariesResponse?> GetPolicySummaries(
        string agentId,
        Status status,
        string? queryString);

    /// <summary>
    ///     Finds and returns a list of policy summaries by AgentId and a Policy Status in an excel document.
    /// </summary>
    /// <param name="agentId">Agent to be found.</param>
    /// <param name="status">Specific Policy Status to search in policies.</param>
    /// <param name="queryString">Query string parameters for customizing Policy Information API response.</param>
    /// <returns>An excel document containing a list of policy summaries by assigned AgentId and Policy Status.</returns>
    Task<FileResponse?> GetPolicySummariesAsExcelDocument(
        string agentId,
        Status status,
        string? queryString);

    /// <summary>
    ///     Finds policy number and returns the async policy object.
    /// </summary>
    /// <param name="policyNumber">Number of Policy to be found.</param>
    /// <param name="agentId">Agent to be found.</param>
    /// <returns>The policy assigned to the AgentId.</returns>
    Task<PolicyResponse?> GetPolicyInfo(string policyNumber, string agentId);

    /// <summary>
    ///     Finds policy status counts assigned by AgentId and returns an async list of the status and count of instances.
    /// </summary>
    /// <param name="agentId">Agent to be found.</param>
    /// <returns>Data related to agent and agent's downline policies organized by policy status.</returns>
    Task<PolicyStatusCountsResponse?> GetPolicyStatusCounts(string agentId);

    /// <summary>
    ///     Finds and returns a list of requirement summaries by AgentId.
    /// </summary>
    /// <param name="agentId">Agent to be found.</param>
    /// <param name="queryString">Query string parameters for customizing Policy Information API response.</param>
    /// <returns>A list of requirement summaries by assigned AgentId.</returns>
    Task<RequirementSummariesResponse?> GetPendingPolicyRequirements(
        string agentId,
        string? queryString);

    /// <summary>
    ///     Checks if an agent has access to a specific policy number.
    /// </summary>
    /// <param name="agentId">Agent Id.</param>
    /// <param name="policyNumber">Policy number to check access to.</param>
    /// <returns>A boolean indicating if the agent has access to the policy or not.</returns>
    Task<bool> CheckAgentAccessToPolicy(string agentId, string policyNumber);
}