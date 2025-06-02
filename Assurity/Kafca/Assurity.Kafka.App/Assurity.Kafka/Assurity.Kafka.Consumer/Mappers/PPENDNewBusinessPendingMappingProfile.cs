namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PPENDNewBusinessPendingMappingProfile : Profile
    {
        public PPENDNewBusinessPendingMappingProfile()
        {
            CreateMap<GenericRecord, PPEND_NEW_BUSINESS_PENDING>()
                .ForMember(
                    dest => dest.PEND_ID,
                    opt => opt.MapFrom(src => src[nameof(PPEND_NEW_BUSINESS_PENDING.PEND_ID)]))
                .ForMember(
                    dest => dest.POLICY_NUMBER,
                    opt => opt.MapFrom(src => src[nameof(PPEND_NEW_BUSINESS_PENDING.POLICY_NUMBER)].ToString().Trim()))
                .ForMember(
                    dest => dest.COMPANY_CODE,
                    opt => opt.MapFrom(src => src[nameof(PPEND_NEW_BUSINESS_PENDING.COMPANY_CODE)].ToString().Trim()))
                .ForMember(
                    dest => dest.FACE_AMOUNT,
                    opt => opt.MapFrom(src => src[nameof(PPEND_NEW_BUSINESS_PENDING.FACE_AMOUNT)]))
                .ForMember(
                    dest => dest.REQUIREMENT_DATE,
                    opt => opt.MapFrom(src => src[nameof(PPEND_NEW_BUSINESS_PENDING.REQUIREMENT_DATE)]))
                .ForMember(
                    dest => dest.LAST_CHANGE_DATE,
                    opt => opt.MapFrom(src => src[nameof(PPEND_NEW_BUSINESS_PENDING.LAST_CHANGE_DATE)]))
                .ForMember(
                    dest => dest.UND_NAME_ID,
                    opt => opt.MapFrom(src => src[nameof(PPEND_NEW_BUSINESS_PENDING.UND_NAME_ID)]))
                .ForMember(
                    dest => dest.REDEF_TYPE,
                    opt => opt.MapFrom(src => src[nameof(PPEND_NEW_BUSINESS_PENDING.REDEF_TYPE)].ToString().Trim()))
                .ForMember(
                    dest => dest.REQ_SEQUENCE,
                    opt => opt.MapFrom(src => src[nameof(PPEND_NEW_BUSINESS_PENDING.REQ_SEQUENCE)]))
                .ReverseMap();
        }
    }
}