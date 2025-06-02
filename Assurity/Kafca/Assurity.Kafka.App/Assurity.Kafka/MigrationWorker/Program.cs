namespace Assurity.Kafka.AppService
{
    using System;
    using System.Reflection;
    using Assurity.Kafka.AppService.InversionOfControl;
    using Assurity.Kafka.MigrationService;
    using Assurity.Logging.Extensions.NLogEx.LogsInContextLogger;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using MigrationWorker;
    using NLog;
    using NLog.Extensions.Logging;
    using NLog.Web;
    using Quartz;

    public class Program
    {
        public static void Main(string[] args)
        {
            const string Application = "Assurity.Kafka.AppService";
            var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

            try
            {
                logger.Info($"Starting {Application}");

                var builder = CreateHostBuilder(args);

                var build = builder.Build();

                var configuration = build.Services.GetRequiredService<IConfiguration>();

                // Get logger updated with container environment information via overridden app settings file
                logger = LogManager.Setup()
                                   .LoadConfigurationFromSection(configuration)
                                   .GetCurrentClassLogger();
                build.Run();

                logger.Info($"Started {Application}");
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{Application} failed to start");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var configurationManager = new Assurity.Kafka.Utilities.Config.ConfigurationManager(hostContext.Configuration);
                    services.AddQuartz(q =>
                    {
                        var jobKey = new JobKey("FlagChangedPoliciesJobKey");
                        q.AddJob<FlagPastDueAndPendingPoliciesJob>(opts => opts.WithIdentity(jobKey));
                        q.AddTrigger(opts => opts
                             .ForJob(jobKey)
                             .WithIdentity("FlagChangedPoliciesJob-trigger")
                             .WithCronSchedule("0 0 11 * * ?")); // this is UTC, so it will run at 0600hrs central
                    });
                    services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
                    services.AddUtilityDependencies();
                    services.AddDbContexts(configurationManager);
                    services.AddAccessorDependencies(configurationManager);
                    services.AddEngineDependencies();
                    services.AddManagerDependencies();
                    services.AddHostedService<MigrationWorker>();
                    services.AddHealthChecks();
                    services.AddLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.AddProvider(new LogsInContextLoggerProvider("LogsInContextLogger"));
                    });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                    webBuilder.Configure(app => { }));
        }
    }
}