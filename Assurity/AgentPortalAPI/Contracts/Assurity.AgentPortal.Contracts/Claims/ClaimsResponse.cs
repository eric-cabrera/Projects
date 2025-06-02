namespace Assurity.AgentPortal.Contracts.Claims;

public class ClaimsResponse
{
    public List<Claim>? Claims { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalRecords { get; set; }
}
