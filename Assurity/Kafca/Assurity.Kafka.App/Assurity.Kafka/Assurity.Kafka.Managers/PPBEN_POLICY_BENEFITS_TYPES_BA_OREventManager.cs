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
    /// updated PPBEN_POLICY_BENEFITS_TYPES_BA_OR record.
    /// </summary>
    public class PPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager : EventManager, IPPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager"/> class.
        /// </summary>
        /// <param name="eventsAccessor">Accesses MongoDB Events database.</param>
        /// <param name="logger">The logger that will write to the New Relic cloud service.</param>
        /// <param name="policyEngine">The engine used to arrange LifePro policy data.</param>
        /// <param name="lifeProAccessor">The accessor used to query LifePro policy data.</param>
        /// <param name="hierarchyEngine">The engine used to arrange LifePro policy hierarchies.</param>
        public PPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager(
            IEventsAccessor eventsAccessor,
            ILogger<PPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager> logger,
            IConsumerPolicyEngine policyEngine,
            ILifeProAccessor lifeProAccessor,
            IConsumerHierarchyEngine hierarchyEngine)
            : base(eventsAccessor, logger, policyEngine, hierarchyEngine)
        {
            EventsAccessor = eventsAccessor;
            Logger = logger;
            LifeProAccessor = lifeProAccessor;
        }

        private ILogger<PPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager> Logger { get; }

        private IEventsAccessor EventsAccessor { get; }

        private ILifeProAccessor LifeProAccessor { get; }

        /// <summary>
        /// Process the event from the PPBEN_POLICY_BENEFITS_TYPES_BA_OREvent Topic.
        /// Updates all policies that are depending on this PPBEN_POLICY_BENEFITS_TYPES_BA_OR.
        /// </summary>
        /// <param name="ppbenTypesBAOR">A PPBEN_POLICY_BENEFITS_TYPES_BA_OR record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        [Transaction]
        public async Task ProcessEvent(PPBEN_POLICY_BENEFITS_TYPES_BA_OR ppbenTypesBAOR)
        {
            Logger.LogInformation("Processing event for PBEN_ID: {PBEN_ID}", ppbenTypesBAOR.PBEN_ID);

            var companyCodeAndPolicyNumber = await LifeProAccessor
                .GetCompanyCodeAndPolicyNumberByPBENID(ppbenTypesBAOR.PBEN_ID);

            if (companyCodeAndPolicyNumber == null)
            {
                Logger.LogWarning(
                    "Unable to find policy record in LifePro for {baorName} {ppbenTypesBAOR.PBEN_ID} associated with the {eventName} event.",
                    nameof(ppbenTypesBAOR.PBEN_ID),
                    ppbenTypesBAOR.PBEN_ID,
                    nameof(PPBEN_POLICY_BENEFITS_TYPES_BA_OR));
                return;
            }

            Logger.LogInformation(
                "Getting policy {PolicyNumber} for PBEN_ID: {PBEN_ID}.",
                companyCodeAndPolicyNumber.PolicyNumber,
                ppbenTypesBAOR.PBEN_ID);

            var policy = await EventsAccessor
                .GetPolicyAsync(
                    companyCodeAndPolicyNumber.PolicyNumber,
                    companyCodeAndPolicyNumber.CompanyCode);

            if (policy == null)
            {
                Logger.LogInformation(
                    "Creating policy {PolicyNumber} for PBEN_ID: {PBEN_ID}.",
                    companyCodeAndPolicyNumber.PolicyNumber,
                    ppbenTypesBAOR.PBEN_ID);

                var success = await GeneratePolicyWithHierarchyAndAgentAccess(companyCodeAndPolicyNumber.PolicyNumber, companyCodeAndPolicyNumber.CompanyCode, nameof(PPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager));

                Logger.LogInformation(
                    "Creating policy {PolicyNumber} for PBEN_ID: {PBEN_ID} was {SuccessResult}.",
                    companyCodeAndPolicyNumber.PolicyNumber,
                    ppbenTypesBAOR.PBEN_ID,
                    success ? "successful" : "not successful");

                return;
            }

            Logger.LogInformation(
                "Updating policy {PolicyNumber} for PBEN_ID: {PBEN_ID}.",
                companyCodeAndPolicyNumber.PolicyNumber,
                ppbenTypesBAOR.PBEN_ID);

            var updateSuccess = await UpdatePolicy(policy, ppbenTypesBAOR);
            Logger.LogInformation(
                "Updating policy {PolicyNumber} for PBEN_ID: {PBEN_ID} was {SuccessResult}.",
                companyCodeAndPolicyNumber.PolicyNumber,
                ppbenTypesBAOR.PBEN_ID,
                updateSuccess ? "successful" : "not successful");
        }

        [Trace]
        private async Task<bool> UpdatePolicy(
            Policy policy,
            PPBEN_POLICY_BENEFITS_TYPES_BA_OR ppbenTypesBAOR)
        {
            Logger.LogInformation(
                "Updating policy {PolicyNumber} for PBEN_ID: {PBEN_ID}",
                policy.PolicyNumber,
                ppbenTypesBAOR.PBEN_ID);

            var benefit = policy?.Benefits?
                .SingleOrDefault(benefit => benefit.BenefitId == ppbenTypesBAOR.PBEN_ID);

            if (benefit == null)
            {
                Logger.LogWarning(
                    "Benefit not found in Mongo policy for policy number '{PolicyNumber}' and {pbenIdName} {pbenId} for the {eventName} event. No updates will be made.",
                    policy.PolicyNumber,
                    nameof(ppbenTypesBAOR.PBEN_ID),
                    ppbenTypesBAOR.PBEN_ID,
                    nameof(PPBEN_POLICY_BENEFITS_TYPES_BA_OR));

                return false;
            }

            Logger.LogInformation(
                "Benefit {PBEN_ID} found for policy {PolicyNumber}.",
                ppbenTypesBAOR.PBEN_ID,
                policy.PolicyNumber);

            benefit.BenefitAmount = ppbenTypesBAOR.VALUE_PER_UNIT * ppbenTypesBAOR.NUMBER_OF_UNITS;
            benefit.DividendOption = ppbenTypesBAOR.DIVIDEND.ToDividendOption();

            var index = policy.Benefits.FindIndex(benefit => benefit.BenefitId == ppbenTypesBAOR.PBEN_ID);
            policy.Benefits[index] = benefit;

            var benefitDictionary = new Dictionary<string, object>
            {
                { BenefitProperties.BenefitAmount, policy.Benefits[index].BenefitAmount },
                { BenefitProperties.DividendOption, policy.Benefits[index].DividendOption }
            };

            Logger.LogInformation(
                "Updating benefit {PBEN_ID} found for policy {PolicyNumber}.",
                ppbenTypesBAOR.PBEN_ID,
                policy.PolicyNumber);

            var numUpdate = await EventsAccessor.UpdatePolicyBenefitsAsync(policy, benefitDictionary, benefit.BenefitId);

            return numUpdate == 1;
        }
    }
}