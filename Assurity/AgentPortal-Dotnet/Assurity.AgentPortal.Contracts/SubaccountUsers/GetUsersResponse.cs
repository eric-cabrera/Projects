namespace Assurity.AgentPortal.Contracts.SubaccountUsers;

public class GetUsersResponse
{
    public string UserId { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; } = string.Empty;

    public bool ActivationStatus { get; set; } = false;

    public List<SubAccountRole>? Roles { get; set; }
}
