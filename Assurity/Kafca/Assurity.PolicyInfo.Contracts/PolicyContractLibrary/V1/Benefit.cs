namespace Assurity.PolicyInfo.Contracts.V1
{
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Benefit
    {
        public decimal? BenefitAmount { get; set; }

        public string BenefitDescription { get; set; }

        public long BenefitId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Status BenefitStatus { get; set; }

        [BsonRepresentation(BsonType.String)]
        public StatusReason? BenefitStatusReason { get; set; }

        [BsonRepresentation(BsonType.String)]
        public CoverageType CoverageType { get; set; }

        [BsonRepresentation(BsonType.String)]
        public DeathBenefitOption? DeathBenefitOption { get; set; }

        [BsonRepresentation(BsonType.String)]
        public DividendOption? DividendOption { get; set; }

        public string PlanCode { get; set; }

        public List<BenefitOption> BenefitOptions { get; set; }
    }
}