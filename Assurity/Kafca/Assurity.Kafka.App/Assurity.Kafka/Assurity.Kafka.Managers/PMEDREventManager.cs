namespace Assurity.Kafka.Managers
{
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Managers.Interfaces;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.Extensions.Logging;
    using NewRelic.Api.Agent;

    public class PMEDREventManager : IPMEDREventManager
    {
        public PMEDREventManager(
            ILogger<PMEDREventManager> logger,
            IConsumerPolicyEngine policyEngine,
            IEventsAccessor eventsAccessor)
        {
            Logger = logger;
            PolicyEngine = policyEngine;
            EventsAccessor = eventsAccessor;
        }

        private ILogger<PMEDREventManager> Logger { get; }

        private IEventsAccessor EventsAccessor { get; }

        private IConsumerPolicyEngine PolicyEngine { get; }

        [Transaction]
        public async Task ProcessEvent(PMEDR pmedr)
        {
            var policies = await EventsAccessor.GetPoliciesAsync(pmedr.REQ_NUMBER);

            if (!(policies?.Any() ?? false))
            {
                Logger.LogWarning(
                    "No policies were found in Mongo for the LifePro policies associated with {reqNumberName}: {reqNumber}",
                    nameof(pmedr.REQ_NUMBER),
                    pmedr.REQ_NUMBER);

                return;
            }

            await UpdatePolicies(policies, pmedr.REQ_NUMBER, pmedr.REQ_DESCRIPTION.Trim());
        }

        [Trace]
        private async Task<bool> UpdatePolicies(List<Policy> policies, short reqNumber, string reqDescription)
        {
            PolicyEngine.UpdateRequirementName(policies, reqNumber, reqDescription);

            long numUpdate = 0;
            foreach (var policy in policies)
            {
                numUpdate += await EventsAccessor.UpdatePolicyAsync(
                    policy, policy.Requirements, nameof(Policy.Requirements));
            }

            return numUpdate >= 1;
        }
    }
}