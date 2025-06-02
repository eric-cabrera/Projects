namespace Assurity.AgentPortal.Contracts.ProductionCredit.Response.Individual;

using Assurity.AgentPortal.Contracts.ProductionCredit.Response.Shared;

public class IndividualProductionCreditSummary
{
    public List<IndividualProductionCredit> ProductionByDownline { get; set; }

    public int ProductionByDownlineCount { get; set; }

    public List<IndividualProductionCredit> ProductionByProduct { get; set; }

    public int ProductionByProductCount { get; set; }

    public decimal TotalPremiumCurrent { get; set; }

    public decimal TotalPremiumPrevious { get; set; }

    public decimal? TotalPremiumChangePercent { get; set; }

    public decimal TotalPolicyCountCurrent { get; set; }

    public decimal TotalPolicyCountPrevious { get; set; }

    public decimal? TotalPolicyCountChangePercent { get; set; }

    /// <summary>
    /// Sub-reports ordered descending by Gross Annual Premium: "Downline," "Writing Agents," "Lines of Business," "Hierarchy".
    /// </summary>
    public List<ProductionCreditSupplementalReport> SupplementalReports { get; set; }

    public ProductionCreditFilterValues Filters { get; set; }
}
