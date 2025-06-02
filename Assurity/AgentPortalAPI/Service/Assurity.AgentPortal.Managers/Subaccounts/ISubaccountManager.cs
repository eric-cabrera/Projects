namespace Assurity.AgentPortal.Managers.Subaccounts;

using Assurity.AgentPortal.Contracts.Enums;
using Assurity.AgentPortal.Contracts.Subaccounts;

public interface ISubaccountManager
{
    Task<List<PendingSubaccount>> GetPendingSubaccounts(string agentId, CancellationToken cancellationToken);

    Task<PendingSubaccountActivationResponse?> ActivateSubaccount(string email, Guid linkGuid);

    Task DeletePendingSubaccount(string id);

    Task DeletePendingSubaccount(string agentId, string email);

    Task<bool> DoesSubaccountExist(string agentId, string email);

    Task<PendingSubaccount> CreateNewSubaccount(string agentId, string parentUsername, string email, List<Role> roles);

    Task UpdateSubaccount(string agentId, string email, List<Role> roles);

    Task ResendActivationEmail(string agentId, string email);
}
