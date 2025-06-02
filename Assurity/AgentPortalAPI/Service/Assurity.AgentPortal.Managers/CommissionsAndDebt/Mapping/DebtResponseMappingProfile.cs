namespace Assurity.AgentPortal.Managers.CommissionsAndDebt.Mapping;

using System.Text;
using Assurity.AgentPortal.Contracts.CommissionsDebt;
using AutoMapper;
using Debt = Assurity.Commissions.Debt.Contracts.Advances;

public class DebtResponseMappingProfile : Profile
{
    public DebtResponseMappingProfile()
    {
        CreateMap<Debt.AgentDetailsResponse, DebtResponse>()
            .ForMember(
                dest => dest.Agents,
                opt => opt.MapFrom(src => src.Agents))
            .ForMember(
                dest => dest.Filters,
                opt => opt.MapFrom(src => src.FilterValues));

        CreateMap<Debt.FilterValues, DebtFilterValues>()
            .ForMember(
                dest => dest.AgentFilterValues,
                opt => opt.MapFrom(src => src.AgentIdentifiers))
            .ForMember(
                dest => dest.HierarchyFilterValues,
                opt => opt.MapFrom(src => src.Hierarchies))
            .ForMember(
                dest => dest.StatusFilterValues,
                opt => opt.MapFrom(src => src.Statuses))
            .ForMember(
                dest => dest.PolicyNumberFilterValues,
                opt => opt.MapFrom(src => src.PolicyNumbers));

        CreateMap<Debt.Hierarchy, Hierarchy>()
            .ForMember(
                dest => dest.AgentName,
                opt => opt.MapFrom(src => src.Name));

        CreateMap<Debt.Agent, DebtAgent>()
            .ForMember(
                dest => dest.AgentStatus,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(
                dest => dest.AgentName,
                opt => opt.MapFrom(src => GetParticipantName(src.Participant)));

        CreateMap<Debt.Policy, DebtPolicy>()
            .ForMember(
                dest => dest.InsuredName,
                opt => opt.MapFrom(src => GetPrimaryInsuredName(src.Insureds)));
    }

    private static string GetPrimaryInsuredName(List<Debt.Insured> insureds)
    {
        var primaryInsured = insureds.FirstOrDefault(
            insured => insured.RelationshipToPrimaryInsured == Debt.RelationshipToPrimaryInsured.Self);

        if (primaryInsured == null)
        {
            return string.Empty;
        }

        return GetParticipantName(primaryInsured.Participant);
    }

    private static string GetParticipantName(Debt.Participant participant)
    {
        if (participant.Business != null)
        {
            return participant.Business.BusinessName ?? string.Empty;
        }

        if (participant.Person != null)
        {
            var sb = new StringBuilder();

            sb.Append(participant.Person.IndividualLast);

            if (participant.Person.IndividualFirst != null)
            {
                sb.Append(", ");
                sb.Append(participant.Person.IndividualFirst);
            }

            return sb.ToString();
        }

        return string.Empty;
    }
}
