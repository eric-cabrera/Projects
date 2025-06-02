namespace Assurity.AgentPortal.Contracts.GroupInventory.Response;

public class Policy
{
    public string? PrimaryOwner { get; set; }

    public string? Number { get; set; }

    public string? Status { get; set; }

    public string? IssueDate { get; set; }

    public string? PaidToDate { get; set; }

    public string? AnnualPremium { get; set; }

    public string? ModePremium { get; set; }

    public string? Mode { get; set; }

    public string? ProductDescription { get; set; }

    public string? CoverageType { get; set; }

    public Insured? PrimaryInsured { get; set; }

    public List<Benefit>? Benefits { get; set; }
}
