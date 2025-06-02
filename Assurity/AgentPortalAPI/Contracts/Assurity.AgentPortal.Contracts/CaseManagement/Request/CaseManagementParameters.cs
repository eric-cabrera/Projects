namespace Assurity.AgentPortal.Contracts.CaseManagement;

using Assurity.AgentPortal.Contracts.Enums;

public class CaseManagementParameters
{
    public string? ViewAsAgentId { get; set; }

    public string? PrimaryInsuredName { get; set; }

    public DateOnly? CreatedDateBegin { get; set; }

    public DateOnly? CreatedDateEnd { get; set; }

    // Semi Colon Seperated String
    public string? ProductTypes { get; set; }

    // Semi Colon Seperated String
    public string? EventTypes { get; set; }

    public string? SortColumn { get; set; }

    public SortDirection SortOrder { get; set; }

    public int? PageNumber { get; set; }

    public int? PageSize { get; set; }
}
