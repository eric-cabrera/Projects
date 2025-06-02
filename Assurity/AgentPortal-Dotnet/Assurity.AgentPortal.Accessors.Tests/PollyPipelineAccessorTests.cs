namespace Assurity.AgentPortal.Accessors.Tests;

using Polly;

public class PollyPipelineAccessorTests
{
    [Fact]
    public void GetRetryPolicy_ShouldReturnPolicy()
    {
        // Arrange
        var pollyPipelineAccessor = new PollyPipelineAccessor();

        // Act
        var policy = pollyPipelineAccessor.GetRetryPolicy();

        // Assert
        Assert.NotNull(policy);
        Assert.IsAssignableFrom<IAsyncPolicy<HttpResponseMessage>>(policy);
    }

    [Fact]
    public async Task GetRetryPolicy_ShouldRetryOnTransientHttpError()
    {
        // Arrange
        var pollyPipelineAccessor = new PollyPipelineAccessor();
        var policy = pollyPipelineAccessor.GetRetryPolicy();
        var handler = new HttpClientHandler();

        // Act
        var retryCount = 0;
        await policy.ExecuteAsync(async () =>
        {
            retryCount++;
            if (retryCount < 6)
            {
                throw new HttpRequestException();
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        });

        // Assert
        Assert.Equal(6, retryCount);
    }

    //[Fact]
    public async Task GetRetryPolicy_ShouldWaitAndRetry()
    {
        // Arrange
        var pollyPipelineAccessor = new PollyPipelineAccessor();
        var policy = pollyPipelineAccessor.GetRetryPolicy();
        var retryCount = 0;
        var delays = new List<TimeSpan>();

        // Act
        await policy.ExecuteAsync(async () =>
        {
            retryCount++;
            if (retryCount < 6)
            {
                delays.Add(TimeSpan.FromSeconds(Math.Pow(2, retryCount)));
                throw new HttpRequestException();
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        });

        // Assert
        Assert.Equal(6, retryCount);
        Assert.Equal(TimeSpan.FromSeconds(2), delays[0]);
        Assert.Equal(TimeSpan.FromSeconds(4), delays[1]);
        Assert.Equal(TimeSpan.FromSeconds(8), delays[2]);
        Assert.Equal(TimeSpan.FromSeconds(16), delays[3]);
        Assert.Equal(TimeSpan.FromSeconds(32), delays[4]);
    }
}
