namespace Assurity.AgentPortal.Contracts.ProductionCredit.Response.Worksite;

public class WorksiteProductionCredit
{
    /// <summary>
    /// Only applies to agent data. Not populated for product data.
    /// </summary>
    public string Grouping { get; set; }

    public string AgentId { get; set; }

    public string AgentName { get; set; }

    public int? AgentCount { get; set; }

    /// <summary>
    /// Product or Group Name.
    /// </summary>
    public string Name { get; set; }

    public DateTime? EffectiveDate { get; set; }

    public string GroupNumber { get; set; }

    public int? GroupCountCurrent { get; set; }

    public int? GroupCountPrevious { get; set; }

    public decimal? GroupCountChangePercent { get; set; }

    public decimal PolicyCountCurrent { get; set; }

    public decimal? PolicyCountPrevious { get; set; }

    public decimal? PolicyCountChangePercent { get; set; }

    public decimal PremiumCurrent { get; set; }

    public decimal? PremiumPrevious { get; set; }

    public decimal? PremiumChangePercent { get; set; }

    public List<WorksiteProductionCredit> Children { get; set; }
}
