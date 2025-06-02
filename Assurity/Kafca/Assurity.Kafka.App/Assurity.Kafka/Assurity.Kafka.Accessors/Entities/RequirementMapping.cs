namespace Assurity.Kafka.Accessors.Entities
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class RequirementMapping
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int RequirementId { get; set; }

        public string FulfillingParty { get; set; }

        public string AgentAction { get; set; }

        public bool Display { get; set; }

        public string Phone { get; set; }
    }
}
