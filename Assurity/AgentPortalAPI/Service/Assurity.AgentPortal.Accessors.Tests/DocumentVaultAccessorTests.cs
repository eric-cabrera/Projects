namespace Assurity.AgentPortal.Accessors.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using Assurity.AgentPortal.Accessors.DTOs;
using Assurity.AgentPortal.Accessors.PolicyInfo;
using Assurity.DocumentVault.Client;
using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

[ExcludeFromCodeCoverage]
public class DocumentVaultAccessorTests
{
    [Fact]
    public async Task GetPolicyAsync_OkResponse_ShouldReturnDocument()
    {
        // Arrange
        var policyNumber = "4750612114";
        var agentId = "1234";
        var fakeResponse = new Faker<PolicyPagesResponse>()
            .RuleFor(x => x.PolicyNumber, f => policyNumber)
            .RuleFor(x => x.IsSigned, f => true)
            .RuleFor(x => x.EncodedFile, f => f.Random.String())
            .RuleFor(x => x.DocumentExtension, f => ".PDF");

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(fakeResponse.Generate()))
        };

        var documentVaultAccessor = GetDocumentVaultAccessor(httpResponseMessage);

        // Act
        var result = await documentVaultAccessor.GetPolicyAsync(policyNumber, agentId);

        // Assert
        Assert.NotNull(result.PolicyNumber);
        Assert.NotNull(result.EncodedFile);
        Assert.NotNull(result.DocumentExtension);
    }

    [Fact]
    public async Task GetPolicyAsync_NullIsSigned_ShouldReturnDocument()
    {
        // Arrange
        var policyNumber = "4750612114";
        var agentId = "1234";

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{ \"PolicyNumber\":\"4750612114\",\"isSigned\": null,\"EncodedFile\":\"Cool File strings\",\"DocumentExtension\":\".PDF\",\"Error\":null}"),
        };

        var documentVaultAccessor = GetDocumentVaultAccessor(httpResponseMessage);

        // Act
        var result = await documentVaultAccessor.GetPolicyAsync(policyNumber, agentId);

        // Assert
        Assert.NotNull(result.PolicyNumber);
        Assert.NotNull(result.EncodedFile);
        Assert.NotNull(result.DocumentExtension);
    }

    [Fact]
    public async Task GetPolicyAsync_NotFoundResponse_ShouldReturnNull()
    {
        // Arrange
        var policyNumber = "4750612114";
        var agentId = "1234";

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
        };

        var documentVaultAccessor = GetDocumentVaultAccessor(httpResponseMessage);

        // Act
        var result = await documentVaultAccessor.GetPolicyAsync(policyNumber, agentId);

        // Assert
        Assert.Null(result);
    }

    private static DocumentVaultAccessor GetDocumentVaultAccessor(HttpResponseMessage httpResponseMessage)
    {
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        mockHttpMessageHandler
           .Protected()
           .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(httpResponseMessage);

        var mockhttpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };

        var mockLogger = new Mock<ILogger<DocumentVaultAccessor>>();

        return new DocumentVaultAccessor(new DocumentVaultClient(mockhttpClient), mockLogger.Object);
    }
}
