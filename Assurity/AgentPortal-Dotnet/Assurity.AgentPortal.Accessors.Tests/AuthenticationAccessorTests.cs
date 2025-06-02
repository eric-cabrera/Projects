namespace Assurity.AgentPortal.Accessors.Tests;

using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

public class AuthenticationAccessorTests
{
    [Fact]
    public async void Refresh_StatusCode200_ResultNotNull()
    {
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler
            .Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"access_token\": \"coolAccessToken\", \"id_token\": \"coolIdToken\", \"refresh_token\": \"coolRefreshToken\", \"expires_in\": 3600}"),
            })
            .Verifiable();
        var httpClient = new HttpClient(mockHttpHandler.Object);
        httpClient.BaseAddress = new Uri("https://example.com/");
        var mockHttpClientFactory = new Mock<IHttpClientFactory>();
        mockHttpClientFactory.Setup(factory => factory.CreateClient("AuthenticationAccessor")).Returns(httpClient);
        var mockLogger = new Mock<ILogger<AuthenticationAccessor>>();

        var authenticationAccessor = new AuthenticationAccessor(mockHttpClientFactory.Object, mockLogger.Object);

        var result = await authenticationAccessor.RefreshTokenAsync("123456", "coolClientId", "coolClientSecret", "scopes", "https://example.com", false);

        Assert.NotNull(result);
        Assert.Equal("coolAccessToken", result.AccessToken);
        mockHttpHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(
                req => req.Method == HttpMethod.Post
                && req.RequestUri == new Uri("https://example.com/token")),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async void Refresh_StatusCode400_ReturnsNull()
    {
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler
            .Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("{\"access_token\": \"coolAccessToken\", \"id_token\": \"coolIdToken\", \"refresh_token\": \"coolRefreshToken\", \"expires_in\": 3600}"),
            })
            .Verifiable();
        var httpClient = new HttpClient(mockHttpHandler.Object);
        httpClient.BaseAddress = new Uri("https://example.com/");
        var mockHttpClientFactory = new Mock<IHttpClientFactory>();
        mockHttpClientFactory.Setup(factory => factory.CreateClient("AuthenticationAccessor")).Returns(httpClient);
        var mockLogger = new Mock<ILogger<AuthenticationAccessor>>();

        var authenticationAccessor = new AuthenticationAccessor(mockHttpClientFactory.Object, mockLogger.Object);

        var result = await authenticationAccessor.RefreshTokenAsync("123456", "coolClientId", "coolClientSecret", "scopes", "https://example.com", false);

        Assert.Null(result);
        mockHttpHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(
                req => req.Method == HttpMethod.Post
                && req.RequestUri == new Uri("https://example.com/token")),
            ItExpr.IsAny<CancellationToken>());
    }
}