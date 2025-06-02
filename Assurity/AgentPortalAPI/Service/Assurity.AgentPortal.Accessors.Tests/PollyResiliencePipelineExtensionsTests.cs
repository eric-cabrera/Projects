namespace Assurity.NetPolly.Tests;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Assurity.AgentPortal.Accessors.Polly;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Polly;
using Polly.Retry;
using Polly.Testing;
using Polly.Timeout;
using Xunit;

public class PollyResiliencePipelineExtensionsTests
{
    [Fact]
    public void TestPipelineConfiguration()
    {
        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 4
            })
            .AddTimeout(TimeSpan.FromSeconds(1))
            .Build();

        var descriptor = pipeline.GetPipelineDescriptor();

        Assert.Equal(2, descriptor.Strategies.Count);

        var retryOptions = Assert.IsType<RetryStrategyOptions>(descriptor.Strategies[0].Options);
        Assert.Equal(4, retryOptions.MaxRetryAttempts);

        var timeoutOptions = Assert.IsType<TimeoutStrategyOptions>(descriptor.Strategies[1].Options);
        Assert.Equal(TimeSpan.FromSeconds(1), timeoutOptions.Timeout);
    }

    [Fact]
    public void TestEmptyPipelineConfiguration()
    {
        var pipeline = new ResiliencePipelineBuilder().Build();
        var descriptor = pipeline.GetPipelineDescriptor();
        Assert.Empty(descriptor.Strategies);
    }

    [Fact]
    public void TestMultipleRetryStrategies()
    {
        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions { MaxRetryAttempts = 3 })
            .AddRetry(new RetryStrategyOptions { MaxRetryAttempts = 5 })
            .Build();

        var descriptor = pipeline.GetPipelineDescriptor();

        Assert.Equal(2, descriptor.Strategies.Count);

        var retryOptions1 = Assert.IsType<RetryStrategyOptions>(descriptor.Strategies[0].Options);
        Assert.Equal(3, retryOptions1.MaxRetryAttempts);

        var retryOptions2 = Assert.IsType<RetryStrategyOptions>(descriptor.Strategies[1].Options);
        Assert.Equal(5, retryOptions2.MaxRetryAttempts);
    }

    [Fact]
    public void TestTimeoutStrategyOnly()
    {
        var pipeline = new ResiliencePipelineBuilder()
            .AddTimeout(TimeSpan.FromSeconds(2))
            .Build();

        var descriptor = pipeline.GetPipelineDescriptor();

        Assert.Single(descriptor.Strategies);

        var timeoutOptions = Assert.IsType<TimeoutStrategyOptions>(descriptor.Strategies[0].Options);
        Assert.Equal(TimeSpan.FromSeconds(2), timeoutOptions.Timeout);
    }

    [Fact]
    public void AddHttpClientRetryPipeline_ShouldConfigureRetryPolicy1()
    {
        var services = new ServiceCollection();
        var loggerMock = new Mock<ILogger<object>>();
        services.AddSingleton(loggerMock.Object);
        services.AddHttpClient();

        services.AddHttpClientRetryPipeline();

        var serviceProvider = services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var serviceDescriptors = services.ToList();

        var pipelineDescriptor = serviceDescriptors.FirstOrDefault(sd => sd.ServiceType == typeof(IHttpClientFactory));
        Assert.NotNull(pipelineDescriptor);
    }

    [Fact]
    public void AddHttpClientRetryPipeline_ConfiguresRetryPolicyAsExpected()
    {
        var services = new ServiceCollection();
        var loggerMock = new Mock<ILogger<object>>();
        services.AddSingleton(loggerMock.Object);
        services.AddHttpClient();

        services.AddHttpClientRetryPipeline();

        var serviceProvider = services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var serviceDescriptors = services.ToList();

        var pipelineDescriptor = serviceDescriptors.FirstOrDefault(sd => sd.ServiceType == typeof(IHttpClientFactory));
        Assert.NotNull(pipelineDescriptor);

        var mockHttpClientHandler = new Mock<HttpMessageHandler>();
        mockHttpClientHandler
             .Protected()
             .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
             .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));

        var httpClient = new HttpClient(mockHttpClientHandler.Object)
        {
            BaseAddress = new Uri("https://test.com")
        };
        httpClient.Timeout = TimeSpan.FromSeconds(10);

        var result = httpClient.GetAsync("/").Result;

        mockHttpClientHandler.Protected().Verify(
            "SendAsync",
            Times.AtLeast(1),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public void AddHttpClientRetryPipeline_ShouldRetryUpToMaxAttempts()
    {
        var services = new ServiceCollection();
        var loggerMock = new Mock<ILogger<object>>();
        services.AddSingleton(loggerMock.Object);

        var mockHttpClientHandler = new Mock<HttpMessageHandler>();

        mockHttpClientHandler
            .Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        services.AddHttpClient("testClient")
            .ConfigurePrimaryHttpMessageHandler(() => mockHttpClientHandler.Object);

        var serviceProvider = services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var httpClient = httpClientFactory.CreateClient("testClient");

        var policy = Policy.Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode &&
                (r.StatusCode == HttpStatusCode.ServiceUnavailable ||
                r.StatusCode == HttpStatusCode.BadGateway ||
                r.StatusCode == HttpStatusCode.RequestTimeout ||
                r.StatusCode == HttpStatusCode.GatewayTimeout ||
                r.StatusCode == HttpStatusCode.InternalServerError))
            .RetryAsync(3, (exception, retryCount, context) =>
            {
                var logger = serviceProvider.GetService<ILogger<object>>();
                logger?.LogWarning("Retrying API call Attempt: {Attempt}", retryCount);
            });

        var result = policy.ExecuteAsync(() => httpClient.GetAsync("https://test.com/")).Result;

        mockHttpClientHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(4),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }
}
