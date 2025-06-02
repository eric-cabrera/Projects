namespace Assurity.AgentPortal.Contracts.PolicyInfo.FileExport;

using System.ComponentModel;

/// <summary>
/// The FileExportEngine will use this class to format the excel document.
/// Properties will be displayed in order as the column headers and information.
/// Property data type will determine the cell's formatting.
/// </summary>
public class PendingPolicyExport
{
    // Requirement AddedDate
    [DisplayName("Entry Date")]
    public DateTime? EntryDate { get; set; }

    [DisplayName("Agent Last Name")]
    public string? AgentLastName { get; set; }

    [DisplayName("Agent First Name")]
    public string? AgentFirstName { get; set; }

    [DisplayName("Agent ID")]
    public string? AgentId { get; set; }

    [DisplayName("Policy Number")]
    public string? PolicyNumber { get; set; }

    [DisplayName("Product Type")]
    public string? ProductCategory { get; set; }

    [DisplayName("Primary Insured Last Name")]
    public string? PrimaryInsuredLastName { get; set; }

    [DisplayName("Primary Insured First Name")]
    public string? PrimaryInsuredFirstName { get; set; }

    [DisplayName("Requirement")]
    public string? Requirement { get; set; }

    [DisplayName("Met")]
    public string? Met { get; set; }

    [DisplayName("Requirement Applies To")]
    public string? RequirementAppliesTo { get; set; }

    // Global Comment + LifePro Comment + Phone Number Comment
    [DisplayName("Requirement Comment")]
    public string? RequirementComment { get; set; }

    // If the RequirementFulfillingParty == Agent, return RequirementActionType in a readable format
    [DisplayName("Action Needed")]
    public string? ActionNeeded { get; set; }

    [DisplayName("Application Signed Date")]
    public DateTime? ApplicationSignedDate { get; set; }

    [DisplayName("Primary Owner Last Name")]
    public string? PrimaryOwnerLastName { get; set; }

    [DisplayName("Primary Owner First Name")]
    public string? PrimaryOwnerFirstName { get; set; }

    [DisplayName("Primary Owner Phone Number")]
    public string? PrimaryOwnerPhoneNumber { get; set; }

    [DisplayName("Primary Owner Email")]
    public string? PrimaryOwnerEmail { get; set; }

    [DisplayName("Employer")]
    public string? EmployerName { get; set; }

    [DisplayName("Employer Number")]
    public string? EmployerNumber { get; set; }
}
