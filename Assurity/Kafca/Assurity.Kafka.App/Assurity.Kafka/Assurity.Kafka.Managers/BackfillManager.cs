using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Assurity.Kafka.Managers.Tests")]

namespace Assurity.Kafka.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Engines.Hierarchy;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Managers.DTOs;
    using Assurity.Kafka.Managers.Interfaces;
    using Assurity.Kafka.Utilities.Enums;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;
    using NewRelic.Api.Agent;

    public class BackfillManager : IBackfillManager
    {
        public BackfillManager(
            IDataStoreAccessor dataStoreAccessor,
            IEventsAccessor eventsAccessor,
            IMigratePolicyEngine migratePolicyEngine,
            IMigrateHierarchyEngine migrateHierarchyEngine,
            ILogger<BackfillManager> logger)
        {
            DataStoreAccessor = dataStoreAccessor;
            EventsAccessor = eventsAccessor;
            PolicyEngine = migratePolicyEngine;
            HierarchyEngine = migrateHierarchyEngine;
            Logger = logger;
        }

        private ILogger<BackfillManager> Logger { get; }

        private IDataStoreAccessor DataStoreAccessor { get; }

        private IEventsAccessor EventsAccessor { get; }

        private IMigrateHierarchyEngine HierarchyEngine { get; }

        private IMigratePolicyEngine PolicyEngine { get; }

        [Transaction]
        public void FlagPastDuePolicies()
        {
            Logger.LogInformation("{methodName} starting.", nameof(FlagPastDuePolicies));

            var flaggedCount = EventsAccessor.FlagPastDuePolicies();

            Logger.LogInformation(
                "{methodName} ending. " +
                "{UpdatedCount} policies flagged.",
                nameof(FlagPastDuePolicies),
                flaggedCount);
        }

        [Transaction]
        public void FlagPendingPolicies()
        {
            Logger.LogInformation("{methodName} starting.", nameof(FlagPendingPolicies));

            var flaggedCount = EventsAccessor.FlagPendingPolicies();

            Logger.LogInformation(
                "{methodName} ending. " +
                "{UpdatedCount} policies flagged.",
                nameof(FlagPendingPolicies),
                flaggedCount);
        }

        /// <summary>
        /// Update, Migrate, or Update And Migrate MongoDB Events collections.
        /// </summary>
        /// <remarks>
        /// The difference between "Update" and "Migrate" is as follows:
        /// - Update deletes all flagged records before processing.
        /// - Migrate should only be generating new/non-existent records.
        /// Both processes:
        /// - Generate new Policy records from existing SQL data.
        /// - Generate new PolicyHierarchy documents/update existing ones.
        /// - Generate new AgentPolicyAccess documents/update existing ones.
        /// </remarks>
        /// <param name="stringExecutionMode"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [Transaction]
        public async Task BackFillPolicies(string stringExecutionMode)
        {
            if (!Enum.TryParse<MongoDbExecutionModes>(stringExecutionMode, true, out var parsedExecutionMode))
            {
                throw new NotImplementedException($"Unsupported `MongoDbExecutionMode` value: {stringExecutionMode}");
            }

            Logger.LogInformation("{methodName} starting. Execution Mode: {mongoDbExecutionMode} policies.", nameof(BackFillPolicies), parsedExecutionMode);

            var recordsToProcess = new List<CompanyCodeAndPolicyNumber>();
            switch (parsedExecutionMode)
            {
                case MongoDbExecutionModes.Update:
                    {
                        recordsToProcess = EventsAccessor.GetCompanyCodeAndPolicyNumberOfFlaggedPolicies();
                        await Backfill(recordsToProcess, true);

                        break;
                    }

                case MongoDbExecutionModes.Migrate:
                    {
                        recordsToProcess = GetUnmigratedPPOLCRecords();
                        await Backfill(recordsToProcess, false);

                        break;
                    }

                case MongoDbExecutionModes.UpdateAndMigrate:
                    {
                        recordsToProcess = EventsAccessor.GetCompanyCodeAndPolicyNumberOfFlaggedPolicies();
                        recordsToProcess.AddRange(GetUnmigratedPPOLCRecords());
                        await Backfill(recordsToProcess, true);

                        break;
                    }

                default:
                    Logger.LogError("Unsupported `MongoDbExecutionMode` value: {parsedExecutionMode}", parsedExecutionMode);

                    throw new NotImplementedException($"Unsupported `MongoDbExecutionMode` value: {parsedExecutionMode}");
            }

            Logger.LogInformation("{methodName} complete. Execution Mode: {mongoDbExecutionMode} policies.", nameof(BackFillPolicies), parsedExecutionMode);
        }

        [Transaction]
        public async Task MigrateSinglePolicy(CompanyCodeAndPolicyNumber lifeProPolicy)
        {
            var policyExists = await EventsAccessor.CheckIfPolicyExists(lifeProPolicy.PolicyNumber, lifeProPolicy.CompanyCode);

            if (policyExists)
            {
                Logger.LogInformation("Policy Number: {PolicyNumber} has not been migrated. It already exists in MongoDb.", lifeProPolicy.PolicyNumber);

                return;
            }

            (var result, var policy) = await PolicyEngine.GetPolicy(lifeProPolicy.PolicyNumber, lifeProPolicy.CompanyCode);

            if (result != GetPolicyResult.Found)
            {
                Logger.LogWarning("{reason}. PolicyNumber: {policyNumber}", result.ToString(), lifeProPolicy.PolicyNumber);

                return;
            }

            if (result == GetPolicyResult.Found && policy?.ApplicationDate == null)
            {
                Logger.LogError("ApplicationDate is null for the policy number: {PolicyNumber}", policy?.PolicyNumber);

                return;
            }

            await StorePolicy(policy);
        }

        /// <summary>
        /// Generates and processes Policy, PolicyHierarchy, and AgentPolicyAccess records.
        /// </summary>
        /// <remarks>
        /// Channels are used to decouple generation and processing of each data type.
        /// TODO:
        /// - Revise <see cref="MigratePolicyEngine"/> to retrieve more data per trip to the database.
        /// - Current approach relies on way too many small trips to our SQL databases to be efficient.
        /// </remarks>
        /// <param name="recordsToProcess"></param>
        /// <returns></returns>
        [Trace]
        private async Task Backfill(List<CompanyCodeAndPolicyNumber> recordsToProcess, bool update)
        {
            Logger.LogInformation("{methodName} starting. Preparing to migrate {policyCount} policies.", nameof(Backfill), recordsToProcess.Count);

            var generatedPolicies = Channel.CreateUnbounded<Policy>(new UnboundedChannelOptions { SingleReader = true });
            var generatePolicyHierarchies = Channel.CreateUnbounded<Policy>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = true });
            var generatedPolicyHierarchies = Channel.CreateUnbounded<PolicyHierarchy>(new UnboundedChannelOptions { SingleReader = true });
            var processedPolicyHierarchies = Channel.CreateUnbounded<PolicyHierarchy>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = true });
            var generatedAgentPolicyAccessRecords = Channel.CreateUnbounded<AgentPolicyAccessRecord>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = true });

            await Task.WhenAll(
                GeneratePolicies(
                    recordsToProcess,
                    update,
                    generatedPolicies.Writer),
                ProcessPolicies(
                    generatedPolicies.Reader,
                    generatePolicyHierarchies.Writer),
                GeneratePolicyHierarchies(
                    generatePolicyHierarchies.Reader,
                    generatedPolicyHierarchies.Writer),
                ProcessPolicyHierarchies(
                    generatedPolicyHierarchies.Reader,
                    processedPolicyHierarchies.Writer),
                GenerateAgentPolicyAccess(
                    processedPolicyHierarchies.Reader,
                    generatedAgentPolicyAccessRecords.Writer),
                ProcessAgentPolicyAccess(
                    generatedAgentPolicyAccessRecords.Reader));

            Logger.LogInformation("{methodName} complete. Migrated {policyCount} policies.", nameof(Backfill), recordsToProcess.Count);
        }

        [Trace]
        private async Task GeneratePolicy(
             CompanyCodeAndPolicyNumber lifeProPolicy,
             ChannelWriter<Policy> successChannel)
        {
            var methodName = nameof(GeneratePolicy);

            try
            {
                (var result, var policy) = await PolicyEngine.GetPolicy(lifeProPolicy.PolicyNumber, lifeProPolicy.CompanyCode);

                if (result == GetPolicyResult.HasInitialPaymentDeclinedThatIsBeyondRetentionDuration)
                {
                    return;
                }

                if (result != GetPolicyResult.Found)
                {
                    LogError(result, lifeProPolicy, methodName);

                    return;
                }

                if (policy.ApplicationDate == null)
                {
                    LogError(GetPolicyResult.ApplicationDateNull, lifeProPolicy, methodName);
                }

                await successChannel.WriteAsync(policy);
            }
            catch (Exception ex)
            {
                LogError(GetPolicyResult.ExceptionThrown, lifeProPolicy, methodName, ex);
            }
        }

        [Trace]
        private async Task StorePolicy(Policy policy)
        {
            await EventsAccessor.CreateOrReplacePolicyAsync(policy);

            Logger.LogInformation("PolicyNumber: {PolicyNumber} has been stored.", policy.PolicyNumber);

            var policyHierarchy = await HierarchyEngine.GetPolicyHierarchy(policy);

            try
            {
                await EventsAccessor.UpdateOrCreatePolicyHierarchyAsync(policyHierarchy);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Processing event for BackfillManager PolicyNumber: Failed to add {PolicyNumber} to PolicyHierarchy.", policy.PolicyNumber);
            }

            var distinctAgents = HierarchyEngine.GetDistinctAgentIds(policyHierarchy.HierarchyBranches);

            foreach (var agent in distinctAgents)
            {
                var agentPolicyAccessId = await EventsAccessor.InsertAgentPolicyAccessAsync(agent, policy.PolicyNumber, policy.CompanyCode);

                if (agentPolicyAccessId == 0)
                {
                    Logger.LogError("Processing event for BackfillManager PolicyNumber: Failed to add {PolicyNumber} to AgentPolicyAccess for {Agent}.", policy.PolicyNumber, agent);
                }
            }
        }

        /// <summary>
        /// Generates Policy records from SQL data.
        /// </summary>
        /// <remarks>
        /// When "update" is true, deletes policies before generating them.
        /// </remarks>
        /// <param name="recordsToMigrate"></param>
        /// <param name="update"></param>
        /// <param name="policyChannelWriter"></param>
        /// <returns></returns>
        [Trace]
        private async Task GeneratePolicies(
            List<CompanyCodeAndPolicyNumber> recordsToMigrate,
            bool update,
            ChannelWriter<Policy> policyChannelWriter)
        {
            foreach (var record in recordsToMigrate)
            {
                if (update)
                {
                    await PolicyEngine.DeletePolicy(record.PolicyNumber, record.CompanyCode);
                }

                await GeneratePolicy(record, policyChannelWriter);
            }

            policyChannelWriter.Complete();
        }

        /// <summary>
        /// Save Policies to the database.
        /// </summary>
        /// <remarks>
        /// Save Policies to MongoDB.Events.Policies.
        /// Writes to the PolicyHierarchy generation channel to continue the chain.
        /// </remarks>
        /// <param name="policyChannelReader"></param>
        /// <param name="generatePolicyHierarchyChannel"></param>
        /// <returns></returns>
        [Trace]
        private async Task ProcessPolicies(
            ChannelReader<Policy> policyChannelReader,
            ChannelWriter<Policy> generatePolicyHierarchyChannel)
        {
            var currentCompanyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber();
            await foreach (var generatedPolicy in policyChannelReader.ReadAllAsync())
            {
                try
                {
                    currentCompanyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber(generatedPolicy.CompanyCode, generatedPolicy.PolicyNumber);
                    generatePolicyHierarchyChannel.TryWrite(generatedPolicy);
                    await EventsAccessor.CreatePolicyAsync(generatedPolicy);
                }
                catch (Exception ex)
                {
                    LogError(
                        GetPolicyResult.ExceptionThrown,
                        currentCompanyCodeAndPolicyNumber,
                        nameof(ProcessPolicies),
                        ex);
                }
            }

            generatePolicyHierarchyChannel.Complete();
        }

        /// <summary>
        /// Generate PolicyHierarchy records from Policy records.
        /// </summary>
        /// <param name="policiesToProcess"></param>
        /// <param name="policyHierarchyWriter"></param>
        /// <returns></returns>
        [Trace]
        private async Task GeneratePolicyHierarchies(
            ChannelReader<Policy> policiesToProcess,
            ChannelWriter<PolicyHierarchy> policyHierarchyWriter)
        {
            var currentCompanyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber();

            await foreach (var policy in policiesToProcess.ReadAllAsync())
            {
                try
                {
                    currentCompanyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber(policy.CompanyCode, policy.PolicyNumber);
                    var policyHierarchy = await HierarchyEngine.GetPolicyHierarchy(policy);
                    policyHierarchyWriter.TryWrite(policyHierarchy);
                }
                catch (Exception ex)
                {
                    LogError(
                        GetPolicyResult.ExceptionThrown,
                        currentCompanyCodeAndPolicyNumber,
                        nameof(GeneratePolicyHierarchies),
                        ex);
                }
            }

            policyHierarchyWriter.Complete();
        }

        /// <summary>
        /// Save PolicyHierarchy records to the database and writes to next channel.
        /// </summary>
        /// <param name="generatedPolicyHierarchies"></param>
        /// <param name="processedPolicyHierarchies"></param>
        /// <returns></returns>
        [Trace]
        private async Task ProcessPolicyHierarchies(
            ChannelReader<PolicyHierarchy> generatedPolicyHierarchies,
            ChannelWriter<PolicyHierarchy> processedPolicyHierarchies)
        {
            var currentCompanyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber();
            await foreach (var policyHierarchy in generatedPolicyHierarchies.ReadAllAsync())
            {
                try
                {
                    currentCompanyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber(policyHierarchy.CompanyCode, policyHierarchy.PolicyNumber);
                    processedPolicyHierarchies.TryWrite(policyHierarchy);
                    await EventsAccessor.UpdateOrCreatePolicyHierarchyAsync(policyHierarchy);
                }
                catch (Exception ex)
                {
                    LogError(
                        GetPolicyResult.ExceptionThrown,
                        currentCompanyCodeAndPolicyNumber,
                        nameof(ProcessPolicyHierarchies),
                        ex);
                }
            }

            processedPolicyHierarchies.Complete();
        }

        /// <summary>
        /// Generate AgentPolicyAccess records from PolicyHierarchy records.
        /// </summary>
        /// <param name="policyHierarchiesToProcess"></param>
        /// <param name="generatedAgentPolicyAccess"></param>
        /// <returns></returns>
        [Trace]
        private async Task GenerateAgentPolicyAccess(
            ChannelReader<PolicyHierarchy> policyHierarchiesToProcess,
            ChannelWriter<AgentPolicyAccessRecord> generatedAgentPolicyAccess)
        {
            var currentCompanyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber();
            await foreach (var policyHierarchy in policyHierarchiesToProcess.ReadAllAsync())
            {
                try
                {
                    var distinctAgents = HierarchyEngine.GetDistinctAgentIds(policyHierarchy.HierarchyBranches);
                    foreach (var agentId in distinctAgents)
                    {
                        currentCompanyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber(policyHierarchy.CompanyCode, policyHierarchy.PolicyNumber);
                        var agentAccessRecord =
                            new AgentPolicyAccessRecord(
                                new AgentIdAndCompanyCode(agentId, policyHierarchy.CompanyCode),
                                policyHierarchy.PolicyNumber);

                        generatedAgentPolicyAccess.TryWrite(agentAccessRecord);
                    }
                }
                catch (Exception ex)
                {
                    LogError(
                        GetPolicyResult.ExceptionThrown,
                        currentCompanyCodeAndPolicyNumber,
                        nameof(GenerateAgentPolicyAccess),
                        ex);
                }
            }

            generatedAgentPolicyAccess.Complete();
        }

        /// <summary>
        /// Save AgentPolicyAccess records to the database.
        /// </summary>
        /// <param name="agentPolicyAccessToProcess"></param>
        /// <returns></returns>
        [Trace]
        private async Task ProcessAgentPolicyAccess(
            ChannelReader<AgentPolicyAccessRecord> agentPolicyAccessToProcess)
        {
            var currentCompanyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber();
            await foreach (var record in agentPolicyAccessToProcess.ReadAllAsync())
            {
                currentCompanyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber(record.AgentIdAndCompanyCode.CompanyCode, record.PolicyNumber);
                try
                {
                    await EventsAccessor.InsertAgentPolicyAccessAsync(
                        record.AgentIdAndCompanyCode.AgentId,
                        record.PolicyNumber,
                        record.AgentIdAndCompanyCode.CompanyCode);
                }
                catch (Exception ex)
                {
                    LogError(
                        GetPolicyResult.ExceptionThrown,
                        currentCompanyCodeAndPolicyNumber,
                        nameof(ProcessAgentPolicyAccess),
                        ex);
                }
            }
        }

        [Trace]
        private List<CompanyCodeAndPolicyNumber> GetUnmigratedPPOLCRecords()
        {
            var ppolcRecords = DataStoreAccessor.GetMigratablePPOLCRecords();
            var policyNumbersInCollection = EventsAccessor.GetAllCompanyCodesAndPolicyNumbers();

            return ppolcRecords.Where(ppolc => !policyNumbersInCollection.Contains(ppolc)).ToList();
        }

        private void LogError(
            GetPolicyResult getPolicyResult,
            CompanyCodeAndPolicyNumber currentCompanyCodeAndPolicyNumber,
            string methodName,
            Exception? ex = null)
        {
            Logger.LogError(
                "Error encountered while migrating policy with policy number '{policyNumber}' in method '{methodName}'. ResultCode: {resultCode}. Exception: {ex}",
                currentCompanyCodeAndPolicyNumber.PolicyNumber,
                methodName,
                getPolicyResult,
                ExceptionToString(ex));
        }

        private string ExceptionToString(Exception exception)
        {
            if (exception == null)
            {
                return string.Empty;
            }

            var message = exception.Message;
            if (exception.InnerException != null)
            {
                message += Environment.NewLine + exception.InnerException.Message + Environment.NewLine;
            }

            message += exception.StackTrace;

            return message;
        }
    }
}