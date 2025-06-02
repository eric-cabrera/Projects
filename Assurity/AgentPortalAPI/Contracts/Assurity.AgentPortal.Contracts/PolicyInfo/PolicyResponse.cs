namespace Assurity.AgentPortal.Contracts.PolicyInfo;

using Assurity.PolicyInfo.Contracts.V1;
using Assurity.PolicyInfo.Contracts.V1.Enums;

public class PolicyResponse
{
    public List<Annuitant> Annuitants { get; set; }

    public List<Agent> Agents { get; set; }

    public decimal AnnualPremium { get; set; }

    public Assignee Assignee { get; set; }

    public List<Beneficiary> Beneficiaries { get; set; }

    public List<BenefitResponse> Benefits { get; set; }

    public short? BillingDay { get; set; }

    public string BillingForm { get; set; }

    public string BillingMode { get; set; }

    public string BillingStatus { get; set; }

    public string BillingReason { get; set; }

    public string CompanyCode { get; set; }

    public DateTime? CreateDate { get; set; }

    public Employer Employer { get; set; }

    public string Id { get; set; }

    public List<Insured> Insureds { get; set; }

    public DateTime? IssueDate { get; set; }

    public State? IssueState { get; set; }

    public DateTime? LastModified { get; set; }

    public DateTime? LastStatusChangeDate { get; set; }

    public string LineOfBusiness { get; set; }

    public decimal ModePremium { get; set; }

    public List<Owner> Owners { get; set; }

    public bool PastDue { get; set; }

    public DateTime? PaidToDate { get; set; }

    public Payee Payee { get; set; }

    public List<Payor> Payors { get; set; }

    public string PolicyNumber { get; set; }

    public string PolicyStatus { get; set; }

    public string PolicyStatusDetail { get; set; }

    public string? PolicyStatusReason { get; set; }

    public string ProductCategory { get; set; }

    public string ProductCode { get; set; }

    public string ProductDescription { get; set; }

    public State? ResidentState { get; set; }

    public List<RequirementResponse> Requirements { get; set; }

    public string ReturnPaymentType { get; set; }

    public DateTime? SubmitDate { get; set; }

    public DateTime? ApplicationDate { get; set; }

    public DateTime? ApplicationReceivedDate { get; set; }

    public string TaxQualificationStatus { get; set; }

    public DateTime? TerminationDate { get; set; }
}
