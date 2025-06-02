namespace Assurity.Kafka.Managers
{
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Hierarchy;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Managers.Interfaces;
    using Microsoft.Extensions.Logging;
    using NewRelic.Api.Agent;

    public class PRQRMEventManager : EventManager, IPRQRMEventManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PRQRMEventManager"/> class.
        /// </summary>
        /// <param name="logger">The logger that will write to the New Relic cloud service.</param>
        /// <param name="eventsAccessor">Accesses MongoDB Events database.</param>
        /// <param name="policyEngine">The engine used to arrange LifePro policy data.</param>
        /// <param name="lifeProAccessor">The accessor used to query LifePro policy data.</param>
        /// <param name="hierarchyEngine">The engine used to arrange LifePro policy hierarchies.</param>
        public PRQRMEventManager(
            ILogger<PRQRMEventManager> logger,
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

        private ILogger<PRQRMEventManager> Logger { get; }

        private IEventsAccessor EventsAccessor { get; }

        private IConsumerPolicyEngine PolicyEngine { get; }

        private ILifeProAccessor LifeProAccessor { get; }

        [Transaction]
        public async Task ProcessEvent(PRQRM prqrm)
        {
            var companyCodeAndPolicyNumber = await LifeProAccessor.GetCompanyCodeAndPolicyNumberByPolicyNumber(prqrm.POLICY_NUMBER);
            if (companyCodeAndPolicyNumber == null)
            {
                Logger.LogWarning("Unable to find policy record in LifePro for '{policyNumber}' for the {eventName} event.", prqrm.POLICY_NUMBER, nameof(PRQRM));

                return;
            }

            var policy = await EventsAccessor
                .GetPolicyAsync(
                    companyCodeAndPolicyNumber.PolicyNumber.Trim(),
                    companyCodeAndPolicyNumber.CompanyCode);

            if (policy == null)
            {
                await GeneratePolicyWithHierarchyAndAgentAccess(
                    companyCodeAndPolicyNumber.PolicyNumber,
                    companyCodeAndPolicyNumber.CompanyCode,
                    nameof(PRQRMEventManager));

                return;
            }

            await UpdatePolicyRequirements(policy);
        }
    }
}