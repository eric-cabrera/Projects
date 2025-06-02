namespace Assurity.AgentPortal.Accessors.Tests;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Accessors.MongoDb.Contracts;
using Assurity.AgentPortal.Accessors.Subaccounts;
using Assurity.AgentPortal.Accessors.Tests.MongoDb;
using Assurity.AgentPortal.Utilities.Configs;
using Assurity.AgentPortal.Utilities.Emails;
using Bogus;
using global::MongoDB.Bson;
using global::MongoDB.Driver;
using MimeKit;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class PendingSubaccountAccessorTests : MongoDbTestContext
{
    [Fact]
    public async Task CreateNewSubaccount_ShouldSucceed()
    {
        // Arrange
        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);

        var mockEmailUtility = new Mock<IEmailUtility>();
        mockEmailUtility.Setup(x => x.CreateSubaccountActivationEmail(It.IsAny<string>(), It.IsAny<string>())).Returns(new MimeMessage());
        mockEmailUtility.Setup(x => x.SendEmail(It.IsAny<MimeMessage>())).Returns(Task.CompletedTask);

        var subaccountEmail = "test@example.com";
        var parentAgentId = "test-agent-id";
        var parentAgentUsername = "test-name";

        var agentRoles = new List<string>
        {
            "testRole1",
            "testRole2",
        };

        var accessor = new SubaccountAccessor(mockEmailUtility.Object, TempConnection);

        // Act
        var result = await accessor.CreateNewSubaccount(parentAgentId, parentAgentUsername, subaccountEmail, agentRoles);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subaccountEmail, result.Email);
        Assert.Equal(parentAgentId, result.ParentAgentId);
        Assert.Equal(parentAgentUsername, result.ParentUsername);
        Assert.NotNull(result.Id);
    }

    [Fact]
    public async Task GetPendingSubaccounts_ShouldSucceed()
    {
        // Arrange
        var agentId = "1234";
        var username = "user";

        SeedPendingSubaccounts(agentId, username);

        var accessor = new SubaccountAccessor(null, TempConnection);

        // Act
        var result = await accessor.GetPendingSubaccounts(agentId, CancellationToken.None);

        // Assert
        Assert.Equal(10, result.Count);
    }

    [Fact]
    public async Task RetriveAndActivateSubaccount_ShouldSucceed()
    {
        // Arrange
        var agentId = "1234";
        var username = "user";

        SeedPendingSubaccounts(agentId, username);

        var record = await TempConnection.PendingSubaccountsCollection.Find(
            Builders<PendingSubaccount>.Filter.Eq(x => x.ParentAgentId, agentId))
            .FirstAsync();

        var accessor = new SubaccountAccessor(null, TempConnection);

        // Act
        var result = await accessor.RetrieveAndActivateSubaccount(record.LinkGuid);

        // Assert
        Assert.Equal(1, result.ActivationAttempts);
    }

    [Fact]
    public async Task DeletePendingSubaccount_ShouldSucceed()
    {
        // Arrange
        var agentId = "1234";
        var username = "user";

        SeedPendingSubaccounts(agentId, username);

        var record = await TempConnection.PendingSubaccountsCollection.Find(
            Builders<PendingSubaccount>.Filter.Eq(x => x.ParentAgentId, agentId))
            .FirstAsync();

        var accessor = new SubaccountAccessor(null, TempConnection);

        // Act
        await accessor.DeletePendingSubaccount(agentId, record.Email);

        // Assert
        var deletedRecord = await TempConnection.PendingSubaccountsCollection.Find(
            Builders<PendingSubaccount>.Filter.Eq(x => x.Email, record.Email)).FirstOrDefaultAsync();

        Assert.Null(deletedRecord);
    }

    [Fact]
    public async Task UpdateSubaccount_ShouldSucceed()
    {
        // Arrange
        var agentId = "1234";
        var username = "user";
        var newRoles = new List<string>
        {
            "Role1",
            "Role2",
        };

        SeedPendingSubaccounts(agentId, username);

        var record = await TempConnection.PendingSubaccountsCollection.Find(
            Builders<PendingSubaccount>.Filter.Eq(x => x.ParentAgentId, agentId))
            .FirstAsync();

        var accessor = new SubaccountAccessor(null, TempConnection);

        // Act
        await accessor.UpdateSubaccount(agentId, record.Email, newRoles);

        // Assert
        var updatedRecord = await TempConnection.PendingSubaccountsCollection.Find(
            Builders<PendingSubaccount>.Filter.Eq(x => x.Email, record.Email)).FirstOrDefaultAsync();

        Assert.Equal(newRoles, updatedRecord.Roles);
    }

    [Fact]
    public async Task ResendActivationEmail_ShouldSucceed()
    {
        // Arrange
        var agentId = "1234";
        var username = "user";

        SeedPendingSubaccounts(agentId, username);

        var record = await TempConnection.PendingSubaccountsCollection.Find(
            Builders<PendingSubaccount>.Filter.Eq(x => x.ParentAgentId, agentId))
            .FirstAsync();

        var mockEmailUtility = new Mock<IEmailUtility>();
        mockEmailUtility.Setup(x => x.CreateSubaccountActivationEmail(It.IsAny<string>(), It.IsAny<string>())).Returns(new MimeMessage());
        mockEmailUtility.Setup(x => x.SendEmail(It.IsAny<MimeMessage>())).Returns(Task.CompletedTask);

        var accessor = new SubaccountAccessor(mockEmailUtility.Object, TempConnection);

        // Act
        await accessor.ResendActivationEmail(agentId, record.Email);

        // Assert
        var updatedRecord = await TempConnection.PendingSubaccountsCollection.Find(
            Builders<PendingSubaccount>.Filter.Eq(x => x.Email, record.Email)).FirstOrDefaultAsync();

        Assert.NotEqual(updatedRecord.LinkGuid, record.LinkGuid);
        Assert.True(updatedRecord.EmailSentAt.CompareTo(record.EmailSentAt) > 0);
    }

    private void SeedPendingSubaccounts(string parentAgentId, string parentUsername)
    {
        var pendingSubaccountFaker = new Faker<PendingSubaccount>()
            .RuleFor(x => x.Id, faker => ObjectId.GenerateNewId())
            .RuleFor(x => x.Email, faker => faker.Person.Email)
            .RuleFor(x => x.ParentAgentId, faker => parentAgentId)
            .RuleFor(x => x.ParentUsername, faker => parentUsername)
            .RuleFor(x => x.Roles, faker => faker.Make(4, () => faker.Random.AlphaNumeric(4)))
            .RuleFor(x => x.LinkGuid, faker => faker.Random.Guid())
            .RuleFor(x => x.ActivationAttempts, faker => 0)
            .RuleFor(x => x.EmailSentAt, faker => DateTime.Now.AddHours(-6));

        TempConnection.PendingSubaccountsCollection.InsertMany(pendingSubaccountFaker.Generate(10));
    }
}
