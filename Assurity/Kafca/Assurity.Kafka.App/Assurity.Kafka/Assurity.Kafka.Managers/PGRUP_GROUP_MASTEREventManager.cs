namespace Assurity.Kafka.Managers
{
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Managers.Interfaces;
    using Microsoft.Extensions.Logging;
    using NewRelic.Api.Agent;

    public class PGRUP_GROUP_MASTEREventManager : IPGRUP_GROUP_MASTEREventManager
    {
        public PGRUP_GROUP_MASTEREventManager(
            IConsumerPolicyEngine policyEngine,
            IEventsAccessor eventsAccessor,
            ILogger<PGRUP_GROUP_MASTEREventManager> logger)
        {
            PolicyEngine = policyEngine;
            EventsAccessor = eventsAccessor;
            Logger = logger;
        }

        private IConsumerPolicyEngine PolicyEngine { get; }

        private IEventsAccessor EventsAccessor { get; }

        private ILogger<PGRUP_GROUP_MASTEREventManager> Logger { get; }

        [Transaction]
        public async Task ProcessEvent(PGRUP_GROUP_MASTER groupMaster)
        {
            try
            {
                var policies = await EventsAccessor.GetPoliciesByGroupNumber(groupMaster.GROUP_NUMBER);

                if (policies == null || !policies.Any())
                {
                    Logger.LogWarning("Unable to find policies in MongoDb with an Employer.Number value of {groupNumber}", groupMaster.GROUP_NUMBER);

                    return;
                }

                long updatedRecords = 0;

                // Rebuild each Policy.Employer object on the list of policies returned
                foreach (var policy in policies)
                {
                    policy.Employer = await PolicyEngine.GetEmployer(policy.PolicyNumber, policy.CompanyCode);
                    updatedRecords += await EventsAccessor.UpdatePolicyAsync(
                        policy, policy.Employer, nameof(policy.Employer));
                }

                Logger
                    .LogInformation("Employer information has updated for {UpdatedRecords} policies.", updatedRecords);
            }
            catch (Exception exception)
            {
                Logger
                    .LogError("An error occurred when updating Policies with Group Number: {GROUP_NUMBER}, Exception: {exception}", groupMaster.GROUP_NUMBER, exception);
            }
        }
    }
}
