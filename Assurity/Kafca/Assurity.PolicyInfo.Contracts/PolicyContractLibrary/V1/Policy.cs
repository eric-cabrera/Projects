namespace Assurity.PolicyInfo.Contracts.V1
{
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class Policy
    {
        public List<Annuitant> Annuitants { get; set; }

        public List<Agent> Agents { get; set; }

        public decimal AnnualPremium { get; set; }

        public Assignee Assignee { get; set; }

        public List<Beneficiary> Beneficiaries { get; set; }

        public List<Benefit> Benefits { get; set; }

        public short? BillingDay { get; set; }

        [BsonRepresentation(BsonType.String)]
        public BillingForm BillingForm { get; set; }

        [BsonRepresentation(BsonType.String)]
        public BillingMode BillingMode { get; set; }

        [BsonRepresentation(BsonType.String)]
        public BillingStatus BillingStatus { get; set; }

        [BsonRepresentation(BsonType.String)]
        public BillingReason BillingReason { get; set; }

        public string CompanyCode { get; set; }

        public DateTime? CreateDate { get; set; }

        public Employer Employer { get; set; }

        public bool Flagged { get; set; }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public List<Insured> Insureds { get; set; }

        public DateTime? IssueDate { get; set; }

        [BsonRepresentation(BsonType.String)]
        public State? IssueState { get; set; }

        public DateTime? LastModified { get; set; }

        public DateTime? LastStatusChangeDate { get; set; }

        [BsonRepresentation(BsonType.String)]
        public LineOfBusiness LineOfBusiness { get; set; }

        public decimal ModePremium { get; set; }

        public List<Owner> Owners { get; set; }

        public bool PastDue { get; set; }

        public DateTime? PaidToDate { get; set; }

        public Payee Payee { get; set; }

        public List<Payor> Payors { get; set; }

        public string PolicyNumber { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Status PolicyStatus { get; set; }

        [BsonRepresentation(BsonType.String)]
        public PolicyStatusDetail PolicyStatusDetail { get; set; }

        [BsonRepresentation(BsonType.String)]
        public StatusReason? PolicyStatusReason { get; set; }

        public string ProductCategory { get; set; }

        public string ProductCode { get; set; }

        public string ProductDescription { get; set; }

        [BsonRepresentation(BsonType.String)]
        public State? ResidentState { get; set; }

        public List<Requirement> Requirements { get; set; }

        public DateTime? ReturnPaymentDate { get; set; }

        [BsonRepresentation(BsonType.String)]
        public ReturnPaymentType ReturnPaymentType { get; set; }

        public DateTime? SubmitDate { get; set; }

        public DateTime? ApplicationDate { get; set; }

        public DateTime? ApplicationReceivedDate { get; set; }

        [BsonRepresentation(BsonType.String)]
        public TaxQualificationStatus TaxQualificationStatus { get; set; }

        public DateTime? TerminationDate { get; set; }
    }
}