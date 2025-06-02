namespace Assurity.AgentPortal.Contracts.ProductionCredit.Request;

public class ProductionCreditParameters : BaseProductionCreditParameters
{
    /// <summary>
    /// Starting date of the report.
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// End date of the report.
    /// </summary>
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Start date of the comparison report.
    /// </summary>
    public DateTime? CompareFromDate { get; set; }

    /// <summary>
    /// End date of the comparison report.
    /// </summary>
    public DateTime? CompareToDate { get; set; }
}
