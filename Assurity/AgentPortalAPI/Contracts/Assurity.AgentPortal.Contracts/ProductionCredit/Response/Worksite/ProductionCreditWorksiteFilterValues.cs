namespace Assurity.AgentPortal.Contracts.ProductionCredit.Response.Worksite;

using Assurity.AgentPortal.Contracts.ProductionCredit.Response.Shared;

public class ProductionCreditWorksiteFilterValues : ProductionCreditFilterValues
{
    public List<string> GroupNames { get; set; }

    public List<string> GroupNumbers { get; set; }
}
