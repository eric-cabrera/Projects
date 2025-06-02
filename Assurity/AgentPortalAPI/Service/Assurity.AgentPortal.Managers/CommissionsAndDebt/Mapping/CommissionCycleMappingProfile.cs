namespace Assurity.AgentPortal.Managers.CommissionsAndDebt.Mapping;

using Assurity.AgentPortal.Contracts.CommissionsDebt.Response;
using Assurity.Commissions.Internal.Contracts.Cycle;
using AutoMapper;

public class CommissionCycleMappingProfile : Profile
{
    public CommissionCycleMappingProfile()
    {
        CreateMap<CycleCommissionsResponse, CommissionCycle>()
            .ForMember(
                dest => dest.FirstYear,
                opt => opt.MapFrom(source => source.FirstYearCommission))
            .ForMember(
                dest => dest.Renewal,
                opt => opt.MapFrom(source => source.RenewalCommission))
            .ForMember(
                dest => dest.CycleStartDate,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.CycleEndDate,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Estimated,
                opt => opt.Ignore());
    }
}
