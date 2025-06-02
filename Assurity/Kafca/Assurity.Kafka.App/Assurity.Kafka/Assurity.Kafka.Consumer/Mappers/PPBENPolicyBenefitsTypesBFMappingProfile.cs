namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PPBENPolicyBenefitsTypesBFMappingProfile : Profile
    {
        public PPBENPolicyBenefitsTypesBFMappingProfile()
        {
            CreateMap<GenericRecord, PPBEN_POLICY_BENEFITS_TYPES_BF>()
                .ForMember(
                    dest => dest.ANN_PREM_PER_UNIT,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS_TYPES_BF.ANN_PREM_PER_UNIT)]))
                .ForMember(
                    dest => dest.VALUE_PER_UNIT,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS_TYPES_BF.VALUE_PER_UNIT)]))
                .ForMember(
                    dest => dest.NUMBER_OF_UNITS,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS_TYPES_BF.NUMBER_OF_UNITS)]))
                .ForMember(
                    dest => dest.PBEN_ID,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS_TYPES_BF.PBEN_ID)]))
                .ForMember(
                    dest => dest.BF_DB_OPTION,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS_TYPES_BF.BF_DB_OPTION)].ToString().Trim()))
                .ForMember(
                    dest => dest.BF_DATE_NEGATIVE,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS_TYPES_BF.BF_DATE_NEGATIVE)]))
                .ForMember(
                    dest => dest.BF_CURRENT_DB,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS_TYPES_BF.BF_CURRENT_DB)]));
        }
    }
}