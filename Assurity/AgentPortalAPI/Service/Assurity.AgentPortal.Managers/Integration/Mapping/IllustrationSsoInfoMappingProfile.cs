namespace Assurity.AgentPortal.Managers.Integration.Mapping
{
    using Assurity.Agent.Contracts;
    using Assurity.AgentPortal.Contracts.Integration;
    using AutoMapper;

    public class IllustrationSsoInfoMappingProfile : Profile
    {
        public IllustrationSsoInfoMappingProfile()
        {
            CreateMap<AgentInformation, IllustrationSsoInfo>()
                .ForMember(
                    dest => dest.AGENCYNAME,
                    opt => opt.MapFrom(src => src.Name != null && src.Name.IsBusiness ? src.Name.BusinessName : null))
                .ForMember(
                    dest => dest.FIRSTNAME,
                    opt => opt.MapFrom(src => src.Name != null && src.Name.IsBusiness ? null : src.Name.IndividualFirst))
                .ForMember(
                    dest => dest.LASTNAME,
                    opt => opt.MapFrom(src => src.Name != null && src.Name.IsBusiness ? null : src.Name.IndividualLast))
                .ForMember(
                    dest => dest.ADDRESS1,
                    opt => opt.MapFrom(src => src.Address != null ? src.Address.Line1 : null))
                .ForMember(
                    dest => dest.ADDRESS2,
                    opt => opt.MapFrom(src => src.Address != null ? src.Address.Line2 : null))
                .ForMember(
                    dest => dest.CITY,
                    opt => opt.MapFrom(src => src.Address != null ? src.Address.City : null))
                .ForMember(
                    dest => dest.STATE,
                    opt => opt.MapFrom(src => src.Address != null ? src.Address.StateAbbreviation : null))
                .ForMember(
                    dest => dest.ZIP,
                    opt => opt.MapFrom(src => src.Address != null ? src.Address.Zip : null))
                .ForMember(
                    dest => dest.PHONE,
                    opt => opt.MapFrom(src => src.Address != null ? src.Address.PhoneNumber : null))
                .ForMember(
                    dest => dest.EMAIL,
                    opt => opt.MapFrom(src => src.Address != null ? src.Address.EmailAddress : null))
                .ForMember(
                    dest => dest.AGENCYID,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.ISBANK,
                    opt => opt.Ignore());
        }
    }
}
