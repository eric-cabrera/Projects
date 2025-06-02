namespace Assurity.AgentPortal.Accessors.SubAccount.DTOs.PingResponses;

public class LastSignOnDTO
{
    public DateTime? At { get; set; }

    public string RemoteIp { get; set; } = string.Empty;
}
