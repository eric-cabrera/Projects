namespace Assurity.AgentPortal.Managers.CommissionsAndDebt.Mapping;

using Assurity.AgentPortal.Contracts.CommissionsDebt;
using AutoMapper;
using Commissions = Assurity.Commissions.Internal.Contracts.Shared;
using Debt = Assurity.Commissions.Debt.Contracts.Advances;

public class AgentMappingProfile : Profile
{
    public AgentMappingProfile()
    {
        CreateMap<Commissions.Agent, Agent>()
            .ForMember(
                dest => dest.AgentId,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(
                dest => dest.AgentName,
                opt => opt.MapFrom(src => src.Name));

        CreateMap<Debt.AgentIdentifier, Agent>();
    }
}
