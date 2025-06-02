namespace Assurity.AgentPortal.Managers.Tests;

using Assurity.AgentPortal.Accessors.DTOs;
using Assurity.AgentPortal.Accessors.Subaccounts;
using Assurity.AgentPortal.Contracts.Enums;
using Assurity.AgentPortal.Contracts.Subaccounts;
using Assurity.AgentPortal.Managers.Subaccounts;
using Bogus;
using Moq;
using Xunit;

public class SubaccountManagerTests
{
    [Fact]
    public async Task GetPendingSubaccounts_ShouldSucceed()
    {
        // Arrange
        var mockAccessor = new Mock<ISubaccountAccessor>();
        mockAccessor.Setup(x => x.GetPendingSubaccounts(
            It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<PendingSubaccountDTO>());

        var manager = new SubaccountManager(mockAccessor.Object);

        // Act
        var result = await manager.GetPendingSubaccounts("abc123", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<PendingSubaccount>>(result);
    }

    [Fact]
    public async Task ActivateSubaccount_ShouldSucceed()
    {
        // Arrange
        var email = "abc@123.com";
        var linkGuid = Guid.NewGuid();

        var subaccountFaker = new Faker<PendingSubaccountDTO>()
            .RuleFor(x => x.Id, f => f.Random.Guid().ToString())
            .RuleFor(x => x.Email, f => email)
            .RuleFor(x => x.ParentAgentId, f => f.Random.AlphaNumeric(4))
            .RuleFor(x => x.ParentUsername, f => f.Person.UserName)
            .RuleFor(x => x.Roles, f => f.Make(2, _ => f.PickRandom<Role>().ToString()))
            .RuleFor(x => x.LinkGuid, f => linkGuid)
            .RuleFor(x => x.ActivationAttempts, f => 1)
            .RuleFor(x => x.EmailSentAt, f => f.Date.Between(DateTime.Now.AddHours(-23), DateTime.Now));

        var mockAccessor = new Mock<ISubaccountAccessor>();
        mockAccessor.Setup(x => x.RetrieveAndActivateSubaccount(
            It.IsAny<Guid>())).ReturnsAsync(subaccountFaker.Generate());

        var manager = new SubaccountManager(mockAccessor.Object);

        // Act
        var result = await manager.ActivateSubaccount(email, linkGuid);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Valid);
        Assert.NotNull(result.Subaccount);
    }

    [Fact]
    public async Task ActivateSubaccount_LinkExpired_ShouldNotSucceed()
    {
        // Arrange
        var email = "abc@123.com";
        var linkGuid = Guid.NewGuid();

        var subaccountFaker = new Faker<PendingSubaccountDTO>()
            .RuleFor(x => x.Id, f => f.Random.Guid().ToString())
            .RuleFor(x => x.Email, f => email)
            .RuleFor(x => x.ParentAgentId, f => f.Random.AlphaNumeric(4))
            .RuleFor(x => x.ParentUsername, f => f.Person.UserName)
            .RuleFor(x => x.Roles, f => f.Make(2, _ => f.PickRandom<Role>().ToString()))
            .RuleFor(x => x.LinkGuid, f => linkGuid)
            .RuleFor(x => x.ActivationAttempts, f => 1)
            .RuleFor(x => x.EmailSentAt, f => f.Date.Between(DateTime.Now.AddHours(-100), DateTime.Now.AddHours(-28)));

        var mockAccessor = new Mock<ISubaccountAccessor>(MockBehavior.Strict);
        mockAccessor.Setup(x => x.RetrieveAndActivateSubaccount(
            It.IsAny<Guid>())).ReturnsAsync(subaccountFaker.Generate());
        mockAccessor.Setup(x => x.DeletePendingSubaccount(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask).Verifiable();

        var manager = new SubaccountManager(mockAccessor.Object);

        // Act
        var result = await manager.ActivateSubaccount(email, linkGuid);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Valid);
    }

    [Fact]
    public async Task ActivateSubaccount_InvalidEmail_ShouldDeleteRecord()
    {
        // Arrange
        var email = "abc@123.com";
        var linkGuid = Guid.NewGuid();

        var subaccountFaker = new Faker<PendingSubaccountDTO>()
            .RuleFor(x => x.Id, f => f.Random.Guid().ToString())
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.ParentAgentId, f => f.Random.AlphaNumeric(4))
            .RuleFor(x => x.ParentUsername, f => f.Person.UserName)
            .RuleFor(x => x.Roles, f => f.Make(2, _ => f.PickRandom<Role>().ToString()))
            .RuleFor(x => x.LinkGuid, f => linkGuid)
            .RuleFor(x => x.ActivationAttempts, f => 3)
            .RuleFor(x => x.EmailSentAt, f => f.Date.Between(DateTime.Now.AddHours(-23), DateTime.Now));

        var mockAccessor = new Mock<ISubaccountAccessor>(MockBehavior.Strict);
        mockAccessor.Setup(x => x.RetrieveAndActivateSubaccount(
            It.IsAny<Guid>())).ReturnsAsync(subaccountFaker.Generate());
        mockAccessor.Setup(x => x.DeletePendingSubaccount(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask).Verifiable();

        var manager = new SubaccountManager(mockAccessor.Object);

        // Act
        var result = await manager.ActivateSubaccount(email, linkGuid);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Valid);

        mockAccessor.Verify(x => x.DeletePendingSubaccount(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ActivateSubaccount_InvalidEmail_ShouldNotDeleteRecord()
    {
        // Arrange
        var email = "abc@123.com";
        var linkGuid = Guid.NewGuid();

        var subaccountFaker = new Faker<PendingSubaccountDTO>()
            .RuleFor(x => x.Id, f => f.Random.Guid().ToString())
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.ParentAgentId, f => f.Random.AlphaNumeric(4))
            .RuleFor(x => x.ParentUsername, f => f.Person.UserName)
            .RuleFor(x => x.Roles, f => f.Make(2, _ => f.PickRandom<Role>().ToString()))
            .RuleFor(x => x.LinkGuid, f => linkGuid)
            .RuleFor(x => x.ActivationAttempts, f => 2)
            .RuleFor(x => x.EmailSentAt, f => f.Date.Between(DateTime.Now.AddHours(-23), DateTime.Now));

        var mockAccessor = new Mock<ISubaccountAccessor>(MockBehavior.Strict);
        mockAccessor.Setup(x => x.RetrieveAndActivateSubaccount(
            It.IsAny<Guid>())).ReturnsAsync(subaccountFaker.Generate());
        mockAccessor.Setup(x => x.DeletePendingSubaccount(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask).Verifiable();

        var manager = new SubaccountManager(mockAccessor.Object);

        // Act
        var result = await manager.ActivateSubaccount(email, linkGuid);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Valid);

        mockAccessor.Verify(x => x.DeletePendingSubaccount(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CreateNewSubaccount_ShouldSucceed()
    {
        // Arrange
        var email = "abc@123.com";
        var agentId = "1234";
        var username = "hello";
        var roles = new List<Role>
        {
            Role.TaxForms,
            Role.Contracting,
        };

        var subaccountFaker = new Faker<PendingSubaccountDTO>()
            .RuleFor(x => x.Id, f => f.Random.Guid().ToString())
            .RuleFor(x => x.Email, f => email)
            .RuleFor(x => x.ParentAgentId, f => agentId)
            .RuleFor(x => x.ParentUsername, f => username)
            .RuleFor(x => x.Roles, f => f.Make(2, _ => f.PickRandom<Role>().ToString()))
            .RuleFor(x => x.LinkGuid, f => f.Random.Guid())
            .RuleFor(x => x.ActivationAttempts, f => 2)
            .RuleFor(x => x.EmailSentAt, f => DateTime.Now);

        var subaccount = subaccountFaker.Generate();

        var accessor = new Mock<ISubaccountAccessor>(MockBehavior.Strict);
        accessor.Setup(x => x.CreateNewSubaccount(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(subaccount);

        var manager = new SubaccountManager(accessor.Object);

        // Act
        var result = await manager.CreateNewSubaccount(agentId, username, email, roles);

        // Assert
        Assert.NotNull(result.Id);
        Assert.Equal(subaccount.ParentAgentId, result.AgentId);
        Assert.Equal(subaccount.Email, result.Email);
    }
}
