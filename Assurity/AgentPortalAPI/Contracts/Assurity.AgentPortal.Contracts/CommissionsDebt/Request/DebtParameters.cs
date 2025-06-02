namespace Assurity.AgentPortal.Contracts.CommissionsDebt.Request;

using Assurity.AgentPortal.Contracts.Enums;

public partial class DebtParameters
{
    public string? AgentId { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public string? PolicyNumber { get; set; }

    public List<string>? WritingAgentIds { get; set; }

    public string? HierarchyAgentId { get; set; }

    public string? HierarchyMarketCode { get; set; }

    public string? HierarchyCompanyCode { get; set; }

    public string? HierarchyLevel { get; set; }

    public string? Status { get; set; }

    public bool IncludeFilters { get; set; } = false;

    public SortDirection? SortDirection { get; set; }
}
