namespace Assurity.AgentPortal.Contracts.CommissionsDebt;

public class DebtAgent
{
    public string AgentName { get; set; }

    public string AgentId { get; set; }

    public string AgentStatus { get; set; }

    public decimal? UnsecuredAdvanceOwed { get; set; }

    public decimal? SecuredAdvanceOwed { get; set; }

    public decimal? ReversedCommissionOwed { get; set; }

    public decimal? BalanceOwed { get; set; }

    public List<DebtPolicy> Policies { get; set; }
}
