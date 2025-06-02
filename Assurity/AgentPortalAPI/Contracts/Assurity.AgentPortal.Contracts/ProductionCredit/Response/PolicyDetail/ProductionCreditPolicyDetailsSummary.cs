namespace Assurity.AgentPortal.Contracts.ProductionCredit.Response.PolicyDetail;

public class ProductionCreditPolicyDetailsSummary
{
    public List<ProductionCreditPolicyDetail> PolicyDetails { get; set; }

    public decimal TotalPremium { get; set; }

    public decimal TotalPolicyCount { get; set; }

    public long TotalRecordCount { get; set; }

    public ProductionCreditPolicyDetailFilterValues Filters { get; set; }
}
