namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PPOLMPolicyBenefitMiscMappingProfile : Profile
    {
        public PPOLMPolicyBenefitMiscMappingProfile()
        {
            CreateMap<GenericRecord, PPOLM_POLICY_BENEFIT_MISC>()
                .ForMember(
                    dest => dest.POLICY_NUMBER,
                    opt => opt.MapFrom(src => src["POLICY_NUMBER"].ToString().Trim()))
                .ForMember(
                    dest => dest.CANCEL_REASON,
                    opt => opt.MapFrom(src => src["CANCEL_REASON"].ToString().Trim()))
                .ForMember(
                    dest => dest.CANCEL_DESC,
                    opt => opt.MapFrom(src => src["CANCEL_DESC"].ToString().Trim()))
                .ForMember(
                    dest => dest.COMPANY_CODE,
                    opt => opt.MapFrom(src => src["COMPANY_CODE"].ToString().Trim()))
                .ForMember(
                    dest => dest.POLM_ID,
                    opt => opt.MapFrom(src => src["POLM_ID"]))
                .ForMember(
                    dest => dest.SEQ,
                    opt => opt.MapFrom(src => src["SEQ"]))
                .ReverseMap();
        }
    }
}