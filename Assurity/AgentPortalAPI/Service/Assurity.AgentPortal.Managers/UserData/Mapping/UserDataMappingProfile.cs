namespace Assurity.AgentPortal.Managers.UserData.Mapping;

using Assurity.AgentPortal.Contracts.AgentContracts;
using AutoMapper;
using AgentAPI = Assurity.Agent.Contracts;
using LocalAgentContracts = Assurity.AgentPortal.Contracts.UserData;

public class UserDataMappingProfile : Profile
{
    public UserDataMappingProfile()
    {
        CreateMap<AgentAPI.AgentAccessResponse, AgentAccessResponse>();
        CreateMap<AgentAPI.AccessLevel, AccessLevel>();
    }
}
