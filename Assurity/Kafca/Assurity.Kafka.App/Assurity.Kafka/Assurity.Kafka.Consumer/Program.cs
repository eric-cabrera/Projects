namespace Assurity.Kafka.Consumer
{
    using System;
    using System.Reflection;
    using Assurity.Kafka.Consumer.InversionOfControl;
    using Assurity.Logging.Extensions.NLogEx.LogsInContextLogger;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NLog;
    using NLog.Extensions.Logging;
    using NLog.Web;

    public class Program
    {
        public static void Main(string[] args)
        {
            const string Application = "Assurity.Kafka.Consumer";
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

                    services.AddUtilityDependencies();
                    services.AddDbContexts(configurationManager);
                    services.AddAccessorDependencies(configurationManager);
                    services.AddEngineDependencies();
                    services.AddManagerDependencies();
                    services.AddControllerDependencies();
                    services.AddHostedService<SubscriptionService>();
                    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
                    services.AddHealthChecks();
                    services.AddLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.AddProvider(new LogsInContextLoggerProvider("LogsInContextLogger"));
                    });
                    services.AddHostedService<SubscriptionService>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                    webBuilder.Configure(app => { }));
        }
    }
}