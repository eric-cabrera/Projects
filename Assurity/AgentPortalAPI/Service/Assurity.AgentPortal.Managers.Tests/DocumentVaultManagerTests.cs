namespace Assurity.AgentPortal.Managers.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Assurity.AgentPortal.Accessors.DTOs;
using Assurity.AgentPortal.Accessors.PolicyInfo;
using Assurity.AgentPortal.Managers.PolicyInfo;
using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class DocumentVaultManagerTests
{
    [Fact]
    public async Task GetPolicyPagesAsync_SuccessfulResponse_ShouldReturnPolicyPagesDetails()
    {
        // Arrange
        var policyNumber = "abc123456789";

        var fakeResponse = new Faker<PolicyPagesResponse>()
            .RuleFor(x => x.PolicyNumber, f => policyNumber)
            .RuleFor(x => x.IsSigned, f => true)
            .RuleFor(x => x.EncodedFile, f => Convert.ToBase64String(f.Random.Bytes(8)))
            .RuleFor(x => x.DocumentExtension, f => ".PDF");

        var response = fakeResponse.Generate();

        var documentVaultManager = GetDocumentVaultManager(response);

        // Act
        var result = await documentVaultManager.GetPolicyPages(policyNumber);

        Assert.NotNull(result);
        Assert.Equal(Convert.FromBase64String(response.EncodedFile), result.FileData);
        Assert.Equal(MediaTypeNames.Application.Pdf, result.FileType);
        Assert.Equal($"policy-{policyNumber}.pdf", result.FileName);
    }

    [Fact]
    public async Task GetPolicyPagesAsync_ErrorResponse_ShouldReturnNull()
    {
        // Arrange
        var policyNumber = "abc123456789";

        var documentVaultManager = GetDocumentVaultManager(null);

        // Act
        var result = await documentVaultManager.GetPolicyPages(policyNumber);

        // Assert
        Assert.Null(result);
    }

    private DocumentVaultManager GetDocumentVaultManager(PolicyPagesResponse documentVaultResponse)
    {
        var mockILogger = new Mock<ILogger<DocumentVaultManager>>();
        var accessToken = "abc123";

        var mockIdentityServerAccessor = new Mock<IIdentityServerAccessor>(MockBehavior.Strict);
        mockIdentityServerAccessor.Setup(accessor => accessor.GetAuthToken(It.IsAny<string[]>()))
            .ReturnsAsync(accessToken);

        var mockDocumentVaultAccessor = new Mock<IDocumentVaultAccessor>(MockBehavior.Strict);
        mockDocumentVaultAccessor.Setup(accessor => accessor.GetPolicyAsync(accessToken, It.IsAny<string>()))
            .ReturnsAsync(documentVaultResponse);

        return new DocumentVaultManager(mockILogger.Object, mockIdentityServerAccessor.Object, mockDocumentVaultAccessor.Object);
    }
}
