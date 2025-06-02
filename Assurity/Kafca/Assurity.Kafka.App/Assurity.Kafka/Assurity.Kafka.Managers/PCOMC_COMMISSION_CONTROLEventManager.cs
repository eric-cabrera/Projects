namespace Assurity.Kafka.Managers
{
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Hierarchy;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Managers.Interfaces;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.Extensions.Logging;
    using NewRelic.Api.Agent;

    /// <summary>
    /// Responsible for managing an updated PCOMC_COMMISSION_CONTROL record.
    /// </summary>
    public class PCOMC_COMMISSION_CONTROLEventManager : EventManager, IPCOMC_COMMISSION_CONTROLEventManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PCOMC_COMMISSION_CONTROLEventManager"/> class.
        /// </summary>
        /// <param name="eventsAccessor">Accesses MongoDB Events database.</param>
        /// <param name="logger">The logger that will write to the New Relic cloud service.</param>
        /// <param name="policyEngine">The engine used to arrange LifePro policy data.</param>
        /// <param name="hierarchyEngine">The engine used to arrange LifePro policy hierarchies.</param>
        public PCOMC_COMMISSION_CONTROLEventManager(
            IEventsAccessor eventsAccessor,
            ILogger<PCOMC_COMMISSION_CONTROLEventManager> logger,
            IConsumerPolicyEngine policyEngine,
            IConsumerHierarchyEngine hierarchyEngine)
            : base(eventsAccessor, logger, policyEngine, hierarchyEngine)
        {
            EventsAccessor = eventsAccessor;
            Logger = logger;
            PolicyEngine = policyEngine;
            HierarchyEngine = hierarchyEngine;
        }

        private ILogger<PCOMC_COMMISSION_CONTROLEventManager> Logger { get; }

        private IEventsAccessor EventsAccessor { get; }

        private IConsumerPolicyEngine PolicyEngine { get; }

        private IConsumerHierarchyEngine HierarchyEngine { get; }

        /// <summary>
        /// Process the event from the PCOMC_COMMISSION_CONTROLEvent Topic.
        /// Updates all policies that are depending on this PCOMC_COMMISSION_CONTROL.
        /// </summary>
        /// <param name="pcomc">A PCOMC_COMMISSION_CONTROL record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        [Transaction]
        public async Task ProcessEvent(PCOMC_COMMISSION_CONTROL pcomc)
        {
            var policy = await EventsAccessor.GetPolicyAsync(pcomc.POLICY_NUMBER.Trim(), pcomc.COMPANY_CODE);
            if (policy == null)
            {
                await GeneratePolicyWithHierarchyAndAgentAccess(pcomc.POLICY_NUMBER, pcomc.COMPANY_CODE, nameof(PCOMC_COMMISSION_CONTROLEventManager));

                return;
            }

            if (policy.ApplicationDate != null)
            {
                await RetrieveAndUpdatePolicyHierarchyAndAgentPolicyAccess(policy);
            }
            else
            {
                Logger.LogError("ApplicationDate is null for the PolicyNumber: {policy.PolicyNumber}", policy.PolicyNumber);
            }

            await UpdatePolicy(policy, pcomc);
        }

        [Trace]
        private async Task RetrieveAndUpdatePolicyHierarchyAndAgentPolicyAccess(
            Policy policy)
        {
            var policyHierarchy = await EventsAccessor
              .GetPolicyHierarchyAsync(policy.PolicyNumber, policy.CompanyCode);

            var newAgents = await PolicyEngine
                .GetAgents(policy.PolicyNumber, policy.CompanyCode, policy.ApplicationDate.GetValueOrDefault());

            var newAgentHierarchies = await HierarchyEngine
                .BuildAgentHierarchy(newAgents, policy.ApplicationDate.GetValueOrDefault());

            var results = HierarchyEngine.CompareAgentHierarchies(
                policy.CompanyCode,
                policy.PolicyNumber,
                policyHierarchy?.HierarchyBranches,
                newAgentHierarchies);

            if ((results?.AddedAgents?.Count ?? 0) > 0 || (results?.RemovedAgents?.Count ?? 0) > 0)
            {
                await EventsAccessor.UpdatePolicyHierarchyAsync(policy.PolicyNumber, policy.CompanyCode, newAgentHierarchies);

                await UpdateAgentPolicyAccess(
                    policy.CompanyCode,
                    policy.PolicyNumber,
                    results.AddedAgents,
                    results.RemovedAgents);
            }
        }

        [Trace]
        private async Task UpdateAgentPolicyAccess(
            string companyCode,
            string policyNumber,
            List<string> addedAgents,
            List<string> removedAgents)
        {
            if (removedAgents != null)
            {
                foreach (var agent in removedAgents)
                {
                    if (removedAgents.Contains(agent))
                    {
                        var agentPolicyAccessId = await EventsAccessor.RemoveAgentPolicyAccessAsync(agent, policyNumber, companyCode);

                        if (agentPolicyAccessId == 0)
                        {
                            Logger.LogError("Processing event for PCOMC_COMMISSION_CONTROL PolicyNumber: Failed to remove {PolicyNumber} from AgentPolicyAccess for {Agent}", policyNumber, agent);
                        }
                    }
                }
            }

            if (addedAgents != null)
            {
                foreach (var agent in addedAgents)
                {
                    if (addedAgents.Contains(agent))
                    {
                        var agentPolicyAccessId = await EventsAccessor.InsertAgentPolicyAccessAsync(agent, policyNumber, companyCode);

                        if (agentPolicyAccessId == 0)
                        {
                            Logger.LogError("Processing event for PCOMC_COMMISSION_CONTROL PolicyNumber: Failed to add {PolicyNumber} to AgentPolicyAccess for {Agent}", policyNumber, agent);
                        }
                    }
                }
            }
        }

        [Trace]
        private async Task<bool> UpdatePolicy(
            Policy policy,
            PCOMC_COMMISSION_CONTROL pcomc)
        {
            policy.Agents = await PolicyEngine
                .GetAgents(pcomc.POLICY_NUMBER, pcomc.COMPANY_CODE, policy.ApplicationDate.GetValueOrDefault());

            var numUpdate = await EventsAccessor
                .UpdatePolicyAsync(policy, policy.Agents, nameof(Policy.Agents));

            return numUpdate == 1;
        }
    }
}