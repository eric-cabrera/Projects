namespace Assurity.AgentPortal.Managers.CommissionsAndDebt.Mapping;

using Assurity.AgentPortal.Contracts.CommissionsDebt;
using AutoMapper;
using Commissions = Assurity.Commissions.Internal.Contracts.PolicyDetails;

public class CommissionDetailsMappingProfile : Profile
{
    public CommissionDetailsMappingProfile()
    {
        CreateMap<Commissions.PolicyDetailsResponse, CommissionDetails>();

        CreateMap<Commissions.PolicyDetail, PolicyDetail>()
             .ForMember(
                dest => dest.CommissionType,
                opt => opt.MapFrom(src => src.CommissionType.ToString()))
            .ForMember(
                dest => dest.LineOfBusiness,
                opt => opt.MapFrom(src => src.LineOfBusiness.ToString()))
            .ForMember(
                dest => dest.Mode,
                opt => opt.MapFrom(src => src.Mode.ToString()))
            .ForMember(
                dest => dest.ChargebackReason,
                opt => opt.MapFrom(src => src.Description));

        CreateMap<Commissions.PolicyDetailFilterValues, PolicyDetailFilterValues>()
            .ForMember(
                dest => dest.CycleDateFilterValues,
                opt => opt.MapFrom(src => src.CycleDateFilterValues))
            .ForMember(
                dest => dest.PolicyNumberFilterValues,
                opt => opt.MapFrom(src => src.PolicyNumberFilterValues.ToList()))
            .ForMember(
                dest => dest.WritingAgentFilterValues,
                opt => opt.MapFrom(src => src.WritingAgentsFilterValues.ToList()))
            .ForMember(
                dest => dest.ViewAsAgentIds,
                opt => opt.MapFrom(src => src.AgentFilterValues));
    }
}
