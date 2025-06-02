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
    /// The class responsible for handling an updated PRELA_RELATIONSHIP_MASTER record.
    /// </summary>
    public class PRELA_RELATIONSHIP_MASTEREventManager : EventManager, IPRELA_RELATIONSHIP_MASTEREventManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PRELA_RELATIONSHIP_MASTEREventManager"/> class.
        /// </summary>
        /// <param name="eventsAccessor">Accesses MongoDB Events database.</param>
        /// <param name="policyEngine">The engine used to arrange LifePro policy data.</param>
        /// <param name="logger">The logger that will write to the New Relic cloud service.</param>
        /// <param name="hierarchyEngine">The engine used to arrange LifePro policy hierarchies.</param>
        public PRELA_RELATIONSHIP_MASTEREventManager(
            IEventsAccessor eventsAccessor,
            IConsumerPolicyEngine policyEngine,
            ILogger<PRELA_RELATIONSHIP_MASTER> logger,
            IConsumerHierarchyEngine hierarchyEngine)
            : base(eventsAccessor, logger, policyEngine, hierarchyEngine)
        {
            EventsAccessor = eventsAccessor;
            PolicyEngine = policyEngine;
            Logger = logger;
        }

        private ILogger<PRELA_RELATIONSHIP_MASTER> Logger { get; }

        private IEventsAccessor EventsAccessor { get; }

        private IConsumerPolicyEngine PolicyEngine { get; }

        /// <summary>
        /// Process the event from the PRELA_RELATIONSHIP_MASTEREvent Topic.
        /// </summary>
        /// <param name="prela">A PRELA_RELATIONSHIP_MASTER record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        [Transaction]
        public async Task ProcessEvent(PRELA_RELATIONSHIP_MASTER prela)
        {
            if (prela.IDENTIFYING_ALPHA.Length < 3)
            {
                Logger.LogWarning(
                    "Unable to find policy record in LifePro for '{prela}' for the {eventName} event.",
                    prela.IDENTIFYING_ALPHA,
                    nameof(PRELA_RELATIONSHIP_MASTER));
                return;
            }

            // First 2 characters of IdentifyingAlpha represents Company Code.
            var companyCode = prela.IDENTIFYING_ALPHA.Substring(0, 2);

            // Except first 2 characters, remaining characters of IdentifyingAlpha represents Policy Number.
            var policyNumber = prela.IDENTIFYING_ALPHA.Substring(2);
            var policy = await EventsAccessor
               .GetPolicyAsync(policyNumber.Trim(), companyCode);
            if (policy == null)
            {
                await GeneratePolicyWithHierarchyAndAgentAccess(policyNumber, companyCode, nameof(PRELA_RELATIONSHIP_MASTEREventManager));

                return;
            }

            await UpdatePolicy(policy, prela, companyCode, policyNumber.Trim());
        }

        [Trace]
        private async Task<bool> UpdatePolicy(
            Policy policy,
            PRELA_RELATIONSHIP_MASTER prela,
            string companyCode,
            string policyNumber)
        {
            long numUpdate = 0;

            if (RelateCodes.AnnuitantRelateCodes.Contains(prela.RELATE_CODE))
            {
                policy.Annuitants = PolicyEngine.GetAnnuitants(policyNumber, companyCode);
                numUpdate = await EventsAccessor.UpdatePolicyAsync(
                    policy, policy.Annuitants, nameof(Policy.Annuitants));
            }
            else if (RelateCodes.AssigneeRelateCodes.Contains(prela.RELATE_CODE))
            {
                policy.Assignee = PolicyEngine.GetAssignee(policyNumber, companyCode);
                numUpdate = await EventsAccessor.UpdatePolicyAsync(
                    policy, policy.Assignee, nameof(Policy.Assignee));
            }
            else if (RelateCodes.BeneficiaryRelateCodes.Contains(prela.RELATE_CODE))
            {
                policy.Beneficiaries = PolicyEngine.GetBeneficiaries(policyNumber, companyCode);
                numUpdate = await EventsAccessor.UpdatePolicyAsync(
                    policy, policy.Beneficiaries, nameof(Policy.Beneficiaries));
            }
            else if (RelateCodes.OwnerRelateCodes.Contains(prela.RELATE_CODE))
            {
                policy.Owners = PolicyEngine.GetOwners(policyNumber, companyCode);
                numUpdate = await EventsAccessor.UpdatePolicyAsync(
                    policy, policy.Owners, nameof(Policy.Owners));
            }
            else if (RelateCodes.PayeeRelateCodes.Contains(prela.RELATE_CODE))
            {
                policy.Payee = PolicyEngine.GetPayee(policyNumber, companyCode);
                numUpdate = await EventsAccessor.UpdatePolicyAsync(
                    policy, policy.Payee, nameof(Policy.Payee));
            }
            else if (RelateCodes.PayorRelateCodes.Contains(prela.RELATE_CODE))
            {
                policy.Payors = PolicyEngine.GetPayors(policyNumber, companyCode);
                numUpdate = await EventsAccessor.UpdatePolicyAsync(
                    policy, policy.Payors, nameof(Policy.Payors));
            }
            else if (RelateCodes.InsuredRelateCodes.Contains(prela.RELATE_CODE))
            {
                policy.Insureds = PolicyEngine.GetInsureds(policyNumber, companyCode);
                numUpdate = await EventsAccessor.UpdatePolicyAsync(
                    policy, policy.Insureds, nameof(Policy.Insureds));
            }

            return numUpdate == 1;
        }
    }
}
