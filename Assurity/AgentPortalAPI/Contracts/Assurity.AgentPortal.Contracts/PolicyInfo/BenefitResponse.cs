namespace Assurity.AgentPortal.Contracts.PolicyInfo;

public class BenefitResponse
{
    public decimal? BenefitAmount { get; set; }

    public string BenefitDescription { get; set; }

    public long BenefitId { get; set; }

    public string BenefitStatus { get; set; }

    public string? BenefitStatusReason { get; set; }

    public string CoverageType { get; set; }

    public string? DeathBenefitOption { get; set; }

    public string? DividendOption { get; set; }

    public string PlanCode { get; set; }

    public List<BenefitOptionResponse> BenefitOptions { get; set; }
}
