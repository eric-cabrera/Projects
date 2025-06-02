namespace Assurity.AgentPortal.Managers.ListBill;

using Assurity.AgentPortal.Contracts.ListBill;
using Assurity.AgentPortal.Contracts.Shared;

public interface IListBillsManager
{
    Task<GroupsResponse?> GetGroups(string agentId, int? page, int? pageSize, CancellationToken cancellationToken = default);

    Task<ListBillsResponse?> GetListBills(string groupId, CancellationToken cancellationToken = default);

    Task<FileResponse?> GetListBillData(string listBillId, CancellationToken cancellationToken = default);
}