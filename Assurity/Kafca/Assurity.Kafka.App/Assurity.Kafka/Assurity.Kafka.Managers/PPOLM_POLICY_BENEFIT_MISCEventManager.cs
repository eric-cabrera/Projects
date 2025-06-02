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
    /// The class responsible for managing the handling an
    /// updated PPOLM_POLICY_BENEFIT_MISC record.
    /// </summary>
    public class PPOLM_POLICY_BENEFIT_MISCEventManager : EventManager, IPPOLM_POLICY_BENEFIT_MISCEventManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PPOLM_POLICY_BENEFIT_MISCEventManager"/> class.
        /// </summary>
        /// <param name="eventsAccessor">Accesses MongoDB Events database.</param>
        /// <param name="logger">The logger that will write to the New Relic cloud service.</param>
        /// <param name="policyEngine">The engine used to arrange LifePro policy data.</param>
        /// <param name="hierarchyEngine">The engine used to arrange LifePro policy hierarchies.</param>
        public PPOLM_POLICY_BENEFIT_MISCEventManager(
            IEventsAccessor eventsAccessor,
            ILogger<PPOLM_POLICY_BENEFIT_MISCEventManager> logger,
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
        /// Process the event from the PPOLM_POLICY_BENEFIT_MISCEvent Topic.
        /// Updates all policies that are depending on this PPOLM_POLICY_BENEFIT_MISC.
        /// </summary>
        /// <param name="ppolm">A PPOLM_POLICY_BENEFIT_MISC record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        [Transaction]
        public async Task ProcessEvent(PPOLM_POLICY_BENEFIT_MISC ppolm)
        {
            // SEQ value should be 12 for the PPOLM_POLICY_BENEFIT_MISC record with CANCEL_REASON changes.
            if (ppolm.SEQ != 12)
            {
                return;
            }

            var policy = await EventsAccessor
                .GetPolicyAsync(ppolm.POLICY_NUMBER.Trim(), ppolm.COMPANY_CODE);
            if (policy == null)
            {
                await GeneratePolicyWithHierarchyAndAgentAccess(ppolm.POLICY_NUMBER, ppolm.COMPANY_CODE, nameof(PPOLM_POLICY_BENEFIT_MISCEventManager));

                return;
            }

            await UpdatePolicy(policy, ppolm);
        }

        [Trace]
        private async Task<bool> UpdatePolicy(
            Policy policy,
            PPOLM_POLICY_BENEFIT_MISC ppolm)
        {
            var returnPaymentData = PolicyEngine.GetReturnPaymentData(ppolm.POLICY_NUMBER, ppolm.COMPANY_CODE);
            policy.ReturnPaymentType = returnPaymentData.returnPaymentType;
            policy.ReturnPaymentDate = returnPaymentData.returnPaymentDate;

            var retrievedPolicyStatusDetail = await PolicyEngine
                .GetPolicyStatusDetail(ppolm.POLICY_NUMBER, ppolm.COMPANY_CODE);

            policy.PolicyStatusDetail = retrievedPolicyStatusDetail;

            var objsDictionary = new Dictionary<string, object>
            {
                { nameof(Policy.ReturnPaymentType), policy.ReturnPaymentType },
                { nameof(Policy.ReturnPaymentDate), policy.ReturnPaymentDate },
                { nameof(Policy.PolicyStatusDetail), policy.PolicyStatusDetail }
            };

            var numUpdate = await EventsAccessor
                .UpdatePolicyAsync(policy, objsDictionary);

            return numUpdate == 1;
        }
    }
}