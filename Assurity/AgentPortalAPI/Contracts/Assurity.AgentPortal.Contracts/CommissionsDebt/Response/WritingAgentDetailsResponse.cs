namespace Assurity.AgentPortal.Contracts.CommissionsDebt;

using Assurity.AgentPortal.Contracts.CommissionsDebt.Response;

public class WritingAgentDetailsResponse
{
    public CommissionCycle? CommissionCycle { get; set; }

    public decimal CyclePaidFirstYearTotal { get; set; }

    public decimal CyclePaidRenewalTotal { get; set; }

    public decimal YearTotDateFirstYearTotal { get; set; }

    public decimal YearToDateRenewalTotal { get; set; }

    public List<WritingAgentDetail>? WritingAgentCommissions { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalRecords { get; set; }

    public WritingAgentFilters? Filters { get; set; }
}
