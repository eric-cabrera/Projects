namespace Assurity.AgentPortal.Contracts.CommissionsDebt;

public class CommissionSummary
{
    public decimal TaxableCommissionsTotal { get; set; }

    public decimal FirstYearCommissionsCurrentYear { get; set; }

    public decimal FirstYearCommissionsPreviousYear { get; set; }

    public decimal RenewalCommissionsCurrentYear { get; set; }

    public decimal RenewalCommissionsPreviousYear { get; set; }
}
