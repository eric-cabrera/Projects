namespace Assurity.AgentPortal.Web.UnitTests;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Web.ClientUtilities;
using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class FeatureFlagMiddlewareHelperTests
{
    [Fact]
    public void CheckPathContains_HappyPath_ReturnsTrue()
    {
        // Arrange
        var featureManagerMock = new Mock<IFeatureManager>();
        var featureFlagMiddlewareHelper = new FeatureFlagMiddlewareHelper(featureManagerMock.Object);

        // Act
        var result = featureFlagMiddlewareHelper.CheckPathContains(new PathString("/items/content/test/test/test"), "/items");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CheckPathContains_DoesNotContain_ReturnsFalse()
    {
        // Arrange
        var featureManagerMock = new Mock<IFeatureManager>();
        var featureFlagMiddlewareHelper = new FeatureFlagMiddlewareHelper(featureManagerMock.Object);

        // Act
        var result = featureFlagMiddlewareHelper.CheckPathContains(new PathString("/co/test/test/test"), "/items");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CheckIsWhiteListed_IsWhiteListed_ReturnsTrue()
    {
        // Arrange
        var featureManagerMock = new Mock<IFeatureManager>();
        var featureFlagMiddlewareHelper = new FeatureFlagMiddlewareHelper(featureManagerMock.Object);

        // Act
        var result = featureFlagMiddlewareHelper.CheckIsWhiteListed(new PathString("/API/PolicyInfo/test/test/test"));

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CheckIsWhiteListed_IsNotWhiteListed_ReturnsFalse()
    {
        // Arrange
        var featureManagerMock = new Mock<IFeatureManager>();
        var featureFlagMiddlewareHelper = new FeatureFlagMiddlewareHelper(featureManagerMock.Object);

        // Act
        var result = featureFlagMiddlewareHelper.CheckIsWhiteListed(new PathString("/items/content/test/test/test"));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CheckFeatureIsInPathAndEnabled_IsInPathAndEnabled_ReturnsTrue()
    {
        // Arrange
        var featureManagerMock = new Mock<IFeatureManager>();
        featureManagerMock.Setup(x => x.IsEnabledAsync(It.IsAny<string>())).ReturnsAsync(true);
        var featureFlagMiddlewareHelper = new FeatureFlagMiddlewareHelper(featureManagerMock.Object);

        // Act
        var result = await featureFlagMiddlewareHelper.CheckFeatureIsInPathAndEnabled(new PathString("/content/test/test/test"));

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CheckFeatureIsInPathAndEnabled_FeatureIsNotInPath_ReturnsFalse()
    {
        // Arrange
        var featureManagerMock = new Mock<IFeatureManager>();
        var featureFlagMiddlewareHelper = new FeatureFlagMiddlewareHelper(featureManagerMock.Object);

        // Act
        var result = await featureFlagMiddlewareHelper.CheckFeatureIsInPathAndEnabled(new PathString("/co/test/test/test"));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CheckFeatureIsInPathAndEnabled_FeatureIsNotInPathAndNotEnabled_ReturnsFalse()
    {
        // Arrange
        var featureManagerMock = new Mock<IFeatureManager>();
        featureManagerMock.Setup(x => x.IsEnabledAsync(It.IsAny<string>())).ReturnsAsync(false);
        var featureFlagMiddlewareHelper = new FeatureFlagMiddlewareHelper(featureManagerMock.Object);

        // Act
        var result = await featureFlagMiddlewareHelper.CheckFeatureIsInPathAndEnabled(new PathString("/co/test/test/test"));

        // Assert
        Assert.False(result);
    }
}
