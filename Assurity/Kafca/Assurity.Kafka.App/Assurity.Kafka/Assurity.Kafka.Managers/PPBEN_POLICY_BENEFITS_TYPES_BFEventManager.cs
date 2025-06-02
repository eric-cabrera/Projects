namespace Assurity.Kafka.Managers
{
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Hierarchy;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Managers.Interfaces;
    using Assurity.Kafka.Utilities.Constants;
    using Assurity.Kafka.Utilities.Extensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.Extensions.Logging;
    using NewRelic.Api.Agent;

    /// <summary>
    /// Responsible for updating policies associated with the changes in the
    /// updated PPBEN_POLICY_BENEFITS_TYPES_BF record.
    /// </summary>
    public class PPBEN_POLICY_BENEFITS_TYPES_BFEventManager : EventManager, IPPBEN_POLICY_BENEFITS_TYPES_BFEventManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PPBEN_POLICY_BENEFITS_TYPES_BFEventManager"/> class.
        /// </summary>
        /// <param name="eventsAccessor">Accesses MongoDB Events database.</param>
        /// <param name="logger">The logger that will write to the New Relic cloud service.</param>
        /// <param name="policyEngine">The engine used to arrange LifePro policy data.</param>
        /// <param name="hierarchyEngine">The engine used to build or operate on LifePro policy hierarcy data.</param>
        /// <param name="lifeProAccessor">The accessor used to query LifePro policy data.</param>
        public PPBEN_POLICY_BENEFITS_TYPES_BFEventManager(
            IEventsAccessor eventsAccessor,
            ILogger<PPBEN_POLICY_BENEFITS_TYPES_BFEventManager> logger,
            IConsumerPolicyEngine policyEngine,
            IConsumerHierarchyEngine hierarchyEngine,
            ILifeProAccessor lifeProAccessor)
            : base(eventsAccessor, logger, policyEngine, hierarchyEngine)
        {
            EventsAccessor = eventsAccessor;
            Logger = logger;
            LifeProAccessor = lifeProAccessor;
        }

        private ILogger<PPBEN_POLICY_BENEFITS_TYPES_BFEventManager> Logger { get; }

        private IEventsAccessor EventsAccessor { get; }

        private ILifeProAccessor LifeProAccessor { get; }

        /// <summary>
        /// Process the event from the PPBEN_POLICY_BENEFITS_TYPES_BFEvent Topic.
        /// Updates all policies that are depending on this PPBEN_POLICY_BENEFITS_TYPES_BF.
        /// </summary>
        /// <param name="ppbenTypesBF">A PPBEN_POLICY_BENEFITS_TYPES_BF record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        [Transaction]
        public async Task ProcessEvent(PPBEN_POLICY_BENEFITS_TYPES_BF ppbenTypesBF)
        {
            var companyCodeAndPolicyNumber = await LifeProAccessor
                .GetCompanyCodeAndPolicyNumberByPBENID(ppbenTypesBF.PBEN_ID);

            if (companyCodeAndPolicyNumber == null)
            {
                Logger.LogWarning(
                    "Unable to find policy record in LifePro for {pbenIdName} {pbenId} associated with the {eventName} event.",
                    nameof(ppbenTypesBF.PBEN_ID),
                    ppbenTypesBF.PBEN_ID,
                    nameof(PPBEN_POLICY_BENEFITS_TYPES_BF));

                return;
            }

            var policy = await EventsAccessor
                .GetPolicyAsync(
                    companyCodeAndPolicyNumber.PolicyNumber,
                    companyCodeAndPolicyNumber.CompanyCode);

            if (policy == null)
            {
                await GeneratePolicyWithHierarchyAndAgentAccess(
                    companyCodeAndPolicyNumber.PolicyNumber,
                    companyCodeAndPolicyNumber.CompanyCode,
                    nameof(PPBEN_POLICY_BENEFITS_TYPES_BFEventManager));

                return;
            }

            await UpdatePolicy(policy, ppbenTypesBF);
        }

        [Trace]
        private async Task<bool> UpdatePolicy(
            Policy policy,
            PPBEN_POLICY_BENEFITS_TYPES_BF ppbenTypesBF)
        {
            var benefit = policy?.Benefits?
                .SingleOrDefault(benefit => benefit.BenefitId == ppbenTypesBF.PBEN_ID);

            if (benefit == null)
            {
                Logger.LogWarning(
                    "Benefit not found in policy for policy number {policy} and {pfBenIdName} {bfPbenId} for the {eventName} event. No updates will be made.",
                    policy.PolicyNumber,
                    nameof(ppbenTypesBF.PBEN_ID),
                    ppbenTypesBF.PBEN_ID,
                    nameof(PPBEN_POLICY_BENEFITS_TYPES_BF));

                return false;
            }

            benefit.BenefitAmount = ppbenTypesBF.BF_CURRENT_DB;
            benefit.DeathBenefitOption = ppbenTypesBF.BF_DB_OPTION.ToDeathBenefitOption();

            var index = policy.Benefits.FindIndex(benefit => benefit.BenefitId == ppbenTypesBF.PBEN_ID);
            policy.Benefits[index] = benefit;

            var benefitDictionary = new Dictionary<string, object>
            {
                { BenefitProperties.BenefitAmount, policy.Benefits[index].BenefitAmount },
                { BenefitProperties.DeathBenefitOption, policy.Benefits[index].DeathBenefitOption }
            };

            var numUpdate = await EventsAccessor.UpdatePolicyBenefitsAsync(policy, benefitDictionary, benefit.BenefitId);

            policy.PastDue = ppbenTypesBF.BF_DATE_NEGATIVE > 0;

            numUpdate = await EventsAccessor.UpdatePolicyAsync(policy, policy.PastDue, nameof(policy.PastDue));
            return numUpdate == 1;
        }
    }
}