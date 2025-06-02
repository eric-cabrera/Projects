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
    /// The class responsible for managing the handling an updated PPOLC record.
    /// </summary>
    public class PPOLCEventManager : EventManager, IPPOLCEventManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PPOLCEventManager"/> class.
        /// </summary>
        /// <param name="eventsAccessor">Accesses MongoDB Events database.</param>
        /// <param name="policyEngine">The policyEngine that will be used for various policy related logic.</param>
        /// <param name="hierarchyEngine">The hierarchyEngine that will be used for various policy hierarchies logic.</param>
        /// <param name="logger">The logger that will write to the New Relic cloud service.</param>
        public PPOLCEventManager(
            IEventsAccessor eventsAccessor,
            IConsumerPolicyEngine policyEngine,
            IConsumerHierarchyEngine hierarchyEngine,
            ILogger<PPOLCEventManager> logger)
            : base(eventsAccessor, logger, policyEngine, hierarchyEngine)
        {
            EventsAccessor = eventsAccessor;
            Logger = logger;
            PolicyEngine = policyEngine;
        }

        private ILogger<PPOLCEventManager> Logger { get; }

        private IEventsAccessor EventsAccessor { get; }

        private IConsumerPolicyEngine PolicyEngine { get; }

        /// <summary>
        /// Process the event from the PPOLCEvent Topic. Updates
        /// all policies that are depending on this PPOLC. If policy
        /// does not yet exist this will create and save one to the mongodb.
        /// </summary>
        /// <param name="ppolc">A PPOLC record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        [Transaction]
        public async Task ProcessEvent(PPOLC ppolc)
        {
            Logger.LogInformation("Starting {methodName} for {policyNumber}", nameof(ProcessEvent), ppolc.POLICY_NUMBER);

            if (ppolc.APPLICATION_DATE.ToString().Length != 8)
            {
                var existingPolicy = await EventsAccessor.GetPolicyAsync(ppolc.POLICY_NUMBER, ppolc.COMPANY_CODE);
                Logger.LogWarning("ApplicationDate is invalid for the policy number: {PolicyNumber}", ppolc.POLICY_NUMBER);
                if (existingPolicy != null)
                {
                    await PolicyEngine.DeletePolicy(ppolc.POLICY_NUMBER, ppolc.COMPANY_CODE);
                }
            }
            else if (ppolc.CONTRACT_CODE == "T" && ppolc.CONTRACT_REASON == "ER")
            {
                await PolicyEngine.DeletePolicy(ppolc.POLICY_NUMBER, ppolc.COMPANY_CODE);
            }
            else
            {
                await GeneratePolicyWithHierarchyAndAgentAccess(ppolc.POLICY_NUMBER, ppolc.COMPANY_CODE, nameof(PPOLCEventManager));
            }
        }
    }
}
