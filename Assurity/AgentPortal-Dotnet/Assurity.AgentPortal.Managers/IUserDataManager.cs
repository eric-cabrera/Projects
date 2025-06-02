namespace Assurity.AgentPortal.Managers;

using Assurity.AgentPortal.Contracts;

public interface IUserDataManager
{
    Task<List<string>> GetProductionCreditBusinessTypes(string accessToken, string? agentId);
}