namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class SysNBRequirementsMappingProfile : Profile
    {
        public SysNBRequirementsMappingProfile()
        {
            CreateMap<GenericRecord, SysNBRequirements>()
                .ForMember(
                    dest => dest.POLICYNUMBER,
                    opt => opt.MapFrom(
                        src => src[nameof(SysNBRequirements.POLICYNUMBER)] != null ?
                        src[nameof(SysNBRequirements.POLICYNUMBER)].ToString().Trim() : null))
                .ForMember(
                    dest => dest.IX,
                    opt => opt.MapFrom(
                        src => src[nameof(SysNBRequirements.IX)]))
                .ForMember(
                    dest => dest.REQSEQ,
                    opt => opt.MapFrom(src => src[nameof(SysNBRequirements.REQSEQ)]))
                .ForMember(
                    dest => dest.REQTYPE,
                    opt => opt.MapFrom(
                        src => src[nameof(SysNBRequirements.REQTYPE)] != null ?
                        src[nameof(SysNBRequirements.REQTYPE)].ToString().Trim() : null))
                .ForMember(
                    dest => dest.REQNOTE,
                    opt => opt.MapFrom(
                        src => src[nameof(SysNBRequirements.REQNOTE)] != null ?
                        src[nameof(SysNBRequirements.REQNOTE)].ToString().Trim() : null))
                .ReverseMap();
        }
    }
}