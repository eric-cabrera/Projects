namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class SysACAgentMarketCodesMappingProfile : Profile
    {
        public SysACAgentMarketCodesMappingProfile()
        {
            CreateMap<GenericRecord, SysACAgentMarketCodes>()
                .ForMember(
                    destination => destination.FOLDERID,
                    opt => opt.MapFrom(
                        source => source["FOLDERID"] != null ?
                        source["FOLDERID"].ToString().Trim() : null))
                .ForMember(
                    destination => destination.MARKETCODE,
                    opt => opt.MapFrom(
                        source => source["MARKETCODE"] != null ?
                        source["MARKETCODE"].ToString().Trim() : null))
                .ForMember(
                    destination => destination.CONTRACTLEVEL,
                    opt => opt.MapFrom(
                        source => source["CONTRACTLEVEL"] != null ?
                        source["CONTRACTLEVEL"].ToString().Trim() : null))
                .ForMember(
                    destination => destination.UPLINEAGENTID,
                    opt => opt.MapFrom(
                        source => source["UPLINEAGENTID"] != null ?
                        source["UPLINEAGENTID"].ToString().Trim() : null))
                .ForMember(
                    destination => destination.UPLINEMARKETCODE,
                    opt => opt.MapFrom(
                        source => source["UPLINEMARKETCODE"] != null ?
                        source["UPLINEMARKETCODE"].ToString().Trim() : null))
                .ForMember(
                    destination => destination.UPLINECONTRACTLEVEL,
                    opt => opt.MapFrom(
                        source => source["UPLINECONTRACTLEVEL"] != null ?
                        source["UPLINECONTRACTLEVEL"].ToString().Trim() : null))
                .ForMember(
                    destination => destination.PENDINGRPTDISABLED,
                    opt => opt.MapFrom(source => source["PENDINGRPTDISABLED"]));
        }
    }
}
