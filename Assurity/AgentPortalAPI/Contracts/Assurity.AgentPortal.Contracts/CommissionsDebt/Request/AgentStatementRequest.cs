namespace Assurity.AgentPortal.Contracts.CommissionsDebt.Request;

using Assurity.AgentPortal.Contracts.Enums;

public class AgentStatementRequest
{
    public string AgentId { get; set; }

    public DateTime? CycleDate { get; set; }

    public AgentStatementType? AgentStatementType { get; set; }
}
