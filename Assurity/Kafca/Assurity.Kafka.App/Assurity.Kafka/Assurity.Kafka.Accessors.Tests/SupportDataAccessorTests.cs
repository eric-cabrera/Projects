namespace Assurity.Kafka.Accessors.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Context;
    using Assurity.Kafka.Accessors.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class SupportDataAccessorTests
    {
        private readonly DbContextOptions<SupportDataContext> supportDataInMemoryOptions =
            new DbContextOptionsBuilder<SupportDataContext>()
            .UseInMemoryDatabase(databaseName: "SupportDataDatabase")
            .Options;

        private SupportDataContext SupportDataContext => new SupportDataContext(supportDataInMemoryOptions);

        [TestCleanup]
        public void Dispose()
        {
            SupportDataContext.Dispose();
            SupportDataContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task GetJustInTimeAgentIds_Success()
        {
            // Arrange
            var mockSupportDataDbContextFactory = GetMockSupportDataContextFactory();

            var supportDataAccessor = new SupportDataAccessor(mockSupportDataDbContextFactory.Object);

            // Act
            var result = await supportDataAccessor.GetQueueDescriptions();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 3);
        }

        [TestMethod]
        public async Task CheckIfQueueExists_ShouldReturn_True()
        {
            // Arrange
            var mockSupportDataDbContextFactory = GetMockSupportDataContextFactory();

            var supportDataAccessor = new SupportDataAccessor(mockSupportDataDbContextFactory.Object);
            var queueName = "abc123";

            // Act
            var result = await supportDataAccessor.IsJustInTimeQueue(queueName);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task CheckIfQueueExists_ShouldReturn_False()
        {
            // Arrange
            var mockSupportDataDbContextFactory = GetMockSupportDataContextFactory();

            var supportDataAccessor = new SupportDataAccessor(mockSupportDataDbContextFactory.Object);
            var queueName = "babc123";

            // Act
            var result = await supportDataAccessor.IsJustInTimeQueue(queueName);

            // Assert
            Assert.IsFalse(result);
        }

        private void SeedData(SupportDataContext context)
        {
            context.AgentUseQueues.AddRange(
                new List<AgentUseQueue>
                {
                    new AgentUseQueue
                    {
                        QueueID = 1,
                        QueueDescription = "abc123"
                    },
                    new AgentUseQueue
                    {
                        QueueID = 2,
                        QueueDescription = "123abc",
                    },
                    new AgentUseQueue
                    {
                        QueueID = 3,
                        QueueDescription = "123456",
                    },
                });

            context.SaveChanges();
        }

        private IMock<IDbContextFactory<SupportDataContext>> GetMockSupportDataContextFactory()
        {
            var mockDbContextFactory = new Mock<IDbContextFactory<SupportDataContext>>(MockBehavior.Strict);
            mockDbContextFactory
                .Setup(dbContextFactory => dbContextFactory.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(SupportDataContext);

            SeedData(SupportDataContext);

            return mockDbContextFactory;
        }
    }
}
