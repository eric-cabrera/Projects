namespace Assurity.AgentPortal.Contracts.ListBill;

public class GroupsResponse
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalRecords { get; set; }

    public List<Group> Groups { get; set; }
}