namespace Assurity.AgentPortal.Managers.Impersonation;

using System.Linq;
using Assurity.AgentPortal.Accessors.Impersonation;
using Assurity.AgentPortal.Contracts.Impersonation;

public class ImpersonationManager : IImpersonationManager
{
    public ImpersonationManager(IImpersonationAccessor impersonationAccessor)
    {
        ImpersonationAccessor = impersonationAccessor;
    }

    private IImpersonationAccessor ImpersonationAccessor { get; }

    public async Task<List<ImpersonationRecord>> SearchAgents(string searchTerm, CancellationToken cancellationToken)
    {
        var records = await ImpersonationAccessor.ExecuteAgentSearch(searchTerm, cancellationToken);

        if (records is null)
        {
            return new List<ImpersonationRecord>();
        }

        return [.. records.Select(x => MapAgentResult(x))];
    }

    public async Task<ImpersonationRecord> ImpersonateAgent(string homeOfficeId, string homeOfficeEmail, string impersonationId)
    {
        var userSearchRecord = await ImpersonationAccessor.GetUserSearchRecord(impersonationId);
        var excludedAgentIds = (await ImpersonationAccessor.GetExcludedAgentIds()).Select(excluded => excluded.AgentId).ToHashSet();

        if (userSearchRecord == null)
        {
            throw new ArgumentNullException("The search did not return any results for the specified impersonation ID.");
        }

        if (userSearchRecord.Agents != null && userSearchRecord.Agents.Any(x => excludedAgentIds.Contains(x.AgentId)))
        {
            throw new UnauthorizedAccessException("Impersonation is prohibited due to the presence of agent IDs that are not allowed.");
        }

        await ImpersonationAccessor.InsertImpersonationLog(homeOfficeId, homeOfficeEmail, userSearchRecord);

        return MapAgentResult(userSearchRecord);
    }

    public async Task<List<ImpersonationRecord>> GetRecentImpersonations(string homeOfficeId, CancellationToken cancellationToken)
    {
        var recentLogs = await ImpersonationAccessor.GetRecentImpersonations(homeOfficeId, cancellationToken);

        var mappedUsers = recentLogs.Select(x => MapAgentResult(x)).ToList();

        return mappedUsers;
    }

    private static ImpersonationRecord MapAgentResult(UserSearch userSearch)
    {
        var groupedAgents = userSearch.Agents.GroupBy(
            x => x.Name,
            (name, agent) => new AgentRecord
            {
                Name = name,
                AgentIds = [.. agent.Select(x => x.AgentId)],
            });

        return new ImpersonationRecord
        {
            Id = userSearch.Id,
            Name = userSearch.Name,
            UserName = userSearch.UserName,
            Email = userSearch.Email,
            RegisteredAgentId = userSearch.RegisteredAgentId,
            Agents = [.. groupedAgents],
        };
    }
}
