namespace Assurity.Kafka.Managers
{
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Hierarchy;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Managers.Interfaces;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.Extensions.Logging;
    using NewRelic.Api.Agent;

    /// <summary>
    /// Responsible for managing an updated PCOMC_COMMISSION_CONTROL_TYPE_S record.
    /// </summary>
    public class PCOMC_COMMISSION_CONTROL_TYPE_SEventManager
        : EventManager, IPCOMC_COMMISSION_CONTROL_TYPE_SEventManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PCOMC_COMMISSION_CONTROL_TYPE_SEventManager"/> class.
        /// </summary>
        /// <param name="eventsAccessor">Accesses MongoDB Events database.</param>
        /// <param name="logger">The logger that will write to the New Relic cloud service.</param>
        /// <param name="policyEngine">The engine used to arrange LifePro policy data.</param>
        /// <param name="hierarchyEngine">The engine used to arrange LifePro policy hierarchies.</param>
        /// <param name="lifeProAccessor">The accessor used to query LifePro policy data.</param>
        public PCOMC_COMMISSION_CONTROL_TYPE_SEventManager(
            IEventsAccessor eventsAccessor,
            ILogger<PCOMC_COMMISSION_CONTROL_TYPE_SEventManager> logger,
            IConsumerPolicyEngine policyEngine,
            IConsumerHierarchyEngine hierarchyEngine,
            ILifeProAccessor lifeProAccessor)
            : base(eventsAccessor, logger, policyEngine, hierarchyEngine)
        {
            EventsAccessor = eventsAccessor;
            Logger = logger;
            PolicyEngine = policyEngine;
            HierarchyEngine = hierarchyEngine;
            LifeProAccessor = lifeProAccessor;
        }

        private ILogger<PCOMC_COMMISSION_CONTROL_TYPE_SEventManager> Logger { get; }

        private IEventsAccessor EventsAccessor { get; }

        private IConsumerPolicyEngine PolicyEngine { get; }

        private IConsumerHierarchyEngine HierarchyEngine { get; }

        private ILifeProAccessor LifeProAccessor { get; }

        /// <summary>
        /// Process the event from the PCOMC_COMMISSION_CONTROL_TYPE_SEvent Topic.
        /// Updates all policies that are depending on this PCOMC_COMMISSION_CONTROL_TYPE_S.
        /// </summary>
        /// <param name="pcomcTypeS">A PCOMC_COMMISSION_CONTROL_TYPE_S record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        [Transaction]
        public async Task ProcessEvent(PCOMC_COMMISSION_CONTROL_TYPE_S pcomcTypeS)
        {
            // Get the policy number and company code associated with the
            // event's COMC_ID from LifePro via the PCOMC tables.
            var companyCodeAndPolicyNumber = await LifeProAccessor
                .GetCompanyCodeAndPolicyNumberByCOMCID(pcomcTypeS.COMC_ID);

            if (companyCodeAndPolicyNumber == null)
            {
                Logger.LogWarning(
                    "Unable to find a Company Code and Policy Number " +
                    "associated with the {comcName}: {comcId} " +
                    "for the {eventName} event.",
                    nameof(pcomcTypeS.COMC_ID),
                    pcomcTypeS.COMC_ID,
                    nameof(PCOMC_COMMISSION_CONTROL_TYPE_S));

                return;
            }

            var policy = await EventsAccessor
                .GetPolicyAsync(
                    companyCodeAndPolicyNumber.PolicyNumber.Trim(),
                    companyCodeAndPolicyNumber.CompanyCode);

            if (policy == null)
            {
                await GeneratePolicyWithHierarchyAndAgentAccess(
                    companyCodeAndPolicyNumber.PolicyNumber,
                    companyCodeAndPolicyNumber.CompanyCode,
                    nameof(PCOMC_COMMISSION_CONTROL_TYPE_S));

                return;
            }

            if (policy.ApplicationDate != null)
            {
                await RetrieveAndUpdatePolicyHierarchyAndAgentPolicyAccess(policy);
            }
            else
            {
                Logger.LogError("ApplicationDate is null for the PolicyNumber: {PolicyNumber}", policy.PolicyNumber);
            }

            await UpdatePolicy(policy, companyCodeAndPolicyNumber);
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
        private async Task<bool> UpdatePolicy(
            Policy policy,
            CompanyCodeAndPolicyNumber companyCodeAndPolicyNumber)
        {
            policy.Agents = await PolicyEngine
                .GetAgents(
                    companyCodeAndPolicyNumber.PolicyNumber,
                    companyCodeAndPolicyNumber.CompanyCode,
                    policy.ApplicationDate.GetValueOrDefault());

            var numUpdate = await EventsAccessor
                .UpdatePolicyAsync(policy, policy.Agents, nameof(Policy.Agents));

            return numUpdate == 1;
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
                            Logger.LogError("Processing event for PCOMC_COMMISSION_CONTROL_TYPE_S PolicyNumber: Failed to remove {PolicyNumber} from AgentPolicyAccess for {Agent}", policyNumber, agent);
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
                            Logger.LogError("Processing event for PCOMC_COMMISSION_CONTROL_TYPE_S PolicyNumber: Failed to add {PolicyNumber} to AgentPolicyAccess for {Agent}", policyNumber, agent);
                        }
                    }
                }
            }
        }
    }
}