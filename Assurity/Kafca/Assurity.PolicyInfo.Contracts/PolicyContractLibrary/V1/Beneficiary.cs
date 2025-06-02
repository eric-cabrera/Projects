namespace Assurity.PolicyInfo.Contracts.V1
{
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Beneficiary
    {
        [BsonRepresentation(BsonType.String)]
        public BeneficiaryType BeneficiaryType { get; set; }

        public Participant Participant { get; set; }
    }
}
