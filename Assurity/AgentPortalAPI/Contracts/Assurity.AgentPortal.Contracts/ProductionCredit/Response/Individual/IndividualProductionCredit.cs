namespace Assurity.AgentPortal.Contracts.ProductionCredit.Response.Individual;

public class IndividualProductionCredit
{
    /// <summary>
    /// Logical grouping of the data. E.g., "Lines of Business" or "Writing Agents".
    /// </summary>
    public string Grouping { get; set; }

    /// <summary>
    /// Only applies to agent data. Not populated for product data.
    /// </summary>
    public string AgentId { get; set; }

    /// <summary>
    /// Value to display on UI. If agent data, this will be agent name in Last, First Middle [...] format.
    /// Otherwise, line of business, product, product type, etc.
    /// </summary>
    public string Name { get; set; }

    public decimal PremiumCurrent { get; set; }

    public decimal? PremiumPrevious { get; set; }

    public decimal? PremiumChangePercent { get; set; }

    public decimal PolicyCountCurrent { get; set; }

    public decimal? PolicyCountPrevious { get; set; }

    public decimal? PolicyCountChangePercent { get; set; }

    /// <summary>
    /// A collection of agents or products. These may in turn contain collections of more agents or products depending on the report.
    /// </summary>
    public List<IndividualProductionCredit> Children { get; set; }
}
