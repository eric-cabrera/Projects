namespace Assurity.AgentPortal.Managers.PolicyInfo.Mapping
{
    using Assurity.PolicyInfo.Contracts.V1;
    using AutoMapper;

    public class AgentMappingProfile : Profile
    {
        public AgentMappingProfile()
        {
            CreateMap<Agent, Agent>();
        }
    }
}