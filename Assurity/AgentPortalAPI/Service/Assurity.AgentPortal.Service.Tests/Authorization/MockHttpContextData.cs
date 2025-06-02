namespace Assurity.AgentPortal.Service.Tests.Authorization;

using Assurity.AgentPortal.Contracts.AgentContracts;

public class MockHttpContextData()
{
    public string Issuer { get; set; } = "AzureAd";

    public bool IsHomeOfficeUser { get; set; } = true;

    public bool IsPingUser { get; set; } = false;

    public bool IsSubaccount { get; set; } = false;

    public string AgentId { get; set; } = string.Empty;

    public string ParentAgentId { get; set; } = string.Empty;

    public string AgentUsername { get; set; } = string.Empty;

    public string AgentEmail { get; set; } = string.Empty;

    public AccessLevel? AgentAccessLevel { get; set; } = AccessLevel.Full;

    public Dictionary<object, object> Items
    {
        get
        {
            return new Dictionary<object, object>()
            {
                { "Issuer", Issuer },
                { "IsHomeOfficeUser", IsHomeOfficeUser },
                { "IsPingUser", IsPingUser },
                { "IsSubaccount", IsSubaccount },
                { "AgentId", AgentId },
                { "ParentAgentId", ParentAgentId },
                { "AgentUsername", AgentUsername },
                { "AgentEmail", AgentEmail },
                { "AgentAccessLevel", AgentAccessLevel },
            };
        }
    }

    public void SetupHomeOfficeUser()
    {
        Issuer = "PingOne";
        IsHomeOfficeUser = true;
        IsPingUser = false;
        AgentId = null;
        AgentAccessLevel = null;
    }

    public void SetupHomeOfficeImpersonationUser(string agentId)
    {
        Issuer = "AzureAd";
        IsHomeOfficeUser = true;
        IsPingUser = false;
        AgentId = agentId;
    }

    public void SetupPingOneUser(string agentId)
    {
        Issuer = "PingOne";
        IsHomeOfficeUser = false;
        IsPingUser = true;
        AgentId = agentId;
    }

    public void SetupSubaccount(string parentAgentId)
    {
        Issuer = "PingOne";
        IsHomeOfficeUser = false;
        IsSubaccount = true;
        IsPingUser = true;
        AgentId = "subaccount";
        ParentAgentId = parentAgentId;
    }
}
