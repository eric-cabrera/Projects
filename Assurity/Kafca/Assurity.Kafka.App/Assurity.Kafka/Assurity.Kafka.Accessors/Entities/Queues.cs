namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// An entity representing the Queues table in the GlobalData database.
    /// </summary>
    [Table("QUEUES")]
    public class QUEUES
    {
        [StringLength(44)]
        [Unicode(false)]
        public string? QUEUE { get; set; }

        [StringLength(44)]
        [Unicode(false)]
        public string ID { get; set; } = null!;
    }
}
