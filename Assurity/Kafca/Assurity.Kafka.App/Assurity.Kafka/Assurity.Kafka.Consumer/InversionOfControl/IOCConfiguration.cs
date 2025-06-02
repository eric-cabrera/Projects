namespace Assurity.Kafka.Consumer.InversionOfControl
{
    using Assurity.Common.Cryptography;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Configuration;
    using Assurity.Kafka.Accessors.Context;
    using Assurity.Kafka.Consumer.Controllers;
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
            IConfigurationManager configurationManager)
        {
            services.AddTransient<IDataStoreAccessor, DataStoreAccessor>();
            services.AddTransient<ILifeProAccessor, LifeProAccessor>();
            services.AddTransient<IGlobalDataAccessor, GlobalDataAccessor>();
            services.AddTransient<ISupportDataAccessor, SupportDataAccessor>();
            services.AddTransient<IEventsAccessor, EventsAccessor>();

            MongoDbConfiguration.ConfigureAndPopulateMongoDB(services, configurationManager);
        }

        public static void AddEngineDependencies(
            this IServiceCollection services)
        {
            services.AddTransient<IConsumerPolicyEngine, ConsumerPolicyEngine>();
            services.AddTransient<IConsumerHierarchyEngine, ConsumerHierarchyEngine>();
            services.AddTransient<IBenefitMapper, BenefitMapper>();
            services.AddTransient<IParticipantMapper, ParticipantMapper>();
            services.AddTransient<IPolicyMapper, PolicyMapper>();
            services.AddTransient<IRequirementsMapper, RequirementsMapper>();
        }

        public static void AddManagerDependencies(
            this IServiceCollection services)
        {
            services.AddTransient<IPPOLCEventManager, PPOLCEventManager>();
            services.AddTransient<IPPOLM_POLICY_BENEFIT_MISCEventManager, PPOLM_POLICY_BENEFIT_MISCEventManager>();
            services.AddTransient<IPADDREventManager, PADDREventManager>();
            services.AddTransient<IPMUIN_MULTIPLE_INSUREDSEventManager, PMUIN_MULTIPLE_INSUREDSEventManager>();
            services.AddTransient<IPNALKEventManager, PNALKEventManager>();
            services.AddTransient<IPNAMEEventManager, PNAMEEventManager>();
            services.AddTransient<IPPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager, PPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager>();
            services.AddTransient<IPPBEN_POLICY_BENEFITS_TYPES_BFEventManager, PPBEN_POLICY_BENEFITS_TYPES_BFEventManager>();
            services.AddTransient<IPPBEN_POLICY_BENEFITS_TYPES_SLEventManager, PPBEN_POLICY_BENEFITS_TYPES_SLEventManager>();
            services.AddTransient<IPPBEN_POLICY_BENEFITS_TYPES_SPEventManager, PPBEN_POLICY_BENEFITS_TYPES_SPEventManager>();
            services.AddTransient<IPPBEN_POLICY_BENEFITS_TYPES_SUEventManager, PPBEN_POLICY_BENEFITS_TYPES_SUEventManager>();
            services.AddTransient<IPPBEN_POLICY_BENEFITSEventManager, PPBEN_POLICY_BENEFITSEventManager>();
            services.AddTransient<IPPEND_NEW_BUSINESS_PENDINGEventManager, PPEND_NEW_BUSINESS_PENDINGEventManager>();
            services.AddTransient<IPPEND_NEW_BUS_PEND_UNDERWRITINGEventManager, PPEND_NEW_BUS_PEND_UNDERWRITINGEventManager>();
            services.AddTransient<IPRELA_RELATIONSHIP_MASTEREventManager, PRELA_RELATIONSHIP_MASTEREventManager>();
            services.AddTransient<IPCOMC_COMMISSION_CONTROLEventManager, PCOMC_COMMISSION_CONTROLEventManager>();
            services.AddTransient<IPCOMC_COMMISSION_CONTROL_TYPE_SEventManager, PCOMC_COMMISSION_CONTROL_TYPE_SEventManager>();
            services.AddTransient<IPHIER_AGENT_HIERARCHYEventManager, PHIER_AGENT_HIERARCHYEventManager>();
            services.AddTransient<IPMEDREventManager, PMEDREventManager>();
            services.AddTransient<IPRQRMEventManager, PRQRMEventManager>();
            services.AddTransient<IPRQRMTBLEventManager, PRQRMTBLEventManager>();
            services.AddTransient<IPGRUP_GROUP_MASTEREventManager, PGRUP_GROUP_MASTEREventManager>();
            services.AddTransient<ISysACAgentDataEventManager, SysACAgentDataEventManager>();
            services.AddTransient<ISysACAgentMarketCodesEventManager, SysACAgentMarketCodesEventManager>();
            services.AddTransient<ISysZ9ProcessEventManager, SysZ9ProcessEventManager>();
            services.AddTransient<ISysNBRequirementsEventManager, SysNBRequirementsEventManager>();
            services.AddTransient<IQUEUESEventManager, QUEUESEventManager>();
            services.AddTransient<IPACTGEventManager, PACTGEventManager>();
            services.AddTransient<IPBDRVEventManager, PBDRVEventManager>();
            services.AddTransient<IPACON_ANNUITY_POLICYEventManager, PACON_ANNUITY_POLICYEventManager>();
        }

        public static void AddControllerDependencies(
            this IServiceCollection services)
        {
            services.AddTransient<ITopicEventProcessor, TopicEventProcessor>();
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

            services.AddDbContextFactory<LifeProContext>(dbContextOptionsBuilder =>
                dbContextOptionsBuilder.UseSqlServer(
                    configuration.LifeProConnectionString,
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