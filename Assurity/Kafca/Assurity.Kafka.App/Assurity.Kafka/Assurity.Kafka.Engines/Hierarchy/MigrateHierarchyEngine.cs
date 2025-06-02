namespace Assurity.Kafka.Engines.Hierarchy
{
    using Assurity.Kafka.Accessors;
    using Assurity.PolicyInfo.Contracts.V1;
    using NewRelic.Api.Agent;

    public class MigrateHierarchyEngine : BaseHierarchyEngine, IMigrateHierarchyEngine
    {
        private const bool IsMigrationWorker = true;

        public MigrateHierarchyEngine(
            IDataStoreAccessor dataStoreAccessor,
            IGlobalDataAccessor globalDataAccessor,
            ISupportDataAccessor supportDataAccessor)
            : base(dataStoreAccessor, null, globalDataAccessor, supportDataAccessor)
        {
        }

        [Trace]
        public async Task<PolicyHierarchy> GetPolicyHierarchy(Policy policy)
        {
            return await GetPolicyHierarchy(IsMigrationWorker, policy);
        }

        [Trace]
        public async Task<List<AgentHierarchy>> BuildAgentHierarchy(List<Agent> agents, DateTime applicationDate)
        {
            return await BuildAgentHierarchy(IsMigrationWorker, agents, applicationDate);
        }
    }
}
