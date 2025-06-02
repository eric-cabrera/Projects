namespace Assurity.AgentPortal.Contracts.PolicyInfo;

public class RequirementSummary
{
    public RequirementResponse Requirement { get; set; }

    public List<AssignedAgent>? AssignedAgents { get; set; }

    public string? EmployerName { get; set; }

    public string? PrimaryInsuredName { get; set; }

    public string? ProductCategory { get; set; }

    public string PolicyNumber { get; set; }
}
