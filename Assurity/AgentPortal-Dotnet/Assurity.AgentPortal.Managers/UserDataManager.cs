namespace Assurity.AgentPortal.Managers;

using Assurity.AgentPortal.Accessors;
using Assurity.AgentPortal.Contracts;

public class UserDataManager : IUserDataManager
{
    public UserDataManager(IUserDataAccessor userDataAccessor)
    {
        UserDataAccessor = userDataAccessor;
    }

    private IUserDataAccessor UserDataAccessor { get; set; }

    public async Task<List<string>> GetProductionCreditBusinessTypes(string accessToken, string? agentId)
    {
        // Default to Individual if we can't retrieve their market business types
        var defaultBusinessTypes = new List<Market> { Market.Individual };

        var returnedBusinessTypes = await UserDataAccessor.GetProductionCreditBusinessTypes(accessToken, agentId);

        var businessTypes = returnedBusinessTypes == null || returnedBusinessTypes.Count == 0 ? defaultBusinessTypes : returnedBusinessTypes;

        return (from businessType in businessTypes
                select businessType.ToString("G")).ToList();
    }
}
