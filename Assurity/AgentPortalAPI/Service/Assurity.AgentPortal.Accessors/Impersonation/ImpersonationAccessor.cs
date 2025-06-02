namespace Assurity.AgentPortal.Accessors.Impersonation;

using Assurity.AgentPortal.Accessors.MongoDb.Contracts;
using Assurity.AgentPortal.MongoDB;
using Assurity.AgentPortal.Utilities.Configs;
using Elastic.Clients.Elasticsearch;
using global::MongoDB.Bson;
using global::MongoDB.Driver;
using global::MongoDB.Driver.Linq;
using Microsoft.Extensions.Logging;

public class ImpersonationAccessor : IImpersonationAccessor
{
    // Elasticsearch fields
    private const string AgentIdPrefixField = "Agents.AgentId.prefix";
    private const string AgentNamePrefixField = "Agents.Name.prefix";
    private const string UsernamePrefixField = "UserName.prefix";
    private const string EmailPrefixField = "Email.prefix";
    private const string NamePrefixField = "Name.prefix";

    public ImpersonationAccessor(
        ILogger<ImpersonationAccessor> logger,
        IConfigurationManager configurationManager,
        ElasticsearchClient client,
        MongoDBConnection mongoDbConnection)
    {
        Logger = logger;
        ConfigurationManager = configurationManager;
        Client = client;
        MongoDbConnection = mongoDbConnection;
    }

    private ILogger<ImpersonationAccessor> Logger { get; }

    private IConfigurationManager ConfigurationManager { get; }

    private ElasticsearchClient Client { get; }

    private MongoDBConnection MongoDbConnection { get; }

    public async Task<IEnumerable<UserSearch>> ExecuteAgentSearch(string keyword, CancellationToken cancellationToken)
    {
        var response = await Client.SearchAsync<UserSearch>(
            search => search
            .Index(ConfigurationManager.ElasticSearchUserSearchIndex)
            .Query(query => query
                .Bool(x => x
                    .Should(
                        s1 => s1
                        .Match(p => p
                            .Field(AgentIdPrefixField!)
                            .Query(keyword)),
                        s2 => s2
                        .Match(p => p
                            .Field(AgentNamePrefixField!)
                            .Query(keyword)),
                        s3 => s3
                        .Match(p => p
                            .Field(UsernamePrefixField!)
                            .Query(keyword)),
                        s4 => s4
                        .Match(p => p
                            .Field(EmailPrefixField!)
                            .Query(keyword)),
                        s5 => s5
                        .Match(p => p
                            .Field(NamePrefixField!)
                            .Query(keyword)))))
            .Size(20),
            cancellationToken);

        Logger.LogDebug(response.DebugInformation);

        if (response is not null)
        {
            return response.Documents ?? [];
        }

        return [];
    }

    public async Task<UserSearch> GetUserSearchRecord(string userSearchId)
    {
        var record = await MongoDbConnection.UserSearchCollection
            .AsQueryable()
            .Where(x => x.Id.Equals(userSearchId))
            .FirstOrDefaultAsync();

        return record;
    }

    public async Task<List<ExcludedAgentId>> GetExcludedAgentIds()
    {
        return await MongoDbConnection.ExcludedAgentIdsCollection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task InsertImpersonationLog(string homeOfficeUserId, string homeOfficeUserEmail, UserSearch impersonatedUser)
    {
        var impersonationDoc = new ImpersonationLog
        {
            HomeOfficeUserId = homeOfficeUserId,
            HomeOfficeUserEmail = homeOfficeUserEmail,
            ImpersonatedUser = impersonatedUser,
        };

        await MongoDbConnection.ImpersonationLogCollection.InsertOneAsync(impersonationDoc);
    }

    public async Task<IEnumerable<UserSearch>> GetRecentImpersonations(string homeofficeUserId, CancellationToken cancellationToken)
    {
        var recentUsers = await MongoDbConnection.ImpersonationLogCollection
            .AsQueryable()
            .Where(x => x.HomeOfficeUserId == homeofficeUserId)
            .OrderByDescending(x => x.Id)
            .Take(200)
            .ToListAsync(cancellationToken);

        return recentUsers
            .GroupBy(x => new { UserId = x.ImpersonatedUser.Id })
            .Select(x => x.First())
            .Take(20)
            .Select(x => new UserSearch
            {
                Id = x.ImpersonatedUser.Id,
                PingUserId = x.ImpersonatedUser.PingUserId,
                UserName = x.ImpersonatedUser.UserName,
                Email = x.ImpersonatedUser.Email,
                Name = x.ImpersonatedUser.Name,
                RegisteredAgentId = x.ImpersonatedUser.RegisteredAgentId,
                Agents = x.ImpersonatedUser.Agents,
            });
    }
}
