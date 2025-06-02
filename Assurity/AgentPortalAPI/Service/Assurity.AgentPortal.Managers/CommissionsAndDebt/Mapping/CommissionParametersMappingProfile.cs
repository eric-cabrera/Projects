namespace Assurity.AgentPortal.Managers.CommissionsAndDebt.Mapping;

using Assurity.AgentPortal.Accessors.DTOs;
using Assurity.AgentPortal.Contracts.CommissionsDebt.Request;
using AutoMapper;

public class CommissionParametersMappingProfile : Profile
{
    public CommissionParametersMappingProfile()
    {
        CreateMap<WritingAgentParameters, Accessors.DTOs.CommissionParameters>()
            .ForMember<string>(
                dest => dest.OrderBy,
                opt => opt.MapFrom<string>(src => src.OrderBy != null ? src.OrderBy.Value.ToString("G") : null))
            .ForMember<string>(
                dest => dest.AgentId,
                opt => opt.MapFrom<string>(src => src.ViewAsAgentId))
            .ForMember<string>(
                dest => dest.PolicyNumber,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.DisablePagination,
                opt => opt.Ignore());

        CreateMap<PolicyDetailsParameters, Accessors.DTOs.CommissionParameters>()
            .ForMember<string>(
                dest => dest.OrderBy,
                opt => opt.MapFrom<string>(src => src.OrderBy != null ? src.OrderBy.Value.ToString("G") : null))
            .ForMember<string>(
                dest => dest.AgentId,
                opt => opt.MapFrom<string>(src => src.ViewAsAgentId))
            .ForMember(
                dest => dest.DisablePagination,
                opt => opt.Ignore());
    }
}
