namespace Assurity.AgentPortal.Contracts.CommissionsDebt.Request;

using Assurity.AgentPortal.Contracts.Enums;

public class UnsecuredAdvanceParameters : DebtParameters
{
    public UnsecuredAdvanceOrderBy? OrderBy { get; set; }
}
