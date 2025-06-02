namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PRQRMMappingProfile : Profile
    {
        public PRQRMMappingProfile()
        {
            CreateMap<GenericRecord, PRQRM>()
                .ForMember(
                    dest => dest.COMPANY_CODE,
                    opt => opt.MapFrom(src => src[nameof(PRQRM.COMPANY_CODE)].ToString().Trim()))
                .ForMember(
                    dest => dest.REQ_SEQUENCE,
                    opt => opt.MapFrom(src => src[nameof(PRQRM.REQ_SEQUENCE)]))
                .ForMember(
                    dest => dest.LAST_CHG_DATE,
                    opt => opt.MapFrom(src => src[nameof(PRQRM.LAST_CHG_DATE)]))
                .ForMember(
                    dest => dest.POLICY_NUMBER,
                    opt => opt.MapFrom(src => src[nameof(PRQRM.POLICY_NUMBER)].ToString().Trim()))
                .ForMember(
                    dest => dest.NAME_ID,
                    opt => opt.MapFrom(src => src[nameof(PRQRM.NAME_ID)]))
                .ReverseMap();
        }
    }
}