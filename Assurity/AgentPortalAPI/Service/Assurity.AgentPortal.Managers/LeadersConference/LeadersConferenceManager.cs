namespace Assurity.AgentPortal.Managers.LeadersConference;

using Assurity.AgentPortal.Accessors.LeadersConference;
using Assurity.AgentPortal.Contracts.LeadersConference;

public class LeadersConferenceManager(ILeadersConferenceAccessor leadersConferenceAccessor) : ILeadersConferenceManager
{
    private ILeadersConferenceAccessor LeadersConferenceAccessor { get; } = leadersConferenceAccessor;

    public QualificationStatusSummary GetQualificationStatus(string agentId, int year, QualiferType qualiferType, CancellationToken cancellationToken)
    {
        return LeadersConferenceAccessor.GetQualificationStatus(agentId, year, qualiferType, cancellationToken);
    }

    public LeadersQualifiersSummary GetLeadersQualifiers(string agentId, int year, CancellationToken cancellationToken)
    {
        return LeadersConferenceAccessor.GetLeadersQualifiers(agentId, year, cancellationToken);
    }
}
