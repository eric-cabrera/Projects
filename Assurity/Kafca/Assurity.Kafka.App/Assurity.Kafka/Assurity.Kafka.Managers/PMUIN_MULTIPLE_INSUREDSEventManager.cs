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
    using KellermanSoftware.CompareNetObjects;
    using Microsoft.Extensions.Logging;
    using NewRelic.Api.Agent;

    /// <summary>
    /// Responsible for managing Policy updates based on an updated PMUIN_MULTIPLE_INSUREDS record.
    /// </summary>
    public class PMUIN_MULTIPLE_INSUREDSEventManager : EventManager, IPMUIN_MULTIPLE_INSUREDSEventManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PMUIN_MULTIPLE_INSUREDSEventManager"/> class.
        /// </summary>
        /// <param name="eventsAccessor">Accesses MongoDB Events database.</param>
        /// <param name="logger">The logger that will write to the New Relic cloud service.</param>
        /// <param name="policyEngine">The engine used to arrange LifePro policy data.</param>
        /// <param name="lifeProAccessor">The accessor used to query LifePro policy data.</param>
        /// <param name="hierarchyEngine">The engine used to arrange LifePro policy hierarchies.</param>
        public PMUIN_MULTIPLE_INSUREDSEventManager(
            IEventsAccessor eventsAccessor,
            ILogger<PMUIN_MULTIPLE_INSUREDSEventManager> logger,
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

        private ILogger<PMUIN_MULTIPLE_INSUREDSEventManager> Logger { get; }

        private IEventsAccessor EventsAccessor { get; }

        private IConsumerPolicyEngine PolicyEngine { get; }

        private ILifeProAccessor LifeProAccessor { get; }

        /// <summary>
        /// Process the event from the PMUIN_MULTIPLE_INSUREDSEvent Topic. Updates
        /// all policies that are depending on this PMUIN_MULTIPLE_INSUREDS.
        /// </summary>
        /// <param name="pmuin">A PMUIN_MULTIPLE_INSUREDS record with updated data.</param>
        /// <param name="changeType">Change Type of the event.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        [Transaction]
        public async Task ProcessEvent(PMUIN_MULTIPLE_INSUREDS pmuin, string changeType)
        {
            if (pmuin.BENEFIT_SEQ <= 0)
            {
                // Changes to the Policy object involving PMUIN are currently only relevant for records
                // that involve a BENEFIT_SEQ greater than 0.
                var logMessage =
                    $"{nameof(ProcessEvent)} for {nameof(PMUIN_MULTIPLE_INSUREDS)} " +
                    $"has been ignored. Currently only handling {pmuin.BENEFIT_SEQ} greater than 0." +
                    FormatEventDataForLogMessage(pmuin);
                Logger.LogWarning(logMessage);

                return;
            }

            var prela = await LifeProAccessor
                .GetPRELA_RELATIONSHIP_MASTER_BENEFIT(
                    pmuin.POLICY_NUMBER.Trim(),
                    pmuin.NAME_ID,
                    pmuin.RELATIONSHIP_CODE,
                    pmuin.BENEFIT_SEQ);

            if (prela == null)
            {
                var logMessage =
                    $"Unable to find Policy and Relationship data to process the {nameof(PMUIN_MULTIPLE_INSUREDS)} event." +
                    FormatEventDataForLogMessage(pmuin);

                Logger.LogWarning(logMessage);

                return;
            }

            // First 2 characters of IdentifyingAlpha represents Company Code.
            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
            {
                CompanyCode = prela.IDENTIFYING_ALPHA.Substring(0, 2),
                PolicyNumber = pmuin.POLICY_NUMBER.Trim()
            };

            var policy = await EventsAccessor
                .GetPolicyAsync(companyCodeAndPolicyNumber.PolicyNumber, companyCodeAndPolicyNumber.CompanyCode);

            if (policy == null)
            {
                Logger.LogWarning(
                    "Policy not found in Mongo for policy number '{PolicyNumber}' for the {eventName} event.",
                    companyCodeAndPolicyNumber.PolicyNumber,
                    nameof(PMUIN_MULTIPLE_INSUREDS));

                await GeneratePolicyWithHierarchyAndAgentAccess(
                    companyCodeAndPolicyNumber.PolicyNumber,
                    companyCodeAndPolicyNumber.CompanyCode,
                    nameof(PMUIN_MULTIPLE_INSUREDSEventManager));

                return;
            }

            policy.Insureds = PolicyEngine.GetInsureds(companyCodeAndPolicyNumber.PolicyNumber, companyCodeAndPolicyNumber.CompanyCode);

            var ppben = await LifeProAccessor.GetPPBEN_POLICY_BENEFITS(companyCodeAndPolicyNumber.PolicyNumber, pmuin.BENEFIT_SEQ, companyCodeAndPolicyNumber.CompanyCode);
            if (ppben == null)
            {
                Logger.LogWarning(
                    "Policy benefit not found in LifePro for policy number '{PolicyNumber}' and Benefit Seq '{BENEFIT_SEQ}' for the {eventName} event.",
                    companyCodeAndPolicyNumber.PolicyNumber,
                    pmuin.BENEFIT_SEQ,
                    nameof(PMUIN_MULTIPLE_INSUREDS));

                return;
            }

            if (changeType == TopicOperations.Delete)
            {
                Logger.LogInformation("Deleting benefit that was deleted in LifePro for PolicyNumber: {policyNumber} is being deleted", pmuin.POLICY_NUMBER.Trim());
                await DeleteBenefit(companyCodeAndPolicyNumber.PolicyNumber, companyCodeAndPolicyNumber.CompanyCode, ppben.PBEN_ID);
                Logger.LogInformation("Processing event for PMUIN PolicyNumber: {policyNumber} Benefit {pbenId} has been deleted", pmuin.POLICY_NUMBER.Trim(), ppben.PBEN_ID);
            }

            await UpdatePolicy(policy, ppben, companyCodeAndPolicyNumber);
        }

        private static string FormatEventDataForLogMessage(PMUIN_MULTIPLE_INSUREDS pmuin)
        {
            return $" {nameof(pmuin.POLICY_NUMBER)}: {pmuin.POLICY_NUMBER}, " +
                $"{nameof(pmuin.NAME_ID)}: {pmuin.NAME_ID}, " +
                $"{nameof(pmuin.MULT_RELATE)}: {pmuin.MULT_RELATE}, " +
                $"{nameof(pmuin.BENEFIT_SEQ)}: {pmuin.BENEFIT_SEQ}";
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
            // Whether there's a change to an existing benefit, or the benefit does not yet exist
            // on the Mongo policy, the Benefit is built and retrieved via the PolicyEngine
            // based on the PPBEN_POLICY_BENEFITS event data and related LifePro policy data.
            var benefit = PolicyEngine
                .GetBenefit(
                    companyCodeAndPolicyNumber,
                    policy.LineOfBusiness,
                    ppben);

            var policyBenefit = policy?.Benefits?
                .SingleOrDefault(benefit => benefit.BenefitId == ppben.PBEN_ID);

            if (policyBenefit == null)
            {
                // Benefit was not found on the Mongo policy, so it is added here.
                var policyBenefitId = await EventsAccessor.InsertPolicyBenefitAsync(benefit, policy.PolicyNumber, policy.CompanyCode);
                if (policyBenefitId == null)
                {
                    Logger.LogError("Processing event for PMUIN_MULTIPLE_INSUREDS PolicyNumber: Failed to add {benefit} to Policy for {PolicyNumber}", benefit.BenefitId, policy.PolicyNumber);
                    return false;
                }
                else
                {
                    // Better to check whether the triggered event's PBEN_ID and BENEFIT_SEQ still exist or not in the LifePro policy data even after inserting the PBEN_ID to MongoDb.
                    // If the PBEN_ID got deleted by PPBEN event manager then we should remove the inserted benefit with the old PBEN_ID from MongoDb.
                    var currentPpben = await LifeProAccessor.GetPPBEN_POLICY_BENEFITS(
                       companyCodeAndPolicyNumber.PolicyNumber,
                       ppben.BENEFIT_SEQ,
                       ppben.PBEN_ID,
                       companyCodeAndPolicyNumber.CompanyCode);

                    if (currentPpben == null)
                    {
                        await EventsAccessor.RemovePolicyBenefitByBenefitIdAsync(
                            companyCodeAndPolicyNumber.PolicyNumber,
                            companyCodeAndPolicyNumber.CompanyCode,
                            ppben.PBEN_ID);
                    }

                    return true;
                }
            }
            else
            {
                var updatedFlag = await UpdatePolicyBenefitAsync(policy, policyBenefit, benefit, ppben.PBEN_ID);
                if (updatedFlag)
                {
                    // Better to check whether any of the properties related to the triggered event's benefit got updated in LifePro or not.
                    // If so then update the Mongo Benefit document based on retrieving the updated benefit record from LifePro.
                    return await CheckAndUpdateBenefitAsync(policy, ppben, companyCodeAndPolicyNumber, benefit);
                }

                return false;
            }
        }

        [Trace]
        private async Task<bool> CheckAndUpdateBenefitAsync(
            Policy policy,
            PPBEN_POLICY_BENEFITS ppben,
            CompanyCodeAndPolicyNumber companyCodeAndPolicyNumber,
            Benefit policyBenefit)
        {
            var currentPpben = await LifeProAccessor.GetPPBEN_POLICY_BENEFITS(
                      companyCodeAndPolicyNumber.PolicyNumber,
                      ppben.BENEFIT_SEQ,
                      ppben.PBEN_ID,
                      companyCodeAndPolicyNumber.CompanyCode);

            var benefit = PolicyEngine
               .GetBenefit(
                   companyCodeAndPolicyNumber,
                   policy.LineOfBusiness,
                   ppben);

            if (currentPpben != null)
            {
                var compareResult = new CompareLogic().Compare(policyBenefit, benefit);

                if (!compareResult.AreEqual)
                {
                    return await UpdatePolicyBenefitAsync(policy, policyBenefit, benefit, ppben.PBEN_ID);
                }
            }

            return true;
        }

        [Trace]
        private async Task<bool> UpdatePolicyBenefitAsync(
            Policy policy,
            Benefit policyBenefit,
            Benefit benefit,
            long ppbenId)
        {
            policyBenefit.BenefitAmount = benefit.BenefitAmount;
            policyBenefit.BenefitDescription = benefit.BenefitDescription;
            policyBenefit.BenefitStatus = benefit.BenefitStatus;
            policyBenefit.BenefitStatusReason = benefit.BenefitStatusReason;
            policyBenefit.BenefitOptions = benefit.BenefitOptions;
            policyBenefit.CoverageType = benefit.CoverageType;
            policyBenefit.PlanCode = benefit.PlanCode;

            var index = policy.Benefits.FindIndex(benefit => benefit.BenefitId == ppbenId);
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
            };

            var numUpdate = await EventsAccessor.UpdatePolicyBenefitsAsync(policy, benefitDictionary, benefit.BenefitId);
            return numUpdate == 1;
        }
    }
}
