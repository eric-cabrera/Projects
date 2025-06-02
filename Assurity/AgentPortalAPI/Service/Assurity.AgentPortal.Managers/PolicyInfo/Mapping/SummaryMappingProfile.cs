namespace Assurity.AgentPortal.Managers.PolicyInfo.Mapping;

using Assurity.AgentPortal.Contracts.PolicyInfo;
using Assurity.AgentPortal.Managers.MappingExtensions;
using Assurity.PolicyInfo.Contracts.V1;
using AutoMapper;
using PolicyInfoAPI = Assurity.PolicyInformation.Contracts.V1;
using V1PolicyInfo = Assurity.PolicyInfo.Contracts.V1;

public class SummaryMappingProfile : Profile
{
    public SummaryMappingProfile()
    {
        CreateMap<PolicyInfoAPI.PolicySummariesResponse, PolicySummariesResponse>();
        CreateMap<PolicyInfoAPI.FilterValues, FilterValues>();

        CreateMap<PolicyInfoAPI.RequirementSummaryResponse, RequirementSummariesResponse>();

        CreateMap<PolicyInfoAPI.PolicySummary, PolicySummary>()
            .ForMember(
                dest => dest.AssignedAgents,
                opt => opt.MapFrom(src => src.AssignedAgents))
            .ForMember(
                dest => dest.PrimaryInsuredName,
                opt => opt.MapFrom(
                    src => src.PrimaryInsured != null
                    ? GetParticipantName(src.PrimaryInsured.Participant)
                    : string.Empty))
            .ForMember(
                dest => dest.BillingMode,
                opt => opt.MapFrom(
                    src => src.BillingMode != null
                    ? MappingExtensions.GetEnumDisplayName(src.BillingMode)
                    : string.Empty))
            .ForMember(
                dest => dest.PolicyStatus,
                opt => opt.MapFrom(
                    src => MappingExtensions.GetEnumDisplayName(src.PolicyStatus)))
            .ForMember(
                dest => dest.PolicyStatusReason,
                opt => opt.MapFrom(
                    src => src.PolicyStatusReason != null
                    ? MappingExtensions.GetEnumDisplayName(src.PolicyStatusReason)
                    : string.Empty))
            .ForMember(
                dest => dest.FirstPaymentFail,
                opt => opt.MapFrom(
                    src => src.ReturnPaymentType == V1PolicyInfo.Enums.ReturnPaymentType.InitialPaymentCardDeclined
                    || src.ReturnPaymentType == V1PolicyInfo.Enums.ReturnPaymentType.InitialPaymentCheckDraftDeclined));

        CreateMap<Agent, AssignedAgent>()
            .ForMember(
                dest => dest.AssignedAgentId,
                opt => opt.MapFrom(src => GetAgentId(src)))
            .ForMember(
                dest => dest.AssignedAgentName,
                opt => opt.MapFrom(
                    src => src.Participant != null
                    ? GetParticipantName(src.Participant)
                    : string.Empty))
            .ForMember(
                dest => dest.IsServicingAgent,
                opt => opt.MapFrom(
                    src => src.IsJustInTimeAgent || src.IsServicingAgent));

        CreateMap<PolicyInfoAPI.RequirementSummary, RequirementSummary>()
            .ForMember(
                dest => dest.AssignedAgents,
                opt => opt.MapFrom(src => src.AssignedAgents))
            .ForMember(
                dest => dest.PrimaryInsuredName,
                opt => opt.MapFrom(
                    src => src.PrimaryInsured != null
                    ? GetParticipantName(src.PrimaryInsured.Participant)
                    : string.Empty));
    }

    private static string GetParticipantName(Participant? participant)
    {
        if (participant?.IsBusiness ?? false)
        {
            return participant.Business?.Name?.BusinessName ?? string.Empty;
        }

        var firstName = participant?.Person?.Name?.IndividualFirst;
        var lastName = participant?.Person?.Name?.IndividualLast;

        if (string.IsNullOrWhiteSpace(firstName))
        {
            return string.IsNullOrWhiteSpace(lastName) ? string.Empty : lastName.Trim();
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            return string.IsNullOrWhiteSpace(firstName) ? string.Empty : firstName.Trim();
        }

        return $"{lastName.Trim()}, {firstName.Trim()}";
    }

    private static string GetAgentId(Agent? agent)
    {
        return agent?.AgentId ?? string.Empty;
    }
}
