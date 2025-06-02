namespace Assurity.AgentPortal.Managers.LeadersConference;

using Assurity.AgentPortal.Contracts.LeadersConference;

public interface ILeadersConferenceManager
{
    QualificationStatusSummary GetQualificationStatus(string agentId, int year, QualiferType qualiferType, CancellationToken cancellationToken);

    LeadersQualifiersSummary GetLeadersQualifiers(string agentId, int year, CancellationToken cancellationToken);
}