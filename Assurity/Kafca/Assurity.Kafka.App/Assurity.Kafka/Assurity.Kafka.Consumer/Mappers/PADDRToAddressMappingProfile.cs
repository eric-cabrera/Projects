namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Utilities.Extensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using AutoMapper;

    public class PADDRToAddressMappingProfile : Profile
    {
        public PADDRToAddressMappingProfile()
        {
            CreateMap<PADDR, Address>()
                .ForMember(
                    dest => dest.AddressId,
                    opt => opt.MapFrom(src => src.ADDRESS_ID))
                .ForMember(
                    dest => dest.Line1,
                    opt => opt.MapFrom(src => src.ADDR_LINE_1.Trim()))
                .ForMember(
                    dest => dest.Line2,
                    opt => opt.MapFrom(src => src.ADDR_LINE_2.Trim()))
                .ForMember(
                    dest => dest.Line3,
                    opt => opt.MapFrom(src => src.ADDR_LINE_3.Trim()))
                .ForMember(
                    dest => dest.BoxNumber,
                    opt => opt.MapFrom(src => src.BOX_NUMBER.Trim()))
                .ForMember(
                    dest => dest.City,
                    opt => opt.MapFrom(src => src.CITY.Trim()))
                .ForMember(
                    dest => dest.StateAbbreviation,
                    opt => opt.MapFrom(src => src.STATE.Trim().ToState()))
                .ForMember(
                    dest => dest.ZipCode,
                    opt => opt.MapFrom(src => src.ZIP.Trim()))
                .ForMember(
                    dest => dest.ZipExtension,
                    opt => opt.MapFrom(src => src.ZIP_EXTENSION.Trim()))
                .ForMember(
                    dest => dest.Country,
                    opt => opt.MapFrom(src => src.COUNTRY.Trim().ToCountry()))
                .ReverseMap();
        }
    }
}
