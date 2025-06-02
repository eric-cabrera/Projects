namespace Assurity.AgentPortal.Accessors.Tests;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Accessors.GlobalData.Context;
using Assurity.AgentPortal.Accessors.Send;
using Assurity.AgentPortal.Accessors.Tests.TestData;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class GlobalDataAccessorTests
{
    private readonly DbContextOptions<GlobalDataContext> globalDataInMemoryOptions =
        new DbContextOptionsBuilder<GlobalDataContext>()
            .UseInMemoryDatabase(databaseName: "GlobalDataDatabase")
            .Options;

    private GlobalDataContext GlobalDataContext => new(globalDataInMemoryOptions);

    [Fact]
    public async Task GetObjectIdForNewBusinessTransaction_NoPolicy_ShouldReturnNull()
    {
        // Arrange
        var mockDbContextFactory = GetMockGlobalDataContextFactory();
        var globalDataAccessor = new GlobalDataAccessor(mockDbContextFactory.Object);

        // Act
        var objectIdForNewBusinessTransaction = await globalDataAccessor
            .GetObjectIdForNewBusinessTransaction("9997779999");

        // Assert
        Assert.Null(objectIdForNewBusinessTransaction);

        CleanUpDatabase();
    }

    [Fact]
    public async Task GetObjectIdForNewBusinessTransaction_HasPolicy_ShouldReturnObjectIdForNewBusinessTransaction()
    {
        // Arrange
        var mockDbContextFactory = GetMockGlobalDataContextFactory();
        var globalDataAccessor = new GlobalDataAccessor(mockDbContextFactory.Object);

        // Act
        var objectIdForNewBusinessTransaction = await globalDataAccessor
            .GetObjectIdForNewBusinessTransaction("1234567890");

        // Assert
        Assert.Equal("13058DEV1000020", objectIdForNewBusinessTransaction);

        CleanUpDatabase();
    }

    [Fact]
    public async Task GetObjectIdForNewBusinessTransaction_HasPolicy_ExtraSpacing_ShouldReturnTrimmedObjectIdForNewBusinessTransaction()
    {
        // Arrange
        var mockDbContextFactory = GetMockGlobalDataContextFactory();
        var globalDataAccessor = new GlobalDataAccessor(mockDbContextFactory.Object);

        // Act
        var objectIdForNewBusinessTransaction = await globalDataAccessor
            .GetObjectIdForNewBusinessTransaction("9999999992");

        // Assert
        Assert.Equal("1701DDEV1000010", objectIdForNewBusinessTransaction);

        CleanUpDatabase();
    }

    [Fact]
    public async Task GetApplicationData_NoPolicy_ShouldReturn_Null()
    {
        // Arrange
        var mockDbContextFactory = GetMockGlobalDataContextFactory();
        var globalDataAccessor = new GlobalDataAccessor(mockDbContextFactory.Object);

        // Act
        var attributeObjects = await globalDataAccessor
            .GetApplicationData("1234567880");

        // Assert
        Assert.Null(attributeObjects);

        CleanUpDatabase();
    }

    [Fact]
    public async Task GetApplicationData_HasPolicy_ShouldReturn_AttributeObjects()
    {
        // Arrange
        var mockDbContextFactory = GetMockGlobalDataContextFactory();
        var globalDataAccessor = new GlobalDataAccessor(mockDbContextFactory.Object);

        // Act
        var attributeObjects = await globalDataAccessor
            .GetApplicationData("1234567890");

        // Assert
        Assert.Single(attributeObjects);
        Assert.Equal("13058DEV1000020", attributeObjects[0].ObjectId);
        Assert.Equal("DSCAN01", attributeObjects[0].ObjectClass);

        CleanUpDatabase();
    }

    [Fact]
    public async Task GetApplicationData_HasPolicy_ExtraSpacing_ShouldReturnTrimmedObjectId_In_AttributeObjects()
    {
        // Arrange
        var mockDbContextFactory = GetMockGlobalDataContextFactory();
        var globalDataAccessor = new GlobalDataAccessor(mockDbContextFactory.Object);

        // Act
        var attributeObjects = await globalDataAccessor
            .GetApplicationData("9999999992");

        // Assert
        Assert.Equal(2, attributeObjects.Count);
        Assert.Equal("1701DDEV1000010", attributeObjects[0].ObjectId);
        Assert.Equal("1701CDEV1000010", attributeObjects[1].ObjectId);

        CleanUpDatabase();
    }

    private static void SeedData(GlobalDataContext context)
    {
        context.Attributes.AddRange(GlobalDataAccessorTestData.TestAttributes);
        context.SaveChanges();
    }

    private void CleanUpDatabase()
    {
        GlobalDataContext.Dispose();
        GlobalDataContext.Database.EnsureDeleted();
    }

    private IMock<IDbContextFactory<GlobalDataContext>> GetMockGlobalDataContextFactory()
    {
        var mockDbContextFactory = new Mock<IDbContextFactory<GlobalDataContext>>(MockBehavior.Strict);
        mockDbContextFactory
            .Setup(dbContextFactory => dbContextFactory.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(GlobalDataContext);

        SeedData(GlobalDataContext);

        return mockDbContextFactory;
    }
}