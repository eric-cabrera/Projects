namespace Assurity.AgentPortal.Utilities.Emails;

using Assurity.AgentPortal.Utilities.Configs;
using MailKit.Net.Smtp;
using MimeKit;

public class EmailUtility : IEmailUtility
{
    public EmailUtility(IConfigurationManager configuration)
    {
        Config = configuration;
    }

    public IConfigurationManager Config { get; }

    public MimeMessage CreateSubaccountActivationEmail(string activationId, string agentEmail)
    {
        var message = new MimeMessage();
        var from = "noreply@assurity.com";
        var subject = "Please activate your account with Assurity";
        message.From.Add(new MailboxAddress(string.Empty, from));
        message.To.Add(new MailboxAddress(string.Empty, agentEmail));
        message.Subject = subject;

        var activationLink = $"{Config.BaseAgentCenterUrl}/account-confirmation?id={activationId}";
        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
                    <h1>Activate your Assurity Agent Center account</h1>
                    <p>You're receiving this email because you created an Agent Center account or 
                        someone else created one for you. To complete your registration, <b>please click the link below within 24 hours.</b>
                        If you received this email in error, no action is necessary.
                    </p>
                    <a href='{activationLink}' style='display:inline-block;padding:10px 20px;margin:10px 0;font-size:16px;color:#fff;background-color:#007bff;text-decoration:none;border-radius:5px;'>Activate Account</a>
                    <p>This is an automated email, please do not reply. For inquiries, please contact the Assurity Help Desk at 
                        <a href=""helpdesk@assurity.com"">helpdesk@assurity.com</a>
                    </p>"
        };

        message.Body = bodyBuilder.ToMessageBody();

        return message;
    }

    public MimeMessage CreateMFAEmailChangeEmail(string emailAddress, bool originalEmail)
    {
        var message = new MimeMessage();
        var from = "noreply@assurity.com";
        var subject = "Account E-mail Changed";
        message.From.Add(new MailboxAddress(string.Empty, from));
        message.To.Add(new MailboxAddress(string.Empty, emailAddress));
        message.Subject = subject;

        if (originalEmail)
        {
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                        <p>Your e-mail address has been changed by your request. This e-mail address will no longer be used by the AssureLink website.</p>
                        <p>If you did not intend to have your e-mail address updated, please contact the Assurity Help Desk at 
                            <a href=""helpdesk@assurity.com"">helpdesk@assurity.com</a>
                            or at (800) 276-7619 Ext. 4333. The help desk is available from 7:00 a.m. to 5:00 p.m. Central Time, Monday through Friday. 
                        </p>"
            };

            message.Body = bodyBuilder.ToMessageBody();
        }
        else
        {
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <p>Your e-mail address has been changed by your request. This e-mail address will now be used by the AssureLink website.</p>
                    <p>If you did not intend to have your e-mail address updated, please contact the Assurity Help Desk at 
                        <a href=""helpdesk@assurity.com"">helpdesk@assurity.com</a>
                        or at (800) 276-7619 Ext. 4333. The help desk is available from 7:00 a.m. to 5:00 p.m. Central Time, Monday through Friday. 
                    </p>"
            };

            message.Body = bodyBuilder.ToMessageBody();
        }

        return message;
    }

    public async Task SendEmail(MimeMessage message)
    {
        if (message != null && !string.IsNullOrEmpty(message.Subject))
        {
            using var smtp = new SmtpClient();

            // TODO: Can we connect to SMTP server using SSL?
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true; // bypass SSL validation & the hostname verification (not recommended for production).
            await smtp.ConnectAsync(Config.SmtpHost, Config.SmtpPort, false); // useSsl = false
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}
