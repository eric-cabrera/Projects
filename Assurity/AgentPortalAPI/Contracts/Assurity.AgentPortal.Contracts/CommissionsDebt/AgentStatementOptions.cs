namespace Assurity.AgentPortal.Contracts.CommissionsDebt;

using CommissionsAPI = Assurity.Commissions.Internal.Contracts.Shared;

public class AgentStatementOptions
{
    public List<CommissionsAPI.Agent> Agents { get; set; }

    public List<int> CycleDates { get; set; }
}
