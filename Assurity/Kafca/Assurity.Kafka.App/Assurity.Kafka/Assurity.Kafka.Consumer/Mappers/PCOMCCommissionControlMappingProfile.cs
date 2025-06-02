namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PCOMCCommissionControlMappingProfile : Profile
    {
        public PCOMCCommissionControlMappingProfile()
        {
            CreateMap<GenericRecord, PCOMC_COMMISSION_CONTROL>()
                .ForMember(
                    dest => dest.COMC_ID,
                    opt => opt.MapFrom(src => src[nameof(PCOMC_COMMISSION_CONTROL.COMC_ID)]))
                .ForMember(
                    dest => dest.COMPANY_CODE,
                    opt => opt.MapFrom(src => src[nameof(PCOMC_COMMISSION_CONTROL.COMPANY_CODE)].ToString().Trim()))
                .ForMember(
                    dest => dest.POLICY_NUMBER,
                    opt => opt.MapFrom(src => src[nameof(PCOMC_COMMISSION_CONTROL.POLICY_NUMBER)].ToString().Trim()))
                .ForMember(
                    dest => dest.RECORD_TYPE,
                    opt => opt.MapFrom(src => src[nameof(PCOMC_COMMISSION_CONTROL.RECORD_TYPE)].ToString().Trim()))
                .ForMember(
                     dest => dest.ISSUE_DATE,
                     opt => opt.MapFrom(src => src[nameof(PCOMC_COMMISSION_CONTROL.ISSUE_DATE)]))
                .ForMember(
                     dest => dest.PCOMC_COMMISSION_CONTROL_TYPE_S,
                     opt => opt.Ignore());
        }
    }
}