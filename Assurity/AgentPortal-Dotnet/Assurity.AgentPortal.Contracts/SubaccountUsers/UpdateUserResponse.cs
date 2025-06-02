namespace Assurity.AgentPortal.Contracts.SubaccountUsers;

public class UpdateUserResponse
{
    public string? UserId { get; set; }

    public List<string>? Roles { get; set; }
}
