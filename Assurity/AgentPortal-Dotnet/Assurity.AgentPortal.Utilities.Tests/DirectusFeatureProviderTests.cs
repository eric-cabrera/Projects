namespace Assurity.AgentPortal.Utilities.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using Assurity.AgentPortal.Utilities.FeatureManagement;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Moq;
using Moq.Protected;

[ExcludeFromCodeCoverage]
public class DirectusFeatureProviderTests
{
    [Fact]
    public async void GetAllFeatureDefinitionsAsync_Dev_FeatureShouldBeOn()
    {
        // Arrange
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler
            .Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"data\": [{\"feature_name\": \"commissions\", \"dev\": true, \"test\": true, \"dev2\": false, \"test2\": false, \"prod\":false}]}"),
            })
            .Verifiable();

        var httpClient = new HttpClient(mockHttpHandler.Object);
        httpClient.BaseAddress = new Uri("https://example.com/");
        var mockHttpClientFactory = new Mock<IHttpClientFactory>();

        mockHttpClientFactory.Setup(factory => factory.CreateClient("DirectusFeatureProvider")).Returns(httpClient);

        var mockConfigurationManager = new Mock<IConfigurationManager>();
        mockConfigurationManager.Setup(config => config.DirectusAccessToken).Returns("123abc");
        mockConfigurationManager.Setup(config => config.DirectusUrl).Returns("https://example.com");
        mockConfigurationManager.Setup(config => config.Environment).Returns("DEV");

        var mockLogger = new Mock<ILogger<DirectusFeatureProvider>>();

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var featureProvider = new DirectusFeatureProvider(mockHttpClientFactory.Object, mockConfigurationManager.Object, memoryCache, mockLogger.Object);

        var features = new List<FeatureDefinition>();

        // Act
        await foreach (var feature in featureProvider.GetAllFeatureDefinitionsAsync())
        {
            features.Add(feature);
        }

        // Assert
        Assert.Equal(
            features.Single().Name,
            memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().FeatureName);
        Assert.True(memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().Dev);
        Assert.True(memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().Test);
        Assert.False(memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().Dev2);
        Assert.False(memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().Test2);
        Assert.False(memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().Prod);
        mockHttpHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(
                req => req.Method == HttpMethod.Get
                && req.RequestUri == new Uri("https://example.com/items/agent_center_features")),
            ItExpr.IsAny<CancellationToken>());
        Assert.Equal("commissions", features.Single().Name);
        Assert.Equal("AlwaysOn", features.Single().EnabledFor.Single().Name);
    }

    [Fact]
    public async void GetAllFeatureDefinitionsAsync_Dev2_FeatureShouldBeOn()
    {
        // Arrange
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler
            .Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"data\": [{\"feature_name\": \"commissions\", \"dev\": false, \"test\": false, \"dev2\": true, \"test2\": true, \"prod\":false}]}"),
            })
            .Verifiable();

        var httpClient = new HttpClient(mockHttpHandler.Object);
        httpClient.BaseAddress = new Uri("https://example.com/");
        var mockHttpClientFactory = new Mock<IHttpClientFactory>();

        mockHttpClientFactory.Setup(factory => factory.CreateClient("DirectusFeatureProvider")).Returns(httpClient);

        var mockConfigurationManager = new Mock<IConfigurationManager>();
        mockConfigurationManager.Setup(config => config.DirectusAccessToken).Returns("123abc");
        mockConfigurationManager.Setup(config => config.DirectusUrl).Returns("https://example.com");
        mockConfigurationManager.Setup(config => config.Environment).Returns("dev2");

        var mockLogger = new Mock<ILogger<DirectusFeatureProvider>>();

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var featureProvider = new DirectusFeatureProvider(mockHttpClientFactory.Object, mockConfigurationManager.Object, memoryCache, mockLogger.Object);

        var features = new List<FeatureDefinition>();

        // Act
        await foreach (var feature in featureProvider.GetAllFeatureDefinitionsAsync())
        {
            features.Add(feature);
        }

        // Assert
        Assert.Equal(
            features.Single().Name,
            memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().FeatureName);
        Assert.False(memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().Dev);
        Assert.False(memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().Test);
        Assert.True(memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().Dev2);
        Assert.True(memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().Test2);
        Assert.False(memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().Prod);
        mockHttpHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(
                req => req.Method == HttpMethod.Get
                && req.RequestUri == new Uri("https://example.com/items/agent_center_features")),
            ItExpr.IsAny<CancellationToken>());
        Assert.Equal("commissions", features.Single().Name);
        Assert.Equal("AlwaysOn", features.Single().EnabledFor.Single().Name);
    }

    [Fact]
    public async void GetAllFeatureDefinitionsAsync_Prod_FeatureShouldBeOff()
    {
        // Arrange
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler
            .Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"data\": [{\"feature_name\": \"commissions\",\"dev\": true,\"test\": false, \"dev2\": false, \"test2\": false, \"prod\":false}]}"),
            })
            .Verifiable();

        var httpClient = new HttpClient(mockHttpHandler.Object);
        httpClient.BaseAddress = new Uri("https://example.com/");
        var mockHttpClientFactory = new Mock<IHttpClientFactory>();

        mockHttpClientFactory.Setup(factory => factory.CreateClient("DirectusFeatureProvider")).Returns(httpClient);

        var mockConfigurationManager = new Mock<IConfigurationManager>();
        mockConfigurationManager.Setup(config => config.DirectusAccessToken).Returns("123abc");
        mockConfigurationManager.Setup(config => config.DirectusUrl).Returns("https://example.com");
        mockConfigurationManager.Setup(config => config.Environment).Returns("PROD");

        var mockLogger = new Mock<ILogger<DirectusFeatureProvider>>();

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var featureProvider = new DirectusFeatureProvider(mockHttpClientFactory.Object, mockConfigurationManager.Object, memoryCache, mockLogger.Object);

        var features = new List<FeatureDefinition>();

        // Act
        await foreach (var feature in featureProvider.GetAllFeatureDefinitionsAsync())
        {
            features.Add(feature);
        }

        // Assert
        Assert.Equal(
            features.Single().Name,
            memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().FeatureName);
        Assert.False(memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().Prod);
        mockHttpHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(
                req => req.Method == HttpMethod.Get
                && req.RequestUri == new Uri("https://example.com/items/agent_center_features")),
            ItExpr.IsAny<CancellationToken>());
        Assert.Equal("commissions", features.Single().Name);
        Assert.Empty(features.Single().EnabledFor);
    }

    [Fact]
    public async void GetAllFeatureDefinitionsAsync_CacheisUsed()
    {
        // Arrange
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler
            .Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"data\": [{\"feature_name\": \"commissions\",\"dev\": true,\"test\": false, \"dev2\": false, \"test2\": false, \"prod\":false}]}"),
            })
            .Verifiable();

        var httpClient = new HttpClient(mockHttpHandler.Object);
        httpClient.BaseAddress = new Uri("https://example.com/");
        var mockHttpClientFactory = new Mock<IHttpClientFactory>();

        mockHttpClientFactory.Setup(factory => factory.CreateClient("DirectusFeatureProvider")).Returns(httpClient);

        var mockConfigurationManager = new Mock<IConfigurationManager>();
        mockConfigurationManager.Setup(config => config.DirectusAccessToken).Returns("123abc");
        mockConfigurationManager.Setup(config => config.DirectusUrl).Returns("https://example.com");
        mockConfigurationManager.Setup(config => config.Environment).Returns("PROD");

        var mockLogger = new Mock<ILogger<DirectusFeatureProvider>>();

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var response = new DirectusFeatureResponse
        {
            Data = new List<DirectusFeature>
            {
                new DirectusFeature
                {
                    FeatureName = "commissions",
                    Dev = true,
                    Test = false,
                    Prod = false,
                }
            }
        };

        memoryCache.Set("agent_center_features", response, DateTime.Now.AddMinutes(1));

        var featureProvider = new DirectusFeatureProvider(mockHttpClientFactory.Object, mockConfigurationManager.Object, memoryCache, mockLogger.Object);

        var features = new List<FeatureDefinition>();

        // Act
        await foreach (var feature in featureProvider.GetAllFeatureDefinitionsAsync())
        {
            features.Add(feature);
        }

        // Assert
        mockHttpHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(0),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
        Assert.Equal("commissions", features.Single().Name);
        Assert.Empty(features.Single().EnabledFor);
    }

    [Fact]
    public async void GetFeatureDefinitionAsync_Dev_FeatureShouldBeOn()
    {
        // Arrange
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler
            .Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"data\": [{\"feature_name\": \"commissions\",\"dev\": true,\"test\": true, \"dev2\": false, \"test2\": false, \"prod\":false}]}"),
            })
            .Verifiable();

        var httpClient = new HttpClient(mockHttpHandler.Object);
        httpClient.BaseAddress = new Uri("https://example.com/");
        var mockHttpClientFactory = new Mock<IHttpClientFactory>();

        mockHttpClientFactory.Setup(factory => factory.CreateClient("DirectusFeatureProvider")).Returns(httpClient);

        var mockConfigurationManager = new Mock<IConfigurationManager>();
        mockConfigurationManager.Setup(config => config.DirectusAccessToken).Returns("123abc");
        mockConfigurationManager.Setup(config => config.DirectusUrl).Returns("https://example.com");
        mockConfigurationManager.Setup(config => config.Environment).Returns("DEV");

        var mockLogger = new Mock<ILogger<DirectusFeatureProvider>>();

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var featureProvider = new DirectusFeatureProvider(mockHttpClientFactory.Object, mockConfigurationManager.Object, memoryCache, mockLogger.Object);

        // Act
        var result = await featureProvider.GetFeatureDefinitionAsync("commissions");

        // Assert
        Assert.Equal("commissions", memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().FeatureName);
        Assert.True(memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().Dev);
        Assert.True(memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().Test);
        Assert.False(memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().Dev2);
        Assert.False(memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().Test2);
        Assert.False(memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().Prod);
        mockHttpHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(
                req => req.Method == HttpMethod.Get
                && req.RequestUri == new Uri("https://example.com/items/agent_center_features")),
            ItExpr.IsAny<CancellationToken>());
        Assert.Equal("commissions", result.Name);
        Assert.Equal("AlwaysOn", result.EnabledFor.Single().Name);
    }

    [Fact]
    public async void GetFeatureDefinitionAsync_Dev2_FeatureShouldBeOn()
    {
        // Arrange
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler
            .Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"data\": [{\"feature_name\": \"commissions\", \"dev\": false, \"test\": false, \"dev2\": true, \"test2\": true, \"prod\":false}]}"),
            })
            .Verifiable();

        var httpClient = new HttpClient(mockHttpHandler.Object);
        httpClient.BaseAddress = new Uri("https://example.com/");
        var mockHttpClientFactory = new Mock<IHttpClientFactory>();

        mockHttpClientFactory.Setup(factory => factory.CreateClient("DirectusFeatureProvider")).Returns(httpClient);

        var mockConfigurationManager = new Mock<IConfigurationManager>();
        mockConfigurationManager.Setup(config => config.DirectusAccessToken).Returns("123abc");
        mockConfigurationManager.Setup(config => config.DirectusUrl).Returns("https://example.com");
        mockConfigurationManager.Setup(config => config.Environment).Returns("dev2");

        var mockLogger = new Mock<ILogger<DirectusFeatureProvider>>();

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var featureProvider = new DirectusFeatureProvider(mockHttpClientFactory.Object, mockConfigurationManager.Object, memoryCache, mockLogger.Object);

        // Act
        var result = await featureProvider.GetFeatureDefinitionAsync("commissions");

        // Assert
        Assert.Equal("commissions", memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().FeatureName);
        Assert.False(memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().Dev);
        Assert.False(memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().Test);
        Assert.True(memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().Dev2);
        Assert.True(memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().Test2);
        Assert.False(memoryCache.Get<DirectusFeatureResponse>("agent_center_features").Data.Single().Prod);
        mockHttpHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(
                req => req.Method == HttpMethod.Get
                && req.RequestUri == new Uri("https://example.com/items/agent_center_features")),
            ItExpr.IsAny<CancellationToken>());
        Assert.Equal("commissions", result.Name);
        Assert.Equal("AlwaysOn", result.EnabledFor.Single().Name);
    }

    [Fact]
    public async void GetFeatureDefinitionAsync_FeatureDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler
            .Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"data\": [{\"feature_name\": \"commissions\",\"dev\": true,\"test\": false, \"dev2\": false, \"test2\": false, \"prod\":false}]}"),
            })
            .Verifiable();

        var httpClient = new HttpClient(mockHttpHandler.Object);
        httpClient.BaseAddress = new Uri("https://example.com/");
        var mockHttpClientFactory = new Mock<IHttpClientFactory>();

        mockHttpClientFactory.Setup(factory => factory.CreateClient("DirectusFeatureProvider")).Returns(httpClient);

        var mockConfigurationManager = new Mock<IConfigurationManager>();
        mockConfigurationManager.Setup(config => config.DirectusAccessToken).Returns("123abc");
        mockConfigurationManager.Setup(config => config.DirectusUrl).Returns("https://example.com");
        mockConfigurationManager.Setup(config => config.Environment).Returns("PROD");

        var mockLogger = new Mock<ILogger<DirectusFeatureProvider>>();

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var featureProvider = new DirectusFeatureProvider(mockHttpClientFactory.Object, mockConfigurationManager.Object, memoryCache, mockLogger.Object);

        // Act/Assert
        await Assert.ThrowsAsync<NotSupportedException>(async () => await featureProvider.GetFeatureDefinitionAsync("hello"));
    }
}