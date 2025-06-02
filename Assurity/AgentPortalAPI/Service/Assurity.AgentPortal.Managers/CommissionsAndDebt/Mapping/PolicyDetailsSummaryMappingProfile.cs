namespace Assurity.AgentPortal.Managers.CommissionsAndDebt.Mapping;

using Assurity.AgentPortal.Contracts.CommissionsDebt;
using Assurity.AgentPortal.Contracts.CommissionsDebt.FileExport;
using Assurity.AgentPortal.Contracts.FileExportEngine;
using AutoMapper;
using Commissions = Assurity.Commissions.Internal.Contracts.PolicyDetails;

public class PolicyDetailsSummaryMappingProfile : Profile
{
    public PolicyDetailsSummaryMappingProfile()
    {
        CreateMap<Commissions.PolicyDetail, PolicyDetailsExport>()
            .ForMember(
                dest => dest.CommissionRate,
                opt => opt.MapFrom(src => MapDecimalToExcelFraction(src.CommissionRate)))
            .ForMember(
                dest => dest.CommissionType,
                opt => opt.MapFrom(src => src.CommissionType.ToString()))
            .ForMember(
                dest => dest.LineOfBusiness,
                opt => opt.MapFrom(src => src.LineOfBusiness.ToString()))
            .ForMember(
                dest => dest.Mode,
                opt => opt.MapFrom(src => src.Mode.ToString()))
            .ForMember(
                dest => dest.ChargebackReason,
                opt => opt.MapFrom(src => src.Description));
    }

    private static ExcelDataCell MapDecimalToExcelFraction(decimal? value)
    {
        return new ExcelDataCell
        {
            Value = value / 100m,
            Format = ExcelFormat.Fraction,
        };
    }
}
