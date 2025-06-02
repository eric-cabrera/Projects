namespace Assurity.AgentPortal.Accessors.PolicyInfo;

using Assurity.PolicyInfo.Contracts.V1;
using Assurity.PolicyInfo.Contracts.V1.Enums;
using PolicyInfoAPI = Assurity.PolicyInformation.Contracts.V1;

public interface IPolicyInfoApiAccessor
{
    /// <summary>
    ///     Finds and returns a policy object based on a Policy Number and Agent Id.
    /// </summary>
    /// <param name="policyNumber">Number of Policy to be found.</param>
    /// <param name="agentId">Agent to be found.</param>
    /// <returns>The policy assigned to the AgentId.</returns>
    Task<Policy?> GetPolicyInfo(string policyNumber, string agentId);

    /// <summary>
    ///     Finds and returns a policy status counts object based on an Agent Id.
    /// </summary>
    /// <param name="agentId">Agent to be found.</param>
    /// <returns>Data related to agent and agent's downline policies organized by policy status.</returns>
    Task<PolicyInfoAPI.PolicyStatusCounts?> GetPolicyStatusCounts(string agentId);

    /// <summary>
    ///     Finds and returns a policy summaries object based on an Agent Id and a specified Policy Status.
    /// </summary>
    /// <param name="agentId">Agent to be found.</param>
    /// <param name="status">Specific Policy Status to search in policies.</param>
    /// <param name="queryString">Query string parameters to customize Policy Information API response.</param>
    /// <returns>A list of policy summaries by assigned AgentId and Policy Status.</returns>
    Task<PolicyInfoAPI.PolicySummariesResponse?> GetPolicySummaries(
        string agentId,
        Status status,
        string? queryString);

    /// <summary>
    ///     Finds and returns a requirement summaries object based on an Agent Id.
    /// </summary>
    /// <param name="agentId">Agent to be found.</param>
    /// <param name="queryString">Query string parameters to customize Policy Information API response.</param>
    /// <returns>A list of requirement summaries by assigned AgentId.</returns>
    Task<PolicyInfoAPI.RequirementSummaryResponse?> GetPendingPolicyRequirementSummaries(string agentId, string? queryString);

    /// <summary>
    ///     Checks if an agent has access to a specific policy number.
    /// </summary>
    /// <param name="agentId">Agent Id.</param>
    /// <param name="policyNumber">Policy number to check access to.</param>
    /// <returns>A boolean indicating if the agent has access to the policy or not.</returns>
    Task<bool> CheckAgentAccessToPolicy(string agentId, string policyNumber);
}