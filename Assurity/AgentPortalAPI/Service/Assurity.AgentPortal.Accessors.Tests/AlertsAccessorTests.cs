namespace Assurity.AgentPortal.Accessors.Tests
{
    using Assurity.AgentPortal.Accessors.Alerts;
    using Assurity.AgentPortal.Accessors.AssureLink.Context;
    using Assurity.AgentPortal.Accessors.AssureLink.Entities;
    using Assurity.AgentPortal.Accessors.Tests.TestData;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    public class AlertsAccessorTests
    {
        private readonly DbContextOptions<AssureLinkContext> dataStoreInMemoryOptions =
        new DbContextOptionsBuilder<AssureLinkContext>()
            .UseInMemoryDatabase(databaseName: "AlertsDatabase")
            .Options;

        private AssureLinkContext AssureLinkContext => new(dataStoreInMemoryOptions);

        [Fact]
        public async Task GetDistributionListsByAgentId_ShouldReturn_DistributionList()
        {
            // Arrange
            var dataStoreAccessor = GetDataStoreAccessor();
            var agentId = "ABC1";

            // Act
            var distributionLists = await dataStoreAccessor.GetDistributionListsByAgentId(agentId, CancellationToken.None);

            // Assert
            Assert.NotNull(distributionLists);
            Assert.Single(distributionLists);
            Assert.Equal(1, distributionLists[0].Id);
            Assert.Equal(agentId, distributionLists[0].AgentId);
            Assert.Equal("ABC1@fake.com", distributionLists[0].Email);

            CleanUpDatabase();
        }

        [Fact]
        public async Task GetDistributionListsByAgentId_DoesNotExist_ShouldReturn_EmptyList()
        {
            // Arrange
            var dataStoreAccessor = GetDataStoreAccessor();
            var agentId = "DoesNotExist";

            // Act
            var distributionLists = await dataStoreAccessor.GetDistributionListsByAgentId(agentId, CancellationToken.None);

            // Assert
            Assert.Empty(distributionLists);

            CleanUpDatabase();
        }

        [Fact]
        public async Task DeleteDistributionList_ShouldReturn_NoExceptionsThrown()
        {
            // Arrange
            var dataStoreAccessor = GetDataStoreAccessor();
            var distributionListToRemove = new DistributionList
            {
                Id = 1,
                AgentId = "ABC1",
                Email = "ABC1@fake.com"
            };

            // Act
            await dataStoreAccessor.DeleteDistributionList(distributionListToRemove.Id, distributionListToRemove.AgentId, CancellationToken.None);

            CleanUpDatabase();
        }

        [Fact]
        public async Task DeleteDistributionList_DoesNotExist_ShouldThrowException()
        {
            // Arrange
            var dataStoreAccessor = GetDataStoreAccessor();
            var distributionListToRemove = new DistributionList
            {
                Id = 10,
                AgentId = "DoesNotExist",
                Email = "DoesNotExist@fake.com"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => dataStoreAccessor.DeleteDistributionList(distributionListToRemove.Id, distributionListToRemove.AgentId, CancellationToken.None));
            Assert.Equal("Unable to delete distribution list with the id 10; it does not exist.", exception.Message);

            CleanUpDatabase();
        }

        [Fact]
        public async Task AddDistributionList_ShouldReturn_NoExceptionsThrown()
        {
            // Arrange
            var dataStoreAccessor = GetDataStoreAccessor();
            var distributionListToAdd = new DistributionList
            {
                AgentId = "ABC2",
                Email = "ABC2@fake.com"
            };

            // Act
            await dataStoreAccessor.AddDistributionList(distributionListToAdd, CancellationToken.None);

            CleanUpDatabase();
        }

        [Fact]
        public async Task GetDistributionMasterByAgentId_ShouldReturn_DistributionList()
        {
            // Arrange
            var dataStoreAccessor = GetDataStoreAccessor();
            var agentId = "ABC1";

            // Act
            var distributionMaster = await dataStoreAccessor.GetDistributionMasterByAgentId(agentId, CancellationToken.None);

            // Assert
            Assert.NotNull(distributionMaster);
            Assert.Equal(1, distributionMaster.Id);
            Assert.Equal(agentId, distributionMaster.AgentId);
            Assert.True(distributionMaster.DisableAll);
            Assert.True(distributionMaster.SelfAdd);
            Assert.True(distributionMaster.SelfMet);
            Assert.True(distributionMaster.SelfOutstanding);
            Assert.True(distributionMaster.HierarchyAdd);
            Assert.True(distributionMaster.HierarchyMet);
            Assert.True(distributionMaster.HierarchyOutstanding);

            CleanUpDatabase();
        }

        [Fact]
        public async Task GetDistributionMasterByAgentId_DoesNotExist_ShouldReturn_Null()
        {
            // Arrange
            var dataStoreAccessor = GetDataStoreAccessor();
            var agentId = "DoesNotExist";

            // Act
            var distributionMaster = await dataStoreAccessor.GetDistributionMasterByAgentId(agentId, CancellationToken.None);

            // Assert
            Assert.Null(distributionMaster);

            CleanUpDatabase();
        }

        [Fact]
        public async Task AddDistributionMaster_ShouldReturn_NoExceptionsThrown()
        {
            // Arrange
            var dataStoreAccessor = GetDataStoreAccessor();
            var distributionMasterToAdd = new DistributionMaster
            {
                AgentId = "ABC2",
                DisableAll = true,
                SelfAdd = true,
                SelfMet = true,
                SelfOutstanding = true,
                HierarchyAdd = true,
                HierarchyMet = true,
                HierarchyOutstanding = true
            };

            // Act
            await dataStoreAccessor.UpsertDistributionMaster(distributionMasterToAdd.AgentId, distributionMasterToAdd, CancellationToken.None);

            // Assert
            var updatedDistributionMaster = await AssureLinkContext.DistributionMasters.Where(x => x.AgentId == distributionMasterToAdd.AgentId).FirstOrDefaultAsync();
            AssertDistributionMaster(updatedDistributionMaster, distributionMasterToAdd);

            CleanUpDatabase();
        }

        [Fact]
        public async Task UpdateDistributionMaster_ShouldReturn_NoExceptionsThrown()
        {
            // Arrange
            var dataStoreAccessor = GetDataStoreAccessor();
            var distributionMasterToAdd = new DistributionMaster
            {
                Id = 1,
                AgentId = "ABC1",
                DisableAll = false,
                SelfAdd = false,
                SelfMet = false,
                SelfOutstanding = false,
                HierarchyAdd = false,
                HierarchyMet = false,
                HierarchyOutstanding = false
            };

            // Act
            await dataStoreAccessor.UpsertDistributionMaster(distributionMasterToAdd.AgentId, distributionMasterToAdd, CancellationToken.None);

            // Assert
            var updatedDistributionMaster = await AssureLinkContext.DistributionMasters.Where(x => x.AgentId == distributionMasterToAdd.AgentId).FirstOrDefaultAsync();
            AssertDistributionMaster(updatedDistributionMaster, distributionMasterToAdd);
            CleanUpDatabase();
        }

        [Fact]
        public async Task DeleteDistributionMaster_ShouldReturn_NoExceptionsThrown()
        {
            // Arrange
            var dataStoreAccessor = GetDataStoreAccessor();
            var distributionMasterToRemove = new DistributionMaster
            {
                Id = 1,
                AgentId = "ABC1",
                DisableAll = true,
                SelfAdd = true,
                SelfMet = true,
                SelfOutstanding = true,
                HierarchyAdd = true,
                HierarchyMet = true,
                HierarchyOutstanding = true
            };

            // Act
            await dataStoreAccessor.DeleteDistributionMaster(distributionMasterToRemove, CancellationToken.None);

            CleanUpDatabase();
        }

        [Fact]
        public async Task DeleteDistributionMaster_DoesNotExist_ShouldThrowException()
        {
            // Arrange
            var dataStoreAccessor = GetDataStoreAccessor();
            var distributionMasterToRemove = new DistributionMaster
            {
                Id = 10,
                AgentId = "DoesNotExist",
                DisableAll = true,
                SelfAdd = true,
                SelfMet = true,
                SelfOutstanding = true,
                HierarchyAdd = true,
                HierarchyMet = true,
                HierarchyOutstanding = true
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => dataStoreAccessor.DeleteDistributionMaster(distributionMasterToRemove, CancellationToken.None));
            Assert.Equal("Attempted to update or delete an entity that does not exist in the store.", exception.Message);

            CleanUpDatabase();
        }

        private void AssertDistributionMaster(DistributionMaster distributionMaster, DistributionMaster distributionMaster2)
        {
            Assert.Equal(distributionMaster.SelfAdd, distributionMaster2.SelfAdd);
            Assert.Equal(distributionMaster.SelfMet, distributionMaster2.SelfMet);
            Assert.Equal(distributionMaster.SelfOutstanding, distributionMaster2.SelfOutstanding);
            Assert.Equal(distributionMaster.HierarchyAdd, distributionMaster2.HierarchyAdd);
            Assert.Equal(distributionMaster.HierarchyMet, distributionMaster2.HierarchyMet);
            Assert.Equal(distributionMaster.HierarchyOutstanding, distributionMaster2.HierarchyOutstanding);
            Assert.Equal(distributionMaster.DisableAll, distributionMaster2.DisableAll);
            Assert.Equal(distributionMaster.AgentId, distributionMaster2.AgentId);
        }

        private void CleanUpDatabase()
        {
            AssureLinkContext.Dispose();
            AssureLinkContext.Database.EnsureDeleted();
        }

        private void SeedData(AssureLinkContext context)
        {
            context.DistributionLists.AddRange(AlertsAccessorTestData.DistributionLists);
            context.DistributionMasters.AddRange(AlertsAccessorTestData.DistributionMasters);
            context.SaveChanges();
        }

        private AlertsAccessor GetDataStoreAccessor()
        {
            var mockDbContextFactory = new Mock<IDbContextFactory<AssureLinkContext>>(MockBehavior.Strict);
            mockDbContextFactory
                .Setup(dbContextFactory => dbContextFactory.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(AssureLinkContext);

            SeedData(AssureLinkContext);

            return new AlertsAccessor(mockDbContextFactory.Object);
        }
    }
}
