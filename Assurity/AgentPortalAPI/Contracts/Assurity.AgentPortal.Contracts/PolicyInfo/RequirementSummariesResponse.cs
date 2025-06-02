namespace Assurity.AgentPortal.Contracts.PolicyInfo;

public class RequirementSummariesResponse
{
    public int TotalRequirements { get; set; }

    public int TotalPages { get; set; }

    public int CurrentPage { get; set; }

    public FilterValues? Filters { get; set; }

    public List<RequirementSummary>? RequirementSummaries { get; set; }
}
