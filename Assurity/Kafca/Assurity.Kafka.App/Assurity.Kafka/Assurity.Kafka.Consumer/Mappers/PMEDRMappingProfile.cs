namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PMEDRMappingProfile : Profile
    {
        public PMEDRMappingProfile()
        {
            CreateMap<GenericRecord, PMEDR>()
                .ForMember(
                    dest => dest.RECORD_TYPE,
                    opt => opt.MapFrom(src => src[nameof(PMEDR.RECORD_TYPE)].ToString().Trim()))
                .ForMember(
                    dest => dest.REQ_NAME,
                    opt => opt.MapFrom(src => src[nameof(PMEDR.REQ_NAME)].ToString().Trim()))
                .ForMember(
                    dest => dest.REQ_DESCRIPTION,
                    opt => opt.MapFrom(src => src[nameof(PMEDR.REQ_DESCRIPTION)].ToString().Trim()))
                .ForMember(
                    dest => dest.REQ_NUMBER,
                    opt => opt.MapFrom(src => src[nameof(PMEDR.REQ_NUMBER)]))
                .ReverseMap();
        }
    }
}