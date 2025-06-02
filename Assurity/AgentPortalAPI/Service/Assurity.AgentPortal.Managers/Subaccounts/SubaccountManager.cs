namespace Assurity.AgentPortal.Managers.Subaccounts;

using Assurity.AgentPortal.Accessors.DTOs;
using Assurity.AgentPortal.Accessors.Subaccounts;
using Assurity.AgentPortal.Contracts.Enums;
using Assurity.AgentPortal.Contracts.Subaccounts;

public class SubaccountManager : ISubaccountManager
{
    public SubaccountManager(ISubaccountAccessor subaccountAccessor)
    {
        SubaccountAccessor = subaccountAccessor;
    }

    private ISubaccountAccessor SubaccountAccessor { get; }

    public async Task<List<PendingSubaccount>> GetPendingSubaccounts(string agentId, CancellationToken cancellationToken)
    {
        var pendingSubaccounts = await SubaccountAccessor.GetPendingSubaccounts(agentId, cancellationToken);
        var mappedSubaccounts = pendingSubaccounts.Select(MapDTOToPendingSubaccount).ToList();

        return mappedSubaccounts;
    }

    public async Task<PendingSubaccountActivationResponse?> ActivateSubaccount(string email, Guid linkGuid)
    {
        var subaccountResponse = new PendingSubaccountActivationResponse
        {
            Valid = false,
        };

        var pendingSubaccount = await SubaccountAccessor.RetrieveAndActivateSubaccount(linkGuid);

        if (pendingSubaccount == null)
        {
            return subaccountResponse;
        }

        if (pendingSubaccount.EmailSentAt.AddHours(24).CompareTo(DateTime.Now) <= 0)
        {
            await SubaccountAccessor.DeletePendingSubaccount(pendingSubaccount.ParentAgentId, pendingSubaccount.Email);

            return subaccountResponse;
        }

        subaccountResponse.ActivationAttempts = pendingSubaccount.ActivationAttempts;

        if (!pendingSubaccount.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
        {
            if (pendingSubaccount.ActivationAttempts >= 3)
            {
                await SubaccountAccessor.DeletePendingSubaccount(pendingSubaccount.ParentAgentId, pendingSubaccount.Email);
            }

            return subaccountResponse;
        }

        var mappedSubaccount = MapDTOToPendingSubaccount(pendingSubaccount);

        if (mappedSubaccount is null)
        {
            return null;
        }

        subaccountResponse.Valid = true;
        subaccountResponse.Subaccount = mappedSubaccount;

        return subaccountResponse;
    }

    public async Task DeletePendingSubaccount(string agentId, string email)
    {
        await SubaccountAccessor.DeletePendingSubaccount(agentId, email);
    }

    public async Task DeletePendingSubaccount(string id)
    {
        await SubaccountAccessor.DeletePendingSubaccount(id);
    }

    public async Task<bool> DoesSubaccountExist(string agentId, string email)
    {
        return await SubaccountAccessor.DoesSubaccountExist(agentId, email);
    }

    public async Task<PendingSubaccount> CreateNewSubaccount(string agentId, string parentUsername, string email, List<Role> roles)
    {
        var mappedRoles = roles.Select(x => x.ToString());

        var subaccount = await SubaccountAccessor.CreateNewSubaccount(agentId, parentUsername, email, mappedRoles);

        return MapDTOToPendingSubaccount(subaccount);
    }

    public async Task UpdateSubaccount(string agentId, string email, List<Role> roles)
    {
        var mappedRoles = roles.Select(x => x.ToString());

        await SubaccountAccessor.UpdateSubaccount(agentId, email, mappedRoles);
    }

    public async Task ResendActivationEmail(string agentId, string email)
    {
        await SubaccountAccessor.ResendActivationEmail(agentId, email);
    }

    private static PendingSubaccount MapDTOToPendingSubaccount(PendingSubaccountDTO dto)
    {
        return new PendingSubaccount
        {
            Id = dto.Id,
            Email = dto.Email,
            AgentId = dto.ParentAgentId,
            ParentUsername = dto.ParentUsername,
            Roles = dto.Roles.Select(x => (Role)Enum.Parse(typeof(Role), x)).ToList(),
        };
    }
}
