namespace Assurity.Kafka.AppService.InversionOfControl
{
    using Assurity.Common.Cryptography;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Configuration;
    using Assurity.Kafka.Accessors.Context;
    using Assurity.Kafka.Engines.Hierarchy;
    using Assurity.Kafka.Engines.Mapping;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Managers;
    using Assurity.Kafka.Managers.Interfaces;
    using Assurity.Kafka.Utilities.Config;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class IOCConfiguration
    {
        public static void AddAccessorDependencies(
            this IServiceCollection services,
            IConfigurationManager manager)
        {
            services.AddTransient<IDataStoreAccessor, DataStoreAccessor>();
            services.AddTransient<IGlobalDataAccessor, GlobalDataAccessor>();
            services.AddTransient<ISupportDataAccessor, SupportDataAccessor>();
            services.AddTransient<IEventsAccessor, EventsAccessor>();

            MongoDbConfiguration.ConfigureAndPopulateMongoDB(services, manager);
        }

        public static void AddEngineDependencies(
            this IServiceCollection services)
        {
            services.AddTransient<IMigratePolicyEngine, MigratePolicyEngine>();
            services.AddTransient<IMigrateHierarchyEngine, MigrateHierarchyEngine>();
            services.AddTransient<IPolicyMapper, PolicyMapper>();
            services.AddTransient<IBenefitMapper, BenefitMapper>();
            services.AddTransient<IParticipantMapper, ParticipantMapper>();
            services.AddTransient<IRequirementsMapper, RequirementsMapper>();
        }

        public static void AddManagerDependencies(
            this IServiceCollection services)
        {
            services.AddTransient<IBackfillManager, BackfillManager>();
        }

        public static void AddDbContexts(
            this IServiceCollection services,
            IConfigurationManager configuration)
        {
            services.AddDbContextFactory<DataStoreContext>(dbContextOptionsBuilder =>
                dbContextOptionsBuilder.UseSqlServer(
                    configuration.DataStoreConnectionString,
                    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure())
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            services.AddDbContextFactory<GlobalDataContext>(dbContextOptionsBuilder =>
                dbContextOptionsBuilder.UseSqlServer(
                    configuration.GlobalDataConnectionString,
                    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure())
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            services.AddDbContextFactory<SupportDataContext>(dbContextOptionsBuilder =>
                dbContextOptionsBuilder.UseSqlServer(
                    configuration.SupportDataConnectionString,
                    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure())
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
        }

        public static void AddUtilityDependencies(
            this IServiceCollection services)
        {
            services.AddTransient<IAesEncryptor, AesEncryptor>();
            services.AddTransient<IConfigurationManager, ConfigurationManager>();
        }
    }
}