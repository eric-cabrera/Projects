namespace Assurity.AgentPortal.Accessors.SubAccount.DTOs.PingResponses;

using System.Text.Json.Serialization;

public class LinksDTO
{
    [JsonPropertyName("password")]
    public LinkDTO? Password { get; set; }

    [JsonPropertyName("password.set")]
    public LinkDTO? PasswordSet { get; set; }

    [JsonPropertyName("account.sendVerificationCode")]
    public LinkDTO? AccountSendVerificationCode { get; set; }

    [JsonPropertyName("linkedAccounts")]
    public LinkDTO? LinkedAccounts { get; set; }

    [JsonPropertyName("self")]
    public LinkDTO? Self { get; set; }

    [JsonPropertyName("password.check")]
    public LinkDTO? PasswordCheck { get; set; }

    [JsonPropertyName("password.reset")]
    public LinkDTO? PasswordReset { get; set; }

    [JsonPropertyName("password.recover")]
    public LinkDTO? PasswordRecover { get; set; }

    [JsonPropertyName("next")]
    public LinkDTO? Next { get; set; }
}
