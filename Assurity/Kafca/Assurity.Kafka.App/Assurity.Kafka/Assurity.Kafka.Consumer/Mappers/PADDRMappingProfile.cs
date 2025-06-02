namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PADDRMappingProfile : Profile
    {
        public PADDRMappingProfile()
        {
            CreateMap<GenericRecord, PADDR>()
                .ForMember(
                    dest => dest.ADDRESS_ID,
                    opt => opt.MapFrom(src => src[nameof(PADDR.ADDRESS_ID)]))
                .ForMember(
                    dest => dest.ADDR_LINE_1,
                    opt => opt.MapFrom(src => src[nameof(PADDR.ADDR_LINE_1)].ToString().Trim()))
                .ForMember(
                    dest => dest.ADDR_LINE_2,
                    opt => opt.MapFrom(src => src[nameof(PADDR.ADDR_LINE_2)].ToString().Trim()))
                .ForMember(
                    dest => dest.ADDR_LINE_3,
                    opt => opt.MapFrom(src => src[nameof(PADDR.ADDR_LINE_3)].ToString().Trim()))
                .ForMember(
                    dest => dest.CITY,
                    opt => opt.MapFrom(src => src[nameof(PADDR.CITY)].ToString().Trim()))
                .ForMember(
                    dest => dest.STATE,
                    opt => opt.MapFrom(src => src[nameof(PADDR.STATE)].ToString().Trim()))
                .ForMember(
                    dest => dest.ZIP,
                    opt => opt.MapFrom(src => src[nameof(PADDR.ZIP)].ToString().Trim()))
                .ForMember(
                    dest => dest.ZIP_EXTENSION,
                    opt => opt.MapFrom(src => src[nameof(PADDR.ZIP_EXTENSION)].ToString().Trim()))
                .ForMember(
                    dest => dest.BOX_NUMBER,
                    opt => opt.MapFrom(src => src[nameof(PADDR.BOX_NUMBER)].ToString().Trim()))
                .ForMember(
                    dest => dest.COUNTRY,
                    opt => opt.MapFrom(src => src[nameof(PADDR.COUNTRY)].ToString().Trim()))
                .ReverseMap();
        }
    }
}
