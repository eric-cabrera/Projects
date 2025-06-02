namespace Assurity.AgentPortal.Contracts.ProductionCredit.Request;

public class WorksiteProductionCreditParameters : ProductionCreditParameters
{
    public string? GroupNames { get; set; }

    public string? GroupNumbers { get; set; }

    public DateTime? EffectiveDateStart { get; set; }

    public DateTime? EffectiveDateEnd { get; set; }
}
