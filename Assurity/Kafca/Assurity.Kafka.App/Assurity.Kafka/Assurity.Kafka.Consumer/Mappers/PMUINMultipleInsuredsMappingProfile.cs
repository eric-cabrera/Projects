namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PMUINMultipleInsuredsMappingProfile : Profile
    {
        public PMUINMultipleInsuredsMappingProfile()
        {
            CreateMap<GenericRecord, PMUIN_MULTIPLE_INSUREDS>()
                .ForMember(
                    dest => dest.COMPANY_CODE,
                    opt => opt.MapFrom(src => src[nameof(PMUIN_MULTIPLE_INSUREDS.COMPANY_CODE)].ToString().Trim()))
                .ForMember(
                    dest => dest.BENEFIT_SEQ,
                    opt => opt.MapFrom(src => src[nameof(PMUIN_MULTIPLE_INSUREDS.BENEFIT_SEQ)]))
                .ForMember(
                    dest => dest.KD_BEN_EXTEND_KEYS,
                    opt => opt.MapFrom(src => src[nameof(PMUIN_MULTIPLE_INSUREDS.KD_BEN_EXTEND_KEYS)].ToString().Trim()))
                .ForMember(
                    dest => dest.KD_DEF_SEGT_ID,
                    opt => opt.MapFrom(src => src[nameof(PMUIN_MULTIPLE_INSUREDS.KD_DEF_SEGT_ID)].ToString().Trim()))
                .ForMember(
                    dest => dest.MULT_RELATE,
                    opt => opt.MapFrom(src => src[nameof(PMUIN_MULTIPLE_INSUREDS.MULT_RELATE)].ToString().Trim()))
                .ForMember(
                    dest => dest.NAME_ID,
                    opt => opt.MapFrom(src => src[nameof(PMUIN_MULTIPLE_INSUREDS.NAME_ID)]))
                .ForMember(
                    dest => dest.POLICY_NUMBER,
                    opt => opt.MapFrom(src => src[nameof(PMUIN_MULTIPLE_INSUREDS.POLICY_NUMBER)].ToString().Trim()))
                .ForMember(
                    dest => dest.RELATIONSHIP_CODE,
                    opt => opt.MapFrom(src => src[nameof(PMUIN_MULTIPLE_INSUREDS.RELATIONSHIP_CODE)].ToString().Trim()))
                .ForMember(
                    dest => dest.UWCLS,
                    opt => opt.MapFrom(src => src[nameof(PMUIN_MULTIPLE_INSUREDS.UWCLS)].ToString().Trim()))
                .ForMember(
                    dest => dest.START_DATE,
                    opt => opt.MapFrom(src => src[nameof(PMUIN_MULTIPLE_INSUREDS.START_DATE)]))
                .ForMember(
                    dest => dest.STOP_DATE,
                    opt => opt.MapFrom(src => src[nameof(PMUIN_MULTIPLE_INSUREDS.STOP_DATE)]));
        }
    }
}