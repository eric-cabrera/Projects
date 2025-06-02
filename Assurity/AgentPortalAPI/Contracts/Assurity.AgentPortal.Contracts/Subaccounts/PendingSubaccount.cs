namespace Assurity.AgentPortal.Contracts.Subaccounts;

using Assurity.AgentPortal.Contracts.Enums;

public class PendingSubaccount
{
    public string Id { get; set; }

    public string? AgentId { get; set; }

    public string? ParentUsername { get; set; }

    public string? Email { get; set; }

    public List<Role>? Roles { get; set; }
}
