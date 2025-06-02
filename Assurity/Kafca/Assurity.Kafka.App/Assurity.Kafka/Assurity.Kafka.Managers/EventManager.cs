namespace Assurity.Kafka.Managers
{
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Engines.Hierarchy;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Utilities.Enums;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.Extensions.Logging;
    using NewRelic.Api.Agent;

    public abstract class EventManager
    {
        public EventManager(
           IEventsAccessor eventsAccessor,
           ILogger logger,
           IConsumerPolicyEngine policyEngine,
           IConsumerHierarchyEngine hierarchyEngine)
        {
            EventsAccessor = eventsAccessor;
            Logger = logger;
            HierarchyEngine = hierarchyEngine;
            PolicyEngine = policyEngine;
        }

        private IEventsAccessor EventsAccessor { get; }

        private ILogger Logger { get; }

        private IConsumerPolicyEngine PolicyEngine { get; }

        private IConsumerHierarchyEngine HierarchyEngine { get; }

        /// <summary>
        /// Create a Policy, then PolicyHierarchy, and update/create AgentPolicyAccess records.
        /// </summary>
        /// <remarks>
        /// This method can handle new policy creation as well as existing policy updates.
        /// </remarks>
        /// <param name="policyNumber"></param>
        /// <param name="companyCode"></param>
        /// <param name="eventManagerName"></param>
        /// <returns></returns>
        [Transaction]
        public async Task<bool> GeneratePolicyWithHierarchyAndAgentAccess(string policyNumber, string companyCode, string eventManagerName)
        {
            Logger.LogInformation(
                "Starting {methodName} for Policy Number and Company Code {policyNumber} - {companyCode}.",
                nameof(GeneratePolicyWithHierarchyAndAgentAccess),
                policyNumber,
                companyCode);

            (var result, var lifeproPolicy) = await PolicyEngine.GetPolicy(policyNumber, companyCode);

            if (result == GetPolicyResult.ExceptionThrown)
            {
                Logger.LogError(
                    "An exception was thrown when attempting to get the policy {PolicyNumber}. "
                    + "The attempt to create policy and access has been aborted for the '{EventManagerName}' event.",
                    policyNumber,
                    eventManagerName);
                return false;
            }

            if (result == GetPolicyResult.NotFound)
            {
                Logger.LogWarning(
                    "Policy Number: {PolicyNumber} for the '{EventManagerName}' event -- failed to get or generate policy from Lifepro. "
                    + "The policy was not found in the database (due to rules of policies we care about or it does not exist).",
                    policyNumber,
                    eventManagerName);
                return false;
            }

            if (result == GetPolicyResult.HasInitialPaymentDeclinedThatIsBeyondRetentionDuration)
            {
                Logger.LogWarning(
                    "Policy Number: {PolicyNumber} for the '{EventManagerName}' event -- Failed to get or generate policy from Lifepro. "
                    + "The initial payment declined is beyond retention duration.",
                    policyNumber,
                    eventManagerName);
                return false;
            }

            if (lifeproPolicy == null)
            {
                Logger.LogError(
                    "Policy Number: '{PolicyNumber}' has not been created. Failed to generate policy from Lifepro.",
                    policyNumber);
                return false;
            }

            try
            {
                await EventsAccessor.CreateOrReplacePolicyAsync(lifeproPolicy);
            }
            catch
            {
                Logger.LogError(
                    "Policy '{PolicyNumber}' failed to be stored by the '{EventManagerName}' event manager.",
                    policyNumber,
                    eventManagerName);
                return false;
            }

            Logger.LogInformation(
                "Policy '{PolicyNumber}' was stored by the '{EventManagerName}' event manager.",
                policyNumber,
                eventManagerName);

            if (result == GetPolicyResult.Found && lifeproPolicy.ApplicationDate != null)
            {
                var policyHierarchy = await HierarchyEngine.GetPolicyHierarchy(lifeproPolicy);

                if (policyHierarchy == null)
                {
                    Logger.LogError(
                        "Hierarchy for policy: '{PolicyNumber}' has not been created. Failed to generate policy hierarchy from Lifepro.",
                        policyNumber);
                    return false;
                }

                try
                {
                    await EventsAccessor.UpdateOrCreatePolicyHierarchyAsync(policyHierarchy);
                }
                catch
                {
                    Logger.LogError(
                        "Hierarchy for policy '{PolicyNumber}' failed to be stored by the '{EventManagerName}' event manager.",
                        policyNumber,
                        eventManagerName);
                    return false;
                }

                Logger.LogInformation(
                    "Hierarchy for policy '{PolicyNumber}' was stored by the '{EventManagerName}' event manager.",
                    policyNumber,
                    eventManagerName);

                var distinctAgents = HierarchyEngine.GetDistinctAgentIds(policyHierarchy.HierarchyBranches);

                foreach (var agent in distinctAgents)
                {
                    try
                    {
                        var agentPolicyAccessId = await EventsAccessor.InsertAgentPolicyAccessAsync(agent, lifeproPolicy.PolicyNumber, lifeproPolicy.CompanyCode);
                    }
                    catch
                    {
                        Logger.LogError(
                            "Failure while attempting to add '{PolicyNumber}' to AgentPolicyAccess for agent '{Agent}'. "
                            + "The call originated in the '{EventManagerName}' event manager.",
                            policyNumber,
                            agent,
                            eventManagerName);
                        return false;
                    }
                }

                Logger.LogInformation("Policy access updated for all agents for policy '{PolicyNumber}'.", policyNumber);
            }

            return true;
        }

        [Trace]
        internal async Task<bool> UpdatePolicyRequirements(Policy policy)
        {
            policy.Requirements = await PolicyEngine.GetRequirements(policy);

            var numUpdate = await EventsAccessor
                .UpdatePolicyAsync(policy, policy.Requirements, nameof(Policy.Requirements));

            return numUpdate == 1;
        }
    }
}
