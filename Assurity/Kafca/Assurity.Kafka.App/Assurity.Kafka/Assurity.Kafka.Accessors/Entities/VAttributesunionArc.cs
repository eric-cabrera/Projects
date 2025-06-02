namespace Assurity.Kafka.Accessors.Entities
{
    /// <summary>
    /// An entity representing the VAttributesunionArc table in the GlobalData database.
    /// </summary>
    public partial class VAttributesunionArc
    {
        public string ObjectId { get; set; } = null!;

        public int ObjectType { get; set; }

        public string? PolicyNumber { get; set; }
    }
}
