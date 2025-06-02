namespace Assurity.AgentPortal.Contracts.ProductionCredit.Response.Worksite;

public class WorksiteProductionCreditSummary
{
    public List<WorksiteProductionCredit> ProductionByAgent { get; set; }

    public List<WorksiteProductionCredit> ProductionByGroup { get; set; }

    public List<WorksiteProductionCredit> ProductionByProduct { get; set; }

    public int TotalGroupCountCurrent { get; set; }

    public int TotalGroupCountPrevious { get; set; }

    public decimal? TotalGroupCountChangePercent { get; set; }

    public decimal TotalPolicyCountCurrent { get; set; }

    public decimal TotalPolicyCountPrevious { get; set; }

    public decimal? TotalPolicyCountChangePercent { get; set; }

    public decimal TotalPremiumCurrent { get; set; }

    public decimal TotalPremiumPrevious { get; set; }

    public decimal? TotalPremiumChangePercent { get; set; }

    public ProductionByAgentSupplementalReport ProductionByAgentSupplementalReport { get; set; }

    public ProductionCreditWorksiteFilterValues Filters { get; set; }
}
