namespace Assurity.AgentPortal.Contracts.Impersonation;

public class ImpersonationRecord
{
    public string Id { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? Name { get; set; }

    public string? RegisteredAgentId { get; set; }

    public List<AgentRecord>? Agents { get; set; }
}
