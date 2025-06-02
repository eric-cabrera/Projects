namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PPOLCMappingProfile : Profile
    {
        public PPOLCMappingProfile()
        {
            CreateMap<GenericRecord, PPOLC>()
                .ForMember(
                    dest => dest.ANNUAL_PREMIUM,
                    opt => opt.MapFrom(src => src["ANNUAL_PREMIUM"]))
                .ForMember(
                    dest => dest.BILLING_DATE,
                    opt => opt.MapFrom(src => src["BILLING_DATE"]))
                .ForMember(
                    dest => dest.CONTRACT_CODE,
                    opt => opt.MapFrom(src => src["CONTRACT_CODE"].ToString().Trim()))
                .ForMember(
                    dest => dest.CONTRACT_DATE,
                    opt => opt.MapFrom(src => src["CONTRACT_DATE"]))
                .ForMember(
                    dest => dest.BILLING_FORM,
                    opt => opt.MapFrom(src => src["BILLING_FORM"].ToString().Trim()))
                .ForMember(
                    dest => dest.BILLING_MODE,
                    opt => opt.MapFrom(src => src["BILLING_MODE"]))
                .ForMember(
                    dest => dest.COMPANY_CODE,
                    opt => opt.MapFrom(src => src["COMPANY_CODE"].ToString().Trim()))
                .ForMember(
                    dest => dest.CONTRACT_REASON,
                    opt => opt.MapFrom(src => src["CONTRACT_REASON"].ToString().Trim()))
                .ForMember(
                    dest => dest.ISSUE_DATE,
                    opt => opt.MapFrom(src => src["ISSUE_DATE"]))
                .ForMember(
                    dest => dest.ISSUE_STATE,
                    opt => opt.MapFrom(src => src["ISSUE_STATE"].ToString().Trim()))
                .ForMember(
                    dest => dest.MODE_PREMIUM,
                    opt => opt.MapFrom(src => src["MODE_PREMIUM"]))
                .ForMember(
                    dest => dest.PAID_TO_DATE,
                    opt => opt.MapFrom(src => src["PAID_TO_DATE"]))
                .ForMember(
                    dest => dest.POLICY_NUMBER,
                    opt => opt.MapFrom(src => src["POLICY_NUMBER"].ToString().Trim()))
                .ForMember(
                    dest => dest.PRODUCT_CODE,
                    opt => opt.MapFrom(src => src["PRODUCT_CODE"].ToString().Trim()))
                .ForMember(
                    dest => dest.RES_STATE,
                    opt => opt.MapFrom(src => src["RES_STATE"].ToString().Trim()))
                .ForMember(
                    dest => dest.POLC_SPECIAL_MODE,
                    opt => opt.MapFrom(src => src["POLC_SPECIAL_MODE"].ToString().Trim()))
                .ForMember(
                    dest => dest.LINE_OF_BUSINESS,
                    opt => opt.MapFrom(src => src["LINE_OF_BUSINESS"].ToString().Trim()))
                .ForMember(
                    dest => dest.BILLING_CODE,
                    opt => opt.MapFrom(src => src["BILLING_CODE"].ToString().Trim()))
                .ForMember(
                    dest => dest.BILLING_REASON,
                    opt => opt.MapFrom(src => src["BILLING_REASON"].ToString().Trim()))
                .ForMember(
                    dest => dest.PAYMENT_REASON,
                    opt => opt.MapFrom(src => src["PAYMENT_REASON"].ToString().Trim()))
                .ForMember(
                    dest => dest.TAX_QUALIFY_CODE,
                    opt => opt.MapFrom(src => src["TAX_QUALIFY_CODE"].ToString().Trim()))
                .ForMember(
                    dest => dest.APPLICATION_DATE,
                    opt => opt.MapFrom(src => src["APPLICATION_DATE"].ToString().Trim()))
                .ForMember(
                    dest => dest.APP_RECEIVED_DATE,
                    opt => opt.MapFrom(src => src["APP_RECEIVED_DATE"].ToString().Trim()))
                .ForMember(
                    dest => dest.POLICY_BILL_DAY,
                    opt => opt.MapFrom(src => src["POLICY_BILL_DAY"].ToString().Trim()))
                .ForMember(
                    dest => dest.GROUP_NUMBER,
                    opt => opt.MapFrom(src => src["GROUP_NUMBER"].ToString().Trim()))
                .ForMember(
                    dest => dest.PACON_ANNUITY_POLICY,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PPEND_NEW_BUSINESS_PENDING,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PGRUP_GROUP_MASTER,
                    opt => opt.Ignore())
                .ReverseMap();
        }
    }
}