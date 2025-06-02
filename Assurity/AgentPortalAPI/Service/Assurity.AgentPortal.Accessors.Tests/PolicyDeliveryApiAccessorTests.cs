namespace Assurity.AgentPortal.Accessors.Tests;

using System.Threading;
using System.Threading.Tasks;
using Assurity.AgentPortal.Accessors.AssureLink.Context;
using Assurity.AgentPortal.Accessors.AssureLink.Entities;
using Assurity.AgentPortal.Accessors.EConsent;
using Assurity.AgentPortal.Contracts.PolicyDelivery.Request;
using Assurity.AgentPortal.Utilities.Configs;
using Assurity.AgentPortal.Utilities.Encryption;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class PolicyDeliveryApiAccessorTests
{
    private readonly Mock<HttpClient> mockHttpClient;
    private readonly Mock<ILogger<PolicyDeliveryApiAccessor>> mockLogger;
    private readonly Mock<IDbContextFactory<AssureLinkContext>> mockDbContextFactory;
    private readonly Mock<IEncryption> mockEncryption;
    private readonly Mock<IConfigurationManager> mockConfigurationManager;
    private readonly Mock<AssureLinkContext> mockAssureLinkContext;
    private readonly PolicyDeliveryApiAccessor policyDeliveryApiAccessor;

    public PolicyDeliveryApiAccessorTests()
    {
        mockDbContextFactory = new Mock<IDbContextFactory<AssureLinkContext>>();
        mockEncryption = new Mock<IEncryption>();
        mockConfigurationManager = new Mock<IConfigurationManager>();
        mockAssureLinkContext = new Mock<AssureLinkContext>();

        mockDbContextFactory
            .Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockAssureLinkContext.Object);

        policyDeliveryApiAccessor = new PolicyDeliveryApiAccessor(
            mockDbContextFactory.Object,
            mockEncryption.Object,
            mockConfigurationManager.Object);
    }

    [Fact]
    public async Task UpdateAgentPolicyDeliveryOptionsShouldUpdateCorrectly()
    {
        // Arrange
        var documentConnectOptions = new DocumentConnectOptions
        {
            ViewAsAgentNumber = "12345",
            ViewAsMarketCode = "MARKET1",
            OptOutForEDelivery = false,
            AgentLinkSelected = true,
            IncludeDownline = false,
            Email = "test@domain.com",
            AccessCode = "secretcode"
        };

        var optionType = "SomeOptionType";

        var agentOptions = new AgentOptions
        {
            AgentId = documentConnectOptions.ViewAsAgentNumber,
            MarketCode = documentConnectOptions.ViewAsMarketCode,
            OptionType = optionType,
            OptOutForEDelivery = documentConnectOptions.OptOutForEDelivery,
            AgentLinkSelected = documentConnectOptions.AgentLinkSelected,
            IncludeDownline = documentConnectOptions.IncludeDownline,
        };

        var agentLinkOptions = new AgentLinkOptions
        {
            AgentId = documentConnectOptions.ViewAsAgentNumber,
            MarketCode = documentConnectOptions.ViewAsMarketCode,
            OptionType = optionType,
            Email = documentConnectOptions.Email,
            AccessCode = "encryptedAccessCode",
        };

        var mockAgentOptionsDbSet = new Mock<DbSet<AgentOptions>>();
        var mockAgentLinkOptionsDbSet = new Mock<DbSet<AgentLinkOptions>>();

        mockAgentOptionsDbSet.As<IQueryable<AgentOptions>>()
            .Setup(m => m.Provider)
            .Returns(new List<AgentOptions> { agentOptions }.AsQueryable().Provider);
        mockAgentOptionsDbSet.As<IQueryable<AgentOptions>>()
            .Setup(m => m.Expression)
            .Returns(new List<AgentOptions> { agentOptions }.AsQueryable().Expression);
        mockAgentOptionsDbSet.As<IQueryable<AgentOptions>>()
            .Setup(m => m.ElementType)
            .Returns(new List<AgentOptions> { agentOptions }.AsQueryable().ElementType);
        mockAgentOptionsDbSet.As<IQueryable<AgentOptions>>()
            .Setup(m => m.GetEnumerator())
            .Returns(new List<AgentOptions> { agentOptions }.AsQueryable().GetEnumerator());

        mockAgentLinkOptionsDbSet.As<IQueryable<AgentLinkOptions>>()
            .Setup(m => m.Provider)
            .Returns(new List<AgentLinkOptions> { agentLinkOptions }.AsQueryable().Provider);
        mockAgentLinkOptionsDbSet.As<IQueryable<AgentLinkOptions>>()
            .Setup(m => m.Expression)
            .Returns(new List<AgentLinkOptions> { agentLinkOptions }.AsQueryable().Expression);
        mockAgentLinkOptionsDbSet.As<IQueryable<AgentLinkOptions>>()
            .Setup(m => m.ElementType)
            .Returns(new List<AgentLinkOptions> { agentLinkOptions }.AsQueryable().ElementType);
        mockAgentLinkOptionsDbSet.As<IQueryable<AgentLinkOptions>>()
            .Setup(m => m.GetEnumerator())
            .Returns(new List<AgentLinkOptions> { agentLinkOptions }.AsQueryable().GetEnumerator());

        mockAssureLinkContext
            .Setup(x => x.AgentOptions)
            .Returns(mockAgentOptionsDbSet.Object);

        mockAssureLinkContext
            .Setup(x => x.AgentLinkOptions)
            .Returns(mockAgentLinkOptionsDbSet.Object);

        mockAssureLinkContext
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mockEncryption
            .Setup(x => x.EncryptGAC(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("encryptedAccessCode");

        // Act
        var result = await policyDeliveryApiAccessor.UpdateAgentPolicyDeliveryOptions(
            documentConnectOptions,
            optionType,
            CancellationToken.None);

        // Assert
        mockAssureLinkContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        mockAgentOptionsDbSet.Verify(
            x => x.Update(It.Is<AgentOptions>(a =>
                a.AgentId == documentConnectOptions.ViewAsAgentNumber &&
                a.MarketCode == documentConnectOptions.ViewAsMarketCode &&
                a.OptionType == optionType &&
                a.OptOutForEDelivery == documentConnectOptions.OptOutForEDelivery &&
                a.AgentLinkSelected == documentConnectOptions.AgentLinkSelected &&
                a.IncludeDownline == documentConnectOptions.IncludeDownline)),
            Times.Once);

        mockAgentLinkOptionsDbSet.Verify(
            x => x.Update(It.Is<AgentLinkOptions>(a =>
                a.AgentId == documentConnectOptions.ViewAsAgentNumber &&
                a.MarketCode == documentConnectOptions.ViewAsMarketCode &&
                a.OptionType == optionType &&
                a.Email == documentConnectOptions.Email &&
                a.AccessCode == "encryptedAccessCode")),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAgentPolicyDeliveryOptionsWithEmptyAccessCodeShouldUpdateWithoutEncryption()
    {
        // Arrange
        var documentConnectOptions = new DocumentConnectOptions
        {
            ViewAsAgentNumber = "12345",
            ViewAsMarketCode = "MARKET1",
            OptOutForEDelivery = false,
            AgentLinkSelected = true,
            IncludeDownline = false,
            Email = "test@domain.com",
            AccessCode = string.Empty
        };

        var optionType = "SomeOptionType";

        var agentOptions = new AgentOptions
        {
            AgentId = documentConnectOptions.ViewAsAgentNumber,
            MarketCode = documentConnectOptions.ViewAsMarketCode,
            OptionType = optionType,
            OptOutForEDelivery = documentConnectOptions.OptOutForEDelivery,
            AgentLinkSelected = documentConnectOptions.AgentLinkSelected,
            IncludeDownline = documentConnectOptions.IncludeDownline,
        };

        var agentLinkOptions = new AgentLinkOptions
        {
            AgentId = documentConnectOptions.ViewAsAgentNumber,
            MarketCode = documentConnectOptions.ViewAsMarketCode,
            OptionType = optionType,
            Email = documentConnectOptions.Email,
            AccessCode = string.Empty
        };

        var mockAgentOptionsDbSet = new Mock<DbSet<AgentOptions>>();
        var mockAgentLinkOptionsDbSet = new Mock<DbSet<AgentLinkOptions>>();

        mockAgentOptionsDbSet.As<IQueryable<AgentOptions>>()
            .Setup(m => m.Provider)
            .Returns(new List<AgentOptions> { agentOptions }.AsQueryable().Provider);
        mockAgentOptionsDbSet.As<IQueryable<AgentOptions>>()
            .Setup(m => m.Expression)
            .Returns(new List<AgentOptions> { agentOptions }.AsQueryable().Expression);
        mockAgentOptionsDbSet.As<IQueryable<AgentOptions>>()
            .Setup(m => m.ElementType)
            .Returns(new List<AgentOptions> { agentOptions }.AsQueryable().ElementType);
        mockAgentOptionsDbSet.As<IQueryable<AgentOptions>>()
            .Setup(m => m.GetEnumerator())
            .Returns(new List<AgentOptions> { agentOptions }.AsQueryable().GetEnumerator());

        mockAgentLinkOptionsDbSet.As<IQueryable<AgentLinkOptions>>()
            .Setup(m => m.Provider)
            .Returns(new List<AgentLinkOptions> { agentLinkOptions }.AsQueryable().Provider);
        mockAgentLinkOptionsDbSet.As<IQueryable<AgentLinkOptions>>()
            .Setup(m => m.Expression)
            .Returns(new List<AgentLinkOptions> { agentLinkOptions }.AsQueryable().Expression);
        mockAgentLinkOptionsDbSet.As<IQueryable<AgentLinkOptions>>()
            .Setup(m => m.ElementType)
            .Returns(new List<AgentLinkOptions> { agentLinkOptions }.AsQueryable().ElementType);
        mockAgentLinkOptionsDbSet.As<IQueryable<AgentLinkOptions>>()
            .Setup(m => m.GetEnumerator())
            .Returns(new List<AgentLinkOptions> { agentLinkOptions }.AsQueryable().GetEnumerator());

        mockAssureLinkContext
            .Setup(x => x.AgentOptions)
            .Returns(mockAgentOptionsDbSet.Object);

        mockAssureLinkContext
            .Setup(x => x.AgentLinkOptions)
            .Returns(mockAgentLinkOptionsDbSet.Object);

        mockAssureLinkContext
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mockEncryption
            .Setup(x => x.EncryptGAC(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("encryptedAccessCode");

        // Act
        var result = await policyDeliveryApiAccessor.UpdateAgentPolicyDeliveryOptions(
            documentConnectOptions,
            optionType,
            CancellationToken.None);

        // Assert
        mockAssureLinkContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        mockAgentOptionsDbSet.Verify(
            x => x.Update(It.Is<AgentOptions>(a =>
                a.AgentId == documentConnectOptions.ViewAsAgentNumber &&
                a.MarketCode == documentConnectOptions.ViewAsMarketCode &&
                a.OptionType == optionType &&
                a.OptOutForEDelivery == documentConnectOptions.OptOutForEDelivery &&
                a.AgentLinkSelected == documentConnectOptions.AgentLinkSelected &&
                a.IncludeDownline == documentConnectOptions.IncludeDownline)),
            Times.Once);

        mockAgentLinkOptionsDbSet.Verify(
            x => x.Update(It.Is<AgentLinkOptions>(a =>
                a.AgentId == documentConnectOptions.ViewAsAgentNumber &&
                a.MarketCode == documentConnectOptions.ViewAsMarketCode &&
                a.OptionType == optionType &&
                a.Email == documentConnectOptions.Email &&
                a.AccessCode == string.Empty)),
            Times.Once);

        mockEncryption.Verify(x => x.EncryptGAC(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetdatePolicyDeliveryOptions_ShouldReturnCorrectDocumentConnectOptions()
    {
        // Arrange
        var agentNumber = "12345";
        var marketCode = "MARKET1";

        var agentOption = new AgentOptions
        {
            AgentId = agentNumber,
            MarketCode = marketCode,
            OptionType = "EDelivery",
            OptOutForEDelivery = true,
            AgentLinkSelected = true,
            IncludeDownline = false
        };

        var agentLinkOption = new AgentLinkOptions
        {
            AgentId = agentNumber,
            MarketCode = marketCode,
            OptionType = "EDelivery",
            Email = "test@domain.com",
            AccessCode = "encryptedAccessCode"
        };

        var mockAgentOptionsDbSet = new Mock<DbSet<AgentOptions>>();
        var mockAgentLinkOptionsDbSet = new Mock<DbSet<AgentLinkOptions>>();

        mockAgentOptionsDbSet.As<IQueryable<AgentOptions>>()
            .Setup(m => m.Provider)
            .Returns(new List<AgentOptions> { agentOption }.AsQueryable().Provider);
        mockAgentOptionsDbSet.As<IQueryable<AgentOptions>>()
            .Setup(m => m.Expression)
            .Returns(new List<AgentOptions> { agentOption }.AsQueryable().Expression);
        mockAgentOptionsDbSet.As<IQueryable<AgentOptions>>()
            .Setup(m => m.ElementType)
            .Returns(new List<AgentOptions> { agentOption }.AsQueryable().ElementType);
        mockAgentOptionsDbSet.As<IQueryable<AgentOptions>>()
            .Setup(m => m.GetEnumerator())
            .Returns(new List<AgentOptions> { agentOption }.AsQueryable().GetEnumerator());

        mockAgentLinkOptionsDbSet.As<IQueryable<AgentLinkOptions>>()
            .Setup(m => m.Provider)
            .Returns(new List<AgentLinkOptions> { agentLinkOption }.AsQueryable().Provider);
        mockAgentLinkOptionsDbSet.As<IQueryable<AgentLinkOptions>>()
            .Setup(m => m.Expression)
            .Returns(new List<AgentLinkOptions> { agentLinkOption }.AsQueryable().Expression);
        mockAgentLinkOptionsDbSet.As<IQueryable<AgentLinkOptions>>()
            .Setup(m => m.ElementType)
            .Returns(new List<AgentLinkOptions> { agentLinkOption }.AsQueryable().ElementType);
        mockAgentLinkOptionsDbSet.As<IQueryable<AgentLinkOptions>>()
            .Setup(m => m.GetEnumerator())
            .Returns(new List<AgentLinkOptions> { agentLinkOption }.AsQueryable().GetEnumerator());

        var mockAssureLinkContextFactory = new Mock<IDbContextFactory<AssureLinkContext>>();
        var mockAssureLinkContext = new Mock<AssureLinkContext>();
        mockAssureLinkContextFactory
            .Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockAssureLinkContext.Object);

        mockAssureLinkContext
            .Setup(x => x.AgentOptions)
            .Returns(mockAgentOptionsDbSet.Object);

        mockAssureLinkContext
            .Setup(x => x.AgentLinkOptions)
            .Returns(mockAgentLinkOptionsDbSet.Object);

        var mockEncryption = new Mock<IEncryption>();
        mockEncryption
            .Setup(x => x.DecryptGAC(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("decryptedAccessCode");

        var mockConfig = new Mock<IConfigurationManager>();
        mockConfig.Setup(x => x.Environment).Returns("TestEnvironment");

        var policyDeliveryApiAccessor = new PolicyDeliveryApiAccessor(
            mockAssureLinkContextFactory.Object,
            mockEncryption.Object,
            mockConfig.Object);

        // Act
        var result = await policyDeliveryApiAccessor.GetPolicyDeliveryOptions(
            agentNumber,
            marketCode,
            CancellationToken.None);

        // Assert
        Assert.Equal(agentNumber, result.ViewAsAgentNumber);
        Assert.Equal(marketCode, result.ViewAsMarketCode);
        Assert.True(result.OptOutForEDelivery);
        Assert.True(result.AgentLinkSelected);
        Assert.False(result.IncludeDownline);
        Assert.Equal("test@domain.com", result.Email);
        Assert.Equal("decryptedAccessCode", result.AccessCode);
    }
}
