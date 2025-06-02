namespace Assurity.AgentPortal.Contracts.CommissionsDebt;

using System.ComponentModel;

public class WritingAgentDetail
{
    [DisplayName("Agent ID")]
    public string? AgentId { get; set; }

    [DisplayName("Agent Name")]
    public string? AgentName { get; set; }

    [DisplayName("First Year Commissions")]
    public decimal CycleFirstYearCommissions { get; set; }

    [DisplayName("Renewal Commissions")]
    public decimal CycleRenewalCommissions { get; set; }

    [DisplayName("First Year Commissions YTD")]
    public decimal FirstYearCommissionsYtd { get; set; }

    [DisplayName("Renewal Commissions YTD")]
    public decimal RenewalCommissionsYtd { get; set; }
}
