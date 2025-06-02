namespace Assurity.AgentPortal.Contracts.CommissionsDebt.Request;

using Assurity.AgentPortal.Contracts.Enums;

public class PolicyDetailsParameters : CommissionParameters
{
    public string? PolicyNumber { get; set; }

    public PolicyDetailsOrderBy? OrderBy { get; set; }
}
