namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PPBENPolicyBenefitsTypesBAORMappingProfile : Profile
    {
        public PPBENPolicyBenefitsTypesBAORMappingProfile()
        {
            CreateMap<GenericRecord, PPBEN_POLICY_BENEFITS_TYPES_BA_OR>()
                .ForMember(
                    dest => dest.ANN_PREM_PER_UNIT,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS_TYPES_BA_OR.ANN_PREM_PER_UNIT)]))
                .ForMember(
                    dest => dest.VALUE_PER_UNIT,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS_TYPES_BA_OR.VALUE_PER_UNIT)]))
                .ForMember(
                    dest => dest.NUMBER_OF_UNITS,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS_TYPES_BA_OR.NUMBER_OF_UNITS)]))
                .ForMember(
                    dest => dest.PBEN_ID,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS_TYPES_BA_OR.PBEN_ID)]))
                .ForMember(
                    dest => dest.DIVIDEND,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS_TYPES_BA_OR.DIVIDEND)].ToString().Trim()));
        }
    }
}