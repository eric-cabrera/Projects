namespace Assurity.AgentPortal.Managers.ListBill.Mapping;

using Assurity.AgentPortal.Contracts.ListBill;
using AutoMapper;
using ListBillAPI = Assurity.ListBill.Service.Contracts;

public class GroupsResponseMappingProfile : Profile
{
    public GroupsResponseMappingProfile()
    {
        CreateMap<ListBillAPI.GroupsResponse, GroupsResponse>();

        CreateMap<ListBillAPI.Group, Group>();
    }
}
