namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PPBENPolicyBenefitsTypesSLMappingProfile : Profile
    {
        public PPBENPolicyBenefitsTypesSLMappingProfile()
        {
            CreateMap<GenericRecord, PPBEN_POLICY_BENEFITS_TYPES_SL>()
                .ForMember(
                    dest => dest.ANN_PREM_PER_UNIT,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS_TYPES_SL.ANN_PREM_PER_UNIT)]))
                .ForMember(
                    dest => dest.VALUE_PER_UNIT,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS_TYPES_SL.VALUE_PER_UNIT)]))
                .ForMember(
                    dest => dest.NUMBER_OF_UNITS,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS_TYPES_SL.NUMBER_OF_UNITS)]))
                .ForMember(
                    dest => dest.PBEN_ID,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS_TYPES_SL.PBEN_ID)]));
        }
    }
}