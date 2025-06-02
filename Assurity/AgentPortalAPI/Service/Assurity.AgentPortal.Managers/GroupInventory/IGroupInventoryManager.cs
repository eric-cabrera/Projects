namespace Assurity.AgentPortal.Managers.GroupInventory;

using System.Threading;
using System.Threading.Tasks;
using Assurity.AgentPortal.Contracts.GroupInventory.Request;
using Assurity.AgentPortal.Contracts.GroupInventory.Response;
using Assurity.AgentPortal.Contracts.Shared;

public interface IGroupInventoryManager
{
        Task<GroupSummaryResponse?> GetGroupSummary(
            string loggedInAgentNumber,
            GroupSummaryQueryParameters queryParameters,
            CancellationToken cancellationToken = default);

        Task<GroupDetailResponse?> GetGroupDetail(
            string loggedInAgentNumber,
            GroupDetailsQueryParameters queryParameters,
            CancellationToken cancellationToken = default);

        Task<FileResponse?> GetGroupSummaryExport(
            string loggedInAgentNumber,
            GroupSummaryQueryParameters queryParameters,
            CancellationToken cancellationToken = default);

        Task<FileResponse?> GetGroupDetailsExport(
            string loggedInAgentNumber,
            GroupDetailsQueryParameters queryParameters,
            CancellationToken cancellationToken = default);
}