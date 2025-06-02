namespace Assurity.AgentPortal.Contracts.CommissionsDebt;

public class CommissionDetails
{
    public List<PolicyDetail>? PolicyDetails { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public long TotalRecords { get; set; }

    public PolicyDetailFilterValues? Filters { get; set; }
}
