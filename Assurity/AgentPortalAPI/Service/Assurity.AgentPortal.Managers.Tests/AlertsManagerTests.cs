namespace Assurity.AgentPortal.Managers.Tests;

using System.Collections.Generic;
using System.Threading.Tasks;
using Assurity.AgentPortal.Accessors.Alerts;
using Assurity.AgentPortal.Accessors.AssureLink.Entities;
using Assurity.AgentPortal.Contracts.Alerts;
using Assurity.AgentPortal.Managers.Alerts;
using Moq;
using Xunit;

public class AlertsManagerTests
{
    [Fact]
    public async Task GetDistributionEmailsByAgentId_Success()
    {
        // Arrange
        string agentId = "ABC1";
        string email = "ABC1@fake.com";
        var accessorResponse = new List<Accessors.AssureLink.Entities.DistributionList> { new Accessors.AssureLink.Entities.DistributionList { Id = 0, AgentId = agentId, Email = email } };
        var managerResponse = new List<string> { email };

        var mockAccessor = new Mock<IAlertsAccessor>();
        mockAccessor
            .Setup(accessor => accessor.GetDistributionListsByAgentId(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessorResponse)
            .Verifiable();

        var alertsManager = new AlertsManager(mockAccessor.Object);

        // Act
        var response = await alertsManager.GetDistributionEmailsByAgentId(agentId, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Single(response);
        Assert.Equal(0, response[0].Id);
        Assert.Equal(email, response[0].Email);

        mockAccessor.Verify(
            accessor => accessor.GetDistributionListsByAgentId(agentId, CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task GetDistributionEmailsByAgentId_ReturnsNull()
    {
        // Arrange
        string agentId = "ABC1";
        string email = "ABC1@fake.com";
        var managerResponse = new List<string> { email };

        var mockAccessor = new Mock<IAlertsAccessor>();
        mockAccessor
            .Setup(accessor => accessor.GetDistributionListsByAgentId(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<Accessors.AssureLink.Entities.DistributionList>)null)
            .Verifiable();

        var alertsManager = new AlertsManager(mockAccessor.Object);

        // Act
        var response = await alertsManager.GetDistributionEmailsByAgentId(agentId, CancellationToken.None);

        // Assert
        Assert.Null(response);

        mockAccessor.Verify(
            accessor => accessor.GetDistributionListsByAgentId(agentId, CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task AddDistributionEmail_Success()
    {
        // Arrange
        string agentId = "ABC1";
        string email = "ABC1@fake.com";
        var distributionList = new Accessors.AssureLink.Entities.DistributionList { AgentId = agentId, Email = email };

        var mockAccessor = new Mock<IAlertsAccessor>();
        mockAccessor
            .Setup(accessor => accessor.AddDistributionList(It.IsAny<Accessors.AssureLink.Entities.DistributionList>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        var alertsManager = new AlertsManager(mockAccessor.Object);

        // Act
        await alertsManager.AddDistributionEmail(agentId, email, CancellationToken.None);

        // Assert
        mockAccessor.Verify(
            accessor => accessor.AddDistributionList(
                It.Is<Accessors.AssureLink.Entities.DistributionList>(x => x.AgentId == distributionList.AgentId && x.Email == distributionList.Email),
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task DeleteDistributionEmail_Success()
    {
        // Arrange
        string agentId = "ABC1";
        string email = "ABC1@fake.com";
        var accessorResponse = new Accessors.AssureLink.Entities.DistributionList { Id = 0, AgentId = agentId, Email = email };

        var mockAccessor = new Mock<IAlertsAccessor>();

        var alertsManager = new AlertsManager(mockAccessor.Object);

        // Act
        await alertsManager.DeleteDistributionEmail(0, agentId, CancellationToken.None);

        // Assert
        mockAccessor.Verify(
            accessor => accessor.DeleteDistributionList(accessorResponse.Id, agentId, CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task GetAlertPreferencesByAgentId_Success()
    {
        // Arrange
        string agentId = "ABC1";
        var accessorResponse = new DistributionMaster
        {
            Id = 0,
            AgentId = agentId,
            DisableAll = true,
            SelfAdd = true,
            SelfMet = true,
            SelfOutstanding = true,
            HierarchyAdd = true,
            HierarchyMet = true,
            HierarchyOutstanding = true
        };

        var mockAccessor = new Mock<IAlertsAccessor>();
        mockAccessor
            .Setup(accessor => accessor.GetDistributionMasterByAgentId(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessorResponse)
            .Verifiable();

        var alertsManager = new AlertsManager(mockAccessor.Object);

        // Act
        var response = await alertsManager.GetAlertPreferencesByAgentId(agentId, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.DisableAll);
        Assert.True(response.SelfAdd);
        Assert.True(response.SelfMet);
        Assert.True(response.SelfOutstanding);
        Assert.True(response.HierarchyAdd);
        Assert.True(response.HierarchyMet);
        Assert.True(response.HierarchyOutstanding);

        mockAccessor.Verify(
            accessor => accessor.GetDistributionMasterByAgentId(agentId, CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task GetAlertPreferencesByAgentId_NotFound_ReturnsDefault()
    {
        // Arrange
        string agentId = "ABC1";
        var mockAccessor = new Mock<IAlertsAccessor>();
        mockAccessor
            .Setup(accessor => accessor.GetDistributionMasterByAgentId(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((DistributionMaster)null)
            .Verifiable();

        var alertsManager = new AlertsManager(mockAccessor.Object);

        // Act
        var response = await alertsManager.GetAlertPreferencesByAgentId(agentId, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.DisableAll);
        Assert.False(response.SelfAdd);
        Assert.False(response.SelfMet);
        Assert.False(response.SelfOutstanding);
        Assert.False(response.HierarchyAdd);
        Assert.False(response.HierarchyMet);
        Assert.False(response.HierarchyOutstanding);

        mockAccessor.Verify(
            accessor => accessor.GetDistributionMasterByAgentId(agentId, CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task AddOrUpdateAlertPreferences_AddAlertPreferences_Success()
    {
        // Arrange
        string agentId = "ABC1";
        var alertPreferences = new AlertPreferences
        {
            DisableAll = true,
            SelfAdd = false,
            SelfMet = true,
            SelfOutstanding = false,
            HierarchyAdd = true,
            HierarchyMet = false,
            HierarchyOutstanding = true
        };

        var distributionMaster = new DistributionMaster
        {
            AgentId = agentId,
            DisableAll = true,
            SelfAdd = false,
            SelfMet = true,
            SelfOutstanding = false,
            HierarchyAdd = true,
            HierarchyMet = false,
            HierarchyOutstanding = true
        };

        var mockAccessor = new Mock<IAlertsAccessor>();

        var alertsManager = new AlertsManager(mockAccessor.Object);

        // Act
        await alertsManager.AddOrUpdateAlertPreferences(agentId, alertPreferences, CancellationToken.None);

        // Assert
        mockAccessor.Verify(
            accessor => accessor.UpsertDistributionMaster(
                It.Is<string>(x => x == agentId),
                It.Is<DistributionMaster>(x =>
                    x.AgentId == distributionMaster.AgentId &&
                    x.DisableAll == distributionMaster.DisableAll &&
                    x.SelfAdd == distributionMaster.SelfAdd &&
                    x.SelfMet == distributionMaster.SelfMet &&
                    x.SelfOutstanding == distributionMaster.SelfOutstanding &&
                    x.HierarchyAdd == distributionMaster.HierarchyAdd &&
                    x.HierarchyMet == distributionMaster.HierarchyMet &&
                    x.HierarchyOutstanding == distributionMaster.HierarchyOutstanding),
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task AddOrUpdateAlertPreferences_UpdateAlertPreferences_Success()
    {
        // Arrange
        string agentId = "ABC1";
        var alertPreferences = new AlertPreferences
        {
            DisableAll = true,
            SelfAdd = false,
            SelfMet = true,
            SelfOutstanding = false,
            HierarchyAdd = true,
            HierarchyMet = false,
            HierarchyOutstanding = true
        };

        var convertedPreferences = new DistributionMaster
        {
            Id = 0,
            AgentId = agentId,
            DisableAll = true,
            SelfAdd = false,
            SelfMet = true,
            SelfOutstanding = false,
            HierarchyAdd = true,
            HierarchyMet = false,
            HierarchyOutstanding = true
        };

        var accessorResponse = new DistributionMaster
        {
            Id = 0,
            AgentId = agentId,
            DisableAll = true,
            SelfAdd = true,
            SelfMet = true,
            SelfOutstanding = true,
            HierarchyAdd = true,
            HierarchyMet = true,
            HierarchyOutstanding = true
        };

        var mockAccessor = new Mock<IAlertsAccessor>();

        var alertsManager = new AlertsManager(mockAccessor.Object);

        // Act
        await alertsManager.AddOrUpdateAlertPreferences(agentId, alertPreferences, CancellationToken.None);

        // Assert
        mockAccessor.Verify(
            accessor => accessor.UpsertDistributionMaster(
                It.Is<string>(x => x == agentId),
                It.Is<DistributionMaster>(x =>
                    x.AgentId == convertedPreferences.AgentId &&
                    x.DisableAll == convertedPreferences.DisableAll &&
                    x.SelfAdd == convertedPreferences.SelfAdd &&
                    x.SelfMet == convertedPreferences.SelfMet &&
                    x.SelfOutstanding == convertedPreferences.SelfOutstanding &&
                    x.HierarchyAdd == convertedPreferences.HierarchyAdd &&
                    x.HierarchyMet == convertedPreferences.HierarchyMet &&
                    x.HierarchyOutstanding == convertedPreferences.HierarchyOutstanding),
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAlertPreferences_Success()
    {
        // Arrange
        string agentId = "ABC1";
        var accessorResponse = new DistributionMaster
        {
            Id = 0,
            AgentId = agentId,
            DisableAll = true,
            SelfAdd = true,
            SelfMet = true,
            SelfOutstanding = true,
            HierarchyAdd = true,
            HierarchyMet = true,
            HierarchyOutstanding = true
        };

        var mockAccessor = new Mock<IAlertsAccessor>();
        mockAccessor
            .Setup(accessor => accessor.GetDistributionMasterByAgentId(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessorResponse)
            .Verifiable();

        var alertsManager = new AlertsManager(mockAccessor.Object);

        // Act
        await alertsManager.DeleteAlertPreferences(agentId, CancellationToken.None);

        // Assert
        mockAccessor.Verify(
            accessor => accessor.GetDistributionMasterByAgentId(agentId, CancellationToken.None),
            Times.Once);

        mockAccessor.Verify(
            accessor => accessor.DeleteDistributionMaster(accessorResponse, CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAlertPreferences_NotFound_ThrowsException()
    {
        // Arrange
        string agentId = "ABC1";

        var mockAccessor = new Mock<IAlertsAccessor>();
        mockAccessor
            .Setup(accessor => accessor.GetDistributionMasterByAgentId(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((DistributionMaster)null)
            .Verifiable();

        var alertsManager = new AlertsManager(mockAccessor.Object);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => alertsManager.DeleteAlertPreferences(agentId, CancellationToken.None));

        // Assert
        mockAccessor.Verify(
            accessor => accessor.GetDistributionMasterByAgentId(agentId, CancellationToken.None),
            Times.Once);
    }
}
