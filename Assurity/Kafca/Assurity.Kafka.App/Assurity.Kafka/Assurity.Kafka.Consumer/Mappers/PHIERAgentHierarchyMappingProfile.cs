namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PHIERAgentHierarchyMappingProfile : Profile
    {
        public PHIERAgentHierarchyMappingProfile()
        {
            CreateMap<GenericRecord, PHIER_AGENT_HIERARCHY>()
                .ForMember(
                    dest => dest.COMPANY_CODE,
                    opt => opt.MapFrom(src => src[nameof(PHIER_AGENT_HIERARCHY.COMPANY_CODE)].ToString().Trim()))
                .ForMember(
                    dest => dest.AGENT_NUM,
                    opt => opt.MapFrom(src => src[nameof(PHIER_AGENT_HIERARCHY.AGENT_NUM)].ToString().Trim()))
                .ForMember(
                    dest => dest.MARKET_CODE,
                    opt => opt.MapFrom(src => src[nameof(PHIER_AGENT_HIERARCHY.MARKET_CODE)].ToString().Trim()))
                .ForMember(
                    dest => dest.AGENT_LEVEL,
                    opt => opt.MapFrom(src => src[nameof(PHIER_AGENT_HIERARCHY.AGENT_LEVEL)].ToString().Trim()))
                .ForMember(
                    dest => dest.STOP_DATE,
                    opt => opt.MapFrom(src => src[nameof(PHIER_AGENT_HIERARCHY.STOP_DATE)]))
                .ForMember(
                    dest => dest.START_DATE,
                    opt => opt.MapFrom(src => src[nameof(PHIER_AGENT_HIERARCHY.START_DATE)]))
                .ForMember(
                    dest => dest.HIERARCHY_AGENT,
                    opt => opt.MapFrom(src => src[nameof(PHIER_AGENT_HIERARCHY.HIERARCHY_AGENT)].ToString().Trim()))
                .ForMember(
                    dest => dest.HIER_MARKET_CODE,
                    opt => opt.MapFrom(src => src[nameof(PHIER_AGENT_HIERARCHY.HIER_MARKET_CODE)].ToString().Trim()))
                .ForMember(
                    dest => dest.HIER_AGENT_LEVEL,
                    opt => opt.MapFrom(src => src[nameof(PHIER_AGENT_HIERARCHY.HIER_AGENT_LEVEL)].ToString().Trim()));
        }
    }
}