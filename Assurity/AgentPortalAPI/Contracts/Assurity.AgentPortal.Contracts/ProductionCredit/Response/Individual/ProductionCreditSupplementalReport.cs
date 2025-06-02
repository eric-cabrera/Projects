namespace Assurity.AgentPortal.Contracts.ProductionCredit.Response.Individual;

public class ProductionCreditSupplementalReport
{
    /// <summary>
    /// E.g., "Writing Agents".
    /// </summary>
    public string Name { get; set; }

    public List<ProductionCreditSupplementalReportTotal> Totals { get; set; }
}
