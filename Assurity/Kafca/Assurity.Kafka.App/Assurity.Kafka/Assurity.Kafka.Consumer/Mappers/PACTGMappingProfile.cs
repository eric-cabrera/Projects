namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PACTGMappingProfile : Profile
    {
        public PACTGMappingProfile()
        {
            CreateMap<GenericRecord, PACTG>()
                .ForMember(
                    dest => dest.COMPANY_CODE,
                    opt => opt.MapFrom(src => src[nameof(PACTG.COMPANY_CODE)]))
                .ForMember(
                    dest => dest.POLICY_NUMBER,
                    opt => opt.MapFrom(src => src[nameof(PACTG.POLICY_NUMBER)].ToString().Trim()))
                .ForMember(
                    dest => dest.BENEFIT_SEQ,
                    opt => opt.MapFrom(src => src[nameof(PACTG.BENEFIT_SEQ)].ToString().Trim()))
                .ForMember(
                    dest => dest.DATE_ADDED,
                    opt => opt.MapFrom(src => src[nameof(PACTG.DATE_ADDED)].ToString().Trim()))
                .ForMember(
                    dest => dest.TIME_ADDED,
                    opt => opt.MapFrom(src => src[nameof(PACTG.TIME_ADDED)].ToString().Trim()))
                .ForMember(
                    dest => dest.RECORD_SEQUENCE,
                    opt => opt.MapFrom(src => src[nameof(PACTG.RECORD_SEQUENCE)].ToString().Trim()))
                .ForMember(
                    dest => dest.LIFEPRO_ID,
                    opt => opt.MapFrom(src => src[nameof(PACTG.LIFEPRO_ID)].ToString().Trim()))
                .ForMember(
                    dest => dest.EFFECTIVE_DATE,
                    opt => opt.MapFrom(src => src[nameof(PACTG.EFFECTIVE_DATE)].ToString().Trim()))
                .ForMember(
                    dest => dest.DEBIT_CODE,
                    opt => opt.MapFrom(src => src[nameof(PACTG.DEBIT_CODE)].ToString().Trim()))
                .ForMember(
                    dest => dest.CREDIT_CODE,
                    opt => opt.MapFrom(src => src[nameof(PACTG.CREDIT_CODE)].ToString().Trim()))
                .ForMember(
                    dest => dest.REVERSAL_CODE,
                    opt => opt.MapFrom(src => src[nameof(PACTG.REVERSAL_CODE)].ToString().Trim()))
                .ReverseMap();
        }
    }
}
