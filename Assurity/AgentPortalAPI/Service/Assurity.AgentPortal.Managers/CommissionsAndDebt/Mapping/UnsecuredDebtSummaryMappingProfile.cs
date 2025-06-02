namespace Assurity.AgentPortal.Managers.CommissionsAndDebt.Mapping;

using Assurity.AgentPortal.Contracts.CommissionsDebt.FileExport;
using Assurity.AgentPortal.Managers.MappingExtensions;
using Assurity.Commissions.Debt.Contracts.Advances;
using AutoMapper;

public class UnsecuredDebtSummaryMappingProfile : Profile
{
    public UnsecuredDebtSummaryMappingProfile()
    {
        CreateMap<Agent, List<UnsecuredDebtExport>>()
            .ConvertUsing(writingAgent => writingAgent.Policies.Select(policy =>
                new UnsecuredDebtExport
                {
                    AgentId = writingAgent.AgentId,
                    AgentName = writingAgent.Participant.GetParticipantName(),
                    AgentStatus = writingAgent.Status.ToString(),
                    UnsecuredAdvanceOwed = policy.UnsecuredAdvanceOwed ?? 0m,
                    PolicyNumber = policy.PolicyNumber,
                    InsuredName = GetPrimaryInsuredName(policy.Insureds),
                    ApplicationDate = policy.ApplicationDate,
                    PaidToDate = policy.PaidToDate,
                }).ToList());
    }

    private static string GetPrimaryInsuredName(List<Insured>? insureds)
    {
        var primaryInsured = insureds?.FirstOrDefault(
            insured => insured.RelationshipToPrimaryInsured == RelationshipToPrimaryInsured.Self);

        return primaryInsured?.Participant?.GetParticipantName() ?? string.Empty;
    }
}
