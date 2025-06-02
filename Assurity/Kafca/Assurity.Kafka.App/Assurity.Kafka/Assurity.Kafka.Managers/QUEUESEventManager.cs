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
    /// Responsible for managing an updated AgentUseQueue record.
    /// </summary>
    public class QUEUESEventManager : IQUEUESEventManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QUEUESEventManager"/> class.
        /// </summary>
        /// <param name="globalDataAccessor">The accessor used to query global data.</param>
        /// <param name="supportDataAccessor">The accessor used to query support data.</param>
        /// <param name="eventsAccessor">Accesses MongoDB Events database.</param>
        /// <param name="logger">The logger that will write to the New Relic cloud service.</param>
        /// <param name="policyEngine">The engine used to arrange LifePro policy data.</param>
        /// <param name="hierarchyEngine">The engine used to arrange LifePro policy hierarchies.</param>
        public QUEUESEventManager(
            IGlobalDataAccessor globalDataAccessor,
            ISupportDataAccessor supportDataAccessor,
            IEventsAccessor eventsAccessor,
            ILogger<QUEUESEventManager> logger,
            IConsumerPolicyEngine policyEngine,
            IConsumerHierarchyEngine hierarchyEngine)
        {
            GlobalDataAccessor = globalDataAccessor;
            SupportDataAccessor = supportDataAccessor;
            EventsAccessor = eventsAccessor;
            Logger = logger;
            PolicyEngine = policyEngine;
            HierarchyEngine = hierarchyEngine;
        }

        private ILogger<QUEUESEventManager> Logger { get; }

        private IGlobalDataAccessor GlobalDataAccessor { get; }

        private ISupportDataAccessor SupportDataAccessor { get; }

        private IEventsAccessor EventsAccessor { get; }

        private IConsumerPolicyEngine PolicyEngine { get; }

        private IConsumerHierarchyEngine HierarchyEngine { get; }

        /// <summary>
        /// Process the event from the QUEUESEvent Topic.
        /// Updates all policies that are depending on this QUEUES.
        /// </summary>
        /// <param name="queues">A QUEUES record with updated data.</param>
        /// <param name="beforeQueue"></param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        [Transaction]
        public async Task ProcessEvent(QUEUES queues, string beforeQueue)
        {
            if (queues == null || beforeQueue == queues.QUEUE)
            {
                return;
            }

            var isBeforeQueueJIT = await SupportDataAccessor.IsJustInTimeQueue(beforeQueue);
            var isAfterQueueJIT = await SupportDataAccessor.IsJustInTimeQueue(queues.QUEUE);

            if ((isBeforeQueueJIT && isAfterQueueJIT) || (!isBeforeQueueJIT && !isAfterQueueJIT))
            {
                return;
            }

            var policyNumber = await GlobalDataAccessor
                .GetPolicyNumber(queues.ID);

            if (string.IsNullOrWhiteSpace(policyNumber))
            {
                Logger.LogWarning("No policy numbers were found in Global associated with Folder Id: {queueID}", queues.ID);

                return;
            }

            var policy = await EventsAccessor
                .GetPolicyAsync(policyNumber);

            if (policy == null)
            {
                Logger.LogWarning("Policy not found in Mongo for policy number '{policyNumber}' for the {eventName} event.", policyNumber, nameof(QUEUES));

                return;
            }

            if (policy.ApplicationDate != null)
            {
                await RetrieveAndUpdatePolicyHierarchyAndAgentPolicyAccess(policy);
            }
            else
            {
                Logger.LogError("ApplicationDate is null for the PolicyNumber: {policyNumber}", policy.PolicyNumber);
            }

            await UpdatePolicy(policy);
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
                    results?.AddedAgents,
                    results?.RemovedAgents);
            }
        }

        [Trace]
        private async Task UpdateAgentPolicyAccess(
            string companyCode,
            string policyNumber,
            List<string>? addedAgents,
            List<string>? removedAgents)
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
                            Logger.LogError("Processing event for QUEUES PolicyNumber: Failed to remove {PolicyNumber} from AgentPolicyAccess for {Agent}", policyNumber, agent);
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
                            Logger.LogError("Processing event for QUEUES PolicyNumber: Failed to add {PolicyNumber} to AgentPolicyAccess for {Agent}", policyNumber, agent);
                        }
                    }
                }
            }
        }

        [Trace]
        private async Task<bool> UpdatePolicy(
            Policy policy)
        {
            policy.Agents = await PolicyEngine
                .GetAgents(policy.PolicyNumber, policy.CompanyCode, policy.ApplicationDate.GetValueOrDefault());

            var numUpdate = await EventsAccessor
                .UpdatePolicyAsync(policy, policy.Agents, nameof(Policy.Agents));

            return numUpdate == 1;
        }
    }
}