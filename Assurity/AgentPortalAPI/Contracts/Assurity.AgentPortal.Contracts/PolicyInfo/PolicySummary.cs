namespace Assurity.AgentPortal.Contracts.PolicyInfo;

public class PolicySummary
{
    public List<AssignedAgent>? AssignedAgents { get; set; }

    public DateTime? IssueDate { get; set; }

    public DateTime? PaidToDate { get; set; }

    public string PolicyNumber { get; set; }

    public string PolicyStatus { get; set; }

    public string PolicyStatusReason { get; set; }

    public bool FirstPaymentFail { get; set; }

    public string? PrimaryInsuredName { get; set; }

    public string? EmployerName { get; set; }

    public string? ProductCategory { get; set; }

    public string? ProductCode { get; set; }

    public string? ProductDescription { get; set; }

    public DateTime? SubmitDate { get; set; }

    public DateTime? TerminationDate { get; set; }

    public decimal FaceAmount { get; set; }

    public string BillingMode { get; set; }

    public bool PastDue { get; set; }
}
