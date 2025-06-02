namespace Assurity.Kafka.Managers
{
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Hierarchy;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Managers.Interfaces;
    using Assurity.Kafka.Utilities.Constants;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.Extensions.Logging;
    using NewRelic.Api.Agent;

    /// <summary>
    /// Responsible for updating policies associated with the changes in the
    /// updated PPBEN_POLICY_BENEFITS_TYPES_SU record.
    /// </summary>
    public class PPBEN_POLICY_BENEFITS_TYPES_SUEventManager : EventManager, IPPBEN_POLICY_BENEFITS_TYPES_SUEventManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PPBEN_POLICY_BENEFITS_TYPES_SUEventManager"/> class.
        /// </summary>
        /// <param name="eventsAccessor">Accesses MongoDB Events database.</param>
        /// <param name="logger">The logger that will write to the New Relic cloud service.</param>
        /// <param name="policyEngine">The engine used to arrange LifePro policy data.</param>
        /// <param name="lifeProAccessor">The accessor used to query LifePro policy data.</param>
        /// <param name="hierarchyEngine">The engine used to arrange LifePro policy hierarchies.</param>
        public PPBEN_POLICY_BENEFITS_TYPES_SUEventManager(
            IEventsAccessor eventsAccessor,
            ILogger<PPBEN_POLICY_BENEFITS_TYPES_SUEventManager> logger,
            IConsumerPolicyEngine policyEngine,
            ILifeProAccessor lifeProAccessor,
            IConsumerHierarchyEngine hierarchyEngine)
            : base(eventsAccessor, logger, policyEngine, hierarchyEngine)
        {
            EventsAccessor = eventsAccessor;
            Logger = logger;
            LifeProAccessor = lifeProAccessor;
        }

        private ILogger<PPBEN_POLICY_BENEFITS_TYPES_SUEventManager> Logger { get; }

        private IEventsAccessor EventsAccessor { get; }

        private ILifeProAccessor LifeProAccessor { get; }

        /// <summary>
        /// Process the event from the PPBEN_POLICY_BENEFITS_TYPES_SUEvent Topic.
        /// Updates all policies that are depending on this PPBEN_POLICY_BENEFITS_TYPES_SU.
        /// </summary>
        /// <param name="ppbenTypesSU">A PPBEN_POLICY_BENEFITS_TYPES_SU record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        [Transaction]
        public async Task ProcessEvent(PPBEN_POLICY_BENEFITS_TYPES_SU ppbenTypesSU)
        {
            var companyCodeAndPolicyNumber = await LifeProAccessor
                .GetCompanyCodeAndPolicyNumberByPBENID(ppbenTypesSU.PBEN_ID);

            if (companyCodeAndPolicyNumber == null)
            {
                Logger.LogWarning(
                    "Unable to find policy record in LifePro for {pbenIdName} {benId} associated with the {eventName} event.",
                    nameof(ppbenTypesSU.PBEN_ID),
                    ppbenTypesSU.PBEN_ID,
                    nameof(PPBEN_POLICY_BENEFITS_TYPES_SU));

                return;
            }

            var policy = await EventsAccessor
                .GetPolicyAsync(
                    companyCodeAndPolicyNumber.PolicyNumber,
                    companyCodeAndPolicyNumber.CompanyCode);

            if (policy == null)
            {
                Logger.LogWarning(
                    "Policy not found in Mongo for policy number '{policyNumber}' for the {eventName} event.",
                    companyCodeAndPolicyNumber.PolicyNumber,
                    nameof(PPBEN_POLICY_BENEFITS_TYPES_SU));

                await GeneratePolicyWithHierarchyAndAgentAccess(
                    companyCodeAndPolicyNumber.PolicyNumber,
                    companyCodeAndPolicyNumber.CompanyCode,
                    nameof(PPBEN_POLICY_BENEFITS_TYPES_SUEventManager));
                return;
            }

            await UpdatePolicy(policy, ppbenTypesSU);
        }

        [Trace]
        private async Task<bool> UpdatePolicy(
            Policy policy,
            PPBEN_POLICY_BENEFITS_TYPES_SU ppbenTypesSU)
        {
            var benefit = policy?.Benefits?
                .SingleOrDefault(benefit => benefit.BenefitId == ppbenTypesSU.PBEN_ID);

            if (benefit == null)
            {
                Logger.LogWarning(
                    "Benefit not found in Mongo policy for policy number '{policy.PolicyNumber}' and {pbenIdName} {pbenId} for the {eventName} event. No updates will be made.",
                    policy.PolicyNumber,
                    nameof(ppbenTypesSU.PBEN_ID),
                    ppbenTypesSU.PBEN_ID,
                    nameof(PPBEN_POLICY_BENEFITS_TYPES_SU));

                return false;
            }

            benefit.BenefitAmount = ppbenTypesSU.VALUE_PER_UNIT * ppbenTypesSU.NUMBER_OF_UNITS;

            var index = policy.Benefits.FindIndex(benefit => benefit.BenefitId == ppbenTypesSU.PBEN_ID);
            policy.Benefits[index] = benefit;

            var benefitDictionary = new Dictionary<string, object>
            {
                { BenefitProperties.BenefitAmount, policy.Benefits[index].BenefitAmount }
            };

            var numUpdate = await EventsAccessor.UpdatePolicyBenefitsAsync(policy, benefitDictionary, benefit.BenefitId);

            return numUpdate == 1;
        }
    }
}