namespace Assurity.AgentPortal.Contracts.ProductionCredit.Response.Shared;

public class ProductionCreditAgent
{
    public string Id { get; set; }

    /// <summary>
    /// Format: Last, First.
    /// </summary>
    public string Name { get; set; }
}
