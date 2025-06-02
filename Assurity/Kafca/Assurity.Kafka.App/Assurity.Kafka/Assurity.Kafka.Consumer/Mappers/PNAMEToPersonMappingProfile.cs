namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Utilities.Extensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using AutoMapper;

    public class PNAMEToPersonMappingProfile : Profile
    {
        public PNAMEToPersonMappingProfile()
        {
            CreateMap<PNAME, Person>()
                .ForPath(
                    dest => dest.Name.BusinessName,
                    opt => opt.MapFrom(src => src.NAME_BUSINESS.Trim()))
                .ForPath(
                    dest => dest.Name.IndividualFirst,
                    opt => opt.MapFrom(src => src.INDIVIDUAL_FIRST.Trim()))
                .ForPath(
                    dest => dest.Name.IndividualLast,
                    opt => opt.MapFrom(src => src.INDIVIDUAL_LAST.Trim()))
                .ForPath(
                    dest => dest.Name.IndividualMiddle,
                    opt => opt.MapFrom(src => src.INDIVIDUAL_MIDDLE.Trim()))
                .ForPath(
                    dest => dest.Name.IndividualPrefix,
                    opt => opt.MapFrom(src => src.INDIVIDUAL_PREFIX.Trim()))
                .ForPath(
                    dest => dest.Name.IndividualSuffix,
                    opt => opt.MapFrom(src => src.INDIVIDUAL_SUFFIX.Trim()))
                .ForPath(
                    dest => dest.Name.NameId,
                    opt => opt.MapFrom(src => src.NAME_ID))
                .ForMember(
                    dest => dest.EmailAddress,
                    opt => opt.MapFrom(src => src.NAME_FORMAT_CODE == "B" ?
                        src.BUSINESS_EMAIL_ADR.ToString().Trim() : src.PERSONAL_EMAIL_ADR.ToString().Trim()))
                .ForMember(
                    dest => dest.DateOfBirth,
                    opt => opt.MapFrom(src => src.DATE_OF_BIRTH.ToNullableDateTime()))
                .ForMember(
                    dest => dest.Gender,
                    opt => opt.MapFrom(src => src.SEX_CODE.Trim().ToGender()))
                .ReverseMap();
        }
    }
}
