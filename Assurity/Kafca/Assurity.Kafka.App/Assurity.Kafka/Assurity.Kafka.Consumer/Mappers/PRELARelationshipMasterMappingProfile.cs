namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PRELARelationshipMasterMappingProfile : Profile
    {
        public PRELARelationshipMasterMappingProfile()
        {
            CreateMap<GenericRecord, PRELA_RELATIONSHIP_MASTER>()
                .ForMember(
                    dest => dest.IDENTIFYING_ALPHA,
                    opt => opt.MapFrom(src => src["IDENTIFYING_ALPHA"].ToString().Trim()))
                .ForMember(
                    dest => dest.NAME_ID,
                    opt => opt.MapFrom(src => src["NAME_ID"]))
                .ForMember(
                    dest => dest.RELATE_CODE,
                    opt => opt.MapFrom(src => src["RELATE_CODE"].ToString().Trim()))
                .ForMember(
                    dest => dest.BENEFIT_SEQ_NUMBER,
                    opt => opt.MapFrom(src => src["BENEFIT_SEQ_NUMBER"]))
                .ForMember(
                    dest => dest.PNAME,
                    opt => opt.Ignore())
                .ReverseMap();
        }
    }
}