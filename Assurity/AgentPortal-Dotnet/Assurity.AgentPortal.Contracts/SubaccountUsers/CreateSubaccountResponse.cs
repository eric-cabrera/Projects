namespace Assurity.AgentPortal.Contracts.SubaccountUsers;

public class CreateSubaccountReponse
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public GetUsersResponse Subaccount { get; set; }

}
