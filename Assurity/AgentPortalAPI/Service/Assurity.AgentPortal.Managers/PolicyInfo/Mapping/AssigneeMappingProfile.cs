namespace Assurity.AgentPortal.Managers.PolicyInfo.Mapping
{
    using Assurity.PolicyInfo.Contracts.V1;
    using AutoMapper;

    public class AssigneeMappingProfile : Profile
    {
        public AssigneeMappingProfile()
        {
            CreateMap<Assignee, Assignee>();
        }
    }
}