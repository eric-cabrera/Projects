namespace Assurity.PolicyInfo.Contracts.V1
{
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Requirement
    {
        public short Id { get; set; }

        public string Name { get; set; }

        [BsonRepresentation(BsonType.String)]
        public RequirementStatus Status { get; set; }

        public DateTime? AddedDate { get; set; }

        public DateTime? ObtainedDate { get; set; }

        public Participant AppliesTo { get; set; }

        public string LifeProComment { get; set; }

        public string GlobalComment { get; set; }

        public string PhoneNumberComment { get; set; }

        [BsonRepresentation(BsonType.String)]
        public RequirementFulfillingParty? FulfillingParty { get; set; }

        [BsonRepresentation(BsonType.String)]
        public RequirementActionType? ActionType { get; set; }

        public bool Display { get; set; }
    }
}
