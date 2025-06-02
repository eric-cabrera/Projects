namespace Assurity.AgentPortal.Accessors.SubAccount.DTOs.AgentCenterAPIResponses;

public class PendingSubaccountActivationResponse
{
    public bool Valid { get; set; }

    public int ActivationAttempts { get; set; }

    public string? Message { get; set; }

    public PendingSubaccount? Subaccount { get; set; }
}