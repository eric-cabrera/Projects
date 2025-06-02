namespace Assurity.AgentPortal.Managers.PolicyInfo.Mapping
{
    using Assurity.PolicyInfo.Contracts.V1;
    using AutoMapper;

    public class ParticipantMappingProfile : Profile
    {
        public ParticipantMappingProfile()
        {
            CreateMap<Participant, Participant>();
        }
    }
}