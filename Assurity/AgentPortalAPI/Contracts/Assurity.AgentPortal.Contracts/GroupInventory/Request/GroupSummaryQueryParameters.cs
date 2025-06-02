namespace Assurity.AgentPortal.Contracts.GroupInventory.Request;

using Assurity.AgentPortal.Contracts.Enums;

public class GroupSummaryQueryParameters
{
    public string? ViewAsAgentId { get; set; }

    /// <summary>
    /// Must match group number exactly.
    /// </summary>
    public string? GroupNumber { get; set; }

    /// <summary>
    /// Must match name of group exactly.
    /// </summary>
    public string? GroupName { get; set; }

    public DateTime? GroupEffectiveStartDate { get; set; }

    public DateTime? GroupEffectiveEndDate { get; set; }

    /// <summary>
    /// Multiple group statuses are accepted in a semicolon separated list.
    /// Must be an exact match (Active, Suspended, Terminated).
    /// </summary>
    public string? GroupStatus { get; set; }

    public SummaryOrderBy? OrderBy { get; set; }

    public SortDirection SortDirection { get; set; }

    public int? Page { get; set; }

    public int? PageSize { get; set; }
}
