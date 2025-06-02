namespace Assurity.Kafka.Managers
{
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Hierarchy;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Managers.Interfaces;
    using Assurity.Kafka.Utilities.Config;
    using Assurity.Kafka.Utilities.Constants;
    using Assurity.Kafka.Utilities.Extensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.Extensions.Logging;
    using NewRelic.Api.Agent;

    /// <summary>
    /// Responsible for managing an updated PHIER_AGENT_HIERARCHY record.
    /// </summary>
    public class PHIER_AGENT_HIERARCHYEventManager : IPHIER_AGENT_HIERARCHYEventManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PHIER_AGENT_HIERARCHYEventManager"/> class.
        /// </summary>
        /// <param name="eventsAccessor">Accesses MongoDB Events database.</param>
        /// <param name="logger">The logger that will write to the New Relic cloud service.</param>
        /// <param name="policyEngine">The engine used to arrange LifePro policy data.</param>
        /// <param name="hierarchyEngine">The engine used to arrange LifePro policy hierarchies.</param>
        /// <param name="configuration"></param>
        /// <param name="dataStoreAccessor">Accessor used to interact with DataStore database.</param>
        public PHIER_AGENT_HIERARCHYEventManager(
            IEventsAccessor eventsAccessor,
            ILogger<PHIER_AGENT_HIERARCHYEventManager> logger,
            IConsumerPolicyEngine policyEngine,
            IConsumerHierarchyEngine hierarchyEngine,
            IConfigurationManager configuration,
            IDataStoreAccessor dataStoreAccessor)
        {
            EventsAccessor = eventsAccessor;
            Logger = logger;
            PolicyEngine = policyEngine;
            HierarchyEngine = hierarchyEngine;
            Config = configuration;
            DataStoreAccessor = dataStoreAccessor;
        }

        private ILogger<PHIER_AGENT_HIERARCHYEventManager> Logger { get; }

        private IEventsAccessor EventsAccessor { get; }

        private IConsumerPolicyEngine PolicyEngine { get; }

        private IConsumerHierarchyEngine HierarchyEngine { get; }

        private IConfigurationManager Config { get; set; }

        /// <summary>
        /// Accessing LifePro within the managers for the Consumer is done via the
        /// LifeProAccessor which connects to lpsqldev.LPDev. The use of DataStoreAccessor
        /// here in this manager is NOT being used to access LifePro; only some tables in the
        /// commdebt schema on the DMDev.DataStore database are being used, which happens to
        /// also have a lifepro_r schema for LifePro on that same database. Since DataStoreAccessor
        /// already exists and connects to DMDev.DataStore, that is being used here for capturing
        /// PHIER changes to be written to commdebt schema tables.
        /// </summary>
        private IDataStoreAccessor DataStoreAccessor { get; }

        /// <summary>
        /// Process the event from the PHIER_AGENT_HIERARCHY Event Topic.
        /// Updates all policies that are depending on this PHIER_AGENT_HIERARCHY and
        /// triggers the Assurity.Commissions.Debt.Identifier service to process changes
        /// to the Debt security which is based on the hierarchy of the agents.
        /// </summary>
        /// <param name="phier">A PHIER_AGENT_HIERARCHY record with updated data.</param>
        /// <param name="changeType">Change Type of the event.</param>
        /// <param name="beforeAgentNum">Before Agent Num of the deleted agent.</param>
        /// <param name="slowConsumer">Flag to treat event in slow consumer mode, which will bypass the
        /// KafkaSlowConsumerUpdateThreshold check.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        [Transaction]
        public async Task ProcessEvent(PHIER_AGENT_HIERARCHY phier, string changeType, string beforeAgentNum, bool slowConsumer = false)
        {
            try
            {
                if (changeType == TopicOperations.Delete)
                {
                    await CheckAndUpdateForAgent(beforeAgentNum?.Trim(), phier.COMPANY_CODE.Trim(), slowConsumer);
                }
                else
                {
                    await CheckAndUpdateForAgent(phier.AGENT_NUM.Trim(), phier.COMPANY_CODE.Trim(), slowConsumer);
                }

                await AddSystemDataLoadWithAgentHierarchyChange(phier, changeType, beforeAgentNum);
            }
            catch (SlowConsumerException)
            {
                throw;
            }
        }

        [Trace]
        private async Task AddSystemDataLoadWithAgentHierarchyChange(
            PHIER_AGENT_HIERARCHY phier,
            string changeType,
            string beforeAgentId)
        {
            var changeTypeEnum = changeType switch
            {
                TopicOperations.Create => ChangeType.Create,
                TopicOperations.Delete => ChangeType.Delete,
                TopicOperations.Update => ChangeType.Update,
                _ => throw new NotImplementedException($"{nameof(ChangeType)} '{changeType}' is not handled."),
            };

            await DataStoreAccessor.AddSystemDataLoadWithAgentHierarchyChange(
                phier,
                changeTypeEnum,
                beforeAgentId);
        }

        [Trace]
        private async Task CheckAndUpdateForAgent(string agentNum, string companyCode, bool slowConsumer = false)
        {
            var agentPolicyAccess = await EventsAccessor
                .GetAgentPolicyAccessAsync(agentNum, companyCode);

            if (agentPolicyAccess == null)
            {
                Logger.LogWarning("AgentPolicyAccess not found in Mongo for Agent number " +
                    $"'{agentNum}' for the {nameof(PHIER_AGENT_HIERARCHY)} event.");
                return;
            }

            if (agentPolicyAccess.PolicyNumbers.Count() > Config.KafkaSlowConsumerUpdateThreshold && !slowConsumer)
            {
                // This is going to be a slow consumer
                Logger.LogWarning(
                    "Slow Consumer - PHIER_AGENT_HIERARCHY event AgentNum: {agentNum}, " +
                    "Number of updates to process: {agentPolicyAccess.PolicyNumbers.Count}. This will be " +
                    "done while disconnected from kafka",
                    agentNum,
                    agentPolicyAccess.PolicyNumbers.Count);

                throw new SlowConsumerException();
            }

            foreach (var policyNumber in agentPolicyAccess.PolicyNumbers)
            {
                var policyHierarchy = await EventsAccessor
                    .GetPolicyHierarchyAsync(policyNumber.Trim(), companyCode);

                await CheckAndUpdatePolicyHierarchyAndAgentPolicyAccess(policyNumber, companyCode, policyHierarchy);
            }
        }

        [Trace]
        private async Task CheckAndUpdatePolicyHierarchyAndAgentPolicyAccess(
            string policyNumber,
            string companyCode,
            PolicyHierarchy policyHierarchy)
        {
            try
            {
                var newAgents = await PolicyEngine
                    .GetAgents(policyHierarchy.PolicyNumber, policyHierarchy.CompanyCode, policyHierarchy.ApplicationDate);

                var newAgentHierarchies = await HierarchyEngine
                    .BuildAgentHierarchy(newAgents, policyHierarchy.ApplicationDate);

                var results = HierarchyEngine.CompareAgentHierarchies(
                    policyHierarchy.CompanyCode,
                    policyHierarchy.PolicyNumber,
                    policyHierarchy?.HierarchyBranches,
                    newAgentHierarchies);

                if ((results?.AddedAgents?.Count ?? 0) > 0 || (results?.RemovedAgents?.Count ?? 0) > 0)
                {
                    await EventsAccessor.UpdatePolicyHierarchyAsync(policyNumber, companyCode, newAgentHierarchies);

                    await UpdateAgentPolicyAccess(
                        companyCode,
                        policyNumber,
                        results?.AddedAgents,
                        results?.RemovedAgents);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Processing event for PHIER_AGENT_HIERARCHY PolicyNumber: {PolicyNumber} failed.", policyNumber);
            }
        }

        [Trace]
        private async Task UpdateAgentPolicyAccess(
            string companyCode,
            string policyNumber,
            List<string>? addedAgents,
            List<string>? removedAgents)
        {
            if (removedAgents != null)
            {
                foreach (var agent in removedAgents)
                {
                    if (removedAgents.Contains(agent))
                    {
                        var agentPolicyAccessId = await EventsAccessor.RemoveAgentPolicyAccessAsync(agent, policyNumber, companyCode);

                        if (agentPolicyAccessId == 0)
                        {
                            Logger.LogError("Processing event for PHIER_AGENT_HIERARCHY PolicyNumber: Failed to remove {PolicyNumber} from AgentPolicyAccess for {Agent}", policyNumber, agent);
                        }
                    }
                }
            }

            if (addedAgents != null)
            {
                foreach (var agent in addedAgents)
                {
                    if (addedAgents.Contains(agent))
                    {
                        var agentPolicyAccessId = await EventsAccessor.InsertAgentPolicyAccessAsync(agent, policyNumber, companyCode);

                        if (agentPolicyAccessId == 0)
                        {
                            Logger.LogError("Processing event for PHIER_AGENT_HIERARCHY PolicyNumber: Failed to add {PolicyNumber} to AgentPolicyAccess for {Agent}", policyNumber, agent);
                        }
                    }
                }
            }
        }
    }
}