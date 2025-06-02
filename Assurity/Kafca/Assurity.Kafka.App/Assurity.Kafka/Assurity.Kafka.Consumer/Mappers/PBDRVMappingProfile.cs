namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PBDRVMappingProfile : Profile
    {
        public PBDRVMappingProfile()
        {
            CreateMap<GenericRecord, PBDRV>()
                .ForMember(
                    dest => dest.DESCRIPTION,
                    opt => opt.MapFrom(src => src[nameof(PBDRV.DESCRIPTION)].ToString().Trim()))
                .ForMember(
                    dest => dest.STATUS_CODE,
                    opt => opt.MapFrom(src => src[nameof(PBDRV.STATUS_CODE)].ToString().Trim()))
                .ForMember(
                    dest => dest.BATCH_START_DATE,
                    opt => opt.MapFrom(src => src[nameof(PBDRV.BATCH_START_DATE)]))
                .ForMember(
                    dest => dest.BATCH_STOP_DATE,
                    opt => opt.MapFrom(src => src[nameof(PBDRV.BATCH_STOP_DATE)]))
                .ReverseMap();
        }
    }
}