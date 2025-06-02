namespace Assurity.AgentPortal.Accessors.SubAccount.DTOs.AgentCenterAPIRequests;

public class DeletePendingSubaccountRequest
{
    public string Email { get; set; } = string.Empty;

    public string ParentAgentId { get; set; } = string.Empty;
}