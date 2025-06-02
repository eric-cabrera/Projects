namespace Assurity.AgentPortal.Managers.PolicyInfo.Mapping
{
    using Assurity.PolicyInfo.Contracts.V1;
    using AutoMapper;

    public class BusinessMappingProfile : Profile
    {
        public BusinessMappingProfile()
        {
            CreateMap<Business, Business>();
        }
    }
}