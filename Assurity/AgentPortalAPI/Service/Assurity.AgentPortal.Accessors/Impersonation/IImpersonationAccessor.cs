namespace Assurity.AgentPortal.Accessors.Impersonation;

using Assurity.AgentPortal.Accessors.MongoDb.Contracts;

public interface IImpersonationAccessor
{
    Task<IEnumerable<UserSearch>> ExecuteAgentSearch(string keyword, CancellationToken cancellationToken);

    Task<UserSearch> GetUserSearchRecord(string userSearchId);

    Task InsertImpersonationLog(string homeOfficeUserId, string homeOfficeUserEmail, UserSearch impersonatedUser);

    Task<IEnumerable<UserSearch>> GetRecentImpersonations(string homeofficeUserId, CancellationToken cancellationToken);

    Task<List<ExcludedAgentId>> GetExcludedAgentIds();
}
