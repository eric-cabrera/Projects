namespace Assurity.AgentPortal.Contracts.Claims;

public class ClaimsParameters
{
    public string? ClaimNumber { get; set; }

    public int? PageNumber { get; set; }

    public int? PageSize { get; set; }

    public string? PolicyNumber { get; set; }

    public string? ClaimantFirstName { get; set; }

    public string? ClaimantLastName { get; set; }
}
