namespace Assurity.AgentPortal.Contracts.PolicyInfo;

public class PolicySummariesResponse
{
    public int TotalPages { get; set; }

    public int CurrentPage { get; set; }

    public int TotalPolicies { get; set; }

    public FilterValues? Filters { get; set; }

    public List<PolicySummary>? Policies { get; set; }
}
