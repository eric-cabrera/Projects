using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Assurity.Kafka.Engines.Tests")]

namespace Assurity.Kafka.Engines.Policy
{
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Engines.Mapping;
    using Assurity.Kafka.Utilities.Config;
    using Assurity.Kafka.Utilities.Enums;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.Extensions.Logging;
    using NewRelic.Api.Agent;

    public class MigratePolicyEngine : BasePolicyEngine, IMigratePolicyEngine
    {
        private const bool IsMigrationWorker = true;

        public MigratePolicyEngine(
            IDataStoreAccessor dataStoreAccessor,
            IGlobalDataAccessor globalDataAccessor,
            ISupportDataAccessor supportDataAccessor,
            IEventsAccessor eventsAccessor,
            IConfigurationManager configurationManager,
            ILogger<MigratePolicyEngine> migrateLogger,
            IPolicyMapper policyMapper)
            : base(dataStoreAccessor, null, globalDataAccessor, supportDataAccessor, eventsAccessor, configurationManager, migrateLogger, policyMapper)
        {
        }

        [Transaction]
        public async Task<(GetPolicyResult, Policy?)> GetPolicy(string policyNumber, string companyCode)
        {
            return await GetPolicy(IsMigrationWorker, policyNumber, companyCode);
        }

        [Trace]
        public async Task DeletePolicy(string policyNumber, string companyCode)
        {
            await DeleteAllPolicyData(policyNumber, companyCode);
        }
    }
}