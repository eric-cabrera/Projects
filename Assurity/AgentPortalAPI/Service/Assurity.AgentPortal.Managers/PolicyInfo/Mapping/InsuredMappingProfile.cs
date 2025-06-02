namespace Assurity.AgentPortal.Managers.PolicyInfo.Mapping
{
    using Assurity.PolicyInfo.Contracts.V1;
    using AutoMapper;

    public class InsuredMappingProfile : Profile
    {
        public InsuredMappingProfile()
        {
            CreateMap<Insured, Insured>();
        }
    }
}