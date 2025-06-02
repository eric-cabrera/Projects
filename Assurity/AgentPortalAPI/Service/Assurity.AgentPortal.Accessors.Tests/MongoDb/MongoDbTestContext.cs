namespace Assurity.AgentPortal.Accessors.Tests.MongoDb;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Assurity.AgentPortal.Accessors.Impersonation;
using Assurity.AgentPortal.Accessors.MongoDb.Contracts;
using Assurity.AgentPortal.MongoDB;
using Assurity.Common.Cryptography;
using Assurity.PolicyInfo.Contracts.V1.Enums;
using Bogus;
using global::MongoDB.Bson;
using global::MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32.SafeHandles;
using Mongo2Go;
using Moq;

[ExcludeFromCodeCoverage]
public class MongoDbTestContext : IDisposable
{
    protected const string HOMEOFFICETESTID = "11e88561-be9d-45f3-971d-9a6320d81d69";

    // Instantiate a SafeHandle instance.
    private readonly SafeHandle safeHandle = new SafeFileHandle(IntPtr.Zero, true);

    // To detect redundant calls
    private bool disposed;

    public MongoDbTestContext()
    {
        MongoRunner = MongoDbRunner.Start();

        var inMemorySettings = new Dictionary<string, string>
        {
            { "Environment", "LOCAL" },
            { "MongoDb:ConnectionString", MongoRunner.ConnectionString },
            { "MongoDb:EventsDatabaseName", "Events" },
            { "MongoDb:AgentCenterDatabaseName", "AgentCenter" },
            { "MongoDb:BenefitOptionsCollection", "BenefitOptionsMapping" },
            { "MongoDb:PendingSubaccountsCollection", "PendingSubaccounts" },
            { "MongoDb:ImpersonationLogCollection", "ImpersonationLog" },
            { "MongoDb:UserSearchCollection", "UserSearch" },
            { "MongoDb:ExcludedAgentIdsCollection", "ExcludedAgentIds" },
            { "MongoDB:UseTls", "false" },
        };

        var mockAesEncryptor = new Mock<IAesEncryptor>(MockBehavior.Strict);
        IConfiguration configuration = new ConfigurationBuilder()
                                      .AddInMemoryCollection(inMemorySettings)
                                      .Build();

        Config = new Utilities.Configs.ConfigurationManager(configuration, mockAesEncryptor.Object);

        var client = new MongoClient(MongoRunner.ConnectionString);
        var eventsDatabase = client.GetDatabase(Config.MongoDbEventsDatabaseName);
        var agentCenterDatabase = client.GetDatabase(Config.MongoDbAgentCenterDatabaseName);

        TempConnection = new MongoDBConnection(Config);
        TempConnection.BenefitOptionsCollection = eventsDatabase.GetCollection<BenefitOptionsMapping>(Config.MongoDbBenefitOptionsCollection);
        TempConnection.PendingSubaccountsCollection = agentCenterDatabase.GetCollection<PendingSubaccount>(Config.MongoDbPendingSubaccountsCollection);
        TempConnection.ImpersonationLogCollection = agentCenterDatabase.GetCollection<ImpersonationLog>(Config.MongoDbImpersonationLogCollection);
        TempConnection.UserSearchCollection = agentCenterDatabase.GetCollection<UserSearch>(Config.MongoDbUserSearchCollection);
        TempConnection.ExcludedAgentIdsCollection = agentCenterDatabase.GetCollection<ExcludedAgentId>(Config.ExcludedAgentIdsCollection);

        SeedData();
    }

    internal Utilities.Configs.ConfigurationManager Config { get; set; }

    internal MongoDbRunner MongoRunner { get; set; } // In memory mongo DB for unit testing

    internal MongoDBConnection TempConnection { get; set; }

    public void Dispose()
    {
        // Dispose of unmanaged resources.
        Dispose(true);

        // Suppress finalization.
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                safeHandle.Dispose();
            }
        }

        MongoRunner.Dispose();
        disposed = true;
    }

    private void SeedData()
    {
        SeedBenefitOptionsMappingData();
        SeedPendingSubaccountData();
        SeedUserSearchData();
        SeedStaticImpersonationLogData();
    }

    private void SeedBenefitOptionsMappingData()
    {
        var benefitOptionMapping = new List<BenefitOptionsMapping>
        {
            new BenefitOptionsMapping
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Category = BenefitOptionName.AccidentalDeathAndDismemberment,
                Option = BenefitOptionValue.OneHundredDollars,
                HideBenefitOption = false,
            },
            new BenefitOptionsMapping
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Category = BenefitOptionName.Benefit,
                Option = BenefitOptionValue.Combined,
                HideBenefitOption = false,
            },
            new BenefitOptionsMapping
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Category = BenefitOptionName.FlatAmount,
                Option = BenefitOptionValue.HomeOfficeNonTobacco,
                HideBenefitOption = true,
            },
            new BenefitOptionsMapping
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Category = BenefitOptionName.OccupationClass,
                Option = BenefitOptionValue.Juvenile,
                HideBenefitOption = true,
            },
        };

        TempConnection.BenefitOptionsCollection.InsertMany(benefitOptionMapping);
    }

    private void SeedPendingSubaccountData()
    {
        var pendingSubaccountFaker = new Faker<PendingSubaccount>()
            .RuleFor(x => x.Id, faker => ObjectId.GenerateNewId())
            .RuleFor(x => x.Email, faker => faker.Person.Email)
            .RuleFor(x => x.ParentAgentId, faker => faker.Random.AlphaNumeric(4))
            .RuleFor(x => x.ParentUsername, faker => faker.Person.UserName)
            .RuleFor(x => x.Roles, faker => faker.Make(4, () => faker.Random.AlphaNumeric(4)))
            .RuleFor(x => x.LinkGuid, faker => faker.Random.Guid())
            .RuleFor(x => x.ActivationAttempts, faker => 0)
            .RuleFor(x => x.EmailSentAt, faker => DateTime.Now.AddHours(-6));

        TempConnection.PendingSubaccountsCollection.InsertMany(pendingSubaccountFaker.Generate(10));
    }

    private void SeedUserSearchData()
    {
        var agentFaker = new Faker<Agent>()
            .RuleFor(x => x.AgentId, f => f.Random.AlphaNumeric(4))
            .RuleFor(x => x.Name, f => f.Person.FullName);

        var userSearchFaker = new Faker<UserSearch>()
            .RuleFor(x => x.Id, faker => ObjectId.GenerateNewId().ToString())
            .RuleFor(x => x.PingUserId, faker => Guid.NewGuid().ToString())
            .RuleFor(x => x.UserName, f => f.Person.UserName)
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.Name, f => $"{f.Name.FirstName()} {f.Name.LastName()}")
            .RuleFor(x => x.Agents, f => f.Make(f.Random.Int(1, 5), _ => agentFaker.Generate()))
            .RuleFor(x => x.RegisteredAgentId, (f, x) => f.PickRandom(x.Agents).AgentId);

        TempConnection.UserSearchCollection.InsertMany(userSearchFaker.Generate(100));

        SeedImpersonationLogData();
    }

    private void SeedImpersonationLogData()
    {
        var userSearchRecords = TempConnection.UserSearchCollection.AsQueryable()
            .Take(20)
            .ToList();

        var userId = Guid.NewGuid().ToString();
        var impersonationLogFaker = new Faker<ImpersonationLog>()
            .RuleFor(x => x.HomeOfficeUserId, f => userId)
            .RuleFor(x => x.HomeOfficeUserEmail, f => "test@test.com")
            .RuleFor(x => x.ImpersonatedUser, f => f.PickRandom(userSearchRecords));

        TempConnection.ImpersonationLogCollection.InsertMany(impersonationLogFaker.Generate(30));
    }

    private void SeedStaticImpersonationLogData()
    {
        var userSearchRecords = new List<UserSearch>()
        {
            new UserSearch
            {
                Id = "123456789012345678901230",
                PingUserId = "PingUserId1234",
                UserName = "Test",
                Email = "Test",
                Name = "Test",
                RegisteredAgentId = "testagentId123",
                Agents = new List<Agent>
                {
                    new Agent
                    {
                        Name = "test1",
                        AgentId = "123"
                    },
                    new Agent
                    {
                        Name = "test2",
                        AgentId = "456"
                    },
                    new Agent
                    {
                        Name = "test3",
                        AgentId = "789"
                    },
                },
                LastSynced = DateTime.Now,
            },
            new UserSearch
            {
                Id = "123456789012345678901231",
                PingUserId = "PingUserId4321",
                UserName = "Test2",
                Email = "Test2",
                Name = "Test2",
                RegisteredAgentId = "testagentId321",
                Agents = new List<Agent>
                {
                    new Agent
                    {
                        Name = "test1",
                        AgentId = "123"
                    },
                    new Agent
                    {
                        Name = "test2",
                        AgentId = "456"
                    },
                    new Agent
                    {
                        Name = "test3",
                        AgentId = "789"
                    },
                },
                LastSynced = DateTime.Now,
            },
        };

        TempConnection.UserSearchCollection.InsertMany(userSearchRecords);

        var impersonationLogRecords = new List<ImpersonationLog>
        {
            new ImpersonationLog
            {
                HomeOfficeUserId = HOMEOFFICETESTID,
                HomeOfficeUserEmail = "HOTest",
                ImpersonatedUser = userSearchRecords[0]
            },
            new ImpersonationLog
            {
                HomeOfficeUserId = HOMEOFFICETESTID,
                HomeOfficeUserEmail = "HOTest",
                ImpersonatedUser = userSearchRecords[1]
            },
            new ImpersonationLog
            {
                HomeOfficeUserId = HOMEOFFICETESTID,
                HomeOfficeUserEmail = "HOTest",
                ImpersonatedUser = userSearchRecords[0]
            },
            new ImpersonationLog
            {
                HomeOfficeUserId = HOMEOFFICETESTID,
                HomeOfficeUserEmail = "HOTest",
                ImpersonatedUser = userSearchRecords[1]
            }
        };

        TempConnection.ImpersonationLogCollection.InsertMany(impersonationLogRecords);
    }
}
