namespace Assurity.AgentPortal.Contracts.CommissionsDebt;

using Assurity.AgentPortal.Contracts.CommissionsDebt.Response;

public class CommissionResponse
{
    public CommissionCycle? CommissionCycle { get; set; }

    public CommissionDetails? CommissionDetails { get; set; }

    public CommissionSummary? CommissionSummary { get; set; }
}
