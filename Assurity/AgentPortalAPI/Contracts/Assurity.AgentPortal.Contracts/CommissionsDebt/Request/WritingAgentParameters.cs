namespace Assurity.AgentPortal.Contracts.CommissionsDebt.Request;

using Assurity.AgentPortal.Contracts.Enums;

public class WritingAgentParameters : CommissionParameters
{
    public WritingAgentOrderBy? OrderBy { get; set; }
}
