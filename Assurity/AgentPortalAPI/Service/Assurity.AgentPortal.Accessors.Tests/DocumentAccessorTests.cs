namespace Assurity.AgentPortal.Accessors.Tests;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Accessors.DTOs;
using Assurity.AgentPortal.Accessors.Send;
using Assurity.AgentPortal.Accessors.Tests.TestData;
using Assurity.Document.Client;
using Assurity.Document.Contracts;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class DocumentAccessorTests
{
    [Fact]
    public void SendDocuments_ActionRequestIsNull_ShouldNotCreateDocuments()
    {
        // Arrange
        var mockDocumentClient = new Mock<IDocumentClient>(MockBehavior.Strict);

        var documentAccessor = new DocumentAccessor(mockDocumentClient.Object);

        // Act
        documentAccessor.SendDocuments(null);

        // Assert
        mockDocumentClient.Verify(
            documentClient => documentClient.CreateDocumentsWithTiffImage(
                It.IsAny<DocumentsJson>()),
            Times.Never);
    }

    [Fact]
    public void SendDocuments_FilesIsNull_ShouldNotCreateDocuments()
    {
        // Arrange
        var mockDocumentClient = new Mock<IDocumentClient>(MockBehavior.Strict);

        var documentAccessor = new DocumentAccessor(mockDocumentClient.Object);

        // Act
        documentAccessor.SendDocuments(new ActionRequest());

        // Assert
        mockDocumentClient.Verify(
            documentClient => documentClient.CreateDocumentsWithTiffImage(
                It.IsAny<DocumentsJson>()),
            Times.Never);
    }

    [Fact]
    public void SendDocuments_FilesCountIsZero_ShouldNotCreateDocuments()
    {
        // Arrange
        var mockDocumentClient = new Mock<IDocumentClient>(MockBehavior.Strict);

        var documentAccessor = new DocumentAccessor(mockDocumentClient.Object);

        // Act
        documentAccessor.SendDocuments(new ActionRequest
        {
            Files = new List<File>()
        });

        // Assert
        mockDocumentClient.Verify(
            documentClient => documentClient.CreateDocumentsWithTiffImage(
                It.IsAny<DocumentsJson>()),
            Times.Never);
    }

    [Fact]
    public void SendDocuments_HasMessageAndImageFiles_Successful_ShouldCreateDocuments()
    {
        // Arrange
        var mockDocumentClient = new Mock<IDocumentClient>(MockBehavior.Strict);
        mockDocumentClient
            .Setup(documentClient => documentClient.CreateDocumentsWithTiffImage(
                It.IsAny<DocumentsJson>()))
            .Returns(new CreateDocumentsResponse { Successful = true });

        var documentAccessor = new DocumentAccessor(mockDocumentClient.Object);

        var actionRequest = DocumentAccessorTestData.ActionRequest;

        // Act
        documentAccessor.SendDocuments(actionRequest);

        // Assert
        mockDocumentClient.Verify(
            documentClient => documentClient.CreateDocumentsWithTiffImage(
                It.IsAny<DocumentsJson>()),
            Times.Once);
    }

    [Fact]
    public void SendDocuments_HasMessageAndImageFiles_Unsuccessful_ShouldThrowExceptionWithMessage()
    {
        // Arrange
        var mockDocumentClient = new Mock<IDocumentClient>(MockBehavior.Strict);
        mockDocumentClient
            .Setup(documentClient => documentClient.CreateDocumentsWithTiffImage(
                It.IsAny<DocumentsJson>()))
            .Returns(new CreateDocumentsResponse
            {
                ModelValidationErrors = new Dictionary<string, List<string>>
                {
                    {
                        "BadPolicy",
                        new List<string>
                        {
                            "For some odd reason, the policy caused a failure during processing.",
                            "Downstream troubles ensued."
                        }
                    },
                    {
                        "Timeout",
                        new List<string>
                        {
                            "Took too long!"
                        }
                    }
                }
            });

        var documentAccessor = new DocumentAccessor(mockDocumentClient.Object);

        var actionRequest = DocumentAccessorTestData.ActionRequest;

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => documentAccessor.SendDocuments(actionRequest));

        mockDocumentClient.Verify(
            documentClient => documentClient.CreateDocumentsWithTiffImage(
                It.IsAny<DocumentsJson>()),
            Times.Once);

        var expectedErrorMessage = $" {nameof(CreateDocumentsResponse.ModelValidationErrors)}: " +
            $"BadPolicy: For some odd reason, the policy caused a failure during processing. " +
            $"Downstream troubles ensued. | Timeout: Took too long!";
        var expectedExceptionMessage = $"{nameof(DocumentClient)} failed during " +
            $"{nameof(DocumentClient.CreateDocumentsWithTiffImage)} call.{expectedErrorMessage}";

        Assert.Equal(expectedExceptionMessage, exception.Message);
    }
}