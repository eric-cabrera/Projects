namespace Assurity.AgentPortal.Contracts.CommissionsDebt.Request;

using Assurity.AgentPortal.Contracts.Enums;

public class SecuredAdvanceParameters : DebtParameters
{
    public SecuredAdvanceOrderBy? OrderBy { get; set; }
}
