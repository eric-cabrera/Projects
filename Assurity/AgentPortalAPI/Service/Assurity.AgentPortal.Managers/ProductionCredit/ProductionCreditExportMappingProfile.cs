namespace Assurity.AgentPortal.Managers.ProductionCredit;

using Assurity.AgentPortal.Contracts.FileExportEngine;
using Assurity.AgentPortal.Contracts.ProductionCredit.FileExport;
using AutoMapper;
using ProductionCredit = Assurity.Production.Contracts.V1;

public class ProductionCreditExportMappingProfile : Profile
{
    public ProductionCreditExportMappingProfile()
    {
        CreateMap<ProductionCredit.Shared.PolicyDetails.PolicyDetail, ProductionCreditExport>()
            .ForMember(
                dest => dest.PolicyCount,
                opt => opt.MapFrom(src => MapDecimalToExcelNoFormat(src.PolicyCount)));
        CreateMap<ProductionCredit.Shared.PolicyDetails.PolicyDetail, ProductionCreditWorksiteExport>()
            .ForMember(
                dest => dest.PolicyCount,
                opt => opt.MapFrom(src => MapDecimalToExcelNoFormat(src.PolicyCount)));
    }

    private static ExcelDataCell MapDecimalToExcelNoFormat(decimal? value)
    {
        return new ExcelDataCell
        {
            Value = value,
            Format = ExcelFormat.NoFormat,
        };
    }
}