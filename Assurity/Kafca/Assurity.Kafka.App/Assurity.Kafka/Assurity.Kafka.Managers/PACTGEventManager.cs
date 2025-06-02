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
    /// The class responsible for handling an updated PACTG record.
    /// </summary>
    public class PACTGEventManager : EventManager, IPACTGEventManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PACTGEventManager"/> class.
        /// </summary>
        /// <param name="eventsAccessor">Accesses MongoDB Events database.</param>
        /// <param name="logger">The logger that will write to the New Relic cloud service.</param>
        /// <param name="policyEngine">The engine used to arrange LifePro policy data.</param>
        /// <param name="hierarchyEngine">The engine used to arrange LifePro policy hierarchies.</param>
        public PACTGEventManager(
            IEventsAccessor eventsAccessor,
            ILogger<PACTGEventManager> logger,
            IConsumerPolicyEngine policyEngine,
            IConsumerHierarchyEngine hierarchyEngine)
            : base(eventsAccessor, logger, policyEngine, hierarchyEngine)
        {
            EventsAccessor = eventsAccessor;
            PolicyEngine = policyEngine;
        }

        private IEventsAccessor EventsAccessor { get; }

        private IConsumerPolicyEngine PolicyEngine { get; }

        /// <summary>
        /// Process the event from the PACTG Event Topic.
        /// Updates all policies that are depending on this PACTG.
        /// </summary>
        /// <param name="pactg">A PACTG record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        [Transaction]
        public async Task ProcessEvent(PACTG pactg)
        {
            var policy = await EventsAccessor.GetPolicyAsync(pactg.POLICY_NUMBER.Trim(), pactg.COMPANY_CODE);

            if (policy == null)
            {
                await GeneratePolicyWithHierarchyAndAgentAccess(pactg.POLICY_NUMBER, pactg.COMPANY_CODE, nameof(PACTGEventManager));

                return;
            }

            await UpdatePolicy(policy, pactg);
        }

        [Trace]
        private async Task<bool> UpdatePolicy(
            Policy policy,
            PACTG pactg)
        {
            var returnPaymentData = PolicyEngine.GetReturnPaymentData(pactg.POLICY_NUMBER, pactg.COMPANY_CODE);
            policy.ReturnPaymentType = returnPaymentData.returnPaymentType;
            policy.ReturnPaymentDate = returnPaymentData.returnPaymentDate;

            var objsDictionary = new Dictionary<string, object>
            {
                { nameof(Policy.ReturnPaymentType), policy.ReturnPaymentType },
                { nameof(Policy.ReturnPaymentDate), policy.ReturnPaymentDate }
            };

            var numUpdate = await EventsAccessor
                .UpdatePolicyAsync(policy, objsDictionary);

            return numUpdate == 1;
        }
    }
}
