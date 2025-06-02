namespace Assurity.AgentPortal.Managers.ProductionCredit;

using Assurity.AgentPortal.Contracts.ProductionCredit.Response.Individual;
using Assurity.AgentPortal.Contracts.ProductionCredit.Response.PolicyDetail;
using Assurity.AgentPortal.Contracts.ProductionCredit.Response.Shared;
using Assurity.AgentPortal.Contracts.ProductionCredit.Response.Worksite;
using AutoMapper;
using ProductionCredit = Assurity.Production.Contracts.V1;
using ProductionCreditFilters = Assurity.Production.Contracts.V1.Filters.Response;

public class ProductionCreditMappingProfile : Profile
{
    public ProductionCreditMappingProfile()
    {
        CreateMap<ProductionCreditFilters.Shared.Type, ProductionCreditLineOfBusinessType>();
        CreateMap<ProductionCreditFilters.Shared.LineOfBusiness, ProductionCreditLineOfBusiness>();
        CreateMap<ProductionCreditFilters.Shared.Agent, ProductionCreditAgent>();
        CreateMap<ProductionCreditFilters.Shared.FilterValues, ProductionCreditFilterValues>();

        CreateMap<ProductionCredit.Individual.Total, ProductionCreditSupplementalReportTotal>();
        CreateMap<ProductionCredit.Individual.SupplementalReport, ProductionCreditSupplementalReport>();
        CreateMap<ProductionCredit.Individual.IndividualProduction, IndividualProductionCredit>();
        CreateMap<ProductionCredit.Individual.IndividualProductionReport, IndividualProductionCreditSummary>();

        CreateMap<ProductionCreditFilters.PolicyDetailFilterValues, ProductionCreditPolicyDetailFilterValues>();
        CreateMap<ProductionCreditFilters.WorksiteFilterValues, ProductionCreditWorksiteFilterValues>();

        CreateMap<ProductionCredit.Shared.PolicyDetails.PolicyDetail, ProductionCreditPolicyDetail>();
        CreateMap<ProductionCredit.Shared.PolicyDetails.PolicyDetailsReport, ProductionCreditPolicyDetailsSummary>();

        CreateMap<ProductionCredit.Worksite.SupplementalReport.GroupAndPremiumTotals, GroupAndPremiumTotals>();
        CreateMap<ProductionCredit.Worksite.SupplementalReport.ProductionByAgentSupplementalReport, ProductionByAgentSupplementalReport>();

        CreateMap<ProductionCredit.Worksite.WorksiteProduction, WorksiteProductionCredit>();
        CreateMap<ProductionCredit.Worksite.WorksiteReport, WorksiteProductionCreditSummary>();
    }
}
