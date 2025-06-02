namespace Assurity.AgentPortal.Managers.CommissionsAndDebt.Mapping;

using Assurity.AgentPortal.Contracts.CommissionsDebt;
using AutoMapper;
using Commissions = Assurity.Commissions.Internal.Contracts.WritingAgent;

public class WritingAgentDetailsMappingProfile : Profile
{
    public WritingAgentDetailsMappingProfile()
    {
        CreateMap<Commissions.WritingAgentsResponse, WritingAgentDetailsResponse>()
            .ForMember(
                dest => dest.Filters,
                opt => opt.MapFrom(src => src.FilterValues))
            .ForMember(
                dest => dest.CommissionCycle,
                opt => opt.Ignore());

        CreateMap<Commissions.WritingAgentDetail, WritingAgentDetail>()
            .ForMember(
                dest => dest.AgentId,
                opt => opt.MapFrom(source => source.WritingAgentNumber))
            .ForMember(
                dest => dest.AgentName,
                opt => opt.MapFrom(source => source.WritingAgentName));

        CreateMap<Commissions.WritingAgentFilterValues, WritingAgentFilters>()
            .ForMember(
                dest => dest.CycleDateFilterValues,
                opt => opt.MapFrom(src => src.CycleDateFilterValues))
            .ForMember(
                dest => dest.WritingAgentFilterValues,
                opt => opt.MapFrom(src => src.WritingAgentsFilterValues))
            .ForMember(
                dest => dest.ViewAsAgentIds,
                opt => opt.MapFrom(src => src.AgentFilterValues));
    }
}
