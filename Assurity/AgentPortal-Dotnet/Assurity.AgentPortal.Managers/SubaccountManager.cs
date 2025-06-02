namespace Assurity.AgentPortal.Managers;

using System.Linq;
using Assurity.AgentPortal.Accessors;
using Assurity.AgentPortal.Accessors.SubAccount.DTOs.AgentCenterAPIResponses;
using Assurity.AgentPortal.Accessors.SubAccount.DTOs.PingResponses;
using Assurity.AgentPortal.Contracts.SubaccountUsers;
using AutoMapper;
using Microsoft.Extensions.Logging;

public class SubaccountManager : BaseManager, ISubaccountManager
{
    private readonly ILogger<SubaccountManager> logger;

    public SubaccountManager(
        ILogger<SubaccountManager> log,
        ISubaccountAccessor userAccessor,
        IUserDataAccessor userDataAccessor,
        IMapper mapper)
    {
        logger = log;
        UserAccessor = userAccessor;
        UserDataAccessor = userDataAccessor;
        Mapper = mapper;
    }

    private ISubaccountAccessor UserAccessor { get; set; }

    private IUserDataAccessor UserDataAccessor { get; set; }

    private IMapper Mapper { get; set; }

    public async Task<List<GetUsersResponse>?> GetSubaccountsAsync(
    string parentAgentId,
    string accessToken,
    CancellationToken cancellationToken,
    bool isHomeOffice = false)
    {
        string? nextPageLink = null;
        var agentId = isHomeOffice ? parentAgentId : null;

        var additionalAgentIds = await UserDataAccessor.GetAdditionalAgentIds(accessToken, agentId);

        additionalAgentIds.AgentIds ??=[parentAgentId];
        var subAccounts = new List<UsersResponseDTO>();

        do
        {
            var agents = await UserAccessor.GetAllPingSubaccountsAsync(nextPageLink, cancellationToken);

            nextPageLink = agents?.Links?.Next?.Href ?? null;

            if (agents?.Embedded?.Users != null)
            {
                var filteredSubaccounts = agents.Embedded.Users
                    .Select(x => new UsersResponseDTO
                    {
                        UserId = x.UserId,
                        Email = x.Email,
                        SubaccountData = x.SubaccountData,
                        Enabled = true,
                        UserName = x.UserName,
                    })
                    .Where(x => x.SubaccountData?.ParentAgentId is not null &&
                        additionalAgentIds.AgentIds.Contains(x.SubaccountData.ParentAgentId, StringComparer.OrdinalIgnoreCase));

                subAccounts.AddRange(filteredSubaccounts);
            }
        }
        while (nextPageLink is not null);

        var pendingSubaccounts = await UserAccessor.GetPendingSubaccountsAsync(accessToken, parentAgentId, cancellationToken);

        if (pendingSubaccounts is not null)
        {
            var mappedPendingSubaccounts = pendingSubaccounts.Select(x => new UsersResponseDTO
            {
                UserId = x.Id,
                Email = x.Email,
                SubaccountData = new SubAccountDataDTO
                {
                    ParentAgentId = x.AgentId,
                    ParentUsername = x.ParentUsername,
                    Roles = x.Roles
                },
                Enabled = false,
                UserName = x.Email,
            }).ToList();

            subAccounts.AddRange(mappedPendingSubaccounts);
        }

        return Mapper.Map<List<GetUsersResponse>>(subAccounts);
    }

    public async Task<UpdateUserResponse?> UpdateSubaccountAsync(
        string email,
        string userId,
        string parentAgentId,
        string parentUserName,
        List<SubAccountRole> roles,
        bool verified,
        string accessToken,
        CancellationToken cancellationToken)
    {
        UsersResponseDTO? response = null;

        if (verified)
        {
            var additionalAgentIds = await UserDataAccessor.GetAdditionalAgentIds(accessToken, parentAgentId);

            if (await UserHasAccessToSubaccount(userId, additionalAgentIds.AgentIds, cancellationToken))
            {
                response = await UserAccessor.UpdatePingSubaccountAsync(userId, email, parentAgentId, parentUserName, roles, cancellationToken);
            }
            else
            {
                return null;
            }
        }
        else
        {
            var pendingResponse = await UserAccessor.UpdatePendingSubaccountAsync(email, roles, accessToken, parentAgentId, cancellationToken);

            if (pendingResponse.HasValue && pendingResponse.Value)
            {
                response = new UsersResponseDTO()
                {
                    UserId = userId
                };
            }
            else
            {
                return null;
            }
        }

        return new UpdateUserResponse
        {
            UserId = response.UserId,
            Roles = response.SubaccountData?.Roles.Select(role => role.ToString()).ToList() ?? new List<string>()
        };
    }

    public async Task<bool> DeleteSubaccountAsync(
        string userId,
        string agentId,
        bool isActive,
        string accessToken,
        CancellationToken cancellationToken)
    {
        var success = false;

        if (isActive)
        {
            var additionalAgentIds = await UserDataAccessor.GetAdditionalAgentIds(accessToken, agentId);

            if (await UserHasAccessToSubaccount(userId, additionalAgentIds.AgentIds, cancellationToken))
            {
                success = await UserAccessor.DeletePingAccountAsync(userId, cancellationToken);
            }
        }
        else
        {
            success = await UserAccessor.DeletePendingSubaccountAsync(userId, cancellationToken);
        }

        return success;
    }

    public async Task<CreateSubaccountReponse> CreatePendingSubaccountAsync(
        string email,
        List<SubAccountRole> roles,
        string accessToken,
        string parentAgentId,
        CancellationToken cancellationToken = default,
        string? parentUsername = null)
    {
        var response = new CreateSubaccountReponse() { Success = true };
        if ((await UserAccessor.DoesPendingSubaccountExistAsync(email, parentAgentId, accessToken, cancellationToken)) ||
            (await UserAccessor.DoesPingSubaccountExistAsync(email, parentAgentId, cancellationToken)))
        {
            response.Success = false;
            response.Message = "Subaccount for provided email already exists";

            return response;
        }

        var result = await UserAccessor.CreatePendingSubaccountAsync(
            email, roles, accessToken, parentAgentId, cancellationToken, parentUsername);

        var usersResponse = new UsersResponseDTO
        {
            UserId = result.Id,
            Email = result.Email,
            SubaccountData = new SubAccountDataDTO
            {
                ParentAgentId = result.AgentId,
                ParentUsername = result.ParentUsername,
                Roles = result.Roles
            },
            Enabled = false,
            UserName = result.Email,
        };

        response.Subaccount = Mapper.Map<GetUsersResponse>(usersResponse);

        return response;
    }

    public async Task<PendingSubaccountActivationResponse?> ActivateSubaccountAsync(
        string email,
        string activationId,
        string password,
        CancellationToken cancellationToken = default)
    {
        var confirmResponse = await UserAccessor.ConfirmEmailAndGetRolesAsync(email, activationId, cancellationToken);

        if (confirmResponse == null)
        {
            return new PendingSubaccountActivationResponse { Valid = false };
        }
        else if (confirmResponse.Valid == false)
        {
            return confirmResponse;
        }

        var pingResponse = await UserAccessor.CreatePingSubaccountAsync(email, confirmResponse.Subaccount.AgentId, confirmResponse.Subaccount.ParentUsername, confirmResponse.Subaccount.Roles, password, cancellationToken);

        if (pingResponse == null)
        {
            confirmResponse.Valid = false;
            return confirmResponse;
        }

        if (string.IsNullOrEmpty(pingResponse.Status) || pingResponse.Status != "OK")
        {
            confirmResponse.Valid = false;
            confirmResponse.Message = GetChangePasswordErrorMessage(pingResponse);
            return confirmResponse;
        }

        var mfaResult = await UserAccessor.CreatePingSubaccountMFADeviceAsync(pingResponse.Id, email, cancellationToken);

        if (mfaResult == null || !mfaResult.Value)
        {
            confirmResponse.Valid = false;
            confirmResponse.Message = "An error occurred when trying to create MFA device";
            return confirmResponse;
        }

        var deletePendingResponse = await UserAccessor.DeletePendingSubaccountAsync(confirmResponse.Subaccount.Id, cancellationToken);

        if (!deletePendingResponse)
        {
            logger.LogError("Error deleting pending subaccount, Subaccount Id: {id}", confirmResponse.Subaccount.Id);
        }

        return confirmResponse;
    }

    public async Task<bool?> ResendSubaccountActivationLinkAsync(
        string email,
        string accessToken,
        string parentAgentId,
        CancellationToken cancellationToken = default)
    {
        var response = await UserAccessor.ResendSubaccountActivationLinkAsync(email, accessToken, parentAgentId, cancellationToken);

        if (response == null)
        {
            return null;
        }

        return response;
    }

    private async Task<bool> UserHasAccessToSubaccount(string subaccountUserId, List<string> agentIds, CancellationToken cancellationToken)
    {
        var hasAccess = false;
        var pingUser = await UserAccessor.GetPingUser(subaccountUserId, cancellationToken);

        // Ensure the user retreived is a Subaccount and the agent has access to the Subaccount.
        if ((pingUser?.AgentId?.Equals("subaccount") ?? false) &&
            agentIds.Contains(pingUser?.SubaccountData?.ParentAgentId ?? string.Empty, StringComparer.OrdinalIgnoreCase))
        {
            hasAccess = true;
        }
        else
        {
            logger.LogError(
                "User does not have access to Subaccount.  {SubaccountUserId}",
                subaccountUserId);
        }

        return hasAccess;
    }
}
