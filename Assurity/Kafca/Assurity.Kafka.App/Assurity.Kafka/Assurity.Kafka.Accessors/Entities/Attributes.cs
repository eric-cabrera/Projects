namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// An entity representing the ATTRIBUTES table in the GlobalData database.
    /// </smmary>
    [Table("ATTRIBUTES")]

    public class Attributes
    {
        public string OBJECT_ID { get; set; } = null!;

        public int OBJECT_TYPE { get; set; }

        public string? OBJECT_NAME { get; set; }

        public string? AgentID { get; set; }

        public string? TransTypeDetail { get; set; }
    }
}
