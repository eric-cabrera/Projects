namespace Assurity.AgentPortal.Managers.ListBill.Mapping;

using Assurity.AgentPortal.Contracts.ListBill;
using AutoMapper;
using ListBillAPI = Assurity.ListBill.Service.Contracts;

public class ListBillsResponseMappingProfile : Profile
{
    public ListBillsResponseMappingProfile()
    {
        CreateMap<ListBillAPI.ListBillResponse, ListBillsResponse>();

        CreateMap<ListBillAPI.ListBill, ListBill>();
    }
}