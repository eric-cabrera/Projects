namespace Assurity.AgentPortal.Managers;

using Assurity.AgentPortal.Contracts;
using Assurity.AgentPortal.Contracts.Directus;

public interface IDirectusManager
{
    Task<CommissionDatesSummary?> GetAgentCenterCommissionDates();

    Task<List<Contract>?> GetContracts(string marketCode, List<string> agentLevels, string accessToken, string agentId);

    Task<byte[]?> GetDirectusFile(string id);

    Task<List<TemporaryMessage>?> GetTemporaryMessages();

    Task<DirectusPageResponse?> GetDirectusPageContent(string slug, bool loggedIn);

    Task<ContactsResponse> GetContacts();
}