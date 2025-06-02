namespace Assurity.AgentPortal.Contracts.CaseManagement;

using Assurity.ApplicationTracker.Contracts.DataTransferObjects;

public class CaseManagementResponse
{
    public List<CaseManagementCase>? Cases { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int NumberOfPages { get; set; }

    public int TotalRecords { get; set; }
}
