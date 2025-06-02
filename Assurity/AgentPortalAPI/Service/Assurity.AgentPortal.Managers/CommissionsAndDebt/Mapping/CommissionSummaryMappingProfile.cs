namespace Assurity.AgentPortal.Managers.CommissionsAndDebt.Mapping;

using Assurity.AgentPortal.Contracts.CommissionsDebt;
using Assurity.Commissions.Internal.Contracts.Summary;
using AutoMapper;

public class CommissionSummaryMappingProfile : Profile
{
    public CommissionSummaryMappingProfile()
    {
        CreateMap<SummaryCommissionsResponse, CommissionSummary>();
    }
}
