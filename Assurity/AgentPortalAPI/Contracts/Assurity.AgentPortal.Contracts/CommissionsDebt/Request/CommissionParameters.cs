namespace Assurity.AgentPortal.Contracts.CommissionsDebt.Request;

using Assurity.AgentPortal.Contracts.Enums;

public class CommissionParameters
{
    public string? ViewAsAgentId { get; set; }

    public DateTimeOffset? CycleBeginDate { get; set; }

    public DateTimeOffset? CycleEndDate { get; set; }

    public SortDirection? SortDirection { get; set; }

    public int? Page { get; set; }

    public int? PageSize { get; set; }

    public List<string>? WritingAgentIds { get; set; }
}
