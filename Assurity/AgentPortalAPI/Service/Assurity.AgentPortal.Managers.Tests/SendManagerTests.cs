namespace Assurity.AgentPortal.Managers.Tests;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Accessors.Send;
using Assurity.AgentPortal.Contracts.Send;
using Assurity.AgentPortal.Contracts.Send.Enums;
using Assurity.AgentPortal.Engines;
using Assurity.AgentPortal.Managers.Send;
using Moq;
using Xunit;
using ActionRequest = Assurity.AgentPortal.Contracts.Send.ActionRequest;
using DTOs = Assurity.AgentPortal.Accessors.DTOs;

[ExcludeFromCodeCoverage]
public class SendManagerTests
{
    private static string ObjectIdForNewBusinessTransaction => "13058DEV1000020";

    private static string TiffImageBase64Content => "TiffImageBase64Content";

    [Fact]
    public async Task SendMessageAndFilesToGlobal_NoFiles_ShouldThrowException()
    {
        // Arrange
        var sendManager = new SendManager(null, null, null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            async () => await sendManager.SendMessageAndFilesToGlobal(new ActionRequest()));

        var expectedMessage = "Expected to create a least one tiff File DTO but created none.";

        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public async Task SendMessageAndFilesToGlobal_NullObjectIdForNewBusinessTransaction_ShouldThrowException()
    {
        await TestSendMessageAndFilesToGlobal_NullOrEmptyObjectIdForNewBusinessTransaction_ShouldThrowException(
            null);
    }

    [Fact]
    public async Task SendMessageAndFilesToGlobal_EmptyObjectIdForNewBusinessTransaction_ShouldThrowException()
    {
        await TestSendMessageAndFilesToGlobal_NullOrEmptyObjectIdForNewBusinessTransaction_ShouldThrowException(
            string.Empty);
    }

    [Fact]
    public async Task SendMessageAndFilesToGlobal_ImageFile_ShouldSucceed()
    {
        // Arrange
        var mockDocumentAccessor = new Mock<IDocumentAccessor>(MockBehavior.Strict);
        mockDocumentAccessor
            .Setup(documentAccessor => documentAccessor.SendDocuments(It.IsAny<DTOs.ActionRequest>()));

        var mockGlobalDataAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
        mockGlobalDataAccessor
            .Setup(globalDataAccessor => globalDataAccessor.GetObjectIdForNewBusinessTransaction(
                It.IsAny<string>()))
            .ReturnsAsync(ObjectIdForNewBusinessTransaction);

        var mockSendEngine = new Mock<ISendEngine>(MockBehavior.Strict);
        mockSendEngine
            .Setup(sendEngine => sendEngine.CreateImageFiles(It.IsAny<List<File>>()))
            .ReturnsAsync(new List<DTOs.File>
            {
                new DTOs.File
                {
                    Name = "SomeImage.png",
                    TiffImageBase64Content = TiffImageBase64Content
                }
            });

        var sendManager = new SendManager(
            mockDocumentAccessor.Object,
            mockGlobalDataAccessor.Object,
            mockSendEngine.Object);

        var actionRequest = new ActionRequest
        {
            AgentId = "1B14",
            Files = new List<File>
            {
                new File
                {
                    Bytes = Array.Empty<byte>(),
                    FileType = FileType.Png,
                    Name = "SomeImage.png"
                }
            },
            PolicyNumber = "1234567890"
        };

        // Act
        await sendManager.SendMessageAndFilesToGlobal(actionRequest);

        // Assert
        mockDocumentAccessor.Verify(
            documentAccessor => documentAccessor.SendDocuments(
                It.Is<DTOs.ActionRequest>(actionRequestDto => IsExpectedActionRequestDto(
                    actionRequest,
                    actionRequestDto))),
            Times.Once);
    }

    [Fact]
    public async Task SendMessageAndFilesToGlobal_JsonMessage_ShouldCallCreateJsonMessageFile()
    {
        // Arrange
        var mockSendEngine = new Mock<ISendEngine>();
        var mockGlobalDataAccessor = new Mock<IGlobalDataAccessor>();
        var mockDocumentAccessor = new Mock<IDocumentAccessor>();

        var sendManager = new SendManager(mockDocumentAccessor.Object, mockGlobalDataAccessor.Object, mockSendEngine.Object);

        var jsonMessage = "{ \"key\": \"value\" }";
        var actionRequest = new ActionRequest
        {
            Message = jsonMessage,
            PolicyNumber = "123456"
        };

        // Set up mock for CreateJsonMessageFile to return a Task<DTOs.File>
        mockSendEngine
            .Setup(engine => engine.CreateJsonMessageFile(It.IsAny<string>()))
            .ReturnsAsync(new DTOs.File
            {
                // Set the properties here as needed
                IsMessage = true,
                TiffImageBase64Content = "someBase64Content"
            });

        mockGlobalDataAccessor
            .Setup(accessor => accessor.GetObjectIdForNewBusinessTransaction(It.IsAny<string>()))
            .ReturnsAsync(ObjectIdForNewBusinessTransaction);

        // Act
        await sendManager.SendMessageAndFilesToGlobal(actionRequest);

        // Assert
        mockSendEngine.Verify(engine => engine.CreateJsonMessageFile(jsonMessage), Times.Once);
        mockSendEngine.Verify(engine => engine.CreateMessageFile(It.IsAny<string>()), Times.Never);
        mockDocumentAccessor.Verify(accessor => accessor.SendDocuments(It.IsAny<DTOs.ActionRequest>()), Times.Once);
    }

    [Fact]
    public async Task SendMessageAndFilesToGlobal_MessageFile_ShouldSucceed()
    {
        // Arrange
        var mockDocumentAccessor = new Mock<IDocumentAccessor>(MockBehavior.Strict);
        mockDocumentAccessor
            .Setup(documentAccessor => documentAccessor.SendDocuments(It.IsAny<DTOs.ActionRequest>()));

        var mockGlobalDataAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
        mockGlobalDataAccessor
            .Setup(globalDataAccessor => globalDataAccessor.GetObjectIdForNewBusinessTransaction(It.IsAny<string>()))
            .ReturnsAsync(ObjectIdForNewBusinessTransaction);

        var mockSendEngine = new Mock<ISendEngine>(MockBehavior.Strict);
        mockSendEngine
            .Setup(sendEngine => sendEngine.CreateMessageFile(It.IsAny<string>()))
            .ReturnsAsync(new DTOs.File
            {
                IsMessage = true,
                TiffImageBase64Content = TiffImageBase64Content
            });

        var sendManager = new SendManager(
            mockDocumentAccessor.Object,
            mockGlobalDataAccessor.Object,
            mockSendEngine.Object);

        var actionRequest = new ActionRequest
        {
            AgentId = "1B14",
            Message = "Something",
            PolicyNumber = "1234567890"
        };

        // Act
        await sendManager.SendMessageAndFilesToGlobal(actionRequest);

        // Assert
        mockDocumentAccessor.Verify(
            documentAccessor => documentAccessor.SendDocuments(It.IsAny<DTOs.ActionRequest>()),
            Times.Once);
    }

    private static bool IsExpectedActionRequestDto(
        ActionRequest actionRequest,
        DTOs.ActionRequest actionRequestDto)
    {
        return actionRequestDto.AgentId == actionRequest.AgentId
            && (actionRequestDto.Files?.Count ?? 0) == 1
            && actionRequestDto.Files[0].IsMessage == !string.IsNullOrWhiteSpace(actionRequest.Message)
            && actionRequestDto.Files[0].TiffImageBase64Content == TiffImageBase64Content
            && actionRequestDto.ObjectIdForNewBusinessTransaction == ObjectIdForNewBusinessTransaction
            && actionRequestDto.PolicyNumber == actionRequest.PolicyNumber;
    }

    private static async Task TestSendMessageAndFilesToGlobal_NullOrEmptyObjectIdForNewBusinessTransaction_ShouldThrowException(
     string objectIdForNewBusinessTransaction)
    {
        // Arrange
        var mockGlobalDataAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
        mockGlobalDataAccessor
            .Setup(globalDataAccessor => globalDataAccessor.GetObjectIdForNewBusinessTransaction(It.IsAny<string>()))
            .ReturnsAsync(objectIdForNewBusinessTransaction);

        var mockSendEngine = new Mock<ISendEngine>(MockBehavior.Strict);
        mockSendEngine
            .Setup(sendEngine => sendEngine.CreateMessageFile(It.IsAny<string>()))
            .ReturnsAsync(new DTOs.File
            {
                IsMessage = true,
                TiffImageBase64Content = "someBase64Content"
            });

        var sendManager = new SendManager(
            null,
            mockGlobalDataAccessor.Object,
            mockSendEngine.Object);

        var actionRequest = new ActionRequest
        {
            AgentId = "1B14",
            Message = "Something",
            PolicyNumber = "1234567890"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            async () => await sendManager.SendMessageAndFilesToGlobal(actionRequest));

        var expectedMessage = "Unable to find ObjectId for NewBusiness Transaction " +
                "(a.k.a. GlobalData.dbo.Attributes.ObjectId) associated with " +
                $"{nameof(actionRequest.PolicyNumber)}: {actionRequest.PolicyNumber}.";

        Assert.Equal(expectedMessage, exception.Message);
    }
}