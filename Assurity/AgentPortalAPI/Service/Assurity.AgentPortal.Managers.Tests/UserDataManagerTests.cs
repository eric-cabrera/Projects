namespace Assurity.AgentPortal.Managers.Tests;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Accessors.Agent;
using Assurity.AgentPortal.Accessors.ProductionCredit;
using Assurity.AgentPortal.Contracts.AgentContracts;
using Assurity.AgentPortal.Managers.UserData;
using Assurity.AgentPortal.Utilities.Emails;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using AgentAPI = Assurity.Agent.Contracts;

[ExcludeFromCodeCoverage]
public class UserDataManagerTests
{
    [Theory]
    [InlineData("ABC1", AgentAPI.AccessLevel.Full, true)]
    [InlineData("DEF1", AgentAPI.AccessLevel.Full, false)]
    [InlineData("GHI1", AgentAPI.AccessLevel.Historical, true)]
    [InlineData("XYZ1", AgentAPI.AccessLevel.Historical, false)]
    public async Task GetAgentAccess_Success(string agentId, AgentAPI.AccessLevel accessLevel, bool isCached)
    {
        // Arrange
        var expectedResponse = new AgentAPI.AgentAccessResponse
        {
            AccessLevel = accessLevel
        };

        var mockProductionCreditAPIAccessor = new Mock<IProductionCreditApiAccessor>();

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor
            .Setup(accessor => accessor.GetAgentAccess(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse)
            .Verifiable();

        var services = new ServiceCollection();
        services.AddMemoryCache();

        var serviceProvider = services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetService<IMemoryCache>();

        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(x => x.Map<AccessLevel>(It.Is<AgentAPI.AccessLevel>(level => level == AgentAPI.AccessLevel.Full))).Returns(AccessLevel.Full);
        mockMapper.Setup(x => x.Map<AccessLevel>(It.Is<AgentAPI.AccessLevel>(level => level == AgentAPI.AccessLevel.Historical))).Returns(AccessLevel.Historical);

        var mappedAccessLevel = mockMapper.Object.Map<AccessLevel>(accessLevel);

        var expectedApiCalls = Times.Once;

        if (isCached)
        {
            memoryCache.Set(agentId, mappedAccessLevel, TimeSpan.FromMinutes(1));
            expectedApiCalls = Times.Never;
        }

        var userDataManager = new UserDataManager(mockProductionCreditAPIAccessor.Object, mockAgentApiAccessor.Object, null, memoryCache, mockMapper.Object);

        // Act
        var response = await userDataManager.GetAgentAccess(agentId);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(response, mappedAccessLevel);

        mockAgentApiAccessor.Verify(
            accessor => accessor.GetAgentAccess(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            expectedApiCalls);
    }

    [Fact]
    public async Task GetAgentAccess_ApiReturnsNull_ReturnsNull()
    {
        // Arrange
        var mockProductionCreditAPIAccessor = new Mock<IProductionCreditApiAccessor>();

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor
            .Setup(accessor => accessor.GetAgentAccess(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AgentAPI.AgentAccessResponse)null)
            .Verifiable();

        var services = new ServiceCollection();
        services.AddMemoryCache();

        var serviceProvider = services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetService<IMemoryCache>();

        var mockMapper = new Mock<IMapper>();

        var userDataManager = new UserDataManager(mockProductionCreditAPIAccessor.Object, mockAgentApiAccessor.Object, null, memoryCache, mockMapper.Object);

        // Act
        var response = await userDataManager.GetAgentAccess("AAXB");

        // Assert
        Assert.Null(response);

        mockAgentApiAccessor.Verify(
            accessor => accessor.GetAgentAccess(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("ABC1", false, true)]
    [InlineData("DEF1", true, false)]
    [InlineData("GHI1", true, true)]
    [InlineData("XYZ1", false, false)]
    public async Task GetBusinessTypesByAgentId_Success(string agentId, bool containsWorksite, bool containsIndividual)
    {
        // Arrange
        var accessorResponse = GetFakeMarketCodes(agentId);

        var mockProductionCreditAPIAccessor = new Mock<IProductionCreditApiAccessor>();
        mockProductionCreditAPIAccessor.Setup(accessor => accessor.GetProductionCreditMarketcodes(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(accessorResponse)
        .Verifiable();

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        var mockMemoryCache = new Mock<IMemoryCache>();
        var mockMapper = new Mock<IMapper>();

        var userDataManager = new UserDataManager(mockProductionCreditAPIAccessor.Object, mockAgentApiAccessor.Object, null, mockMemoryCache.Object, mockMapper.Object);

        // Act
        var response = await userDataManager.GetBusinessTypesByAgentId(agentId, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(containsWorksite, response.Contains(Contracts.UserData.Market.Worksite));
        Assert.Equal(containsIndividual, response.Contains(Contracts.UserData.Market.Individual));

        mockProductionCreditAPIAccessor.Verify(
            accessor => accessor.GetProductionCreditMarketcodes(
            agentId,
            CancellationToken.None),
            Times.Once);
    }

    private static List<string> GetFakeMarketCodes(string agentId)
    {
        return agentId switch
        {
            "ABC1" => ["IG"],
            "DEF1" => ["WS"],
            "GHI1" => ["IG", "WS"],
            _ => [],
        };
    }
}
