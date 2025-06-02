namespace Assurity.PolicyInfo.Contracts.V1
{
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Insured
    {
        public Participant Participant { get; set; }

        [BsonRepresentation(BsonType.String)]
        public RelationshipToPrimaryInsured RelationshipToPrimaryInsured { get; set; }
    }
}