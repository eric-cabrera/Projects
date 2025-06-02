namespace Assurity.AgentPortal.Accessors.DTOs;

public class PendingSubaccountDTO
{
    public string Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string ParentAgentId { get; set; } = string.Empty;

    public string ParentUsername { get; set; } = string.Empty;

    public IEnumerable<string> Roles { get; set; }

    public Guid LinkGuid { get; set; }

    public int ActivationAttempts { get; set; }

    public DateTime EmailSentAt { get; set; }
}
