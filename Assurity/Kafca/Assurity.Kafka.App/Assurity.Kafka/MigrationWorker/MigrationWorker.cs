namespace MigrationWorker
{
    using Assurity.Kafka.Managers.Interfaces;
    using Assurity.Kafka.Utilities.Config;

    public class MigrationWorker : BackgroundService
    {
        public MigrationWorker(
            IServiceProvider services,
            IConfigurationManager configurationManager)
        {
            Services = services;
            Config = configurationManager;
        }

        private IServiceProvider Services { get; }

        private IConfigurationManager Config { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = Services.CreateScope();
                try
                {
                    var backfillManager = scope.ServiceProvider.GetRequiredService<IBackfillManager>();
                    await backfillManager.BackFillPolicies(Config.MongoDbExecutionMode);
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<Logger<MigrationWorker>>();
                    logger.LogError("MigrationWorker encountered the following exception while executing {executionMode}: {ex}", Config.MongoDbExecutionMode, ex);
                }

                // All policies in the list have been processed. Wait 15 mins to check again
                await Task.Delay(900000, stoppingToken);
            }
        }
    }
}