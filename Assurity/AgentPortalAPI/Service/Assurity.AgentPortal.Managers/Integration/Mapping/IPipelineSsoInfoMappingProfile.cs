namespace Assurity.AgentPortal.Managers.Integration.Mapping
{
    using Assurity.Agent.Contracts;
    using Assurity.AgentPortal.Contracts.Integration;
    using AutoMapper;

    public class IPipelineSsoInfoMappingProfile : Profile
    {
        public IPipelineSsoInfoMappingProfile()
        {
            CreateMap<AgentInformation, IPipelineSsoInfo>()
                .ForMember(
                    dest => dest.Agentname,
                    opt => opt.MapFrom(src => (src.Name != null && src.Name.IsBusiness) ? src.Name.BusinessName :
                        (src.Name != null ? src.Name.IndividualFirst + " " + src.Name.IndividualLast : null)))
                .ForMember(
                    dest => dest.FirstName,
                    opt => opt.MapFrom(src => (src.Name != null && src.Name.IsBusiness) ? null :
                        (src.Name != null ? src.Name.IndividualFirst : null)))
                .ForMember(
                    dest => dest.LastName,
                    opt => opt.MapFrom(src => (src.Name != null && src.Name.IsBusiness) ? null :
                        (src.Name != null ? src.Name.IndividualLast : null)))
                .ForMember(
                    dest => dest.Address1,
                    opt => opt.MapFrom(src => src.Address != null ? src.Address.Line1 + " " + src.Address.Line2 : null))
                .ForMember(
                    dest => dest.City,
                    opt => opt.MapFrom(src => src.Address != null ? src.Address.City : null))
                .ForMember(
                    dest => dest.State,
                    opt => opt.MapFrom(src => src.Address != null ? src.Address.StateAbbreviation : null))
                .ForMember(
                    dest => dest.ZipCode,
                    opt => opt.MapFrom(src => src.Address != null ? src.Address.Zip : null))
                .ForMember(
                    dest => dest.Phone,
                    opt => opt.MapFrom(src => src.Address != null ? src.Address.PhoneNumber : null))
                 .ForMember(
                    dest => dest.Fax,
                    opt => opt.MapFrom(src => src.Address != null ? src.Address.FaxNumber : null))
                .ForMember(
                    dest => dest.Email,
                    opt => opt.MapFrom(src => src.Address != null ? src.Address.EmailAddress : null))
                .ForMember(
                    dest => dest.AIAgency,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.ShowAgency,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Initial_DisconnectedLogonID,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Initial_DisconnectedPassword,
                    opt => opt.Ignore());
        }
    }
}
