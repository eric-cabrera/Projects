namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PPENDNewBusPendUnderwritingMappingProfile : Profile
    {
        public PPENDNewBusPendUnderwritingMappingProfile()
        {
            CreateMap<GenericRecord, PPEND_NEW_BUS_PEND_UNDERWRITING>()
                .ForMember(
                    dest => dest.PEND_ID,
                    opt => opt.MapFrom(src => src[nameof(PPEND_NEW_BUS_PEND_UNDERWRITING.PEND_ID)]))
                .ForMember(
                    dest => dest.IDX,
                    opt => opt.MapFrom(src => src[nameof(PPEND_NEW_BUS_PEND_UNDERWRITING.IDX)]))
                .ForMember(
                    dest => dest.UND_FLAG,
                    opt => opt.MapFrom(src => src[nameof(PPEND_NEW_BUS_PEND_UNDERWRITING.UND_FLAG)].ToString().Trim()))
                .ForMember(
                    dest => dest.UND_DATE,
                    opt => opt.MapFrom(src => src[nameof(PPEND_NEW_BUS_PEND_UNDERWRITING.UND_DATE)]))
                .ForMember(
                    dest => dest.UND_O_DATE,
                    opt => opt.MapFrom(src => src[nameof(PPEND_NEW_BUS_PEND_UNDERWRITING.UND_O_DATE)]))
                .ForMember(
                    dest => dest.UND_CODE,
                    opt => opt.MapFrom(src => src[nameof(PPEND_NEW_BUS_PEND_UNDERWRITING.UND_CODE)]))
                .ForMember(
                    dest => dest.COMMENTS,
                    opt => opt.MapFrom(src => src[nameof(PPEND_NEW_BUS_PEND_UNDERWRITING.COMMENTS)].ToString().Trim()))
                .ForMember(
                    dest => dest.NOTE_SEQ,
                    opt => opt.MapFrom(src => src[nameof(PPEND_NEW_BUS_PEND_UNDERWRITING.NOTE_SEQ)].ToString().Trim()))

                .ReverseMap();
        }
    }
}