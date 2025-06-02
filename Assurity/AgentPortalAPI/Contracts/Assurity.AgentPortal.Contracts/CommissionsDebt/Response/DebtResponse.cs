namespace Assurity.AgentPortal.Contracts.CommissionsDebt;

public class DebtResponse
{
    public List<DebtAgent>? Agents { get; set; }

    public DebtFilterValues? Filters { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public long TotalRecords { get; set; }

    public decimal TotalDebt { get; set; }
}
