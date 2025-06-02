namespace Assurity.Kafka.Accessors.Entities
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class BenefitOptionsMapping
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Category { get; set; }

        public string Option { get; set; }

        public bool HideBenefitOption { get; set; }
    }
}
