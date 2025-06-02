namespace Assurity.AgentPortal.Accessors.DTOs;

using Assurity.AgentPortal.Contracts.Enums;

public class CommissionParameters
{
    public string? AgentId { get; set; }

    public DateTimeOffset? CycleBeginDate { get; set; }

    public DateTimeOffset? CycleEndDate { get; set; }

    public int? Page { get; set; }

    public int? PageSize { get; set; }

    public List<string>? WritingAgentIds { get; set; }

    public string? PolicyNumber { get; set; }

    public string? OrderBy { get; set; }

    public SortDirection? SortDirection { get; set; }

    public bool? DisablePagination { get; set; }
}
