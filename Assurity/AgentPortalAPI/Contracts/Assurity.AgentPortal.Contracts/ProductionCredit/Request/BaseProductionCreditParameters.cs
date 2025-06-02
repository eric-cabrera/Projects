namespace Assurity.AgentPortal.Contracts.ProductionCredit.Request;

public class BaseProductionCreditParameters
{
    /// <summary>
    /// Which field to order the results by.
    /// </summary>
    public string? OrderBy { get; set; }

    public string? HierarchyAgentId { get; set; }

    /// <summary>
    /// Restricts results to this agent.
    /// </summary>
    public string? ViewAsAgentId { get; set; }

    /// <summary>
    /// Semicolon separated list.
    /// </summary>
    public string? WritingAgentIds { get; set; }

    /// <summary>
    /// Semicolon separated list.
    /// </summary>
    public string? LinesOfBusiness { get; set; }

    /// <summary>
    /// Semicolon separated list.
    /// </summary>
    public string? MarketCodes { get; set; }

    /// <summary>
    /// Semicolon separated list.
    /// </summary>
    public string? ProductDescriptions { get; set; }

    /// <summary>
    /// Semicolon separated list.
    /// </summary>
    public string? ProductTypes { get; set; }

    /// <summary>
    /// <list type="bullet">
    /// How to sort the data. Possible values:
    /// <item><description>ASC</description></item>
    /// <item><description>DESC</description></item>
    /// </list>
    /// </summary>
    public string? SortDirection { get; set; }
}
