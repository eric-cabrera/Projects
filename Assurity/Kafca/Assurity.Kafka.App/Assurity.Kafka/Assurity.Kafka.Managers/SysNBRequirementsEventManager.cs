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
    /// The class responsible for handling an updated SysNBRequirement record.
    /// </summary>
    public class SysNBRequirementsEventManager : EventManager, ISysNBRequirementsEventManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SysNBRequirementsEventManager"/> class.
        /// </summary>
        /// <param name="eventsAccessor">Accesses MongoDB Events database.</param>
        /// <param name="newRelicLogger">The logger that will write to the New Relic cloud service.</param>
        /// <param name="policyEngine">The engine used to arrange LifePro policy data.</param>
        /// <param name="hierarchyEngine">The engine used to arrange LifePro policy hierarchies.</param>
        /// <param name="globalDataAccessor">The accessor used to query global data.</param>
        /// <param name="lifeProAccessor">The accessor used to query LifePro policy data.</param>
        public SysNBRequirementsEventManager(
            IEventsAccessor eventsAccessor,
            ILogger<SysNBRequirements> newRelicLogger,
            IConsumerPolicyEngine policyEngine,
            IGlobalDataAccessor globalDataAccessor,
            ILifeProAccessor lifeProAccessor,
            IConsumerHierarchyEngine hierarchyEngine)
            : base(eventsAccessor, newRelicLogger, policyEngine, hierarchyEngine)
        {
            EventsAccessor = eventsAccessor;
            Logger = newRelicLogger;
            PolicyEngine = policyEngine;
            LifeProAccessor = lifeProAccessor;
        }

        private ILogger<SysNBRequirements> Logger { get; }

        private IEventsAccessor EventsAccessor { get; }

        private IConsumerPolicyEngine PolicyEngine { get; }

        private ILifeProAccessor LifeProAccessor { get; }

        /// <summary>
        /// Process the event from the SysNBRequirementsEvent Topic. Updates
        /// all policies that are dependent on this SysNBRequirements.
        /// </summary>
        /// <param name="sysNBRequirements">A sysNBRequirements record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        [Transaction]
        public async Task ProcessEvent(SysNBRequirements sysNBRequirements)
        {
            var companyCodeAndPolicyNumber = await LifeProAccessor
                .GetCompanyCodeAndPolicyNumberByPolicyNumber(sysNBRequirements.POLICYNUMBER);

            if (companyCodeAndPolicyNumber == null)
            {
                Logger.LogWarning(
                    "Unable to find policy record in LifePro for Policy Number '{policyNumber}' for the {eventName} event.",
                    sysNBRequirements.POLICYNUMBER,
                    nameof(SysNBRequirements));

                return;
            }

            var policy = await EventsAccessor
                .GetPolicyAsync(
                    companyCodeAndPolicyNumber.PolicyNumber.Trim(),
                    companyCodeAndPolicyNumber.CompanyCode);

            if (policy == null)
            {
                await GeneratePolicyWithHierarchyAndAgentAccess(companyCodeAndPolicyNumber.PolicyNumber, companyCodeAndPolicyNumber.CompanyCode, nameof(SysNBRequirementsEventManager));
                return;
            }

            await UpdatePolicyRequirements(policy);
        }
    }
}
