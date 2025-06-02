namespace Assurity.Kafka.Engines.Hierarchy
{
    using Assurity.Kafka.Accessors;
    using Assurity.PolicyInfo.Contracts.V1;
    using NewRelic.Api.Agent;

    public class ConsumerHierarchyEngine : BaseHierarchyEngine, IConsumerHierarchyEngine
    {
        private const bool IsMigrationWorker = false;

        public ConsumerHierarchyEngine(
            ILifeProAccessor lifeProAccessor,
            IGlobalDataAccessor globalDataAccessor,
            ISupportDataAccessor supportDataAccessor)
            : base(null, lifeProAccessor, globalDataAccessor, supportDataAccessor)
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
