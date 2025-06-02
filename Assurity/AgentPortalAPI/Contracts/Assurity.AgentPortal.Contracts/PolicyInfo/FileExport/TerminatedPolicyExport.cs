namespace Assurity.AgentPortal.Contracts.PolicyInfo.FileExport;

using System.ComponentModel;

/// <summary>
/// The FileExportEngine will use this class to format the excel document.
/// Properties will be displayed in order as the column headers and information.
/// Property data type will determine the cell's formatting.
/// </summary>
public class TerminatedPolicyExport
{
    [DisplayName("Agent Last Name")]
    public string? AgentLastName { get; set; }

    [DisplayName("Agent First Name")]
    public string? AgentFirstName { get; set; }

    [DisplayName("Agent ID")]
    public string? AgentId { get; set; }

    [DisplayName("Policy Number")]
    public string? PolicyNumber { get; set; }

    [DisplayName("Primary Insured Last Name")]
    public string? PrimaryInsuredLastName { get; set; }

    [DisplayName("Primary Insured First Name")]
    public string? PrimaryInsuredFirstName { get; set; }

    [DisplayName("Product Type")]
    public string? ProductCategory { get; set; }

    [DisplayName("Product Description")]
    public string? ProductDescription { get; set; }

    [DisplayName("Status")]
    public string? Status { get; set; }

    [DisplayName("Termination Date")]
    public DateTime? TerminationDate { get; set; }

    [DisplayName("Termination Reason")]
    public string? TerminationReason { get; set; }

    [DisplayName("Termination Detail")]
    public string? TerminationDetail { get; set; }

    [DisplayName("Billing Reason")]
    public string? BillingReason { get; set; }

    [DisplayName("Paid To Date")]
    public DateTime? PaidToDate { get; set; }

    [DisplayName("Billing Mode")]
    public string? BillingMode { get; set; }

    [DisplayName("Mode Premium")]
    public decimal ModePremium { get; set; }

    [DisplayName("Annual Premium")]
    public decimal AnnualPremium { get; set; }

    [DisplayName("Face Amount")]
    public decimal FaceAmount { get; set; }

    [DisplayName("Issue State")]
    public string? IssueState { get; set; }

    [DisplayName("Primary Owner Last Name")]
    public string? PrimaryOwnerLastName { get; set; }

    [DisplayName("Primary Owner First Name")]
    public string? PrimaryOwnerFirstName { get; set; }

    [DisplayName("Primary Owner Phone Number")]
    public string? PrimaryOwnerPhoneNumber { get; set; }

    [DisplayName("Primary Owner Email")]
    public string? PrimaryOwnerEmail { get; set; }

    [DisplayName("Primary Owner Address Line 1")]
    public string? PrimaryOwnerAddressLine1 { get; set; }

    [DisplayName("Primary Owner Address Line 2")]
    public string? PrimaryOwnerAddressLine2 { get; set; }

    [DisplayName("Primary Owner City")]
    public string? PrimaryOwnerCity { get; set; }

    [DisplayName("Primary Owner State")]
    public string? PrimaryOwnerState { get; set; }

    [DisplayName("Primary Owner Zip")]
    public string? PrimaryOwnerZip { get; set; }

    [DisplayName("Employer")]
    public string? EmployerName { get; set; }

    [DisplayName("Employer Number")]
    public string? EmployerNumber { get; set; }

    [DisplayName("Application Signed Date")]
    public DateTime? ApplicationSignedDate { get; set; }

    [DisplayName("Issue Date")]
    public DateTime? IssueDate { get; set; }

    // If ReturnPaymentType is InitialPaymentCreditCardDeclined or InitialPaymentCheckDraftDeclined, then display "First Payment Fail" in this export field, else null.
    [DisplayName("First Payment Fail")]
    public string? FirstPaymentFail { get; set; }
}
