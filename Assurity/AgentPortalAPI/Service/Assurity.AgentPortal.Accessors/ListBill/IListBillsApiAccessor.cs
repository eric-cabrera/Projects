namespace Assurity.AgentPortal.Accessors.ListBill;

using Assurity.ListBill.Service.Contracts;

public interface IListBillsApiAccessor
{
    Task<GroupsResponse?> GetGroups(string agentId, int? page, int? pageSize, CancellationToken cancellationToken);

    Task<ListBillResponse?> GetListBills(string groupId, CancellationToken cancellationToken);

    Task<Stream?> GetListBillData(string listBillId, CancellationToken cancellationToken);
}
