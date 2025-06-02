namespace Assurity.Kafka.Managers
{
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Hierarchy;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Managers.Interfaces;
    using Microsoft.Extensions.Logging;
    using NewRelic.Api.Agent;

    /// <summary>
    /// The class responsible for managing the handling an
    /// updated PPEND_NEW_BUSINESS_PENDING record.
    /// </summary>
    public class PPEND_NEW_BUSINESS_PENDINGEventManager : EventManager, IPPEND_NEW_BUSINESS_PENDINGEventManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PPEND_NEW_BUSINESS_PENDINGEventManager"/> class.
        /// </summary>
        /// <param name="logger">The logger that will write to the New Relic cloud service.</param>
        /// <param name="eventsAccessor">Accesses MongoDB Events database.</param>
        /// <param name="policyEngine">The engine used to arrange LifePro policy data.</param>
        /// <param name="lifeProAccessor">The accessor used to query LifePro policy data.</param>
        /// <param name="hierarchyEngine">The engine used to arrange LifePro policy hierarchies.</param>
        public PPEND_NEW_BUSINESS_PENDINGEventManager(
            ILogger<PPEND_NEW_BUSINESS_PENDINGEventManager> logger,
            IEventsAccessor eventsAccessor,
            IConsumerPolicyEngine policyEngine,
            ILifeProAccessor lifeProAccessor,
            IConsumerHierarchyEngine hierarchyEngine)
            : base(eventsAccessor, logger, policyEngine, hierarchyEngine)
        {
            Logger = logger;
            EventsAccessor = eventsAccessor;
            PolicyEngine = policyEngine;
            LifeProAccessor = lifeProAccessor;
        }

        private ILogger<PPEND_NEW_BUSINESS_PENDINGEventManager> Logger { get; }

        private IEventsAccessor EventsAccessor { get; }

        private IConsumerPolicyEngine PolicyEngine { get; }

        private ILifeProAccessor LifeProAccessor { get; }

        /// <summary>
        /// Process the event from the PPEND_NEW_BUSINESS_PENDINGEvent Topic.
        /// Updates all policies that are depended on this
        /// PPEND_NEW_BUSINESS_PENDING event.
        /// </summary>
        /// <param name="ppend">A PPEND_NEW_BUSINESS_PENDING record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        [Transaction]
        public async Task ProcessEvent(PPEND_NEW_BUSINESS_PENDING ppend)
        {
            var companyCodeAndPolicyNumber = await LifeProAccessor
                   .GetCompanyCodeAndPolicyNumberByPolicyNumber(ppend.POLICY_NUMBER);

            if (companyCodeAndPolicyNumber == null)
            {
                Logger.LogWarning(
                    "Unable to find policy record in LifePro for '{policyNumber}' for the {eventName} event.",
                    ppend.POLICY_NUMBER,
                    nameof(PPEND_NEW_BUSINESS_PENDING));

                return;
            }

            var policy = await EventsAccessor
                .GetPolicyAsync(
                    companyCodeAndPolicyNumber.PolicyNumber.Trim(),
                    companyCodeAndPolicyNumber.CompanyCode);

            if (policy == null)
            {
                await GeneratePolicyWithHierarchyAndAgentAccess(companyCodeAndPolicyNumber.PolicyNumber, companyCodeAndPolicyNumber.CompanyCode, nameof(PPEND_NEW_BUSINESS_PENDINGEventManager));

                return;
            }

            await UpdatePolicyRequirements(policy);
        }
    }
}