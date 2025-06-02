namespace Assurity.AgentPortal.Managers.PolicyInfo.Mapping
{
    using Assurity.AgentPortal.Contracts.PolicyInfo;
    using Assurity.AgentPortal.Managers.MappingExtensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using AutoMapper;

    public class BenefitMappingProfile : Profile
    {
        public BenefitMappingProfile()
        {
            CreateMap<Benefit, BenefitResponse>()
                .ForMember(
                    dest => dest.BenefitStatusReason,
                    opt => opt.MapFrom(
                        src => src.BenefitStatusReason != null
                        ? MappingExtensions.GetEnumDisplayName(src.BenefitStatusReason)
                        : string.Empty))
                .ForMember(
                    dest => dest.BenefitStatus,
                    opt => opt.MapFrom(src => MappingExtensions.GetEnumDisplayName(src.BenefitStatus)))
                .ForMember(
                    dest => dest.DeathBenefitOption,
                    opt => opt.MapFrom(
                        src => src.DeathBenefitOption != null
                        ? MappingExtensions.GetEnumDisplayName(src.DeathBenefitOption)
                        : string.Empty))
                .ForMember(
                    dest => dest.DividendOption,
                    opt => opt.MapFrom(
                        src => src.DividendOption != null
                        ? MappingExtensions.GetEnumDisplayName(src.DividendOption)
                        : string.Empty))
                .ForMember(
                    dest => dest.CoverageType,
                    opt => opt.MapFrom(src => MappingExtensions.GetEnumDisplayName(src.CoverageType)));
        }
    }
}