namespace Assurity.AgentPortal.Contracts.PolicyInfo;

public class ActiveStatusCounts
{
    public int TotalPolicies { get; set; }

    public decimal AnnualizedPremium { get; set; }

    public decimal PastDuePremium { get; set; }
}
