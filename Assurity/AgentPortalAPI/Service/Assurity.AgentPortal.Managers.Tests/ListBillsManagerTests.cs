namespace Assurity.AgentPortal.Managers.Tests;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Accessors.ListBill;
using Assurity.AgentPortal.Contracts.Shared;
using Assurity.AgentPortal.Engines;
using Assurity.AgentPortal.Managers.ListBill;
using Assurity.AgentPortal.Managers.ListBill.Mapping;
using Assurity.ListBill.Service.Contracts;
using AutoBogus;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class ListBillsManagerTests
{
    private readonly Mapper mapper;

    public ListBillsManagerTests()
    {
        var mapperProfiles = new List<Profile>
        {
            new GroupsResponseMappingProfile(),
            new ListBillsResponseMappingProfile()
        };

        var mapperConfig = new MapperConfiguration(config => config.AddProfiles(mapperProfiles));
        mapper = new Mapper(mapperConfig);
    }

    [Fact]
    public async Task GetGroupsAsync_Success()
    {
        // Arrange
        var agentId = "ABC1";
        var page = 1;
        var pageSize = 10;

        var mockPdfUtilities = new Mock<IPdfUtilitiesEngine>();
        var groupsResponse = new AutoFaker<GroupsResponse>().Generate();
        var mockListBillApiAccessor = new Mock<IListBillsApiAccessor>();
        mockListBillApiAccessor.Setup(accessor => accessor.GetGroups(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(groupsResponse)
            .Verifiable();

        var listBillManager = new ListBillsManager(mockListBillApiAccessor.Object, mockPdfUtilities.Object, mapper);

        // Act
        var response = await listBillManager.GetGroups(agentId, page, pageSize, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        mockListBillApiAccessor.Verify(
            accessor => accessor.GetGroups(
            agentId,
            page,
            pageSize,
            CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task GetGroupsAsync_NullResponse_ShouldReturn_Null()
    {
        // Arrange
        var agentId = "ABC1";
        var page = 1;
        var pageSize = 10;

        var mockPdfUtilities = new Mock<IPdfUtilitiesEngine>();
        var mockLogger = new Mock<ILogger<ListBillsManager>>(MockBehavior.Loose);
        var mockListBillApiAccessor = new Mock<IListBillsApiAccessor>(MockBehavior.Strict);
        mockListBillApiAccessor.Setup(accessor => accessor.GetGroups(
                agentId,
                page,
                pageSize,
                CancellationToken.None))
            .ReturnsAsync(() => null);

        var listBillManager = new ListBillsManager(mockListBillApiAccessor.Object, mockPdfUtilities.Object, mapper);

        // Act
        var response = await listBillManager.GetGroups(
            agentId,
            page,
            pageSize,
            CancellationToken.None);

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public async Task GetListBillsAsync_Success()
    {
        // Arrange
        var groupId = "12345678";

        var mockPdfUtilities = new Mock<IPdfUtilitiesEngine>();
        var mockLogger = new Mock<ILogger<ListBillsManager>>(MockBehavior.Loose);
        var listBillsResponse = new AutoFaker<ListBillResponse>().Generate();
        var mockListBillApiAccessor = new Mock<IListBillsApiAccessor>();
        mockListBillApiAccessor.Setup(accessor => accessor.GetListBills(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(listBillsResponse)
            .Verifiable();

        var listBillsManager = new ListBillsManager(mockListBillApiAccessor.Object, mockPdfUtilities.Object, mapper);

        // Act
        var response = await listBillsManager.GetListBills(groupId, CancellationToken.None);

        // Assert
        Assert.NotNull(response);

        mockListBillApiAccessor.Verify(
            accessor => accessor.GetListBills(
                groupId, CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task GetListBillsAsync_NullResponse_ShouldReturn_Null()
    {
        // Arrange
        var groupId = "12345678";

        var mockPdfUtilities = new Mock<IPdfUtilitiesEngine>();
        var mockLogger = new Mock<ILogger<ListBillsManager>>(MockBehavior.Loose);
        var mockListBillsApiAccessor = new Mock<IListBillsApiAccessor>(MockBehavior.Strict);
        mockListBillsApiAccessor.Setup(accessor => accessor.GetListBills(groupId, CancellationToken.None))
            .ReturnsAsync(() => null);

        var listBillsManager = new ListBillsManager(mockListBillsApiAccessor.Object, mockPdfUtilities.Object, mapper);

        // Act
        var response = await listBillsManager.GetListBills(groupId, CancellationToken.None);

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public async Task GetListBillData_Success_ShouldReturnFileResponse()
    {
        // Arrange
        var listBillId = "24211DEV1000050";

        var mockPdfUtilities = new Mock<IPdfUtilitiesEngine>();
        mockPdfUtilities.Setup(accessor => accessor.EnsurePortraitOrientation(
                It.IsAny<MemoryStream>()))
            .Returns(new MemoryStream())
            .Verifiable();

        var mockListBillApiAccessor = new Mock<IListBillsApiAccessor>();
        mockListBillApiAccessor.Setup(accessor => accessor.GetListBillData(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemoryStream())
            .Verifiable();

        var listBillManager = new ListBillsManager(
            mockListBillApiAccessor.Object,
            mockPdfUtilities.Object,
            mapper);

        // Act
        var response = await listBillManager.GetListBillData(listBillId, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        mockListBillApiAccessor.Verify(
            accessor => accessor.GetListBillData(
                listBillId, CancellationToken.None),
            Times.Once);
        Assert.IsType<FileResponse>(response);
        Assert.Equal("application/pdf", response.FileType);
    }

    [Fact]
    public async Task GetListBillData_NullResponseStream_ShouldReturnNull()
    {
        // Arrange
        var listBillId = "18297TEST10000A7";

        var mockPdfUtilities = new Mock<IPdfUtilitiesEngine>();
        mockPdfUtilities.Setup(accessor => accessor.EnsurePortraitOrientation(
                It.IsAny<MemoryStream>()))
            .Returns(new MemoryStream())
            .Verifiable();

        var mockListBillApiAccessor = new Mock<IListBillsApiAccessor>();
        mockListBillApiAccessor.Setup(accessor => accessor.GetListBillData(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Stream.Null)
            .Verifiable();

        var listBillsManager = new ListBillsManager(
            mockListBillApiAccessor.Object,
            mockPdfUtilities.Object,
            mapper);

        // Act
        var response = await listBillsManager.GetListBillData(listBillId, CancellationToken.None);

        // Assert
        Assert.Null(response);
    }
}