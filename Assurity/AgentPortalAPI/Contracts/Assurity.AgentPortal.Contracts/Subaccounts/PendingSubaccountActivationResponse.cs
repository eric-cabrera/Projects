namespace Assurity.AgentPortal.Contracts.Subaccounts;

public class PendingSubaccountActivationResponse
{
    public bool Valid { get; set; }

    public int ActivationAttempts { get; set; }

    public PendingSubaccount? Subaccount { get; set; }
}
