namespace Assurity.AgentPortal.Accessors.LeadersConference;

using Assurity.AgentPortal.Contracts.LeadersConference;

public interface ILeadersConferenceAccessor
{
    QualificationStatusSummary GetQualificationStatus(string agentId, int year, QualiferType qualiferType, CancellationToken cancellationToken);

    LeadersQualifiersSummary GetLeadersQualifiers(string agentId, int year, CancellationToken cancellationToken);
}