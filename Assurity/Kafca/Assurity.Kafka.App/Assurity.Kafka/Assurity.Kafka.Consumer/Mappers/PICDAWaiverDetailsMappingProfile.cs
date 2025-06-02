namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PICDAWaiverDetailsMappingProfile : Profile
    {
        public PICDAWaiverDetailsMappingProfile()
        {
            CreateMap<GenericRecord, PICDA_WAIVER_DETAILS>()
                .ForMember(
                    dest => dest.TYPE_CODE,
                    opt => opt.MapFrom(src => src["TYPE_CODE"].ToString().Trim()))
                .ForMember(
                    dest => dest.KEY_DATA,
                    opt => opt.MapFrom(src => src["KEY_DATA"].ToString().Trim()))
                .ReverseMap();
        }
    }
}