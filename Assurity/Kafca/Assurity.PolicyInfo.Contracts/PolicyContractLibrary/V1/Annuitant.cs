namespace Assurity.PolicyInfo.Contracts.V1
{
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Annuitant
    {
        [BsonRepresentation(BsonType.String)]
        public AnnuitantType AnnuitantType { get; set; }

        public Participant Participant { get; set; }
    }
}
