namespace Assurity.AgentPortal.Contracts.ProductionCredit.Request;

public class ProductionCreditPolicyDetailsParameters : BaseProductionCreditParameters
{
    /// <summary>
    /// Starting transaction date of the report.
    /// </summary>
    public DateTime? TransactionFromDate { get; set; }

    /// <summary>
    /// End transaction date of the report.
    /// </summary>
    public DateTime? TransactionToDate { get; set; }

    /// <summary>
    /// Insured name to filter policy details data. Semi-colon separated string.
    /// </summary>
    public string? InsuredNames { get; set; }

    /// <summary>
    /// Policy number to filter policy detail data. Semi-colon separated string.
    /// </summary>
    public string? PolicyNumbers { get; set; }

    /// <summary>
    /// Semicolon separated string. Transaction types to filter policy details data. Possible values:
    /// <list type="bullet">
    /// <item><description>ADJUSTMENT</description></item>
    /// <item><description>CANCEL</description></item>
    /// <item><description>DECLINED</description></item>
    /// <item><description>ISSUED</description></item>
    /// <item><description>INCOMPLETE</description></item>
    /// <item><description>INELIGIBLE</description></item>
    /// <item><description>NOT ISSUED</description></item>
    /// <item><description>NOT TAKEN</description></item>
    /// <item><description>POSTPONED</description></item>
    /// <item><description>SUBMITTED</description></item>
    /// <item><description>WITHDRAWN</description></item>
    /// <item><description>UNKNOWN</description></item>
    /// </list>
    /// </summary>
    public string? TransactionTypes { get; set; }
}
