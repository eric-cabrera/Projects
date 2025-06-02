using Assurity.AgentPortal.Contracts.SubaccountUsers;

namespace Assurity.AgentPortal.Accessors.SubAccount.DTOs.AgentCenterAPIRequests;

public class PendingSubaccountRequest
{
    public string Email { get; set; } = string.Empty;

    public List<SubAccountRole> Roles { get; set; }
}