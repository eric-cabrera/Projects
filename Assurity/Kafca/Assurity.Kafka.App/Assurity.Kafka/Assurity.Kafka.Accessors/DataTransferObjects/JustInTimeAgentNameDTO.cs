namespace Assurity.Kafka.Accessors.DataTransferObjects
{
    public class JustInTimeAgentNameDTO
    {
        public string AgentId { get; set; } = null!;

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string BusinessName { get; set; }
    }
}
