namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PRQRMTBLMappingProfile : Profile
    {
        public PRQRMTBLMappingProfile()
        {
            CreateMap<GenericRecord, PRQRMTBL>()
                .ForMember(
                    dest => dest.REQ_SEQUENCE,
                    opt => opt.MapFrom(src => src[nameof(PRQRMTBL.REQ_SEQUENCE)]))
                .ForMember(
                    dest => dest.UND_DESC_CODE,
                    opt => opt.MapFrom(src => src[nameof(PRQRMTBL.UND_DESC_CODE)]))
                .ForMember(
                    dest => dest.COMPANY_CODE,
                    opt => opt.MapFrom(src => src[nameof(PRQRMTBL.COMPANY_CODE)].ToString().Trim()))
                .ForMember(
                    dest => dest.UND_REQ_MET,
                    opt => opt.MapFrom(src => src[nameof(PRQRMTBL.UND_REQ_MET)].ToString().Trim()))
                .ForMember(
                    dest => dest.UND_REQ_DATE,
                    opt => opt.MapFrom(src => src[nameof(PRQRMTBL.UND_REQ_DATE)]))
                .ForMember(
                    dest => dest.UND_OBTAIN_DATE,
                    opt => opt.MapFrom(src => src[nameof(PRQRMTBL.UND_OBTAIN_DATE)]))
                .ForMember(
                    dest => dest.SEQ,
                    opt => opt.MapFrom(src => src[nameof(PRQRMTBL.SEQ)]))
                .ForMember(
                    dest => dest.NAME_ID,
                    opt => opt.MapFrom(src => src[nameof(PRQRMTBL.NAME_ID)]))
                .ForMember(
                    dest => dest.POLICY_NUMBER,
                    opt => opt.MapFrom(src => src[nameof(PRQRMTBL.POLICY_NUMBER)].ToString().Trim()))
                .ForMember(
                    dest => dest.UND_COMMENTS,
                    opt => opt.MapFrom(src => src[nameof(PRQRMTBL.UND_COMMENTS)].ToString().Trim()))
                .ForMember(
                    dest => dest.UND_REQ_NOTE_SEQ,
                    opt => opt.MapFrom(src => src[nameof(PRQRMTBL.UND_REQ_NOTE_SEQ)].ToString().Trim()))

                .ReverseMap();
        }
    }
}