namespace Assurity.Kafka.Utilities.Config
{
    public interface IConfigurationManager
    {
        public string LifeProConnectionString { get; }

        public string DataStoreConnectionString { get; }

        public string GlobalDataConnectionString { get; }

        public string SupportDataConnectionString { get; }

        public string MongoDbConnectionString { get; }

        public string MongoDbDatabaseName { get; }

        public string MongoPolicyCollectionName { get; }

        public string MongoPolicyHierarchyCollectionName { get; }

        public string MongoAgentPolicyAccessCollectionName { get; }

        public string MongoPPOLCEventsCollectionName { get; }

        public string MongoDbClientCertificatePassword { get; }

        public string MongoRequirementMappingCollectionName { get; }

        public string MongoBenefitOptionsMappingCollectionName { get; }

        public string MongoDbExecutionMode { get; }

        public string UseTls { get; }

        public string KafkaClientId { get; }

        public string KafkaGroupId { get; }

        public int KafkaAutoOffsetReset { get; }

        public string KafkaBootstrapServers { get; }

        public string KafkaSaslUsername { get; }

        public string KafkaSaslPassword { get; }

        public string KafkaSslCaLocation { get; }

        public string KafkaSchemaRegistryURL { get; }

        public string KafkaSchemaRegistryBasicAuthUserInfo { get; }

        public bool KafkaEnableAutoOffsetStore { get; }

        public int KafkaPartitionAssignmentStrategy { get; }

        public int KafkaConsumerFailRetry { get; }

        public int KafkaSlowConsumerUpdateThreshold { get; }

        public string DeadLetterTopicName { get; }

        public string KafkaDebug { get; }

        public int KafkaFetchMaxBytes { get; }

        public int KafkaMaxPartitionFetchBytes { get; }

        public int KafkaQueuedMaxMessagesKilobytes { get; }

        public int KafkaMessageMaxBytes { get; }

        public double LogOffsetInfoIntervalSec { get; }

        public int InitialPaymentDeclinedRetentionDays { get; }

        public int TerminationRetentionYears { get; }

        public string Topics { get; }

        public string Topic_PPOLC { get; }

        public string Topic_PPOLM_POLICY_BENEFIT_MISC { get; }

        public string Topic_PNAME { get; }

        public string Topic_PADDR { get; }

        public string Topic_PPEND_NEW_BUSINESS_PENDING { get; }

        public string Topic_PRELA_RELATIONSHIP_MASTER { get; }

        public string Topic_PCOMC_COMMISSION_CONTROL_TYPE_S { get; }

        public string Topic_PCOMC_COMMISSION_CONTROL { get; }

        public string Topic_PHIER_AGENT_HIERARCHY { get; }

        public string Topic_PMUIN_MULTIPLE_INSUREDS { get; }

        public string Topic_PNALK { get; }

        public string Topic_PPBEN_POLICY_BENEFITS { get; }

        public string Topic_PPBEN_POLICY_BENEFITS_TYPES_BA_OR { get; }

        public string Topic_PPBEN_POLICY_BENEFITS_TYPES_BF { get; }

        public string Topic_PPBEN_POLICY_BENEFITS_TYPES_SL { get; }

        public string Topic_PPBEN_POLICY_BENEFITS_TYPES_SP { get; }

        public string Topic_PPBEN_POLICY_BENEFITS_TYPES_SU { get; }

        public string Topic_ProductDescription { get; }

        public string Topic_PMEDR { get; }

        public string Topic_PRQRM { get; }

        public string Topic_PRQRMTBL { get; }

        public string Topic_PPEND_NEW_BUS_PEND_UNDERWRITING { get; }

        public string Topic_PGRUP_GROUP_MASTER { get; }

        public string Topic_SysNBRequirements { get; }

        public string Topic_SysACAgentData { get; }

        public string Topic_SysACAgentMarketCodes { get; }

        public string Topic_SysZ9Process { get; }

        public string Topic_QUEUES { get; }

        public string Topic_PACTG { get; }

        public string Topic_PBDRV { get; }

        public string Topic_PACON_ANNUITY_POLICY { get; }

        public string? POD_TEMPLATE_HASH { get; }

        public string? METADATA_NAME { get; }

        public string Decrypt(string value);

        public List<string> GetTopics();
    }
}