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
    /// updated PPEND_NEW_BUS_PEND_UNDERWRITING record.
    /// </summary>
    public class PPEND_NEW_BUS_PEND_UNDERWRITINGEventManager : EventManager, IPPEND_NEW_BUS_PEND_UNDERWRITINGEventManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PPEND_NEW_BUS_PEND_UNDERWRITINGEventManager"/> class.
        /// </summary>
        /// <param name="logger">The logger that will write to the New Relic cloud service.</param>
        /// <param name="eventsAccessor">Accesses MongoDB Events database.</param>
        /// <param name="policyEngine">The engine used to arrange LifePro policy data.</param>
        /// <param name="lifeProAccessor">The accessor used to query LifePro policy data.</param>
        /// <param name="hierarchyEngine">The engine used to arrange LifePro policy hierarchies.</param>
        public PPEND_NEW_BUS_PEND_UNDERWRITINGEventManager(
            ILogger<PPEND_NEW_BUS_PEND_UNDERWRITINGEventManager> logger,
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

        private ILogger<PPEND_NEW_BUS_PEND_UNDERWRITINGEventManager> Logger { get; }

        private IEventsAccessor EventsAccessor { get; }

        private IConsumerPolicyEngine PolicyEngine { get; }

        private ILifeProAccessor LifeProAccessor { get; }

        /// <summary>
        /// Process the event from the PPEND_NEW_BUS_PEND_UNDERWRITINGEvent Topic.
        /// Updates all policies that are depended on this
        /// PPEND_NEW_BUS_PEND_UNDERWRITING event.
        /// </summary>
        /// <param name="pendu">A PPEND_NEW_BUS_PEND_UNDERWRITING record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        [Transaction]
        public async Task ProcessEvent(PPEND_NEW_BUS_PEND_UNDERWRITING pendu)
        {
            var companyCodeAndPolicyNumber = await LifeProAccessor
                   .GetCompanyCodeAndPolicyNumberByPENDID(pendu.PEND_ID);

            if (companyCodeAndPolicyNumber == null)
            {
                Logger.LogWarning(
                    "Unable to find policy record in LifePro for PEND_ID '{pendu.PEND_ID}' for the {eventName} event.",
                    pendu.PEND_ID,
                    nameof(PPEND_NEW_BUS_PEND_UNDERWRITING));

                return;
            }

            var policy = await EventsAccessor
                .GetPolicyAsync(
                    companyCodeAndPolicyNumber.PolicyNumber.Trim(),
                    companyCodeAndPolicyNumber.CompanyCode);

            if (policy == null)
            {
                await GeneratePolicyWithHierarchyAndAgentAccess(companyCodeAndPolicyNumber.PolicyNumber, companyCodeAndPolicyNumber.CompanyCode, nameof(PPEND_NEW_BUS_PEND_UNDERWRITINGEventManager));
                return;
            }

            await UpdatePolicyRequirements(policy);
        }
    }
}