namespace Assurity.AgentPortal.Contracts.GroupInventory.Response;

public class GroupSummaryFilters
{
    public List<GroupNameAndNumber>? GroupNamesAndNumbers { get; set; }

    public List<string>? GroupStatusValues { get; set; }

    public int? Page { get; set; }

    public int? PageSize { get; set; }

    public int? TotalPageCount { get; set; }

    public List<string>? ViewAsAgents { get; set; }
}
