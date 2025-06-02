namespace Assurity.AgentPortal.Managers.PolicyInfo.Mapping
{
    using Assurity.PolicyInfo.Contracts.V1;
    using AutoMapper;

    public class PayeeMappingProfile : Profile
    {
        public PayeeMappingProfile()
        {
            CreateMap<Payee, Payee>();
        }
    }
}