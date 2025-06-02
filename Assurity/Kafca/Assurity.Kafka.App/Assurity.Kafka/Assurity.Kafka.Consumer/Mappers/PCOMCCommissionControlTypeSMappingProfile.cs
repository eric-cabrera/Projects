namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PCOMCCommissionControlTypeSMappingProfile : Profile
    {
        public PCOMCCommissionControlTypeSMappingProfile()
        {
            CreateMap<GenericRecord, PCOMC_COMMISSION_CONTROL_TYPE_S>()
                .ForMember(
                    dest => dest.AGENT,
                    opt => opt.MapFrom(src => src[nameof(PCOMC_COMMISSION_CONTROL_TYPE_S.AGENT)].ToString().Trim()))
                .ForMember(
                    dest => dest.AGENT_LEVEL,
                    opt => opt.MapFrom(src => src[nameof(PCOMC_COMMISSION_CONTROL_TYPE_S.AGENT_LEVEL)].ToString().Trim()))
                .ForMember(
                    dest => dest.COMC_ID,
                    opt => opt.MapFrom(src => src[nameof(PCOMC_COMMISSION_CONTROL_TYPE_S.COMC_ID)]))
                .ForMember(
                    dest => dest.MARKET_CODE,
                    opt => opt.MapFrom(src => src[nameof(PCOMC_COMMISSION_CONTROL_TYPE_S.MARKET_CODE)].ToString().Trim()))
                .ForMember(
                    dest => dest.SERVICE_AGENT_IND,
                    opt => opt.MapFrom(src => src[nameof(PCOMC_COMMISSION_CONTROL_TYPE_S.SERVICE_AGENT_IND)].ToString().Trim()))
                .ForMember(
                    dest => dest.COMM_PCNT,
                    opt => opt.MapFrom(src => src[nameof(PCOMC_COMMISSION_CONTROL_TYPE_S.COMM_PCNT)]))
                .ForMember(
                    dest => dest.I,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PROD_PCNT,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PAGNT_AGENT_MASTERs,
                    opt => opt.Ignore());
        }
    }
}