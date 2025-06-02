namespace Assurity.AgentPortal.Contracts.CommissionsDebt.FileExport;

using System.ComponentModel;

public class UnsecuredDebtExport
{
    [DisplayName("Agent Name")]
    public string AgentName { get; set; }

    [DisplayName("Agent ID")]
    public string AgentId { get; set; }

    [DisplayName("Agent Status")]
    public string AgentStatus { get; set; }

    [DisplayName("PolicyNumber")]
    public string PolicyNumber { get; set; }

    [DisplayName("InsuredName")]
    public string InsuredName { get; set; }

    [DisplayName("Application Date")]
    public DateTime? ApplicationDate { get; set; }

    [DisplayName("Paid to Date")]
    public DateTime? PaidToDate { get; set; }

    [DisplayName("Unsecured Advance Owed")]
    public decimal UnsecuredAdvanceOwed { get; set; }
}
