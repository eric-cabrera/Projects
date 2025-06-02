namespace Assurity.AgentPortal.Managers.UserData;

using Assurity.AgentPortal.Contracts.AgentContracts;
using Assurity.AgentPortal.Contracts.UserData;

public interface IUserDataManager
{
    Task<AccessLevel?> GetAgentAccess(string agentId, CancellationToken cancellationToken = default);

    Task<HashSet<Market>> GetBusinessTypesByAgentId(string agentId, CancellationToken cancellationToken);

    Task SendEmailNotifications(string originalEmail, string newEmail);

    Task<List<string>?> GetAdditionalAgentIds(string agentId, CancellationToken cancellationToken);
}
