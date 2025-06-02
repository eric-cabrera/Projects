namespace Assurity.AgentPortal.Contracts.CommissionsDebt;

public class DebtPolicy
{
    public string PolicyNumber { get; set; }

    public string InsuredName { get; set; }

    public DateTime ApplicationDate { get; set; }

    public DateTime? PaidToDate { get; set; }

    public decimal? UnsecuredAdvanceOwed { get; set; }

    public decimal? SecuredAdvanceOwed { get; set; }
}
