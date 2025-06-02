namespace Assurity.AgentPortal.Accessors.Tests
{
    using Assurity.AgentPortal.Accessors.DataStore.Context;
    using Assurity.AgentPortal.Accessors.Integration;
    using Assurity.AgentPortal.Accessors.Tests.TestData;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    public class IntegrationAccessorTests
    {
        private readonly DbContextOptions<DataStoreContext> dataStoreInMemoryOptions =
        new DbContextOptionsBuilder<DataStoreContext>()
            .UseInMemoryDatabase(databaseName: "IntegrationDatabase")
            .Options;

        private DataStoreContext DataStoreContext => new(dataStoreInMemoryOptions);

        [Fact]
        public async Task GetFiservDistributionChannelForLifePortraits_ShouldReturn_DistChannel()
        {
            // Arrange
            var dataStoreAccessor = GetDataStoreAccessor();
            var marketCodes = new List<string> { "IS" };

            // Act
            var distChannel = await dataStoreAccessor
                .GetFiservDistributionChannelForLifePortraits(marketCodes);

            // Assert
            Assert.Equal("IS", distChannel);

            CleanUpDatabase();
        }

        [Fact]
        public async Task GetFiservDistributionChannelForLifePortraits_ShouldReturn_DEFAULT_DistChannel()
        {
            // Arrange
            var dataStoreAccessor = GetDataStoreAccessor();
            var marketCodes = new List<string> { "WSRN1" };

            // Act
            var distChannel = await dataStoreAccessor
                .GetFiservDistributionChannelForLifePortraits(marketCodes);

            // Assert
            Assert.Equal(string.Empty, distChannel);

            CleanUpDatabase();
        }

        [Fact]
        public async Task GetFiservDistributionChannelForLifePortraits_ShouldReturn_IS_DistChannel()
        {
            // Arrange
            var dataStoreAccessor = GetDataStoreAccessor();
            var marketCodes = new List<string> { "WSRN1", "IS" };

            // Act
            var distChannel = await dataStoreAccessor
                .GetFiservDistributionChannelForLifePortraits(marketCodes);

            // Assert
            Assert.Equal("IS", distChannel);

            CleanUpDatabase();
        }

        private static void SeedData(DataStoreContext context)
        {
            context.FiservDistributionChannel.AddRange(IntegrationAccessorTestData.FiservDistributionChannel);
            context.SaveChanges();
        }

        private void CleanUpDatabase()
        {
            DataStoreContext.Dispose();
            DataStoreContext.Database.EnsureDeleted();
        }

        private IntegrationAccessor GetDataStoreAccessor()
        {
            var mockDbContextFactory = new Mock<IDbContextFactory<DataStoreContext>>(MockBehavior.Strict);
            mockDbContextFactory
                .Setup(dbContextFactory => dbContextFactory.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(DataStoreContext);

            SeedData(DataStoreContext);

            return new IntegrationAccessor(mockDbContextFactory.Object);
        }
    }
}
