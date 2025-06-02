namespace Assurity.Kafka.Utilities.Config
{
    using System;
    using Assurity.Common.Cryptography;
    using Microsoft.Extensions.Configuration;

    public class ConfigurationManager : IConfigurationManager
    {
        private const string SharedSecret = "kk3U1BpfFAIcAMXcu5Sx2g==";

        public ConfigurationManager(
            IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            AesEncryptor = new AesEncryptor();
        }

        public IConfiguration Configuration { get; }

        public IAesEncryptor AesEncryptor { get; }

        public string Environment => Configuration["Environment"];

        // LP databases
        public string LifeProConnectionString => Decrypt(Configuration["ConnectionStrings:LifeProConnectionString"]);

        public string DataStoreConnectionString => Decrypt(Configuration["ConnectionStrings:DataStoreConnectionString"]);

        public string GlobalDataConnectionString => Decrypt(Configuration["ConnectionStrings:GlobalDataConnectionString"]);

        public string SupportDataConnectionString => Decrypt(Configuration["ConnectionStrings:SupportDataConnectionString"]);

        // Mongo Db
        public string MongoDbConnectionString => Decrypt(Configuration["Cache:MongoDbConnectionString"]);

        public string MongoDbDatabaseName => Configuration["Cache:MongoDbDatabaseName"];

        public string MongoPolicyCollectionName => Configuration["Cache:MongoDbPolicyCollectionName"];

        public string MongoPolicyHierarchyCollectionName => Configuration["Cache:MongoDbPolicyHierarchyCollectionName"];

        public string MongoAgentPolicyAccessCollectionName => Configuration["Cache:MongoDbAgentPolicyAccessCollectionName"];

        public string MongoPPOLCEventsCollectionName => Configuration["Cache:MongoDbPPOLCEventsCollectionName"];

        public string MongoDbClientCertificatePassword => Decrypt(Configuration["Cache:MongoDbClientCertificatePassword"]);

        public string MongoRequirementMappingCollectionName => Configuration["Cache:MongoDbRequirementMappingCollectionName"];

        public string MongoBenefitOptionsMappingCollectionName => Configuration["Cache:MongoDbBenefitOptionsMappingCollectionName"];

        public string MongoDbExecutionMode => Configuration["Cache:MongoDbExecutionMode"];

        public string UseTls => Configuration["Cache:UseTls"];

        // Kafka
        public string KafkaClientId => Configuration["Kafka:ClientId"];

        public string KafkaGroupId => Configuration["Kafka:GroupId"];

        public int KafkaAutoOffsetReset => int.Parse(Configuration["Kafka:AutoOffsetReset"]);

        public string KafkaBootstrapServers => Configuration["Kafka:BootstrapServers"];

        public string KafkaSaslUsername => Configuration["Kafka:SaslUsername"];

        public string KafkaSaslPassword => Configuration["Kafka:SaslPassword"];

        public string KafkaSslCaLocation => Configuration["Kafka:SslCaLocation"];

        public string KafkaSchemaRegistryURL => Configuration["Kafka:SchemaRegistryURL"];

        public string KafkaSchemaRegistryBasicAuthUserInfo => Configuration["Kafka:SchemaRegistryBasicAuthUserInfo"];

        public bool KafkaEnableAutoOffsetStore => bool.Parse(Configuration["Kafka:EnableAutoOffsetStore"]);

        public int KafkaPartitionAssignmentStrategy => int.Parse(Configuration["Kafka:PartitionAssignmentStrategy"]);

        public int KafkaConsumerFailRetry => int.Parse(Configuration["Kafka:KafkaConsumerFailRetry"] ?? "600");

        public int KafkaSlowConsumerUpdateThreshold => int.Parse(Configuration["Kafka:KafkaSlowConsumerUpdateThreshold"] ?? "25");

        public string DeadLetterTopicName => Configuration["Kafka:DeadLetterTopicName"] ?? "ConsumerDeadLetter";

        public string KafkaDebug => string.IsNullOrEmpty(Configuration["Kafka:KafkaDebug"]) ? null : Configuration["Kafka:KafkaDebug"];

        public int KafkaFetchMaxBytes => int.Parse(Configuration["Kafka:KafkaFetchMaxBytes"] ?? "52428800");

        public int KafkaMaxPartitionFetchBytes => int.Parse(Configuration["Kafka:KafkaMaxPartitionFetchBytes"] ?? "1048576");

        public int KafkaQueuedMaxMessagesKilobytes => int.Parse(Configuration["Kafka:KafkaQueuedMaxMessagesKilobytes"] ?? "65536");

        public int KafkaMessageMaxBytes => int.Parse(Configuration["Kafka:KafkaMessageMaxBytes"] ?? "1000000");

        public double LogOffsetInfoIntervalSec => double.Parse(Configuration["Kafka:LogOffsetInfoIntervalSec"] ?? "120");

        public int InitialPaymentDeclinedRetentionDays => int.Parse(Configuration["Kafka:ReturnPaymentRetentionDuration"] ?? "45");

        public int TerminationRetentionYears => int.Parse(Configuration["Kafka:TerminationRetentionYears"] ?? "3");

        public string Topics => Configuration["Kafka:Topics"];

        public string Topic_PPOLC => Configuration["Kafka:Topic.PPOLC"];

        public string Topic_PPOLM_POLICY_BENEFIT_MISC => Configuration["Kafka:Topic.PPOLM_POLICY_BENEFIT_MISC"];

        public string Topic_PNAME => Configuration["Kafka:Topic.PNAME"];

        public string Topic_PADDR => Configuration["Kafka:Topic.PADDR"];

        public string Topic_PPEND_NEW_BUSINESS_PENDING => Configuration["Kafka:Topic.PPEND_NEW_BUSINESS_PENDING"];

        public string Topic_PRELA_RELATIONSHIP_MASTER => Configuration["Kafka:Topic.PRELA_RELATIONSHIP_MASTER"];

        public string Topic_PCOMC_COMMISSION_CONTROL_TYPE_S => Configuration["Kafka:Topic.PCOMC_COMMISSION_CONTROL_TYPE_S"];

        public string Topic_PCOMC_COMMISSION_CONTROL => Configuration["Kafka:Topic.PCOMC_COMMISSION_CONTROL"];

        public string Topic_PHIER_AGENT_HIERARCHY => Configuration["Kafka:Topic.PHIER_AGENT_HIERARCHY"];

        public string Topic_PMUIN_MULTIPLE_INSUREDS => Configuration["Kafka:Topic.PMUIN_MULTIPLE_INSUREDS"];

        public string Topic_PNALK => Configuration["Kafka:Topic.PNALK"];

        public string Topic_PPBEN_POLICY_BENEFITS => Configuration["Kafka:Topic.PPBEN_POLICY_BENEFITS"];

        public string Topic_PPBEN_POLICY_BENEFITS_TYPES_BA_OR => Configuration["Kafka:Topic.PPBEN_POLICY_BENEFITS_TYPES_BA_OR"];

        public string Topic_PPBEN_POLICY_BENEFITS_TYPES_BF => Configuration["Kafka:Topic.PPBEN_POLICY_BENEFITS_TYPES_BF"];

        public string Topic_PPBEN_POLICY_BENEFITS_TYPES_SL => Configuration["Kafka:Topic.PPBEN_POLICY_BENEFITS_TYPES_SL"];

        public string Topic_PPBEN_POLICY_BENEFITS_TYPES_SP => Configuration["Kafka:Topic.PPBEN_POLICY_BENEFITS_TYPES_SP"];

        public string Topic_PPBEN_POLICY_BENEFITS_TYPES_SU => Configuration["Kafka:Topic.PPBEN_POLICY_BENEFITS_TYPES_SU"];

        public string Topic_ProductDescription => Configuration["Kafka:Topic.ProductDescription"];

        public string Topic_PMEDR => Configuration["Kafka:Topic.PMEDR"];

        public string Topic_PRQRM => Configuration["Kafka:Topic.PRQRM"];

        public string Topic_PRQRMTBL => Configuration["Kafka:Topic.PRQRMTBL"];

        public string Topic_PPEND_NEW_BUS_PEND_UNDERWRITING => Configuration["Kafka:Topic.PPEND_NEW_BUS_PEND_UNDERWRITING"];

        public string Topic_PGRUP_GROUP_MASTER => Configuration["Kafka:Topic.PGRUP_GROUP_MASTER"];

        public string Topic_SysNBRequirements => Configuration["Kafka:Topic.SysNBRequirements"];

        public string Topic_SysACAgentData => Configuration["Kafka:Topic.SysACAgentData"];

        public string Topic_SysACAgentMarketCodes => Configuration["Kafka:Topic.SysACAgentMarketCodes"];

        public string Topic_SysZ9Process => Configuration["Kafka:Topic.SysZ9Process"];

        public string Topic_QUEUES => Configuration["Kafka:Topic.QUEUES"];

        public string Topic_PACTG => Configuration["Kafka:Topic.PACTG"];

        public string Topic_PBDRV => Configuration["Kafka:Topic.PBDRV"];

        public string Topic_PACON_ANNUITY_POLICY => Configuration["Kafka:Topic.PACON_ANNUITY_POLICY"];

        // Pod related hashes
        public string? POD_TEMPLATE_HASH => System.Environment.GetEnvironmentVariable("POD_TEMPLATE_HASH");

        public string? METADATA_NAME => System.Environment.GetEnvironmentVariable("METADATA_NAME");

        public string Decrypt(string value)
        {
            if (AesEncryptor == null)
            {
                throw new ArgumentNullException(nameof(AesEncryptor));
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException($"{nameof(value)} cannot be null");
            }

            return Environment.Equals("LOCAL", StringComparison.InvariantCultureIgnoreCase)
                ? value
                : AesEncryptor.DecryptGAC(value, Environment, SharedSecret);
        }

        public List<string> GetTopics()
        {
            var kafkaTopicNames = new List<string>();
            var configTopics = Topics.Split(',').ToList();

            if (configTopics.Contains(Constants.Topics.PPOLC_EVENT))
            {
                kafkaTopicNames.Add(Topic_PPOLC);
            }

            if (configTopics.Contains(Constants.Topics.PPOLM_POLICY_BENEFIT_MISC_EVENT))
            {
                kafkaTopicNames.Add(Topic_PPOLM_POLICY_BENEFIT_MISC);
            }

            if (configTopics.Contains(Constants.Topics.PNAME_EVENT))
            {
                kafkaTopicNames.Add(Topic_PNAME);
            }

            if (configTopics.Contains(Constants.Topics.PADDR_EVENT))
            {
                kafkaTopicNames.Add(Topic_PADDR);
            }

            if (configTopics.Contains(Constants.Topics.PPEND_NEW_BUSINESS_PENDING_EVENT))
            {
                kafkaTopicNames.Add(Topic_PPEND_NEW_BUSINESS_PENDING);
            }

            if (configTopics.Contains(Constants.Topics.PRELA_RELATIONSHIP_MASTER_EVENT))
            {
                kafkaTopicNames.Add(Topic_PRELA_RELATIONSHIP_MASTER);
            }

            if (configTopics.Contains(Constants.Topics.PCOMC_COMMISSION_CONTROL_TYPE_S_EVENT))
            {
                kafkaTopicNames.Add(Topic_PCOMC_COMMISSION_CONTROL_TYPE_S);
            }

            if (configTopics.Contains(Constants.Topics.PCOMC_COMMISSION_CONTROL_EVENT))
            {
                kafkaTopicNames.Add(Topic_PCOMC_COMMISSION_CONTROL);
            }

            if (configTopics.Contains(Constants.Topics.PGRUP_GROUP_MASTER_EVENT))
            {
                kafkaTopicNames.Add(Topic_PGRUP_GROUP_MASTER);
            }

            if (configTopics.Contains(Constants.Topics.PHIER_AGENT_HIERARCHY_EVENT))
            {
                kafkaTopicNames.Add(Topic_PHIER_AGENT_HIERARCHY);
            }

            if (configTopics.Contains(Constants.Topics.PMUIN_MULTIPLE_INSUREDS_EVENT))
            {
                kafkaTopicNames.Add(Topic_PMUIN_MULTIPLE_INSUREDS);
            }

            if (configTopics.Contains(Constants.Topics.PNALK_EVENT))
            {
                kafkaTopicNames.Add(Topic_PNALK);
            }

            if (configTopics.Contains(Constants.Topics.PPBEN_POLICY_BENEFITS_EVENT))
            {
                kafkaTopicNames.Add(Topic_PPBEN_POLICY_BENEFITS);
            }

            if (configTopics.Contains(Constants.Topics.PPBEN_POLICY_BENEFITS_TYPES_BA_OR_EVENT))
            {
                kafkaTopicNames.Add(Topic_PPBEN_POLICY_BENEFITS_TYPES_BA_OR);
            }

            if (configTopics.Contains(Constants.Topics.PPBEN_POLICY_BENEFITS_TYPES_BF_EVENT))
            {
                kafkaTopicNames.Add(Topic_PPBEN_POLICY_BENEFITS_TYPES_BF);
            }

            if (configTopics.Contains(Constants.Topics.PPBEN_POLICY_BENEFITS_TYPES_SL_EVENT))
            {
                kafkaTopicNames.Add(Topic_PPBEN_POLICY_BENEFITS_TYPES_SL);
            }

            if (configTopics.Contains(Constants.Topics.PPBEN_POLICY_BENEFITS_TYPES_SP_EVENT))
            {
                kafkaTopicNames.Add(Topic_PPBEN_POLICY_BENEFITS_TYPES_SP);
            }

            if (configTopics.Contains(Constants.Topics.PPBEN_POLICY_BENEFITS_TYPES_SU_EVENT))
            {
                kafkaTopicNames.Add(Topic_PPBEN_POLICY_BENEFITS_TYPES_SU);
            }

            if (configTopics.Contains(Constants.Topics.ProductDescription_EVENT))
            {
                kafkaTopicNames.Add(Topic_ProductDescription);
            }

            if (configTopics.Contains(Constants.Topics.PMEDR_EVENT))
            {
                kafkaTopicNames.Add(Topic_PMEDR);
            }

            if (configTopics.Contains(Constants.Topics.PRQRM_EVENT))
            {
                kafkaTopicNames.Add(Topic_PRQRM);
            }

            if (configTopics.Contains(Constants.Topics.PRQRMTBL_EVENT))
            {
                kafkaTopicNames.Add(Topic_PRQRMTBL);
            }

            if (configTopics.Contains(Constants.Topics.PPEND_NEW_BUS_PEND_UNDERWRITING_EVENT))
            {
                kafkaTopicNames.Add(Topic_PPEND_NEW_BUS_PEND_UNDERWRITING);
            }

            if (configTopics.Contains(Constants.Topics.SysNBRequirements_EVENT))
            {
                kafkaTopicNames.Add(Topic_SysNBRequirements);
            }

            if (configTopics.Contains(Constants.Topics.SysACAgentData_EVENT))
            {
                kafkaTopicNames.Add(Topic_SysACAgentData);
            }

            if (configTopics.Contains(Constants.Topics.SysACAgentMarketCodes_EVENT))
            {
                kafkaTopicNames.Add(Topic_SysACAgentMarketCodes);
            }

            if (configTopics.Contains(Constants.Topics.SysZ9Process_EVENT))
            {
                kafkaTopicNames.Add(Topic_SysZ9Process);
            }

            if (configTopics.Contains(Constants.Topics.QUEUES_EVENT))
            {
                kafkaTopicNames.Add(Topic_QUEUES);
            }

            if (configTopics.Contains(Constants.Topics.PACTG_EVENT))
            {
                kafkaTopicNames.Add(Topic_PACTG);
            }

            if (configTopics.Contains(Constants.Topics.PBDRV_EVENT))
            {
                kafkaTopicNames.Add(Topic_PBDRV);
            }

            if (configTopics.Contains(Constants.Topics.PACON_ANNUITY_POLICY_EVENT))
            {
                kafkaTopicNames.Add(Topic_PACON_ANNUITY_POLICY);
            }

            return kafkaTopicNames;
        }
    }
}