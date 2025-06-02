namespace Assurity.Kafka.Consumer
{
    using System.Text;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Consumer.Controllers;
    using Assurity.Kafka.Managers.Interfaces;
    using Assurity.Kafka.Utilities.Config;
    using Assurity.Kafka.Utilities.Constants;
    using Assurity.Kafka.Utilities.Extensions;
    using Avro.Generic;
    using Confluent.Kafka;
    using Confluent.Kafka.SyncOverAsync;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;
    using Microsoft.Extensions.Logging;
    using NewRelic.Api.Agent;

    public class SubscriptionService : BackgroundService
    {
        public SubscriptionService(
            ILogger<SubscriptionService> log,
            IServiceProvider serviceProvider,
            IConfigurationManager configuration)
        {
            Logger = log;
            Services = serviceProvider;
            Config = configuration;
        }

        private IConfigurationManager Config { get; }

        private ILogger<SubscriptionService> Logger { get; }

        private IServiceProvider Services { get; }

        [Transaction]
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Graceful shutdown initiated...");

            // Perform cleanup tasks here...
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var topics = Config.GetTopics();

            if (topics.Count == 0)
            {
                Logger.LogError("Consumer SubscriptionService - There are no Topics configured correctly to subscribe to.");
                return;
            }

            using var scope = Services.CreateScope();
            var topicEventProcessor = scope.ServiceProvider.GetRequiredService<ITopicEventProcessor>();

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var consumer = CreateConsumer())
                {
                    ConsumeResult<string, GenericRecord>? record = null;

                    // TODO - config
                    bool debugMode = false;
                    await Subscribe(topics, scope, consumer, debugMode);

                    try
                    {
                        var printTimeStamp = DateTime.Now;

                        while (!stoppingToken.IsCancellationRequested)
                        {
                            record = consumer.Consume(stoppingToken);

                            // Post Processing Commit
                            if (record != null)
                            {
                                await ProcessRecord(consumer, topicEventProcessor, record);
                            }

                            if (DateTime.Now.CompareTo(printTimeStamp.AddSeconds(Config.LogOffsetInfoIntervalSec)) > 0)
                            {
                                printTimeStamp = DateTime.Now;
                                LogConsumerOffsetInfo(consumer);
                            }
                        }
                    }
                    catch (SlowConsumerException)
                    {
                        var now = DateTimeOffset.Now.ToString("MM-dd-yyyy HH:mm:ss.fff");
                        var offset = record?.TopicPartitionOffset;

                        // Pre Processing Commit
                        Logger.LogWarning(
                            "Slow Consumer Detected - Topic Partition offset will be processed while not connected: {offset} Time {time}",
                            offset,
                            now);

                        consumer.Commit(record);

                        Logger.LogWarning(
                            "Closing Consumer. Last Topic Partition offset: {offset} Time {time}",
                            offset,
                            now);

                        consumer.Close();

                        await HandleSlowConsumer(topicEventProcessor, record);
                    }
                    catch (OperationCanceledException ex)
                    {
                        // The service will stop because the stoppingToken likely indicates that this process has been canceled.
                        var errorString =
                            "OperationCanceledException. Something is probably trying to shut down the service. " +
                            "All processed msgs will have been committed. If this " +
                            "was thrown during msg processing, the msg has not been " +
                            "committed or sent to the DeadLetterTopic. " +
                            "This service is stopping." +
                            "Time {time}";

                        Logger.LogError(ex, errorString, DateTimeOffset.Now.ToString("MM-dd-yyyy HH:mm:ss.fff"));
                    }
                    catch (Exception ex)
                    {
                        // Post Publish to DeadLetterTopic Commit
                        var success = await HandleConsumerException(record, ex);

                        if (success)
                        {
                            // CFM Does this need a try catch?
                            consumer.Commit(record);
                        }
                    }
                    finally
                    {
                        // Set record null here. We want to make sure we don't
                        // reuse this record in the next loop if consume throws an error before
                        // consuming another records.
                        // Also, since th consumer get disposed after this, there is no need to call
                        // consumer.close
                        record = null;

                        // Let's wait a bit so we don't spam repartitions or to protect if we hit a repeating error
                        Thread.Sleep(Config.KafkaConsumerFailRetry * 1000);
                    }
                }
            }
        }

        [Trace]
        private static long CalculateConsumerMessageLag(WatermarkOffsets watermarkOffsets, long currentOffset)
        {
            long behind;

            if (currentOffset == Offset.Unset)
            {
                behind = watermarkOffsets.High.Value - watermarkOffsets.Low.Value;
            }
            else
            {
                behind = watermarkOffsets.High.Value - currentOffset;
            }

            return behind;
        }

        [Transaction]
        private static async Task Subscribe(List<string> topics, IServiceScope scope, IConsumer<string, GenericRecord> consumer, bool debugMode)
        {
            if (debugMode)
            {
                await DebugSinglePolicySetup(scope, consumer);
            }
            else
            {
                consumer.Subscribe(topics);
            }
        }

        [Trace]
        private static async Task DebugSinglePolicySetup(IServiceScope scope, IConsumer<string, GenericRecord> consumer)
        {
            // Specify needed topicName, Partition#, offset#. These can often be found with
            // the offset explorer or in the new relic logging on info mode. If you don't know
            // the policy number you can comment out the MigrateSinglePolicy and you maybe able to
            // find it by stepping though the code while process the event.
            // TODO - config
            string companyCode = "01";
            string topicName = "sink-PNALK-EVENT";
            int partition = 0;
            long offset = 260860;
            string policyNumber = "4370349765";

            // If you don't delete the PolicyHierarchy object or one exists locally, you may see
            // and error. The program will continue.
            await MigrateSinglePolicy(scope, companyCode, policyNumber);

            consumer.Assign(new TopicPartitionOffset(topicName, partition, offset));
        }

        private static async Task MigrateSinglePolicy(IServiceScope scope, string companyCode, string policyNumber)
        {
            // This will allow local backfill of specific policy for testing
            var backfillManager = scope.ServiceProvider.GetRequiredService<IBackfillManager>();

            var lifeProPolicy = new CompanyCodeAndPolicyNumber
            {
                CompanyCode = companyCode,
                PolicyNumber = policyNumber
            };

            await backfillManager.MigrateSinglePolicy(lifeProPolicy);
        }

        private static Message<string, GenericRecord> BuildMessage(ConsumeResult<string, GenericRecord> record)
        {
            var headers = new Confluent.Kafka.Headers
            {
                { "Topic", Encoding.UTF8.GetBytes(record.Topic) },
                { "PartionOffset", Encoding.UTF8.GetBytes(record.Offset.ToString()) }
            };

            return new Message<string, GenericRecord>
            {
                Key = record.Message.Key,
                Value = record.Message.Value,
                Headers = headers,
            };
        }

        private string ExtractControllerIdFromMetaDataName(string metaDataName)
        {
            int lastIndex = (int)metaDataName.LastIndexOf('-');
            string result = lastIndex >= 0 ? metaDataName.Substring(lastIndex + 1) : metaDataName;
            return result;
        }

        [Trace]
        private ConsumerConfig BuildConsumerConfig()
        {
            var clientId = $"{Config.KafkaClientId}{Config.POD_TEMPLATE_HASH}-{ExtractControllerIdFromMetaDataName(Config.METADATA_NAME ?? string.Empty)}";
            var config = new ConsumerConfig
            {
                ClientId = clientId,
                GroupId = Config.KafkaGroupId,
                BootstrapServers = Config.KafkaBootstrapServers,
                AutoOffsetReset = (AutoOffsetReset?)Config.KafkaAutoOffsetReset,
                SaslUsername = Config.KafkaSaslUsername,
                SaslPassword = Config.KafkaSaslPassword,
                EnableAutoCommit = false,
                EnableAutoOffsetStore = Config.KafkaEnableAutoOffsetStore,
                PartitionAssignmentStrategy = (PartitionAssignmentStrategy?)Config.KafkaPartitionAssignmentStrategy,
                SaslMechanism = SaslMechanism.Plain,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SslCaLocation = Config.KafkaSslCaLocation,

                Debug = Config.KafkaDebug, // comma-separated list of debug lvls: Consumer: "consumer,cgrp,topic,fetch"  Producer : "broker,topic,msg"
                FetchMaxBytes = Config.KafkaFetchMaxBytes,
                MaxPartitionFetchBytes = Config.KafkaMaxPartitionFetchBytes,
                QueuedMaxMessagesKbytes = Config.KafkaQueuedMaxMessagesKilobytes,
                MessageMaxBytes = Config.KafkaMessageMaxBytes
            };

            return config;
        }

        [Transaction]
        private IConsumer<string, GenericRecord> CreateConsumer()
        {
            Logger.LogInformation($"Creating Consumer. Time {DateTimeOffset.Now.ToString("MM-dd-yyyy HH:mm:ss.fff")}");
            var consumerConfig = BuildConsumerConfig();

            var schemaRegistry = BuildSchemaRegistry();

            var deserializer = new AvroDeserializer<GenericRecord>(schemaRegistry);

            return new ConsumerBuilder<string, GenericRecord>(consumerConfig)
               .SetValueDeserializer(deserializer.AsSyncOverAsync())
               .SetErrorHandler((_, e) => Logger.LogError("{ErrorReason}", e.Reason))
               .SetPartitionsAssignedHandler((c, partitions) =>
               {
                   Logger.LogInformation(
                       "Partitions incrementally assigned: [" +
                       string.Join(',', partitions.Select(p => p.Partition.Value)) +
                       "], all: [" +
                       string.Join(',', c.Assignment.Concat(partitions).Select(p => p.Topic + '[' + p.Partition.Value.ToString() + ']')) +
                       "]");
               })
               .SetPartitionsRevokedHandler((c, partitions) =>
               {
                   var remaining = c.Assignment.Where(atp => partitions.Where(rtp => rtp.TopicPartition == atp).Count() == 0);
                   Logger.LogInformation(
                       "Partitions incrementally revoked: [" +
                       string.Join(',', partitions.Select(p => p.Partition.Value)) +
                       "], remaining: [" +
                       string.Join(',', remaining.Select(p => p.Partition.Value)) +
                       "]");
               })
               .SetPartitionsLostHandler((c, partitions) =>
               {
                   Logger.LogInformation($"Partitions were lost: [{string.Join(", ", partitions)}]");
               })
               .Build();
        }

        [Transaction]
        private async Task ProcessRecord(IConsumer<string, GenericRecord> consumer, ITopicEventProcessor topicEventProcessor, ConsumeResult<string, GenericRecord> record)
        {
            await topicEventProcessor.ProcessEvent(record);
            consumer.Commit(record);

            LogCommit(consumer, record);
        }

        [Trace]
        private void LogCommit(IConsumer<string, GenericRecord> consumer, ConsumeResult<string, GenericRecord> record)
        {
            record.Message.Value.TryGetValue(TopicFields.ChangeTimeDb, out var dbChangeTimeString);
            DateTimeOffset.TryParse((string?)dbChangeTimeString, out DateTimeOffset dbChangeTime);

            var timeDifference = DateTimeOffset.Now - dbChangeTime;
            var watermarks = consumer.GetWatermarkOffsets(record.TopicPartition);

            Logger.LogInformation($"Committed offset: " +
                $"{record.TopicPartitionOffset} of {watermarks.High.Value} " +
                $"Time: {DateTimeOffset.Now.ToString("MM-dd-yyyy HH:mm:ss.fff")} " +
                $"ChangeTimeDb: {dbChangeTime.ToString("MM-dd-yyyy HH:mm:ss.fff")} " +
                $"TimeDiff: {timeDifference.ToString(@"dd\-hh\:mm\:ss\.fff")}");
        }

        [Trace]
        private async Task<bool> HandleConsumerException(
            ConsumeResult<string, GenericRecord>? record,
            Exception ex)
        {
            Logger.LogError(
                ex,
                $"{ex.GetType().Name} caught in SubscriptionService. Last Topic Partition Offset: {record?.TopicPartitionOffset} Time {DateTimeOffset.Now.ToString("MM-dd-yyyy HH:mm:ss.fff")}");

            if (record != null)
            {
                Logger.LogInformation(
                    $"Publishing to DeadLettertopic: " +
                    $"TopicPartition offset: {record.TopicPartitionOffset} " +
                    $"Time {DateTimeOffset.Now.ToString("MM-dd-yyyy HH:mm:ss.fff")}");
                return await PublishToDeadLetterTopic(record);
            }

            return false;
        }

        [Transaction]
        private async Task HandleSlowConsumer(ITopicEventProcessor topicEventProcessor, ConsumeResult<string, GenericRecord> record)
        {
            if (record is null)
            {
                // This should never happen.
                Logger.LogError($"Slow Consumer Detected but record is null.");
                return;
            }

            try
            {
                await topicEventProcessor.ProcessEvent(record, true);
                Logger.LogWarning($"Slow Consumer Finished offline processing - Topic Partition offset processed while not connected. Will pause and reconnect." +
                    $"{record.TopicPartitionOffset}, Time {DateTimeOffset.Now}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Slow Consumer FAILED offline processing - Topic Partition offset processed while not connected: " +
                    $"{record?.TopicPartitionOffset}, Time {DateTimeOffset.Now}." +
                    $"Slow Consumer Processing threw exception: {ex.Message}");

                // Log full msg value so we have some record of it (in case the error is not discovered
                // before the retention period is up)
                Logger.LogError($"Slow Consumer FAILED. Will attempt to publish to DeadLetterTopic." +
                    $"Topic: {record?.Topic} " +
                    $"Partition: {record?.Partition} " +
                    $"Offset: {record?.Offset} ");

                await HandleConsumerException(record, ex);
            }
        }

        private async Task<bool> PublishToDeadLetterTopic(ConsumeResult<string, GenericRecord> record)
        {
            Logger.LogError($"Publishing to DeadLetterTopic: " +
                 $"{Config.DeadLetterTopicName} because of exception - " +
                 $"Topic: {record.Topic} " +
                 $"Partition: {record.Partition} " +
                 $"Offset: {record.Offset} " +
                 $"Time {DateTimeOffset.Now.ToString("MM-dd-yyyy HH:mm:ss.fff")}");

            if (record == null)
            {
                Logger.LogError("Failed to publish to Dead-Letter Topic - Record is null");
                return false;
            }

            try
            {
                var producerConfig = BuildProducerConfig();

                using var schemaRegistry = BuildSchemaRegistry();
                var serializer = new AvroSerializer<GenericRecord>(schemaRegistry);

                using var producer = new ProducerBuilder<string, GenericRecord>(producerConfig)
                    .SetValueSerializer(serializer)
                    .Build();

                var message = BuildMessage(record);
                var deliveryReport = await producer.ProduceAsync(Config.DeadLetterTopicName, message);

                Logger.LogInformation($"Published to Dead-Letter Topic - " +
                    $"{Config.DeadLetterTopicName} - " +
                    $"Delivery Report Message - " +
                    $"Topic: {deliveryReport.Topic} " +
                    $"Partition: {deliveryReport.Partition} " +
                    $"Offset: {deliveryReport.Offset} " +
                    $"Time {DateTimeOffset.Now.ToString("MM-dd-yyyy HH:mm:ss.fff")}");

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to Publish to DeadLetterTopic: " +
                     $"{Config.DeadLetterTopicName} because of exception - " +
                     $"Topic: {record.Topic} " +
                     $"Partition: {record.Partition} " +
                     $"Offset: {record.Offset} " +
                     $"Time {DateTimeOffset.Now.ToString("MM-dd-yyyy HH:mm:ss.fff")}" +
                     $"{ex.Message}");

                return false;
            }
        }

        [Trace]
        private CachedSchemaRegistryClient BuildSchemaRegistry()
        {
            return new CachedSchemaRegistryClient(new SchemaRegistryConfig
            {
                Url = Config.KafkaSchemaRegistryURL,
                BasicAuthUserInfo = Config.KafkaSchemaRegistryBasicAuthUserInfo,
            });
        }

        private ProducerConfig BuildProducerConfig()
        {
            return new ProducerConfig
            {
                BootstrapServers = Config.KafkaBootstrapServers,
                SaslUsername = Config.KafkaSaslUsername,
                SaslPassword = Config.KafkaSaslPassword,
                SaslMechanism = SaslMechanism.Plain,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SslCaLocation = Config.KafkaSslCaLocation,
                Debug = Config.KafkaDebug,
            };
        }

        [Transaction]
        private void LogConsumerOffsetInfo(IConsumer<string, GenericRecord> consumer)
        {
            foreach (var partition in consumer.Assignment)
            {
                // Note: Current Offset will be 'Unset' if this consumer has not yet consumed from
                // the specified TopicPartition.
                var watermarkOffsets = consumer.GetWatermarkOffsets(partition);
                var offsetInfo = new TopicPartitionOffsetInfo
                {
                    Name = partition.Topic,
                    Partition = partition.Partition.Value,
                    LowWatermarkOffset = watermarkOffsets.Low,
                    HighWatermarkOffset = watermarkOffsets.High,
                    CurrentOffset = consumer.Position(partition),
                    ConsumerMessagesBehind = CalculateConsumerMessageLag(watermarkOffsets, consumer.Position(partition)),
                };

                Logger.LogWarning(offsetInfo.ToString() + $" Time {DateTimeOffset.Now.ToString("MM-dd-yyyy HH:mm:ss.fff")}");
            }
        }

        private class TopicPartitionOffsetInfo
        {
            public string Name { get; set; }

            public int Partition { get; set; }

            public Offset LowWatermarkOffset { get; set; }

            public Offset HighWatermarkOffset { get; set; }

            public Offset CurrentOffset { get; set; }

            public long ConsumerMessagesBehind { get; set; }

            public override string ToString()
            {
                return $"MessagesBehind: {ConsumerMessagesBehind} " +
                       $"Topic: {Name}[{Partition}] " +
                       $"CurrentOffset: {CurrentOffset} " +
                       $"HighWatermark: {HighWatermarkOffset} " +
                       $"LowWatermark: {LowWatermarkOffset}";
            }
        }
    }
}