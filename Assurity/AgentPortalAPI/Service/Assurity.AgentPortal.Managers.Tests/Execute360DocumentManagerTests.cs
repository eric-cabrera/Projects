namespace Assurity.AgentPortal.Managers.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using Assurity.AgentPortal.Accessors.DTOs;
    using Assurity.AgentPortal.Accessors.PolicyInfo;
    using Assurity.AgentPortal.Accessors.Send;
    using Assurity.AgentPortal.Contracts.Shared;
    using Assurity.AgentPortal.Managers.PolicyInfo;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class Execute360DocumentManagerTests
    {
        [Fact]
        public async Task GetApplication_FoundPolicyNumber_ShouldReturn_FileResponse()
        {
            // Arrange
            var policyNumber = "1234567890";

            var attributeObjects = new List<AttributeObject>
            {
                new AttributeObject
                {
                    ObjectClass = "DSCAN01",
                    ObjectId = "13058DEV1000020"
                }
            };

            var mockGlobalDataAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            mockGlobalDataAccessor.Setup(accessor => accessor.GetApplicationData(policyNumber))
                .ReturnsAsync(attributeObjects);

            var mockDocumentServiceAccessor = new Mock<IDocumentServiceAccessor>(MockBehavior.Strict);
            mockDocumentServiceAccessor.Setup(accessor => accessor.GetImageByIdAsync(policyNumber, "DSCAN01"))
                .ReturnsAsync(new byte[100]);

            var mockLogger = new Mock<ILogger<Execute360DocumentManager>>();

            var execute360DocumentManager = new Execute360DocumentManager(mockGlobalDataAccessor.Object, mockDocumentServiceAccessor.Object, mockLogger.Object);

            // Act
            var response = await execute360DocumentManager.GetApplication(policyNumber);

            // Assert
            Assert.NotNull(response);
            Assert.IsType<FileResponse>(response);
            Assert.IsType<byte[]>(response.FileData);
            Assert.True(response.FileData.Length > 0);
            Assert.Equal("application-1234567890.pdf", response.FileName);
            Assert.Equal("application/pdf", response.FileType);
        }

        [Fact]
        public async Task GetApplication_NotFoundPolicyNumber_ShouldReturnNull()
        {
            // Arrange
            var policyNumber = "1234567890";

            var attributeObjects = new List<AttributeObject>
            {
                new AttributeObject
                {
                    ObjectClass = "DSCAN01",
                    ObjectId = "13058DEV1000021"
                }
            };

            var mockGlobalDataAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            mockGlobalDataAccessor.Setup(accessor => accessor.GetApplicationData(policyNumber))
                .ReturnsAsync(() => null);

            var mockDocumentServiceAccessor = new Mock<IDocumentServiceAccessor>(MockBehavior.Strict);

            var mockLogger = new Mock<ILogger<Execute360DocumentManager>>();

            var execute360DocumentManager = new Execute360DocumentManager(mockGlobalDataAccessor.Object, mockDocumentServiceAccessor.Object, mockLogger.Object);

            // Act
            var response = await execute360DocumentManager.GetApplication(policyNumber);

            // Assert
            Assert.Null(response);
        }

        [Fact]
        public async Task GetApplication_MultipleAttributeObjects_For_PolicyNumber_ShouldReturnNull()
        {
            // Arrange
            var policyNumber = "1234567890";

            var attributeObjects = new List<AttributeObject>
            {
                new AttributeObject
                {
                    ObjectClass = "DSCAN01",
                    ObjectId = "13058DEV1000021"
                },
                new AttributeObject
                {
                    ObjectClass = "DSCAN01",
                    ObjectId = "13058DEV1000022"
                }
            };

            var mockGlobalDataAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            mockGlobalDataAccessor.Setup(accessor => accessor.GetApplicationData(policyNumber))
                .ReturnsAsync(attributeObjects);

            var mockDocumentServiceAccessor = new Mock<IDocumentServiceAccessor>(MockBehavior.Strict);

            var mockLogger = new Mock<ILogger<Execute360DocumentManager>>();

            var execute360DocumentManager = new Execute360DocumentManager(mockGlobalDataAccessor.Object, mockDocumentServiceAccessor.Object, mockLogger.Object);

            // Act
            var response = await execute360DocumentManager.GetApplication(policyNumber);

            // Assert
            Assert.Null(response);
        }
    }
}
