namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PPBENPolicyBenefitsMappingProfile : Profile
    {
        public PPBENPolicyBenefitsMappingProfile()
        {
            CreateMap<GenericRecord, PPBEN_POLICY_BENEFITS>()
                .ForMember(
                    dest => dest.BENEFIT_SEQ,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS.BENEFIT_SEQ)]))
                .ForMember(
                    dest => dest.BENEFIT_TYPE,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS.BENEFIT_TYPE)].ToString().Trim()))
                .ForMember(
                    dest => dest.PBEN_ID,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS.PBEN_ID)]))
                .ForMember(
                    dest => dest.PLAN_CODE,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS.PLAN_CODE)].ToString().Trim()))
                .ForMember(
                    dest => dest.POLICY_NUMBER,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS.POLICY_NUMBER)].ToString().Trim()))
                .ForMember(
                    dest => dest.COMPANY_CODE,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS.COMPANY_CODE)].ToString().Trim()))
                .ForMember(
                    dest => dest.STATUS_CODE,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS.STATUS_CODE)].ToString().Trim()))
                .ForMember(
                    dest => dest.STATUS_REASON,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS.STATUS_REASON)].ToString().Trim()))
                .ForMember(
                    dest => dest.STATUS_DATE,
                    opt => opt.MapFrom(src => src[nameof(PPBEN_POLICY_BENEFITS.STATUS_DATE)]))
                .ForMember(
                    dest => dest.PCOVR_PRODUCT_COVERAGES,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PCEXP_COVERAGE_EXPANSION,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PPBEN_POLICY_BENEFITS_TYPES_BA_OR,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PPBEN_POLICY_BENEFITS_TYPES_BF,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PPBEN_POLICY_BENEFITS_TYPES_SU,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PPBEN_POLICY_BENEFITS_TYPES_SL,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PPBEN_POLICY_BENEFITS_TYPES_SP,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PMUIN_MULTIPLE_INSUREDs,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PCEXP_COVERAGE_EXPANSION,
                    opt => opt.Ignore());
        }
    }
}