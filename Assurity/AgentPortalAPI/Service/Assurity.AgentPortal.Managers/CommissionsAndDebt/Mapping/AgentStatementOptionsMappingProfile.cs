namespace Assurity.AgentPortal.Managers.CommissionsAndDebt.Mapping;

using Assurity.AgentPortal.Contracts.CommissionsDebt;
using Assurity.Commissions.Internal.Contracts.AgentStatementOptions;
using AutoMapper;

public class AgentStatementOptionsMappingProfile : Profile
{
    public AgentStatementOptionsMappingProfile()
    {
        CreateMap<AgentStatementOptionsResponse, AgentStatementOptions>();
    }
}
