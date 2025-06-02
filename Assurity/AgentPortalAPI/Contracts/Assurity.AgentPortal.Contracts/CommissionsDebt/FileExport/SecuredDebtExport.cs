namespace Assurity.AgentPortal.Contracts.CommissionsDebt.FileExport;

using System.ComponentModel;

public class SecuredDebtExport
{
    [DisplayName("Agent Name")]
    public string AgentName { get; set; }

    [DisplayName("Agent ID")]
    public string AgentId { get; set; }

    [DisplayName("Agent Status")]
    public string AgentStatus { get; set; }

    [DisplayName("Policy Number")]
    public string PolicyNumber { get; set; }

    [DisplayName("Insured Name")]
    public string InsuredName { get; set; }

    [DisplayName("Application Date")]
    public DateTime? ApplicationDate { get; set; }

    [DisplayName("Paid to Date")]
    public DateTime? PaidToDate { get; set; }

    [DisplayName("Secured Advance Owed")]
    public decimal SecuredAdvanceOwed { get; set; }
}
