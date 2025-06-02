namespace Assurity.AgentPortal.Accessors.Subaccounts;

using Assurity.AgentPortal.Accessors.DTOs;

public interface ISubaccountAccessor
{
    Task<List<PendingSubaccountDTO>> GetPendingSubaccounts(string agentId, CancellationToken cancellationToken);

    Task<PendingSubaccountDTO?> RetrieveAndActivateSubaccount(Guid linkGuid);

    Task DeletePendingSubaccount(string agentId, string email);

    Task DeletePendingSubaccount(string id);

    Task<bool> DoesSubaccountExist(string agentId, string email);

    Task<PendingSubaccountDTO> CreateNewSubaccount(string subaccountEmail, string parentAgentId, string parentUsername, IEnumerable<string> roles);

    Task UpdateSubaccount(string agentId, string email, IEnumerable<string> roles);

    Task ResendActivationEmail(string agentId, string email);
}
