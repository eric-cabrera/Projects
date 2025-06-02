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
    /// The class responsible for handling an updated PACON_ANNUITY_POLICY record.
    /// </summary>
    public class PACON_ANNUITY_POLICYEventManager : EventManager, IPACON_ANNUITY_POLICYEventManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PACON_ANNUITY_POLICYEventManager"/> class.
        /// </summary>
        /// <param name="eventsAccessor">Accesses MongoDB Events database.</param>
        /// <param name="policyEngine">The policyEngine that will be used for various policy related logic.</param>
        /// <param name="hierarchyEngine">The hierarchyEngine that will be used for various policy hierarchies logic.</param>
        /// <param name="logger">The logger that will write to the New Relic cloud service.</param>
        public PACON_ANNUITY_POLICYEventManager(
            IEventsAccessor eventsAccessor,
            IConsumerPolicyEngine policyEngine,
            IConsumerHierarchyEngine hierarchyEngine,
            ILogger<PACON_ANNUITY_POLICYEventManager> logger)
            : base(eventsAccessor, logger, policyEngine, hierarchyEngine)
        {
            EventsAccessor = eventsAccessor;
            Logger = logger;
            PolicyEngine = policyEngine;
        }

        private ILogger<PACON_ANNUITY_POLICYEventManager> Logger { get; }

        private IEventsAccessor EventsAccessor { get; }

        private IConsumerPolicyEngine PolicyEngine { get; }

        /// <summary>
        /// Process the event from the PACON_ANNUITY_POLICYEvent Topic. Updates
        /// all policies that are depending on this PACON_ANNUITY_POLICY. If policy
        /// does not yet exist this will create and save one to the mongodb.
        /// </summary>
        /// <param name="pacon">A PACON_ANNUITY_POLICY record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        [Transaction]
        public async Task ProcessEvent(PACON_ANNUITY_POLICY pacon)
        {
            Logger.LogInformation("Getting from MDB PACON_ANNUITY_POLICY PolicyNumber: {pacon.POLICY_NUMBER}", pacon.POLICY_NUMBER);
            var existingPolicy = await EventsAccessor.GetPolicyAsync(pacon.POLICY_NUMBER, pacon.COMPANY_CODE);

            if (pacon.STATUS_CODE == "T" && pacon.STATUS_REASON == "ER")
            {
                await PolicyEngine.DeletePolicy(pacon.POLICY_NUMBER, pacon.COMPANY_CODE);
            }
            else if (existingPolicy == null)
            {
                await GeneratePolicyWithHierarchyAndAgentAccess(pacon.POLICY_NUMBER, pacon.COMPANY_CODE, nameof(PACON_ANNUITY_POLICYEventManager));
            }
            else
            {
                await UpdateExistingPolicy(existingPolicy, pacon);
            }
        }

        /// <summary>
        /// Updates a policy object for a given policyNumber.
        /// </summary>
        /// <param name="mdbPolicy">The existing policy that will be updated.</param>
        /// <param name="pacon">A PACON_ANNUITY_POLICY that contains any changes to be applied to the policy.</param>
        /// <returns>A boolean indicated if the update was successful or not.</returns>
        [Trace]
        private async Task<bool> UpdateExistingPolicy(Policy mdbPolicy, PACON_ANNUITY_POLICY pacon)
        {
            mdbPolicy.PolicyStatus = pacon.STATUS_CODE.ToPolicyStatus();
            mdbPolicy.PolicyStatusReason = pacon.STATUS_REASON.ToPolicyStatusReason(pacon.STATUS_CODE);
            mdbPolicy.IssueDate = pacon.ISSUE_DATE.ToNullableDateTime();
            mdbPolicy.TaxQualificationStatus = pacon.TAX_QUALIFICATION.ToTaxQualificationStatus(LineOfBusinessTypes.ImmediateAnnuity);
            if (pacon.STATUS_CODE.Equals("T", StringComparison.InvariantCultureIgnoreCase))
            {
                mdbPolicy.TerminationDate = pacon.STATUS_DATE.ToNullableDateTime();
            }
            else
            {
                mdbPolicy.TerminationDate = null;
            }

            var objsDictionary = new Dictionary<string, object>
            {
                { nameof(Policy.PolicyStatus), mdbPolicy.PolicyStatus },
                { nameof(Policy.PolicyStatusReason), mdbPolicy.PolicyStatusReason },
                { nameof(Policy.TaxQualificationStatus), mdbPolicy.TaxQualificationStatus },
                { nameof(Policy.IssueDate), mdbPolicy.IssueDate },
                { nameof(Policy.TerminationDate), mdbPolicy.TerminationDate }
            };

            var numUpdate = await EventsAccessor.UpdatePolicyAsync(mdbPolicy, objsDictionary);

            if (numUpdate == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
