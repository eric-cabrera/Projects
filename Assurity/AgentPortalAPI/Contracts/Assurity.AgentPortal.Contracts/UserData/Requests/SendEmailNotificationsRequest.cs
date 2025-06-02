namespace Assurity.AgentPortal.Contracts.UserData.Requests;

public class SendEmailNotificationsRequest
{
    public string OriginalEmail { get; set; }

    public string NewEmail { get; set; }
}
