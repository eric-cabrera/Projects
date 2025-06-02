namespace Assurity.Kafka.Consumer.Controllers
{
    using Avro.Generic;
    using Confluent.Kafka;

    public interface ITopicEventProcessor
    {
        public Task ProcessEvent(ConsumeResult<string, GenericRecord> record, bool slowConsumer = false);
    }
}
