namespace Assurity.AgentPortal.Managers.Mapping.SubaccountUsers;

using Assurity.AgentPortal.Accessors.SubAccount.DTOs.PingResponses;
using Assurity.AgentPortal.Contracts.SubaccountUsers;
using AutoMapper;

public class GetSubaccountUsersMappingProfile : Profile
{
    public GetSubaccountUsersMappingProfile()
    {
        CreateMap<UsersResponseDTO, GetUsersResponse>()
            .ForMember(dest => dest.ActivationStatus, opt => opt.MapFrom(src => src.Enabled))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.SubaccountData.Roles));
    }
}