namespace Assurity.Kafka.Accessors
{
    using Assurity.Kafka.Accessors.Context;
    using Assurity.Kafka.Utilities.Extensions;
    using Microsoft.EntityFrameworkCore;
    using MongoDB.Driver;
    using NewRelic.Api.Agent;

    public class SupportDataAccessor : ISupportDataAccessor
    {
        public SupportDataAccessor(IDbContextFactory<SupportDataContext> supportDataContextFactory)
        {
            SupportDataContextFactory = supportDataContextFactory;
        }

        private IDbContextFactory<SupportDataContext> SupportDataContextFactory { get; }

        [Trace]
        public async Task<List<string?>> GetQueueDescriptions()
        {
            using var supportDataContext = await SupportDataContextFactory.CreateDbContextAsync();

            var result = await supportDataContext.AgentUseQueues
                .Select(agentUseQueue =>
                    !string.IsNullOrWhiteSpace(agentUseQueue.QueueDescription) ? agentUseQueue.QueueDescription.ToLower() : null)
                .ToListAsync();

            PolicyInfoExtensions.TrimStringProperties(result);
            return result;
        }

        [Trace]
        public async Task<bool> IsJustInTimeQueue(string queue)
        {
            using var supportDataContext = await SupportDataContextFactory.CreateDbContextAsync();

            var result = await supportDataContext.AgentUseQueues
                .Where(x => x.QueueDescription == queue)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return result != null;
        }
    }
}
