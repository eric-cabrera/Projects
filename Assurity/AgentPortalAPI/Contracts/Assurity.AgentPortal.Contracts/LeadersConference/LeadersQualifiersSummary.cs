namespace Assurity.AgentPortal.Contracts.LeadersConference;

public class LeadersQualifiersSummary(DateTime asOfDate, List<string> hierarchy, List<string> personal, List<string> presidentsCircle)
{
    public string AsOfDate { get; set; } = asOfDate.ToString("MM/dd/yyyy");

    public List<string> Hierarchy { get; set; } = hierarchy;

    public List<string> Personal { get; set; } = personal;

    public List<string> PresidentsCircle { get; set; } = presidentsCircle;
}
