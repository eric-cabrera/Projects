namespace Assurity.Kafka.Engines.Hierarchy
{
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.PolicyInfo.Contracts.V1;

    /// <summary>
    /// The class responsible for managing data to create policy hierarchies.
    /// </summary
    public interface IHierarchyEngine
    {
        /// <summary>
        /// Builds and returns a policy hierarchy object for a given policy.
        /// </summary>
        /// <param name="policy">A policy object.</param>
        /// <returns>A policy hierarchy object. </returns>
        Task<PolicyHierarchy> GetPolicyHierarchy(Policy policy);

        Task<List<AgentHierarchy>> BuildAgentHierarchy(List<Agent> agents, DateTime applicationDate);

        /// <summary>
        /// Returns a distinct list of agents for a given list of agents.
        /// </summary>
        /// <param name="hierarchy">A list of AgentHierarchy objects.</param>
        /// <returns>A hashset containing distinct agent Ids. </returns>
        HashSet<string> GetDistinctAgentIds(List<AgentHierarchy> hierarchy);

        AgentHierarchiesDTO CompareAgentHierarchies(string companyCode, string policyNum, List<AgentHierarchy>? oldAgentHierarchies, List<AgentHierarchy> newAgentHierarchies);

        List<string> RetrieveAgentIds(List<AgentHierarchy> agentHierarchyList);
    }
}
