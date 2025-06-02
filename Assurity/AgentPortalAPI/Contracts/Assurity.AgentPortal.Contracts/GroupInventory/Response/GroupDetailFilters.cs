namespace Assurity.AgentPortal.Contracts.GroupInventory.Response;

public class GroupDetailFilters
{
    public List<string>? PolicyOwners { get; set; }

    public List<string>? PolicyNumbers { get; set; }

    public List<string>? PolicyStatuses { get; set; }

    public List<string>? ProductDescriptions { get; set; }

    public int? Page { get; set; }

    public int? PageSize { get; set; }

    public int? TotalPageCount { get; set; }
}