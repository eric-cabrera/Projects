namespace Assurity.AgentPortal.Contracts.Subaccounts.Request;

using Assurity.AgentPortal.Contracts.Enums;

public class SubaccountRequest
{
    public string Email { get; set; }

    public List<Role> Roles { get; set; }
}
