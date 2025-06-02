namespace Assurity.AgentPortal.MongoDB;

using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Assurity.AgentPortal.Accessors.Impersonation;
using Assurity.AgentPortal.Accessors.MongoDb.Contracts;
using Assurity.AgentPortal.Utilities.Configs;
using global::MongoDB.Driver;

public class MongoDBConnection
{
    public MongoDBConnection(IConfigurationManager configuration, bool createCollections = true)
    {
        Config = configuration;
        if (createCollections)
        {
            CreateCollections();
        }
    }

    public IConfigurationManager Config { get; }

    public IMongoCollection<BenefitOptionsMapping> BenefitOptionsCollection { get; set; }

    public IMongoCollection<PendingSubaccount> PendingSubaccountsCollection { get; set; }

    public IMongoCollection<ImpersonationLog> ImpersonationLogCollection { get; set; }

    public IMongoCollection<UserSearch> UserSearchCollection { get; set; }

    public IMongoCollection<ExcludedAgentId> ExcludedAgentIdsCollection { get; set; }

    public MongoClientSettings GetMongoClientSettings()
    {
        var connString = Config.MongoDbConnectionString;

        var clientSettings = MongoClientSettings.FromConnectionString(connString);

        if (Config.MongoDbUseTls == "true")
        {
            clientSettings.UseTls = true;
            SetSslSettings(clientSettings);
        }

        return clientSettings;
    }

    private static void SetSslSettings(MongoClientSettings mongoSettings)
    {
        var clientCerts = new List<X509Certificate2>();
        using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine, OpenFlags.ReadOnly))
        {
            // Add all certificates from Personal certificate store. MongoDB selects the right one based on the database server
            clientCerts.AddRange(store.Certificates.OfType<X509Certificate2>());
        }

        mongoSettings.SslSettings = new SslSettings
        {
            CheckCertificateRevocation = false,
            EnabledSslProtocols = SslProtocols.Tls12,
            ClientCertificates = clientCerts
        };
    }

    private void CreateCollections()
    {
        var dbClient = GetMongoClient();
        var eventsDatabase = dbClient.GetDatabase(Config.MongoDbEventsDatabaseName);
        var agentCenterDatabase = dbClient.GetDatabase(Config.MongoDbAgentCenterDatabaseName);

        BenefitOptionsCollection = eventsDatabase.GetCollection<BenefitOptionsMapping>(Config.MongoDbBenefitOptionsCollection);
        PendingSubaccountsCollection = agentCenterDatabase.GetCollection<PendingSubaccount>(Config.MongoDbPendingSubaccountsCollection);
        ImpersonationLogCollection = agentCenterDatabase.GetCollection<ImpersonationLog>(Config.MongoDbImpersonationLogCollection);
        UserSearchCollection = agentCenterDatabase.GetCollection<UserSearch>(Config.MongoDbUserSearchCollection);
        ExcludedAgentIdsCollection = agentCenterDatabase.GetCollection<ExcludedAgentId>(Config.ExcludedAgentIdsCollection);

        var impersonationLogIndex = new CreateIndexModel<ImpersonationLog>(
            Builders<ImpersonationLog>.IndexKeys.Ascending(x => x.HomeOfficeUserId));

        ImpersonationLogCollection.Indexes.CreateOne(impersonationLogIndex);

        var uniqueIndexOptions = new CreateIndexOptions
        {
            Unique = true,
        };

        var pendingSubaccountsIndexes = new List<CreateIndexModel<PendingSubaccount>>
        {
            new(Builders<PendingSubaccount>.IndexKeys.Ascending(x => x.Email), uniqueIndexOptions),
            new(Builders<PendingSubaccount>.IndexKeys.Ascending(x => x.ParentAgentId).Ascending(x => x.Email)),
            new(Builders<PendingSubaccount>.IndexKeys.Ascending(x => x.LinkGuid), uniqueIndexOptions),
        };

        PendingSubaccountsCollection.Indexes.CreateMany(pendingSubaccountsIndexes);
    }

    private MongoClient GetMongoClient()
    {
        var clientSettings = GetMongoClientSettings();
        return new MongoClient(clientSettings);
    }
}
