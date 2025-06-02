namespace Assurity.AgentPortal.Utilities.Emails;

using global::MongoDB.Bson;
using MimeKit;

public interface IEmailUtility
{
    MimeMessage CreateSubaccountActivationEmail(string activationId, string agentEmail);

    MimeMessage CreateMFAEmailChangeEmail(string emailAddress, bool originalEmail);

    Task SendEmail(MimeMessage message);
}