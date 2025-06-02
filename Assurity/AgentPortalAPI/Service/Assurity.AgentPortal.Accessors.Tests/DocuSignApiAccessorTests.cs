namespace Assurity.AgentPortal.Accessors.Tests;

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Assurity.AgentPortal.Accessors.DocuSign;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

public class DocuSignApiAccessorTests
{
    [Fact]
    public async Task ResendEmail_Success_ReturnsTrue()
    {
        var envelopeId = "12345";
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject("Success"))
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(mockResponse);

        var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        var mockLogger = new Mock<ILogger<DocuSignApiAccessor>>();

        var accessor = new DocuSignApiAccessor(mockHttpClient, mockLogger.Object);

        var result = await accessor.ResendEmail(envelopeId);

        var expectedUrl = $"{mockHttpClient.BaseAddress}v2/Envelope/{envelopeId}/Account/QuickStart/resend";

        Assert.True(result);

        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Put && expectedUrl.Equals(req.RequestUri.ToString())),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task ResendEmail_Fails_ReturnsFalse()
    {
        var envelopeId = "12345";
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = new StringContent("Error")
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(mockResponse);

        var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        var mockLogger = new Mock<ILogger<DocuSignApiAccessor>>();

        var accessor = new DocuSignApiAccessor(mockHttpClient, mockLogger.Object);

        var result = await accessor.ResendEmail(envelopeId);

        var expectedUrl = $"{mockHttpClient.BaseAddress}v2/Envelope/{envelopeId}/Account/QuickStart/resend";

        Assert.False(result);

        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Put && expectedUrl.Equals(req.RequestUri.ToString())),
            ItExpr.IsAny<CancellationToken>());
    }

    private Mock<HttpMessageHandler> GetMockHttpMessageHandler(HttpResponseMessage httpResponseMessage)
    {
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
}
