namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// An entity representing the AgentUseQueue table in the SupportData database.
    /// </summary>
    [Table("AgentUseQueue")]

    public class AgentUseQueue
    {
        public int QueueID { get; set; }

        public string? QueueDescription { get; set; }
    }
}
