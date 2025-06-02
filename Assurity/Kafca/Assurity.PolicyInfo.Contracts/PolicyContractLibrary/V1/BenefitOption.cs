namespace Assurity.PolicyInfo.Contracts.V1
{
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class BenefitOption
    {
        [BsonRepresentation(BsonType.String)]
        public BenefitOptionName BenefitOptionName { get; set; }

        [BsonRepresentation(BsonType.String)]
        public BenefitOptionValue BenefitOptionValue { get; set; }

        [BsonRepresentation(BsonType.String)]
        public RelationshipToPrimaryInsured RelationshipToPrimaryInsured { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? StopDate { get; set; }
    }
}