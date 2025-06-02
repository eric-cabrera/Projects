namespace Assurity.AgentPortal.Managers.CommissionsAndDebt.Mapping;

using Assurity.AgentPortal.Contracts.CommissionsDebt.FileExport;
using Assurity.Commissions.Debt.Contracts.Advances;
using AutoMapper;

public class SecuredDebtSummaryMappingProfile : Profile
{
    public SecuredDebtSummaryMappingProfile()
    {
        CreateMap<Agent, List<SecuredDebtExport>>()
            .ConvertUsing(agent => agent.Policies.Select(policy =>
                new SecuredDebtExport
                {
                    AgentId = agent.AgentId,
                    AgentName = agent.Participant.FormattedName,
                    AgentStatus = agent.Status.ToString(),
                    SecuredAdvanceOwed = policy.SecuredAdvanceOwed ?? 0m,
                    PolicyNumber = policy.PolicyNumber,
                    InsuredName = GetPrimaryInsuredName(policy.Insureds),
                    ApplicationDate = policy.ApplicationDate,
                    PaidToDate = policy.PaidToDate,
                })
            .ToList());
    }

    private static string GetPrimaryInsuredName(List<Insured>? insureds)
    {
        var primaryInsured = insureds?.FirstOrDefault(
            insured => insured.RelationshipToPrimaryInsured == RelationshipToPrimaryInsured.Self);

        return primaryInsured?.Participant?.FormattedName ?? string.Empty;
    }
}
