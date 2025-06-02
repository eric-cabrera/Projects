namespace Assurity.AgentPortal.Managers;

using System.Threading;
using Assurity.AgentPortal.Accessors.SubAccount.DTOs.AgentCenterAPIResponses;
using Assurity.AgentPortal.Contracts.SubaccountUsers;

public interface ISubaccountManager
{
    Task<List<GetUsersResponse>?> GetSubaccountsAsync(
        string agentId,
        string accessToken,
        CancellationToken cancellationToken,
        bool isHomeOffice = false);

    Task<UpdateUserResponse>? UpdateSubaccountAsync(
        string email,
        string userId,
        string parentAgentId,
        string parentUserName,
        List<SubAccountRole> roles,
        bool verified,
        string accessToken,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteSubaccountAsync(
        string userId,
        string parentAgentId,
        bool verified,
        string accessToken,
        CancellationToken cancellationToken);

    Task<CreateSubaccountReponse> CreatePendingSubaccountAsync(
    string email,
    List<SubAccountRole> roles,
    string accessToken,
    string parentAgentId,
    CancellationToken cancellationToken = default,
    string? parentUsername = null);

    Task<PendingSubaccountActivationResponse?> ActivateSubaccountAsync(
        string email,
        string activationId,
        string passWord,
        CancellationToken cancellationToken = default);

    Task<bool?> ResendSubaccountActivationLinkAsync(
        string email,
        string accessToken,
        string parentAgentId,
        CancellationToken cancellationToken = default);
}
