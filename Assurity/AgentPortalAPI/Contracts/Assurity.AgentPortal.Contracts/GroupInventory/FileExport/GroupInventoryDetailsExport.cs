namespace Assurity.AgentPortal.Contracts.GroupInventory.FileExport;

using System;
using System.ComponentModel;

public class GroupInventoryDetailsExport
{
    [DisplayName("Primary Owner")]
    public string? PrimaryOwner { get; set; }

    [DisplayName("Policy Number")]
    public string? PolicyNumber { get; set; }

    [DisplayName("Policy Status")]
    public string? PolicyStatus { get; set; }

    [DisplayName("Issue Date")]
    public DateTime? IssueDate { get; set; }

    [DisplayName("Paid to Date")]
    public DateTime? PaidToDate { get; set; }

    [DisplayName("Annual Premium")]
    public string? AnnualPremium { get; set; }

    [DisplayName("Mode Premium")]
    public string? ModePremium { get; set; }

    [DisplayName("Mode")]
    public string? Mode { get; set; }

    [DisplayName("Product Description")]
    public string? ProductDescription { get; set; }

    [DisplayName("Coverage Type")]
    public string? CoverageType { get; set; }

    [DisplayName("Primary Insured")]
    public string? PrimaryInsured { get; set; }

    [DisplayName("Insured DOB")]
    public DateTime? InsuredDOB { get; set; }

    [DisplayName("Current City")]
    public string? CurrentCity { get; set; }

    [DisplayName("Benefits")]
    public string? Benefits { get; set; }

    [DisplayName("Benefit Description")]
    public string? BenefitDescription { get; set; }

    [DisplayName("Benefit Amount")]
    public string? BenefitAmount { get; set; }

    [DisplayName("Coverage Options")]
    public string? CoverageOptions { get; set; }
}
