namespace Assurity.AgentPortal.Engines.Tests;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Engines.Integration;
using Xunit;

[ExcludeFromCodeCoverage]
public class IllustrationProEngineTests
{
    [Fact]
    public void GetFiservDistributionChannelForIllustrationPro_ShouldReturn_DistributionChannel_IS()
    {
        // Arrange
        var illustrationProEngine = new IllustrationProEngine();
        var marketCodes = new List<string> { "IS", "AGTDTC" };

        // Act
        var distributionChannel = illustrationProEngine.GetFiservDistributionChannelForIllustrationPro(marketCodes);

        // Assert
        Assert.Equal("IS", distributionChannel);
    }

    [Fact]
    public void GetFiservDistributionChannelForIllustrationPro_ShouldReturn_DistributionChannel_IG()
    {
        // Arrange
        var illustrationProEngine = new IllustrationProEngine();
        var marketCodes = new List<string> { "AGTDTC", "IGNY", "WSR11" };

        // Act
        var distributionChannel = illustrationProEngine.GetFiservDistributionChannelForIllustrationPro(marketCodes);

        // Assert
        Assert.Equal("IG", distributionChannel);
    }

    [Fact]
    public void GetFiservDistributionChannelForIllustrationPro_ShouldReturn_DistributionChannel_DEFAULT()
    {
        // Arrange
        var illustrationProEngine = new IllustrationProEngine();
        var marketCodes = new List<string> { "AGTDTC", "WSR11", "WSR12", "WSR15" };

        // Act
        var distributionChannel = illustrationProEngine.GetFiservDistributionChannelForIllustrationPro(marketCodes);

        // Assert
        Assert.Equal("DEFAULT", distributionChannel);
    }

    [Fact]
    public void GetFiservDistributionChannelForIllustrationPro_Null_MarketCodes_ShouldReturn_DistributionChannel_DEFAULT()
    {
        // Arrange
        var illustrationProEngine = new IllustrationProEngine();

        // Act
        var distributionChannel = illustrationProEngine.GetFiservDistributionChannelForIllustrationPro(null);

        // Assert
        Assert.Equal("DEFAULT", distributionChannel);
    }
}