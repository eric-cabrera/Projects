namespace Assurity.AgentPortal.Contracts.ProductionCredit.Response.PolicyDetail;

using Assurity.AgentPortal.Contracts.ProductionCredit.Response.Shared;

public class ProductionCreditPolicyDetailFilterValues : ProductionCreditFilterValues
{
    public List<string> InsuredNames { get; set; }

    public List<string> PolicyNumbers { get; set; }

    public List<string> TransactionTypes { get; set; }
}
