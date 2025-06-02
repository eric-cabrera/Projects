namespace Assurity.AgentPortal.Contracts.ProductionCredit.Response.PolicyDetail;

public class ProductionCreditPolicyDetail
{
    public string AgentId { get; set; }

    public string AgentName { get; set; }

    public decimal AnnualPremium { get; set; }

    /// <summary>
    /// Format: Last, First.
    /// </summary>
    public string InsuredName { get; set; }

    public string LineOfBusiness { get; set; }

    /// <summary>
    /// Payment frequency. Possible values:
    /// <list type="bullet">
    /// <item><description>Annual</description></item>
    /// <item><description>Bi-Weekly</description></item>
    /// <item><description>FiftyTwoPay</description></item>
    /// <item><description>Monthly</description></item>
    /// <item><description>Ninthly</description></item>
    /// <item><description>None</description></item>
    /// <item><description>Quarterly</description></item>
    /// <item><description>Semi-Annually</description></item>
    /// <item><description>Tenthly</description></item>
    /// <item><description>Thirteenthly</description></item>
    /// <item><description>TwentySixPay</description></item>
    /// <item><description>Unknown</description></item>
    /// <item><description>Weekly</description></item>
    /// </list>
    /// </summary>
    public string Mode { get; set; }

    public decimal ModePremium { get; set; }

    public decimal PolicyCount { get; set; }

    public string PolicyNumber { get; set; }

    public string ProductType { get; set; }

    public string ProductDescription { get; set; }

    public DateTime ApplicationDate { get; set; }

    public DateTime? TransactionDate { get; set; }

    public string TransactionType { get; set; }

    public decimal Volume { get; set; }
}
