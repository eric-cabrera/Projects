namespace Assurity.AgentPortal.Managers.MappingExtensions;

using System.Text;
using Assurity.Commissions.Debt.Contracts.Advances;

public static class ParticipantExtensions
{
    public static string GetParticipantName(this Participant participant)
    {
        if (participant.Business?.BusinessName != null)
        {
            return participant.Business.BusinessName;
        }

        var firstName = participant.Person?.IndividualFirst;

        var stringBuilder = new StringBuilder(participant.Person?.IndividualLast);

        if (!string.IsNullOrWhiteSpace(firstName))
        {
            stringBuilder.Append($", {firstName}");
        }

        return stringBuilder.ToString();
    }
}
