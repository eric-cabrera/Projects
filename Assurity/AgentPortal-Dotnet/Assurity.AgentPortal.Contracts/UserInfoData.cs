namespace Assurity.AgentPortal.Contracts;

public record UserInfoData
{
    public string Username { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string AgentId { get; set; }

    public string EmailAddress { get; set; }

    public bool IsSubaccount { get; set; }

    public bool IsAuthenticated { get; set; }

    public bool IsHomeOffice { get; set; }

    public List<string> Roles { get; set; }

    public List<string> ProductionCreditBusinessTypes { get; set; }

    public string ExpiresAt { get; set; }

    public bool IsPingOneAudience { get; set; }

    public AgentData AgentData { get; set; }
}
