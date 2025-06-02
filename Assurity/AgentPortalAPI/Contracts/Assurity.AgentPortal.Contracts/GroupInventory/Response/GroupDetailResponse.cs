namespace Assurity.AgentPortal.Contracts.GroupInventory.Response;

public class GroupDetailResponse
{
    public Group? Group { get; set; }

    public List<Policy>? Policies { get; set; }

    public GroupDetailFilters? Filters { get; set; }
}
