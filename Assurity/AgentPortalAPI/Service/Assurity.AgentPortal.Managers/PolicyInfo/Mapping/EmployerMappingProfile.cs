namespace Assurity.AgentPortal.Managers.PolicyInfo.Mapping
{
    using Assurity.PolicyInfo.Contracts.V1;
    using AutoMapper;

    public class EmployerMappingProfile : Profile
    {
        public EmployerMappingProfile()
        {
            CreateMap<Employer, Employer>();
        }
    }
}