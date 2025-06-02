namespace Assurity.AgentPortal.Accessors.Profile;

public class SendConfirmationEmailsRequest
{
    public string OriginalEmail { get; set; } = string.Empty;

    public string NewEmail { get; set; } = string.Empty;
}