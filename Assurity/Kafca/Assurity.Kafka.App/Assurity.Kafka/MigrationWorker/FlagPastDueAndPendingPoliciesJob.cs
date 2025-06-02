namespace Assurity.Kafka.MigrationService
{
    using Assurity.Kafka.Managers.Interfaces;
    using Quartz;

    public class FlagPastDueAndPendingPoliciesJob : IJob
    {
        private readonly IServiceProvider services;

        public FlagPastDueAndPendingPoliciesJob(IServiceProvider services)
        {
            this.services = services;
        }

        public Task Execute(IJobExecutionContext context)
        {
            using (var scope = services.CreateScope())
            {
                var backfillManager = scope.ServiceProvider.GetRequiredService<IBackfillManager>();

                backfillManager.FlagPastDuePolicies();

                backfillManager.FlagPendingPolicies();
            }

            return Task.CompletedTask;
        }
    }
}
