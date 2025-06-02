namespace Assurity.PolicyInfo.Contracts.V1
{
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Owner
    {
        [BsonRepresentation(BsonType.String)]
        public OwnerType OwnerType { get; set; }

        public Participant Participant { get; set; }
    }
}