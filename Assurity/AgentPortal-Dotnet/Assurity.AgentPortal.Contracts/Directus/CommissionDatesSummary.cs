namespace Assurity.AgentPortal.Contracts.Directus;

public class CommissionDatesSummary
{
    public List<string> CommissionsProcessed { get; set; }

    public List<string> StatementsAvailable { get; set; }

    public List<string> DirectDeposit { get; set; }
}
