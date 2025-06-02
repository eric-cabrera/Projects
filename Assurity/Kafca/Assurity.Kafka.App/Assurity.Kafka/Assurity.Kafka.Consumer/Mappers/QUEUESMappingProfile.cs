namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class QUEUESMappingProfile : Profile
    {
        public QUEUESMappingProfile()
        {
            CreateMap<GenericRecord, QUEUES>()
                .ForMember(
                    dest => dest.ID,
                    opt => opt.MapFrom(src => src[nameof(QUEUES.ID)]))
                .ForMember(
                    dest => dest.QUEUE,
                    opt => opt.MapFrom(src => src[nameof(QUEUES.QUEUE)]))
                .ReverseMap();
        }
    }
}