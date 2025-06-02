namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class SysZ9ProcessMappingProfile : Profile
    {
        public SysZ9ProcessMappingProfile()
        {
            CreateMap<GenericRecord, SysZ9Process>()
                .ForMember(
                    destination => destination.RECORDID,
                    opt => opt.Ignore())
                .ForMember(
                    destination => destination.AGENTID,
                    opt => opt.MapFrom(
                        src => src[nameof(SysZ9Process.AGENTID)] != null ?
                        src[nameof(SysZ9Process.AGENTID)].ToString().Trim() : null))
                .ForMember(
                    destination => destination.AGENTMARKETCODE,
                    opt => opt.MapFrom(
                        src => src[nameof(SysZ9Process.AGENTMARKETCODE)] != null ?
                        src[nameof(SysZ9Process.AGENTMARKETCODE)].ToString().Trim() : null))
                .ForMember(
                    destination => destination.AGENTLEVEL,
                    opt => opt.MapFrom(
                        src => src[nameof(SysZ9Process.AGENTLEVEL)] != null ?
                        src[nameof(SysZ9Process.AGENTLEVEL)].ToString().Trim() : null))
                .ForMember(
                    destination => destination.NBFOLDEROBJID,
                    opt => opt.MapFrom(
                        src => src[nameof(SysZ9Process.NBFOLDEROBJID)] != null ?
                        src[nameof(SysZ9Process.NBFOLDEROBJID)].ToString().Trim() : null));
        }
    }
}
