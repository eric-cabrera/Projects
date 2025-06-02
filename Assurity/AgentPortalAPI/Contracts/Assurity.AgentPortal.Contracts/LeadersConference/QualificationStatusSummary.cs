namespace Assurity.AgentPortal.Contracts.LeadersConference;

public class QualificationStatusSummary(DateTime asOfDate, decimal totalCreditToDate, decimal annualRequirement)
{
    public string AsOfDate { get; set; } = asOfDate.ToString("MM/dd/yyyy");

    public string TotalCreditToDate { get; set; } = $"${string.Format("{0:N0}", totalCreditToDate)}";

    public string AnnualRequirement { get; set; } = $"${string.Format("{0:N0}", annualRequirement)}";

    public int QualificationMeterPercentage { get; set; } = (int)Math.Round(totalCreditToDate / annualRequirement * 100, 0);
}
