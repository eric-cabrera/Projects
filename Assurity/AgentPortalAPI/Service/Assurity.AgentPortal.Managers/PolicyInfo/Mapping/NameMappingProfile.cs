namespace Assurity.AgentPortal.Managers.PolicyInfo.Mapping
{
    using Assurity.PolicyInfo.Contracts.V1;
    using AutoMapper;

    public class NameMappingProfile : Profile
    {
        public NameMappingProfile()
        {
            CreateMap<Name, Name>();
        }
    }
}