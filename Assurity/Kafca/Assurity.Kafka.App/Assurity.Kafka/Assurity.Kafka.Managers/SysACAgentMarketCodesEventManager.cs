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

    public class SysACAgentMarketCodesEventManager : ISysACAgentMarketCodesEventManager
    {
        public SysACAgentMarketCodesEventManager(
            IGlobalDataAccessor globalDataAccessor,
            IConsumerHierarchyEngine hierarchyEngine,
            ILogger<SysACAgentMarketCodesEventManager> logger,
            IEventsAccessor eventsAccessor,
            IConsumerPolicyEngine policyEngine)
        {
            GlobalDataAccessor = globalDataAccessor;
            HierarchyEngine = hierarchyEngine;
            EventsAccessor = eventsAccessor;
            Logger = logger;
            PolicyEngine = policyEngine;
        }

        private IGlobalDataAccessor GlobalDataAccessor { get; }

        private IConsumerHierarchyEngine HierarchyEngine { get; }

        private ILogger<SysACAgentMarketCodesEventManager> Logger { get; }

        private IEventsAccessor EventsAccessor { get; }

        private IConsumerPolicyEngine PolicyEngine { get; }

        [Transaction]
        public async Task ProcessEvent(SysACAgentMarketCodes sysACAgentMarketCodes)
        {
            var policyNumber = await GlobalDataAccessor
                .GetPolicyNumber(sysACAgentMarketCodes.FOLDERID);

            if (string.IsNullOrWhiteSpace(policyNumber))
            {
                Logger.LogWarning("No policy numbers were found in Global associated with Folder Id: {folderId}", sysACAgentMarketCodes.FOLDERID);

                return;
            }

            var policy = await EventsAccessor
                .GetPolicyAsync(policyNumber);

            if (policy == null)
            {
                Logger.LogWarning("No policy was found in MongoDB with PolicyNumber: {policyNumber}", policyNumber);

                return;
            }

            if (policy.ApplicationDate != null)
            {
                await RetrieveAndUpdatePolicyHierarchyAndAgentPolicyAccess(policy);
            }
            else
            {
                Logger.LogError("ApplicationDate is null for the PolicyNumber: {policyNumber}", policyNumber);
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
                            Logger.LogError("Processing event for SysACAgentMarketCode PolicyNumber: Failed to remove {PolicyNumber} from AgentPolicyAccess for {Agent}", policyNumber, agent);
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
                            Logger.LogError("Processing event for SysACAgentMarketCode PolicyNumber: Failed to add {PolicyNumber} to AgentPolicyAccess for {Agent}", policyNumber, agent);
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