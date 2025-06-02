namespace Assurity.AgentPortal.Managers.PolicyInfo.Mapping;

using Assurity.AgentPortal.Contracts.PolicyInfo;
using AutoMapper;
using PolicyInfoAPI = Assurity.PolicyInformation.Contracts.V1;

public class PolicyStatusCountsMappingProfile : Profile
{
    public PolicyStatusCountsMappingProfile()
    {
        CreateMap<PolicyInfoAPI.PolicyStatusCounts, PolicyStatusCountsResponse>();
        CreateMap<PolicyInfoAPI.PendingStatusCounts, PendingStatusCounts>();
        CreateMap<PolicyInfoAPI.ActiveStatusCounts, ActiveStatusCounts>();
        CreateMap<PolicyInfoAPI.TerminatedStatusCounts, TerminatedStatusCounts>();
    }
}
