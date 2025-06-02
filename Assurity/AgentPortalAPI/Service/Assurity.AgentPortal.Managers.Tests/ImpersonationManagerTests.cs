namespace Assurity.AgentPortal.Managers.Tests;

using Assurity.AgentPortal.Accessors.Impersonation;
using Assurity.AgentPortal.Accessors.MongoDb.Contracts;
using Assurity.AgentPortal.Contracts.Impersonation;
using Assurity.AgentPortal.Managers.Impersonation;
using Bogus;
using global::MongoDB.Bson;
using Moq;
using Xunit;

public class ImpersonationManagerTests
{
    [Fact]
    public async Task SearchAgents_ShouldReturnMappedResults()
    {
        // Arrange
        var mockResult = GetMockUserSearchList();
        var mockAccessor = new Mock<IImpersonationAccessor>();
        mockAccessor.Setup(x => x.ExecuteAgentSearch(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResult);

        var manager = new ImpersonationManager(mockAccessor.Object);

        // Act
        var results = await manager.SearchAgents("abc123", CancellationToken.None);

        // Assert
        Assert.NotNull(results);

        foreach (var result in results)
        {
            var index = results.IndexOf(result);
            var expectedAgents = mockResult[index].Agents.GroupBy(
                x => x.Name,
                (name, agent) => new AgentRecord
                {
                    Name = name,
                    AgentIds = [.. agent.Select(x => x.AgentId)],
                });
            Assert.Equal(result.Id, mockResult[index].Id);
            Assert.Equal(result.UserName, mockResult[index].UserName);
            Assert.Equal(result.Email, mockResult[index].Email);
            Assert.Equal(result.Name, mockResult[index].Name);
            Assert.Equal(result.RegisteredAgentId, mockResult[index].RegisteredAgentId);
            Assert.Equivalent(result.Agents, expectedAgents);
        }
    }

    [Fact]
    public async Task ImpersonateAgent_Success()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var userSearchData = GetMockUserSearchList();
        var excludedAgentIds = new List<ExcludedAgentId>
        {
            new ExcludedAgentId { AgentId = "00001" },
            new ExcludedAgentId { AgentId = "01491" }
        };
        var testImpersonationData = new List<ImpersonationLog>();

        var mockAccessor = new Mock<IImpersonationAccessor>();
        mockAccessor.Setup(x => x.GetUserSearchRecord(It.IsAny<string>()))
            .ReturnsAsync(userSearchData.First());
        mockAccessor.Setup(x => x.GetExcludedAgentIds())
            .ReturnsAsync(excludedAgentIds);
        mockAccessor.Setup(x => x.InsertImpersonationLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserSearch>()))
            .Verifiable();

        var impersonationManager = new ImpersonationManager(mockAccessor.Object);

        // Act
        var result = await impersonationManager.ImpersonateAgent(userId, "test@test.com", userSearchData.First().Id);

        // Assert
        var expectedImpersonationRecord = MapImpersonationRecord(userSearchData.First());
        Assert.NotNull(result);
        Assert.Equivalent(expectedImpersonationRecord, result);

        mockAccessor.Verify(
            accessor => accessor.InsertImpersonationLog(userId, "test@test.com", userSearchData.First()),
            Times.Once);
    }

    [Fact]
    public async Task GetRecentImpersonations_Success()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();

        var expectedRecords = GetMockImpersonationLogs(userId);

        var mockAccessor = new Mock<IImpersonationAccessor>();
        mockAccessor.Setup(x => x.GetRecentImpersonations(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedRecords);

        var impersonationManager = new ImpersonationManager(mockAccessor.Object);

        // Act
        var result = await impersonationManager.GetRecentImpersonations(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedRecords.Count, result.Count);
    }

    private static List<UserSearch> GetMockUserSearchList()
    {
        var agentFaker = new Faker<Agent>()
            .RuleFor(x => x.AgentId, f => f.Random.AlphaNumeric(4))
            .RuleFor(x => x.Name, f => f.Person.FullName);
        var userSearchFaker = new Faker<UserSearch>()
            .RuleFor(x => x.Id, f => ObjectId.GenerateNewId().ToString())
            .RuleFor(x => x.PingUserId, f => Guid.NewGuid().ToString())
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.RegisteredAgentId, f => f.Random.String(4))
            .RuleFor(x => x.Agents, f => f.Make(f.Random.Int(1, 10), () => agentFaker.Generate()))
            .RuleFor(x => x.UserName, f => f.Person.UserName);

        return userSearchFaker.Generate(20);
    }

    private static List<UserSearch> GetMockImpersonationLogs(string userId)
    {
        var impersonationLogFaker = new Faker<ImpersonationLog>()
            .RuleFor(x => x.HomeOfficeUserId, f => userId)
            .RuleFor(x => x.HomeOfficeUserEmail, f => "test@test.com")
            .RuleFor(x => x.ImpersonatedUser, f => f.PickRandom(GetMockUserSearchList()));

        return impersonationLogFaker.Generate(20).Select(x => x.ImpersonatedUser).ToList();
    }

    private static ImpersonationRecord MapImpersonationRecord(UserSearch userSearch)
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
            UserName = userSearch.UserName,
            Email = userSearch.Email,
            Name = userSearch.Name,
            RegisteredAgentId = userSearch.RegisteredAgentId,
            Agents = [.. groupedAgents],
        };
    }
}
