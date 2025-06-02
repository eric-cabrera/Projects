namespace Assurity.AgentPortal.Managers.PolicyInfo.Mapping
{
    using Assurity.PolicyInfo.Contracts.V1;
    using AutoMapper;

    public class PayorMappingProfile : Profile
    {
        public PayorMappingProfile()
        {
            CreateMap<Payor, Payor>();
        }
    }
}