namespace Assurity.AgentPortal.Accessors;

using System.Threading;
using Assurity.AgentPortal.Accessors.ProfilePingDTOs;
using Assurity.AgentPortal.Accessors.SubAccount.DTOs;
using Assurity.AgentPortal.Accessors.SubAccount.DTOs.AgentCenterAPIResponses;
using Assurity.AgentPortal.Accessors.SubAccount.DTOs.PingResponses;
using Assurity.AgentPortal.Contracts.SubaccountUsers;

public interface ISubaccountAccessor
{
    Task<SubAccountResponseDTO?> GetAllPingSubaccountsAsync(
        string? pagedUrl,
        CancellationToken cancellationToken);

    Task<List<PendingSubaccount>?> GetPendingSubaccountsAsync(
        string accessToken,
        string parentAgentId,
        CancellationToken cancellationToken);

    Task<UsersResponseDTO?> UpdatePingSubaccountAsync(
        string userId,
        string userName,
        string parentAgentId,
        string parentUserName,
        List<SubAccountRole> roles,
        CancellationToken cancellationToken);

    Task<bool?> UpdatePendingSubaccountAsync(
        string email,
        List<SubAccountRole> roles,
        string accessToken,
        string parentAgentId,
        CancellationToken cancellationToken);

    Task<bool> DeletePingAccountAsync(
        string userId,
        CancellationToken cancellationToken);

    Task<bool> DeletePendingSubaccountAsync(
        string userId,
        CancellationToken cancellationToken);

    Task<bool> DoesPendingSubaccountExistAsync(string email, string parentAgentId, string accessToken, CancellationToken cancellationToken);

    Task<bool> DoesPingSubaccountExistAsync(string email, string parentAgentId, CancellationToken cancellationToken);

    Task<PendingSubaccount> CreatePendingSubaccountAsync(
        string email,
        List<SubAccountRole> roles,
        string accessToken,
        string parentAgentId,
        CancellationToken cancellationToken,
        string? parentUsername = null);

    Task<PendingSubaccountActivationResponse?> ConfirmEmailAndGetRolesAsync(
        string email,
        string activationId,
        CancellationToken cancellationToken);

    Task<bool?> ResendSubaccountActivationLinkAsync(
        string email,
        string accessToken,
        string parentAgentId,
        CancellationToken cancellationToken);

    Task<PingOneResponse?> CreatePingSubaccountAsync(
        string email,
        string parentAgentId,
        string parentUserName,
        List<SubAccountRole> roles,
        string passWord,
        CancellationToken cancellationToken);

    Task<bool?> CreatePingSubaccountMFADeviceAsync(string userId, string email, CancellationToken cancellationToken);

    Task<UsersResponseDTO?> GetPingUser(string userId, CancellationToken cancellationToken);
}
