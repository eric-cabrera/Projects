namespace Assurity.AgentPortal.Contracts.CommissionsDebt.FileExport;

using System.ComponentModel;
using Assurity.AgentPortal.Contracts.FileExportEngine;

public class PolicyDetailsExport
{
    [DisplayName("Agent ID Paid")]
    public string? AgentId { get; set; }

    [DisplayName("Writing Agent")]
    public string? WritingAgent { get; set; }

    [DisplayName("Writing Agent ID")]
    public string? WritingAgentId { get; set; }

    [DisplayName("Policy Number")]
    public string? PolicyNumber { get; set; }

    [DisplayName("Primary Insured")]
    public string? PrimaryInsured { get; set; }

    [DisplayName("Transaction Date")]
    public DateTime? TransactionDate { get; set; }

    [DisplayName("Premium Due Date")]
    public DateTime? PremiumDueDate { get; set; }

    [DisplayName("Payment Date")]
    public DateTime? PaymentDate { get; set; }

    [DisplayName("Employer ID")]
    public string? EmployerId { get; set; }

    [DisplayName("Employer Name")]
    public string? EmployerName { get; set; }

    [DisplayName("Line of Business")]
    public string? LineOfBusiness { get; set; }

    [DisplayName("Product Description")]
    public string? ProductDescription { get; set; }

    [DisplayName("Contract")]
    public string? Contract { get; set; }

    [DisplayName("Mode")]
    public string? Mode { get; set; }

    [DisplayName("Mode Premium")]
    public decimal? ModePremium { get; set; }

    [DisplayName("Commission Type")]
    public string? CommissionType { get; set; }

    [DisplayName("Commission Rate")]
    public ExcelDataCell? CommissionRate { get; set; }

    [DisplayName("Chargeback Reason")]
    public string? ChargebackReason { get; set; }

    [DisplayName("Commission")]
    public decimal? Commission { get; set; }

    [DisplayName("Advance Recovery")]
    public decimal? AdvanceRecovery { get; set; }

    [DisplayName("Net Commission")]
    public decimal? NetCommission { get; set; }
}
