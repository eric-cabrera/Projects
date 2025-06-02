namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PACONAnnuityPolicyMappingProfile : Profile
    {
        public PACONAnnuityPolicyMappingProfile()
        {
            CreateMap<GenericRecord, PACON_ANNUITY_POLICY>()
                .ForMember(
                    dest => dest.COMPANY_CODE,
                    opt => opt.MapFrom(src => src["COMPANY_CODE"].ToString().Trim()))
                .ForMember(
                    dest => dest.POLICY_NUMBER,
                    opt => opt.MapFrom(src => src["POLICY_NUMBER"].ToString().Trim()))
                .ForMember(
                    dest => dest.ISSUE_DATE,
                    opt => opt.MapFrom(src => src["ISSUE_DATE"]))
                .ForMember(
                    dest => dest.STATUS_CODE,
                    opt => opt.MapFrom(src => src["STATUS_CODE"].ToString().Trim()))
                .ForMember(
                    dest => dest.STATUS_REASON,
                    opt => opt.MapFrom(src => src["STATUS_REASON"].ToString().Trim()))
                .ForMember(
                    dest => dest.STATUS_DATE,
                    opt => opt.MapFrom(src => src["STATUS_DATE"]))
                .ForMember(
                    dest => dest.TAX_QUALIFICATION,
                    opt => opt.MapFrom(src => src["TAX_QUALIFICATION"].ToString().Trim()))
                .ReverseMap();
        }
    }
}
