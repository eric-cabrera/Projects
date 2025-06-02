namespace Assurity.AgentPortal.Contracts.ProductionCredit.Response.Individual;

public class ProductionCreditSupplementalReportTotal
{
    /// <summary>
    /// E.g., "Term Life".
    /// </summary>
    public string Name { get; set; }

    public decimal PolicyCount { get; set; }

    public decimal Premium { get; set; }
}
