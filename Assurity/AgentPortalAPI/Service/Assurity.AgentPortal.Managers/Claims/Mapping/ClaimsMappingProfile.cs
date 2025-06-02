namespace Assurity.AgentPortal.Managers.Claims.Mapping;

using Assurity.AgentPortal.Contracts.Claims;
using AutoMapper;
using ClaimsAPI = Assurity.Claims.Contracts.AssureLink;

public class ClaimsMappingProfile : Profile
{
    public ClaimsMappingProfile()
    {
        CreateMap<ClaimsAPI.AssureLinkClaimResponse, ClaimsResponse>();
        CreateMap<ClaimsAPI.Claim, Claim>();
        CreateMap<ClaimsAPI.ClaimDetail, ClaimDetail>();
        CreateMap<ClaimsAPI.Name, Name>();
    }
}
