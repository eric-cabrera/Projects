namespace Assurity.AgentPortal.Accessors.GroupInventory;

using Assurity.Groups.Contracts;

public interface IGroupInventoryApiAccessor
{
    public Task<GroupSummaryResponse?> GetGroupSummary(
        string agentNumber,
        GroupSummaryQueryParameters queryParameters,
        CancellationToken cancellationToken = default);

    public Task<GroupDetailResponse?> GetGroupDetail(
        string loggedInAgentNumber,
        string groupNumber,
        GroupDetailsQueryParameters queryParameters,
        CancellationToken cancellationToken = default);
}