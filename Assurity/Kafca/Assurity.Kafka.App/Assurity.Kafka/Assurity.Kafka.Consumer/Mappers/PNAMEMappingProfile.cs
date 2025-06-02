namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PNAMEMappingProfile : Profile
    {
        public PNAMEMappingProfile()
        {
            CreateMap<GenericRecord, PNAME>()
                .ForMember(
                    dest => dest.BUSINESS_EMAIL_ADR,
                    opt => opt.MapFrom(src => src[nameof(PNAME.BUSINESS_EMAIL_ADR)].ToString().Trim()))
                .ForMember(
                    dest => dest.DATE_OF_BIRTH,
                    opt => opt.MapFrom(src => src[nameof(PNAME.DATE_OF_BIRTH)]))
                .ForMember(
                    dest => dest.INDIVIDUAL_PREFIX,
                    opt => opt.MapFrom(src => src[nameof(PNAME.INDIVIDUAL_PREFIX)].ToString().Trim()))
                .ForMember(
                    dest => dest.INDIVIDUAL_FIRST,
                    opt => opt.MapFrom(src => src[nameof(PNAME.INDIVIDUAL_FIRST)].ToString().Trim()))
                .ForMember(
                    dest => dest.INDIVIDUAL_LAST,
                    opt => opt.MapFrom(src => src[nameof(PNAME.INDIVIDUAL_LAST)].ToString().Trim()))
                .ForMember(
                    dest => dest.INDIVIDUAL_MIDDLE,
                    opt => opt.MapFrom(src => src[nameof(PNAME.INDIVIDUAL_MIDDLE)].ToString().Trim()))
                .ForMember(
                    dest => dest.INDIVIDUAL_SUFFIX,
                    opt => opt.MapFrom(src => src[nameof(PNAME.INDIVIDUAL_SUFFIX)].ToString().Trim()))
                .ForMember(
                    dest => dest.NAME_BUSINESS,
                    opt => opt.MapFrom(src => src[nameof(PNAME.NAME_BUSINESS)].ToString().Trim()))
                 .ForMember(
                    dest => dest.NAME_FORMAT_CODE,
                    opt => opt.MapFrom(src => src[nameof(PNAME.NAME_FORMAT_CODE)].ToString().Trim()))
                .ForMember(
                    dest => dest.NAME_ID,
                    opt => opt.MapFrom(src => Convert.ToInt32(src[nameof(PNAME.NAME_ID)])))
                .ForMember(
                    dest => dest.PERSONAL_EMAIL_ADR,
                    opt => opt.MapFrom(src => src[nameof(PNAME.PERSONAL_EMAIL_ADR)].ToString().Trim()))
                .ForMember(
                    dest => dest.SEX_CODE,
                    opt => opt.MapFrom(src => src[nameof(PNAME.SEX_CODE)].ToString().Trim()))
                .ForMember(
                    dest => dest.PNALKs,
                    opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
