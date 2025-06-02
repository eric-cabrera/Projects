namespace Assurity.AgentPortal.Contracts.ProductionCredit.Response.Worksite;

public class ProductionByAgentSupplementalReport
{
    public string Name { get; set; }

    public List<GroupAndPremiumTotals> Totals { get; set; }
}
