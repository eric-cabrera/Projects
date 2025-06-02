namespace Assurity.AgentPortal.Managers.PolicyInfo.Mapping
{
    using Assurity.AgentPortal.Contracts.PolicyInfo;
    using Assurity.AgentPortal.Managers.MappingExtensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using AutoMapper;

    public class RequirementMappingProfile : Profile
    {
        public RequirementMappingProfile()
        {
            CreateMap<Requirement, RequirementResponse>()
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(
                         src => MappingExtensions.GetEnumDisplayName(src.Status)))
                .ForMember(
                    dest => dest.FulfillingParty,
                    opt => opt.MapFrom(
                        src => src.FulfillingParty != null
                        ? MappingExtensions.GetEnumDisplayName(src.FulfillingParty)
                        : string.Empty))
                .ForMember(
                    dest => dest.ActionType,
                    opt => opt.MapFrom(
                        src => src.ActionType != null
                        ? MappingExtensions.GetEnumDisplayName(src.ActionType)
                     : string.Empty));
        }
    }
}