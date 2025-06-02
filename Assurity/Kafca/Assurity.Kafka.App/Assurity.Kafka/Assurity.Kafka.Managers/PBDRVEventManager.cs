namespace Assurity.Kafka.Managers
{
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Managers.Interfaces;
    using Assurity.Kafka.Utilities.Extensions;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;
    using NewRelic.Api.Agent;

    public class PBDRVEventManager : IPBDRVEventManager
    {
        public PBDRVEventManager(
            ILogger<PBDRVEventManager> logger,
            IEventsAccessor eventsAccessor,
            ILifeProAccessor lifeProAccessor,
            IMongoClient mongoClient)
        {
            Logger = logger;
            EventsAccessor = eventsAccessor;
            LifeProAccessor = lifeProAccessor;
            MongoClient = mongoClient;
        }

        private ILogger<PBDRVEventManager> Logger { get; }

        private IEventsAccessor EventsAccessor { get; }

        private ILifeProAccessor LifeProAccessor { get; }

        private IMongoClient MongoClient { get; }

        [Transaction]
        public async Task ProcessEvent(PBDRV pbdrv)
        {
            await UpdatePastDuePolicies(pbdrv.BATCH_START_DATE);
            await DeleteAgingData(pbdrv.BATCH_START_DATE);
        }

        [Trace]
        private async Task UpdatePastDuePolicies(int batchStartDate)
        {
            try
            {
                var pastDuePolicyNumbers = await LifeProAccessor.GetPastDuePolicyNumbers(batchStartDate);

                // TODO: The PastDue policies returned above exclude UL when processing PastDue but our Migrator code does not. It instead checks ULs to see if they are in
                // their grace period and, if so, mark them as PastDue. We need code here to 1) retrieve all UL policies with a BF_DATE_NEGATIVE value greater than 0 but less
                // than today and 2) append the policy numbers to the above policyNumbers variable.
                if (pastDuePolicyNumbers?.Any() ?? false)
                {
                    var updatedRecords = await EventsAccessor
                        .UpdatePastDuePoliciesAsync(pastDuePolicyNumbers);

                    Logger.LogWarning("PastDue information has updated for {updatedRecords} policies.", updatedRecords);
                }
                else
                {
                    Logger.LogWarning(
                        "No policies were found in Mongo for the LifePro policies associated with PaidToDate < BATCH_START_DATE: {batchStartDate}",
                        batchStartDate);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Processing event for PBDRV with BATCH_START_DATE: {batchStartDate}", batchStartDate);
            }
        }

        [Trace]
        private async Task DeleteAgingData(int batchStartDate)
        {
            var batchStartDateTime = batchStartDate.ToLifeProDateInteger().ToNullableDateTime();

            if (batchStartDateTime.HasValue)
            {
                await DeleteAgingInitialPaymentDeclinedData(batchStartDateTime.Value);
                await DeleteRecordsWithNullEmployer(batchStartDateTime.Value);
            }
        }

        [Trace]
        private async Task DeleteAgingInitialPaymentDeclinedData(DateTime batchStartDateTime)
        {
            using (var session = await MongoClient.StartSessionAsync())
            {
                const int batchSize = 1000;
                var passedRetentionDurationPolicies = await EventsAccessor
                    .GetInitialPaymentDeclinedPoliciesWithPassedRetentionDurationAsync(batchStartDateTime);

                if (passedRetentionDurationPolicies?.Any() ?? false)
                {
                    var passedRetentionDurationPolicyNumbers = passedRetentionDurationPolicies.Select(policy => policy.PolicyNumber).ToList();

                    int indexOfLastPolicyNumbersTaken = 0;
                    while (indexOfLastPolicyNumbersTaken < passedRetentionDurationPolicyNumbers.Count)
                    {
                        var policyNumbersTaken = passedRetentionDurationPolicyNumbers.Skip(indexOfLastPolicyNumbersTaken)
                            .Take(batchSize).ToList();

                        try
                        {
                            session.StartTransaction();

                            var deletedRecords = await EventsAccessor.DeletePoliciesAsync(session, policyNumbersTaken);
                            await EventsAccessor.DeletePolicyHierarchiesAsync(session, policyNumbersTaken);
                            await EventsAccessor.UpdateAgentPolicyAccessListAsync(session, policyNumbersTaken);

                            await session.CommitTransactionAsync();
                            Logger
                                .LogWarning("{deletedRecords} policies were deleted due to 1st payment fail.", deletedRecords);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, "Processing event for PBDRV: Failed to DeleteAgingInitialPaymentDeclinedData for BatchStartDateTime: {batchStartDateTime}", batchStartDateTime);
                            await session.AbortTransactionAsync();
                        }

                        indexOfLastPolicyNumbersTaken += policyNumbersTaken.Count();
                    }
                }
                else
                {
                    Logger.LogWarning("No policies were deleted due to 1st payment fail.");
                }
            }
        }

        [Trace]
        private async Task DeleteRecordsWithNullEmployer(DateTime batchStartDateTime)
        {
            using (var session = await MongoClient.StartSessionAsync())
            {
                const int batchSize = 1000;
                var policyNumbersReadyForDeletion = EventsAccessor.GetPolicyNumbersForDeletion(batchStartDateTime);

                if (policyNumbersReadyForDeletion?.Any() ?? false)
                {
                    int indexOfLastPolicyNumbersTaken = 0;
                    while (indexOfLastPolicyNumbersTaken < policyNumbersReadyForDeletion.Count())
                    {
                        var policyNumbersTaken =
                            policyNumbersReadyForDeletion
                            .Skip(indexOfLastPolicyNumbersTaken)
                            .Take(batchSize)
                            .ToList();

                        try
                        {
                            session.StartTransaction();

                            var deletedRecords = await EventsAccessor.DeletePoliciesAsync(session, policyNumbersTaken);
                            await EventsAccessor.DeletePolicyHierarchiesAsync(session, policyNumbersTaken);
                            await EventsAccessor.UpdateAgentPolicyAccessListAsync(session, policyNumbersTaken);

                            await session.CommitTransactionAsync();
                            Logger
                                .LogWarning("{deletedRecords} policies were deleted due to terminated age.", deletedRecords);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, "Processing event for PBDRV: Failed to DeleteAgingTerminatedData for BatchStartDateTime: {batchStartDateTime}", batchStartDateTime);
                            await session.AbortTransactionAsync();
                        }

                        indexOfLastPolicyNumbersTaken += policyNumbersTaken.Count();
                    }
                }
            }
        }
    }
}