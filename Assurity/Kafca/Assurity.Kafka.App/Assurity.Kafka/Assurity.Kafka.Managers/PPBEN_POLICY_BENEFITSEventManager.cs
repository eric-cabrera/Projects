namespace Assurity.Kafka.Managers
{
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.DataTransferObjects;
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
    /// updated PPBEN_POLICY_BENEFITS record.
    /// </summary>
    public class PPBEN_POLICY_BENEFITSEventManager : EventManager, IPPBEN_POLICY_BENEFITSEventManager
    {
        private static readonly List<string> RelevantBenefitTypes = new List<string>
        {
            BenefitTypes.Base,
            BenefitTypes.BaseForUniversalLife,
            BenefitTypes.OtherRider,
            BenefitTypes.SpecifiedAmountIncrease,
            BenefitTypes.TableRating,
            BenefitTypes.Supplemental
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="PPBEN_POLICY_BENEFITSEventManager"/> class.
        /// </summary>
        /// <param name="eventsAccessor">Accesses MongoDB Events database.</param>
        /// <param name="logger">The logger that will write to the New Relic cloud service.</param>
        /// <param name="policyEngine">The engine used to arrange LifePro policy data.</param>
        /// <param name="lifeProAccessor">The accessor used to query LifePro policy data.</param>
        /// <param name="hierarchyEngine">The engine used to arrange LifePro policy hierarchies.</param>
        public PPBEN_POLICY_BENEFITSEventManager(
            IEventsAccessor eventsAccessor,
            ILogger<PPBEN_POLICY_BENEFITSEventManager> logger,
            IConsumerPolicyEngine policyEngine,
            ILifeProAccessor lifeProAccessor,
            IConsumerHierarchyEngine hierarchyEngine)
            : base(eventsAccessor, logger, policyEngine, hierarchyEngine)
        {
            EventsAccessor = eventsAccessor;
            Logger = logger;
            PolicyEngine = policyEngine;
            LifeProAccessor = lifeProAccessor;
        }

        private ILogger<PPBEN_POLICY_BENEFITSEventManager> Logger { get; }

        private IEventsAccessor EventsAccessor { get; }

        private IConsumerPolicyEngine PolicyEngine { get; }

        private ILifeProAccessor LifeProAccessor { get; }

        /// <summary>
        /// Process the event from the PPBEN_POLICY_BENEFITSEvent Topic.
        /// Updates all policies that are depending on this PPBEN_POLICY_BENEFITS.
        /// </summary>
        /// <param name="ppben">A PPBEN_POLICY_BENEFITS record with updated data.</param>
        /// <param name="changeType">Change Type of the event.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        [Transaction]
        public async Task ProcessEvent(PPBEN_POLICY_BENEFITS ppben, string changeType)
        {
            Logger.LogInformation(
                "Processing event for policy {PolicyNumber} and PBEN_ID: {PBEN_ID}",
                ppben.POLICY_NUMBER,
                ppben.PBEN_ID);

            if (!RelevantBenefitTypes.Contains(ppben.BENEFIT_TYPE))
            {
                // Changes to the Policy object involving PPBEN_POLICY_BENEFITS are currently
                // only relevant for certain benefit types, stored in the above relevantBenefitTypes list.
                var logMessage =
                    $"{nameof(ProcessEvent)} for {nameof(PPBEN_POLICY_BENEFITS)} " +
                    "has been ignored. Currently only handling the following benefit types: " +
                    $"{string.Join(", ", RelevantBenefitTypes)}." +
                    FormatEventDataForLogMessage(ppben);

                Logger.LogWarning(logMessage);

                return;
            }

            var companyCodeAndPolicyNumber = await LifeProAccessor
                .GetCompanyCodeAndPolicyNumberByPolicyNumber(ppben.POLICY_NUMBER);

            if (companyCodeAndPolicyNumber == null)
            {
                var logMessage =
                    "Unable to find policy record in LifePro for policy number " +
                    $"associated with the {nameof(PPBEN_POLICY_BENEFITS)} event." +
                    FormatEventDataForLogMessage(ppben);

                Logger.LogWarning(logMessage);

                return;
            }

            Logger.LogInformation(
                "Getting policy {PolicyNumber} and PBEN_ID: {PBEN_ID}",
                ppben.POLICY_NUMBER,
                ppben.PBEN_ID);

            var policy = await EventsAccessor
                .GetPolicyAsync(
                    companyCodeAndPolicyNumber.PolicyNumber,
                    companyCodeAndPolicyNumber.CompanyCode);

            if (policy == null)
            {
                Logger.LogInformation(
                    "Policy {PolicyNumber} was not found. Creating policy and access. || PBEN_ID: {PBEN_ID}",
                    ppben.POLICY_NUMBER,
                    ppben.PBEN_ID);

                await GeneratePolicyWithHierarchyAndAgentAccess(companyCodeAndPolicyNumber.PolicyNumber, companyCodeAndPolicyNumber.CompanyCode, nameof(PPBEN_POLICY_BENEFITSEventManager));

                return;
            }

            if (changeType == TopicOperations.Delete)
            {
                Logger.LogInformation("Deleting benefit that was deleted in LifePro for PolicyNumber: {policyNumber} is being deleted", ppben.POLICY_NUMBER);
                await DeleteBenefit(companyCodeAndPolicyNumber.PolicyNumber, companyCodeAndPolicyNumber.CompanyCode, ppben.PBEN_ID);
                Logger.LogInformation("Processing event for PPBEN PolicyNumber: {policyNumber} Benefit has been deleted", ppben.POLICY_NUMBER);
            }
            else
            {
                Logger.LogInformation(
                    "Updating policy {PolicyNumber}. || PBEN_ID: {PBEN_ID}",
                    ppben.POLICY_NUMBER,
                    ppben.PBEN_ID);

                var updateWasSuccessful = await UpdatePolicy(policy, ppben, companyCodeAndPolicyNumber);
                Logger.LogInformation(
                    "Updating the existing benefit PBEN_ID {PBEN_ID} on the policy {PolicyNumber} was {SuccessResult}.",
                    ppben.PBEN_ID,
                    policy.PolicyNumber,
                    updateWasSuccessful ? "successful" : "not successful");
            }
        }

        private static string FormatEventDataForLogMessage(PPBEN_POLICY_BENEFITS ppben)
        {
            return $" {nameof(ppben.POLICY_NUMBER)}: '{ppben.POLICY_NUMBER}', " +
                $"{nameof(ppben.PLAN_CODE)}: '{ppben.PLAN_CODE}', " +
                $"{nameof(ppben.BENEFIT_TYPE)}: '{ppben.BENEFIT_TYPE}', " +
                $"{nameof(ppben.BENEFIT_SEQ)}: {ppben.BENEFIT_SEQ}";
        }

        [Trace]
        private async Task<string> DeleteBenefit(string policyNumber, string companyCode, long benefitId)
        {
            return await EventsAccessor.RemovePolicyBenefitByBenefitIdAsync(policyNumber, companyCode, benefitId);
        }

        [Trace]
        private async Task<bool> UpdatePolicy(
            Policy policy,
            PPBEN_POLICY_BENEFITS ppben,
            CompanyCodeAndPolicyNumber companyCodeAndPolicyNumber)
        {
            Logger.LogInformation(
                "Updating policy {PolicyNumber} for PPBEN_POLICY_BENEFITS_EVENT PBEN_ID {PBEN_ID}.",
                policy.PolicyNumber,
                ppben.PBEN_ID);

            if (ppben.BENEFIT_SEQ == 1)
            {
                var productDto = await GetProductCategoryAndDescription(policy.PolicyNumber, ppben.PLAN_CODE);
                if (policy.ProductCode != productDto.ProductCode)
                {
                    policy.ProductCode = productDto.ProductCode;
                    policy.ProductCategory = productDto.ProductCategory;
                    policy.ProductDescription = productDto.ProductDescription;

                    var policyDictionary = new Dictionary<string, object>
                    {
                        { nameof(Policy.ProductCode), policy.ProductCode },
                        { nameof(Policy.ProductCategory), policy.ProductCategory },
                        { nameof(Policy.ProductDescription), policy.ProductDescription },
                    };

                    var numUpdate = await EventsAccessor.UpdatePolicyAsync(policy, policyDictionary);
                }
            }

            // Whether there's a change to an existing benefit, or the benefit does not yet exist
            // on the Mongo policy, the Benefit is built and retrieved via the PolicyEngine
            // based on the PPBEN_POLICY_BENEFITS event data and related LifePro policy data.
            var benefit = PolicyEngine
                .GetBenefit(
                    companyCodeAndPolicyNumber,
                    policy.LineOfBusiness,
                    ppben);

            if (benefit == null)
            {
                Logger.LogError(
                    "Processing event for PPBEN_POLICY_BENEFITS PolicyNumber: Failed to get benefit for {PolicyNumber} with PBEN_ID {pbenId}.",
                    policy.PolicyNumber,
                    ppben.PBEN_ID);

                return false;
            }

            var policyBenefit = policy?.Benefits?
                .SingleOrDefault(benefit => benefit.BenefitId == ppben.PBEN_ID);

            if (policyBenefit == null)
            {
                Logger.LogInformation("Benefit PBEN_ID {PBEN_ID} was not found in mongo on the policy {PolicyNumber} so it is being inserted.", ppben.PBEN_ID, policy.PolicyNumber);

                // Benefit was not found on the Mongo policy, so it is added here.
                var policyBenefitId = await EventsAccessor.InsertPolicyBenefitAsync(benefit, policy.PolicyNumber, policy.CompanyCode);
                if (policyBenefitId == null)
                {
                    Logger.LogError("Processing event for PPBEN_POLICY_BENEFITS PolicyNumber: Failed to add {benefit} to Policy for {PolicyNumber}", benefit.BenefitId, policy.PolicyNumber);
                    return false;
                }
                else
                {
                    Logger.LogInformation("Benefit PBEN_ID {PBEN_ID} successfully inserted into the policy {PolicyNumber}.", ppben.PBEN_ID, policy.PolicyNumber);
                    return true;
                }
            }
            else
            {
                Logger.LogInformation("Updating the existing benefit PBEN_ID {PBEN_ID} on the policy {PolicyNumber}.", ppben.PBEN_ID, policy.PolicyNumber);

                policyBenefit.BenefitAmount = benefit.BenefitAmount;
                policyBenefit.BenefitDescription = benefit.BenefitDescription;
                policyBenefit.BenefitStatus = benefit.BenefitStatus;
                policyBenefit.BenefitStatusReason = benefit.BenefitStatusReason;
                policyBenefit.BenefitOptions = benefit.BenefitOptions;
                policyBenefit.CoverageType = benefit.CoverageType;
                policyBenefit.PlanCode = benefit.PlanCode;
                policyBenefit.DividendOption = benefit.DividendOption;

                var index = policy.Benefits.FindIndex(benefit => benefit.BenefitId == ppben.PBEN_ID);
                policy.Benefits[index] = policyBenefit;

                var benefitDictionary = new Dictionary<string, object>
                {
                    { BenefitProperties.BenefitAmount, policy.Benefits[index].BenefitAmount },
                    { BenefitProperties.BenefitDescription, policy.Benefits[index].BenefitDescription },
                    { BenefitProperties.BenefitStatus, policy.Benefits[index].BenefitStatus },
                    { BenefitProperties.BenefitStatusReason, policy.Benefits[index].BenefitStatusReason },
                    { BenefitProperties.BenefitOptions, policy.Benefits[index].BenefitOptions },
                    { BenefitProperties.CoverageType, policy.Benefits[index].CoverageType },
                    { BenefitProperties.PlanCode, policy.Benefits[index].PlanCode },
                    { BenefitProperties.DividendOption, policy.Benefits[index].DividendOption }
                };

                var numUpdate = await EventsAccessor.UpdatePolicyBenefitsAsync(policy, benefitDictionary, benefit.BenefitId);
                return numUpdate == 1;
            }
        }

        [Trace]
        private async Task<ProductDTO> GetProductCategoryAndDescription(string policyNumber, string planCode)
        {
            var productDto = new ProductDTO();

            var baseProductDescription = await PolicyEngine.GetBaseProductDescriptionByPlanCode(planCode);
            if (baseProductDescription != null)
            {
                if (baseProductDescription.ProdNumber != null)
                {
                    productDto.ProductCode = baseProductDescription.ProdNumber;
                }
                else
                {
                    Logger.LogError(
                        "PolicyNumber: {PolicyNumber} - ProductNumber was null with PlanCode: {PlanCode}",
                        policyNumber,
                        planCode);
                }

                if (baseProductDescription.ProdCategory != null)
                {
                    productDto.ProductCategory = baseProductDescription.ProdCategory;
                }
                else
                {
                    Logger.LogWarning(
                        "PolicyNumber: {PolicyNumber} - ProductCategory was null with PlanCode: {PlanCode}",
                        policyNumber,
                        planCode);
                }

                if (baseProductDescription.AltProdDesc != null)
                {
                    productDto.ProductDescription = baseProductDescription.AltProdDesc;
                }
                else
                {
                    Logger.LogWarning(
                        "PolicyNumber: {PolicyNumber} - AltProdDesc was null with PlanCode: {PlanCode}",
                        policyNumber,
                        planCode);
                }
            }
            else
            {
                Logger.LogError(
                    "PolicyNumber: {PolicyNumber} - unable to find base product description with PlanCode: {PlanCode}",
                    policyNumber,
                    planCode);
            }

            return productDto;
        }
    }
}