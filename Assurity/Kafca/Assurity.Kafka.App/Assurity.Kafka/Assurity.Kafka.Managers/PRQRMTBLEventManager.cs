namespace Assurity.Kafka.Managers
{
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Hierarchy;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Managers.Interfaces;
    using Microsoft.Extensions.Logging;
    using NewRelic.Api.Agent;

    public class PRQRMTBLEventManager : EventManager, IPRQRMTBLEventManager
    {
        public PRQRMTBLEventManager(
            ILogger<PRQRMTBLEventManager> logger,
            IEventsAccessor eventsAccessor,
            IConsumerPolicyEngine policyEngine,
            IConsumerHierarchyEngine hierarchyEngine,
            ILifeProAccessor lifeProAccessor)
            : base(eventsAccessor, logger, policyEngine, hierarchyEngine)
        {
            Logger = logger;
            EventsAccessor = eventsAccessor;
            PolicyEngine = policyEngine;
            LifeProAccessor = lifeProAccessor;
        }

        private ILogger<PRQRMTBLEventManager> Logger { get; }

        private IEventsAccessor EventsAccessor { get; }

        private IConsumerPolicyEngine PolicyEngine { get; }

        private ILifeProAccessor LifeProAccessor { get; }

        [Transaction]
        public async Task ProcessEvent(PRQRMTBL prqrmtbl)
        {
            var companyCodeAndPolicyNumber = await LifeProAccessor
                .GetCompanyCodeAndPolicyNumberByPolicyNumber(prqrmtbl.POLICY_NUMBER);

            if (companyCodeAndPolicyNumber == null)
            {
                Logger.LogWarning(
                    "Unable to find policy record in LifePro for '{policyNumber}' for the {eventName} event.",
                    prqrmtbl.POLICY_NUMBER,
                    nameof(PRQRMTBL));

                return;
            }

            var policy = await EventsAccessor
                .GetPolicyAsync(
                    companyCodeAndPolicyNumber.PolicyNumber.Trim(),
                    companyCodeAndPolicyNumber.CompanyCode);

            if (policy == null)
            {
                Logger.LogWarning(
                    "Policy not found in Mongo for policy number '{policyNumber}' for the {eventName} event.",
                    companyCodeAndPolicyNumber.PolicyNumber,
                    nameof(PRQRMTBL));

                await GeneratePolicyWithHierarchyAndAgentAccess(
                    companyCodeAndPolicyNumber.PolicyNumber.Trim(),
                    companyCodeAndPolicyNumber.CompanyCode,
                    nameof(PRQRMTBLEventManager));

                return;
            }

            await UpdatePolicyRequirements(policy);
        }
    }
}