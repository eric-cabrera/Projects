namespace Assurity.AgentPortal.Accessors.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using Assurity.AgentPortal.Accessors.TaxForms;
using Assurity.AgentPortal.Service.Handlers;
using Assurity.TaxForms.Contracts.V1;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

[ExcludeFromCodeCoverage]
public class TaxFormsApiAccessorTests
{
    private readonly Uri baseAddress = new("http://localhost");

    [Fact]
    public async Task GetTaxForms_Success()
    {
        // Arrange
        var agentId = "123A";

        var mockResponse = new GetAgentFormsResponse()
        {
            AgentForms = new List<AgentForm>
            {
                new()
                {
                    Id = "testbkLl3zuL3zYJn0D4kNQh5H==",
                    DisplayName = "2024"
                }
            }
        };

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(mockResponse)),
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(httpResponseMessage);
        var taxFormsApiAccessor = GetTaxFormsApiAccessor(mockHttpMessageHandler);
        var expectedUrl = baseAddress + "agents/" + agentId + "/forms";

        // Act
        var result = await taxFormsApiAccessor.GetTaxForms(agentId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockResponse.AgentForms[0].DisplayName, result.AgentForms[0].DisplayName);
        Assert.Equal(mockResponse.AgentForms[0].Id, result.AgentForms[0].Id);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && expectedUrl.Equals(req.RequestUri.ToString())),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetTaxForms_IfUnsuccessful_ThrowsException()
    {
        // Arrange
        var agentId = "123A";

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = new StringContent(JsonConvert.SerializeObject(null)),
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(httpResponseMessage);
        var taxFormsApiAccessor = GetTaxFormsApiAccessor(mockHttpMessageHandler);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await taxFormsApiAccessor.GetTaxForms(agentId, CancellationToken.None));
    }

    [Fact]
    public async Task GetTaxForm_Success()
    {
        // Arrange
        var agentId = "123A";
        var formId = "4N913P8C5F202F32650!F195DDF9D2DE921F849";

        var stream = new MemoryStream();

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(new StreamContent(stream))),
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(httpResponseMessage);
        var taxFormsApiAccessor = GetTaxFormsApiAccessor(mockHttpMessageHandler);
        var expectedUrl = baseAddress + "agents/" + agentId + "/forms/" + formId;

        var result = await taxFormsApiAccessor.GetTaxForm(agentId, formId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && expectedUrl.Equals(req.RequestUri.ToString())),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetTaxFormsData_IfUnsuccessful_ThrowsException()
    {
        // Arrange
        var agentId = "123A";
        var formId = "4N913P8C5F202F32650!F195DDF9D2DE921F849";

        var stream = new MemoryStream();

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = new StringContent(JsonConvert.SerializeObject(new StreamContent(stream))),
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(httpResponseMessage);
        var taxFormsApiAccessor = GetTaxFormsApiAccessor(mockHttpMessageHandler);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await taxFormsApiAccessor.GetTaxForm(agentId, formId, CancellationToken.None));
    }

    private static Mock<HttpMessageHandler> GetMockHttpMessageHandler(HttpResponseMessage httpResponseMessage)
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

    private TaxFormsApiAccessor GetTaxFormsApiAccessor(Mock<HttpMessageHandler> mockHttpMessageHandler)
    {
        var errorResponseHandler = new ErrorResponseHandler()
        {
            InnerHandler = mockHttpMessageHandler.Object
        };

        var mockhttpClient = new HttpClient(errorResponseHandler)
        {
            BaseAddress = baseAddress
        };

        var mockLogger = new Mock<ILogger<TaxFormsApiAccessor>>(MockBehavior.Loose);

        return new TaxFormsApiAccessor(mockhttpClient, mockLogger.Object);
    }
}