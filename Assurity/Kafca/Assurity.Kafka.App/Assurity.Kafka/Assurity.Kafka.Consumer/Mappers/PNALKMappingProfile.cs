namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PNALKMappingProfile : Profile
    {
        public PNALKMappingProfile()
        {
            CreateMap<GenericRecord, PNALK>()
                .ForMember(
                    dest => dest.ADDRESS_CODE,
                    opt => opt.MapFrom(src => src[nameof(PNALK.ADDRESS_CODE)].ToString().Trim()))
                .ForMember(
                    dest => dest.ADDRESS_ID,
                    opt => opt.MapFrom(src => src[nameof(PNALK.ADDRESS_ID)]))
                .ForMember(
                    dest => dest.CANCEL_DATE,
                    opt => opt.MapFrom(src => src[nameof(PNALK.CANCEL_DATE)]))
                .ForMember(
                    dest => dest.NAME_ID,
                    opt => opt.MapFrom(src => src[nameof(PNALK.NAME_ID)]))
                .ForMember(
                    dest => dest.TELE_NUM,
                    opt => opt.MapFrom(src => src[nameof(PNALK.TELE_NUM)].ToString().Trim()))
                .ReverseMap();
        }
    }
}