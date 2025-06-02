namespace Assurity.PolicyInfo.Contracts.V1
{
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Payor
    {
        public Participant Participant { get; set; }

        [BsonRepresentation(BsonType.String)]
        public PayorType PayorType { get; set; }
    }
}