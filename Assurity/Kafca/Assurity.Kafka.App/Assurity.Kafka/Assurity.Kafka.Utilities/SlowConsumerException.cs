namespace Assurity.Kafka.Utilities.Extensions
{
    public class SlowConsumerException : Exception
    {
        public SlowConsumerException()
            : base("SlowConsumerException - The number of potential updates created by this " +
                  "message is greater that the configured threshold")
        {
        }
    }
}