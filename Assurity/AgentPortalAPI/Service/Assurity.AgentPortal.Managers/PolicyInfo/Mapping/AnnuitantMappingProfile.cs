namespace Assurity.AgentPortal.Managers.PolicyInfo.Mapping
{
    using Assurity.PolicyInfo.Contracts.V1;
    using AutoMapper;

    public class AnnuitantMappingProfile : Profile
    {
        public AnnuitantMappingProfile()
        {
            CreateMap<Annuitant, Annuitant>();
        }
    }
}