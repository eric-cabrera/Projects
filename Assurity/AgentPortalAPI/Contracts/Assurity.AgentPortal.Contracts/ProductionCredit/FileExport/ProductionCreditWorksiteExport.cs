namespace Assurity.AgentPortal.Contracts.ProductionCredit.FileExport;

using System.ComponentModel;
using Assurity.AgentPortal.Contracts.FileExportEngine;

public class ProductionCreditWorksiteExport
{
    [DisplayName("Agent Name")]
    public string? AgentName { get; set; }

    [DisplayName("Agent ID")]
    public string? AgentId { get; set; }

    [DisplayName("Policy Number")]
    public string? PolicyNumber { get; set; }

    [DisplayName("Line of Business")]
    public string? LineOfBusiness { get; set; }

    [DisplayName("Product Type")]
    public string? ProductType { get; set; }

    [DisplayName("Application Status")]
    public string? TransactionType { get; set; }

    [DisplayName("Product Description")]
    public string? ProductDescription { get; set; }

    [DisplayName("Insured Name")]
    public string? InsuredName { get; set; }

    [DisplayName("Employer")]
    public string? Employer { get; set; }

    [DisplayName("Group Number")]
    public string? GroupNumber { get; set; }

    [DisplayName("Transaction Date")]
    public DateTime? TransactionDate { get; set; }

    [DisplayName("Application Date")]
    public DateTime? ApplicationDate { get; set; }

    [DisplayName("Group Effective Date")]
    public DateTime? GroupEffectiveDate { get; set; }

    [DisplayName("Product Effective Date")]
    public DateTime? ProductEffectiveDate { get; set; }

    [DisplayName("Mode")]
    public string? Mode { get; set; }

    [DisplayName("Mode Premium")]
    public decimal? ModePremium { get; set; }

    [DisplayName("Policy Count")]
    public ExcelDataCell? PolicyCount { get; set; }

    [DisplayName("Annualized Premium")]
    public decimal? AnnualPremium { get; set; }
}
