namespace Assurity.AgentPortal.Accessors.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using Assurity.AgentPortal.Accessors.LifePortraits;
using Assurity.AgentPortal.Service.Handlers;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

[ExcludeFromCodeCoverage]
public class LifePortraitsAccessorTests
{
    private readonly Uri baseAddress = new Uri("http://localhost");

    [Fact]
    public async Task GetURL_Success()
    {
        // Arrange
        var agentId = "1234";
        var mockResponse = "<LPESLOGINRESPONSE><STATUS>0</STATUS><STATUSINFO></STATUSINFO><TEMPAGENTID>AE63B955DDC53B45A5DAA1E678D92F04</TEMPAGENTID></LPESLOGINRESPONSE>";

        var mockHttpMessageHandler = GetMockHttpMessageHandler(mockResponse, HttpStatusCode.OK);

        var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = baseAddress
        };

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(config => config.LifePortraitsUri).Returns(new Uri("https://fiservtest.assurity.com/"));

        var lifePortraitsAccessor = GetLifePortraitsAccessor(mockHttpMessageHandler, mockConfigurationManager, mockHttpClient);

        // Act
        var result = await lifePortraitsAccessor.GetURL(agentId, CancellationToken.None);

        var expectedUrl = new Uri("https://fiservtest.assurity.com/" + "Fipsco.ASP?PageName=LoginRequest");

        // Assert
        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && expectedUrl.Equals(req.RequestUri.ToString())),
            ItExpr.IsAny<CancellationToken>());
    }

    private static Mock<HttpMessageHandler> GetMockHttpMessageHandler(
        string mockResponse,
        HttpStatusCode statusCode)
    {
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(mockResponse)
        };

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage)
            .Verifiable();

        return mockHttpMessageHandler;
    }

    private LifePortraitsApiAccessor GetLifePortraitsAccessor(
        Mock<HttpMessageHandler> mockHttpMessageHandler,
        Mock<IConfigurationManager> mockConfigurationManager,
        HttpClient httpClient)
    {
        var errorResponseHandler = new ErrorResponseHandler()
        {
            InnerHandler = mockHttpMessageHandler.Object
        };

        var mockLogger = new Mock<ILogger<LifePortraitsApiAccessor>>(MockBehavior.Loose);

        return new LifePortraitsApiAccessor(httpClient, mockConfigurationManager.Object, mockLogger.Object);
    }
}