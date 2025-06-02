namespace Assurity.AgentPortal.Managers.PolicyInfo.Mapping
{
    using Assurity.AgentPortal.Contracts.PolicyInfo;
    using Assurity.AgentPortal.Managers.MappingExtensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using AutoMapper;

    public class BenefitOptionMappingProfile : Profile
    {
        public BenefitOptionMappingProfile()
        {
            CreateMap<BenefitOption, BenefitOptionResponse>()
                .ForMember(
                    dest => dest.BenefitOptionName,
                    opt => opt.MapFrom(src => MappingExtensions.GetEnumDisplayName(src.BenefitOptionName)))
                .ForMember(
                    dest => dest.BenefitOptionValue,
                    opt => opt.MapFrom(src => MappingExtensions.GetEnumDisplayName(src.BenefitOptionValue)))
                .ForMember(
                    dest => dest.RelationshipToPrimaryInsured,
                    opt => opt.MapFrom(src => MappingExtensions.GetEnumDisplayName(src.RelationshipToPrimaryInsured)));
        }
    }
}