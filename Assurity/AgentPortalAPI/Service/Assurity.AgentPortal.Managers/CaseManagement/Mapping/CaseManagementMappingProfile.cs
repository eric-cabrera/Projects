namespace Assurity.AgentPortal.Managers.Claims.Mapping;

using Assurity.AgentPortal.Contracts.CaseManagement;
using Assurity.ApplicationTracker.Contracts;
using Assurity.ApplicationTracker.Contracts.DataTransferObjects;
using AutoMapper;

public class CaseManagementMappingProfile : Profile
{
    public CaseManagementMappingProfile()
    {
        CreateMap<ApplicationTracker, CaseManagementCase>();
        CreateMap<PagedEvents, CaseManagementResponse>()
            .ForMember(
                dest => dest.Cases,
                opt => opt.MapFrom(src => src.Data));
        CreateMap<AppTrackerEvent, CaseManagementEvent>();
        CreateMap<ApplicationTrackerEvent, CaseManagementEvent>();
    }
}
