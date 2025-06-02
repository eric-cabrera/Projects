namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PPBENPolicyBenefitsTypesSUMappingProfile : Profile
    {
        public PPBENPolicyBenefitsTypesSUMappingProfile()
        {
            CreateMap<GenericRecord, PPBEN_POLICY_BENEFITS_TYPES_SU>()
                .ForMember(
                    dest => dest.ANN_PREM_PER_UNIT,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS_TYPES_SU.ANN_PREM_PER_UNIT)]))
                .ForMember(
                    dest => dest.VALUE_PER_UNIT,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS_TYPES_SU.VALUE_PER_UNIT)]))
                .ForMember(
                    dest => dest.NUMBER_OF_UNITS,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS_TYPES_SU.NUMBER_OF_UNITS)]))
                .ForMember(
                    dest => dest.PBEN_ID,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS_TYPES_SU.PBEN_ID)]));
        }
    }
}