namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class SysACAgentDataMappingProfile : Profile
    {
        public SysACAgentDataMappingProfile()
        {
            CreateMap<GenericRecord, SysACAgentData>()
                .ForMember(
                    dest => dest.AGENTID,
                    opt => opt.MapFrom(
                        src => src[nameof(SysACAgentData.AGENTID)] != null ?
                        src[nameof(SysACAgentData.AGENTID)].ToString().Trim() : null))
                .ForMember(
                    dest => dest.FOLDERID,
                    opt => opt.MapFrom(
                        src => src[nameof(SysACAgentData.FOLDERID)] != null ?
                        src[nameof(SysACAgentData.FOLDERID)].ToString().Trim() : null))
                .ForMember(
                    dest => dest.FIRSTNAME,
                    opt => opt.MapFrom(
                        src => src[nameof(SysACAgentData.FIRSTNAME)] != null ?
                        src[nameof(SysACAgentData.FIRSTNAME)].ToString().Trim() : null))
                .ForMember(
                    dest => dest.MIDDLENAME,
                    opt => opt.MapFrom(
                        src => src[nameof(SysACAgentData.MIDDLENAME)] != null ?
                        src[nameof(SysACAgentData.MIDDLENAME)].ToString().Trim() : null))
                .ForMember(
                    dest => dest.LASTNAME,
                    opt => opt.MapFrom(
                        src => src[nameof(SysACAgentData.LASTNAME)] != null ?
                        src[nameof(SysACAgentData.LASTNAME)].ToString().Trim() : null))
                .ForMember(
                    dest => dest.BUSINESSNAME,
                    opt => opt.MapFrom(
                        src => src[nameof(SysACAgentData.BUSINESSNAME)] != null ?
                        src[nameof(SysACAgentData.BUSINESSNAME)].ToString().Trim() : null));
        }
    }
}
