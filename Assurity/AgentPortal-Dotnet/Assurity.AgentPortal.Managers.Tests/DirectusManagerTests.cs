namespace Assurity.AgentPortal.Managers.Tests;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Accessors;
using Assurity.AgentPortal.Accessors.DirectusDTOs;
using AutoBogus;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class DirectusManagerTests
{
    private readonly Mock<IDirectusAccessor> _mockDirectusAccessor;
    private readonly Mock<IUserDataAccessor> _mockUserDataAccessor;

    public DirectusManagerTests()
    {
        _mockDirectusAccessor = new Mock<IDirectusAccessor>();
        _mockUserDataAccessor = new Mock<IUserDataAccessor>();
    }

    [Fact]
    public async Task GetTemporaryMessages_Success()
    {
        var temporaryMessages = new List<Contracts.Directus.TemporaryMessage>
        {
            new Contracts.Directus.TemporaryMessage 
            {
                Id = 1, Heading = "Heading1",
                Message = "Message1",
                PublishDate = DateTime.Now,
                CtaLabel = "Label1",
                CtaLink = "Link1"
            },
            new Contracts.Directus.TemporaryMessage
            {
                Id = 2,
                Heading = "Heading2",
                Message = "Message2",
                PublishDate = DateTime.Now,
                CtaLabel = "Label2", CtaLink = "Link2"
            }
        };

        var temporaryMessagesResponse = new AutoFaker<TemporaryMessagesQueryResponse>().Generate();
        _mockDirectusAccessor.Setup(accessor => accessor.GetTemporaryMessages())
            .ReturnsAsync(temporaryMessagesResponse)
            .Verifiable();

        var directusManager = new DirectusManager(null, _mockDirectusAccessor.Object, _mockUserDataAccessor.Object);

        // Act
        var response = await directusManager.GetTemporaryMessages();

        // Assert
        Assert.NotNull(response);
        _mockDirectusAccessor.Verify(accessor => accessor.GetTemporaryMessages(), Times.Once());
    }

    [Fact]
    public async Task GetTemporaryMessages_ReturnsNull_WhenResponseIsNull()
    {
        // Arrange
        _mockDirectusAccessor.Setup(x => x.GetTemporaryMessages()).ReturnsAsync((TemporaryMessagesQueryResponse)null);
        var directusManager = new DirectusManager(null, _mockDirectusAccessor.Object, _mockUserDataAccessor.Object);

        // Act
        var result = await directusManager.GetTemporaryMessages();

        // Assert
        Assert.Null(result);
    }
}