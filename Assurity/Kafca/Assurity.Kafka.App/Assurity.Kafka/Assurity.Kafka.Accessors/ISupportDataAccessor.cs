namespace Assurity.Kafka.Accessors
{
    public interface ISupportDataAccessor
    {
        Task<List<string?>> GetQueueDescriptions();

        Task<bool> IsJustInTimeQueue(string queue);
    }
}
