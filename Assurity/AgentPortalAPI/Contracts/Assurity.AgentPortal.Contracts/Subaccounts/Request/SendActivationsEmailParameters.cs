namespace Assurity.AgentPortal.Contracts.Emails.Request;

using Assurity.AgentPortal.Contracts.Enums;

public class SendActivationsEmailParameters
{
    public string? AgentEmail { get; set; } = string.Empty;

    public string? AgentName { get; set; } = string.Empty;

    public List<Role>? AgentRoles { get; set; }
}
