namespace Assurity.Kafka.Managers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Managers.Tests.Extensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mongo2Go;
    using MongoDB.Driver;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PBDRVEventManagerTests
    {
        private static MongoDbRunner? mongoRunner; // In memory mongo DB for unit testing

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            mongoRunner = MongoDbRunner.Start(singleNodeReplSet: true);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            mongoRunner.Dispose();
        }

        [TestMethod]
        public async Task ProcessEvent_PoliciesAreNull_ShouldLogWarningAndReturnEarly()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PBDRVEventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePastDuePoliciesAsync(
                    It.IsAny<List<string>>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetInitialPaymentDeclinedPoliciesWithPassedRetentionDurationAsync(
                    It.IsAny<DateTime>()))
                .ReturnsAsync(null as List<Policy>);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyNumbersForDeletion(It.IsAny<DateTime>()))
                .Returns(null as List<string>);

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(accessor => accessor.GetPastDuePolicyNumbers(It.IsAny<int>()))
                .ReturnsAsync(null as List<string>);

            var pbdrvEventManager = new PBDRVEventManager(
                mockLogger.Object,
                mockEventsAccessor.Object,
                mockLifeProAccessor.Object,
                MockHelpers.GetMockMongoClient().Object);

            var pbdrv = new PBDRV
            {
                DESCRIPTION = "YEAREND",
                STATUS_CODE = "B",
                BATCH_START_DATE = 5132023,
                BATCH_STOP_DATE = 5152023
            };

            // Act
            await pbdrvEventManager.ProcessEvent(pbdrv);

            // Assert
            var expectedMessage = "No policies were found in Mongo for the LifePro " +
                $"policies associated with PaidToDate < {nameof(pbdrv.BATCH_START_DATE)}: {pbdrv.BATCH_START_DATE}";

            mockLogger.VerifyLog(LogLevel.Warning, expectedMessage);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePastDuePoliciesAsync(
                    It.IsAny<List<string>>()),
                Times.Never);

            mockLifeProAccessor.Verify(
                accessor => accessor.GetPastDuePolicyNumbers(
                    It.IsAny<int>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_ShouldUpdatePastDuePolicies()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PBDRVEventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePastDuePoliciesAsync(
                    It.IsAny<List<string>>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetInitialPaymentDeclinedPoliciesWithPassedRetentionDurationAsync(
                    It.IsAny<DateTime>()))
                .ReturnsAsync(() => null);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyNumbersForDeletion(It.IsAny<DateTime>()))
                .Returns(() => null);

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(accessor => accessor.GetPastDuePolicyNumbers(It.IsAny<int>()))
                .ReturnsAsync(new List<string> { "123456789" });

            var pbdrvEventManager = new PBDRVEventManager(
                mockLogger.Object,
                mockEventsAccessor.Object,
                mockLifeProAccessor.Object,
                MockHelpers.GetMockMongoClient().Object);

            var pbdrv = new PBDRV
            {
                DESCRIPTION = "YEAREND",
                STATUS_CODE = "B",
                BATCH_START_DATE = 5132023,
                BATCH_STOP_DATE = 5152023
            };

            // Act
            await pbdrvEventManager.ProcessEvent(pbdrv);

            // Assert
            mockLifeProAccessor.Verify(
                accessor => accessor.GetPastDuePolicyNumbers(
                    It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePastDuePoliciesAsync(
                    It.IsAny<List<string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_ShouldDeletePassedRetentionDurationPolicies()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PBDRVEventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePastDuePoliciesAsync(
                    It.IsAny<List<string>>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetInitialPaymentDeclinedPoliciesWithPassedRetentionDurationAsync(
                    It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Policy> { new Policy { PolicyNumber = "123456789" } });

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyNumbersForDeletion(It.IsAny<DateTime>()))
                .Returns(() => null);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.DeletePoliciesAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.DeletePolicyHierarchiesAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdateAgentPolicyAccessListAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()))
                .ReturnsAsync(1);

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(accessor => accessor.GetPastDuePolicyNumbers(It.IsAny<int>()))
                .ReturnsAsync(() => null);

            var pbdrvEventManager = new PBDRVEventManager(
                mockLogger.Object,
                mockEventsAccessor.Object,
                mockLifeProAccessor.Object,
                MockHelpers.GetMockMongoClient().Object);

            var pbdrv = new PBDRV
            {
                DESCRIPTION = "YEAREND",
                STATUS_CODE = "B",
                BATCH_START_DATE = 5132023,
                BATCH_STOP_DATE = 5152023
            };

            // Act
            await pbdrvEventManager.ProcessEvent(pbdrv);

            // Assert
            mockLifeProAccessor.Verify(
                accessor => accessor.GetPastDuePolicyNumbers(
                    It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePastDuePoliciesAsync(
                    It.IsAny<List<string>>()),
                Times.Never);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.DeletePoliciesAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.DeletePolicyHierarchiesAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdateAgentPolicyAccessListAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_ShouldDeleteOlderTerminationDatePolicies()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PBDRVEventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePastDuePoliciesAsync(
                    It.IsAny<List<string>>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetInitialPaymentDeclinedPoliciesWithPassedRetentionDurationAsync(
                    It.IsAny<DateTime>()))
                .ReturnsAsync(() => null);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyNumbersForDeletion(It.IsAny<DateTime>()))
                .Returns(new List<string> { "123456789" });

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.DeletePoliciesAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.DeletePolicyHierarchiesAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdateAgentPolicyAccessListAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()))
                .ReturnsAsync(1);

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(accessor => accessor.GetPastDuePolicyNumbers(It.IsAny<int>()))
                .ReturnsAsync(() => null);

            var pbdrvEventManager = new PBDRVEventManager(
                mockLogger.Object,
                mockEventsAccessor.Object,
                mockLifeProAccessor.Object,
                MockHelpers.GetMockMongoClient().Object);

            var pbdrv = new PBDRV
            {
                DESCRIPTION = "YEAREND",
                STATUS_CODE = "B",
                BATCH_START_DATE = 5132023,
                BATCH_STOP_DATE = 5152023
            };

            // Act
            await pbdrvEventManager.ProcessEvent(pbdrv);

            // Assert
            mockLifeProAccessor.Verify(
                accessor => accessor.GetPastDuePolicyNumbers(
                    It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePastDuePoliciesAsync(
                    It.IsAny<List<string>>()),
                Times.Never);

            mockEventsAccessor.Verify(
               eventsAccessor => eventsAccessor.GetInitialPaymentDeclinedPoliciesWithPassedRetentionDurationAsync(
                   It.IsAny<DateTime>()),
               Times.Once);

            mockEventsAccessor.Verify(
               eventsAccessor => eventsAccessor.GetPolicyNumbersForDeletion(
                   It.IsAny<DateTime>()),
               Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.DeletePoliciesAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.DeletePolicyHierarchiesAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdateAgentPolicyAccessListAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_ShouldDeletePassedRetentionDuration_And_OlderTerminationDatePolicies()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PBDRVEventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePastDuePoliciesAsync(
                    It.IsAny<List<string>>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetInitialPaymentDeclinedPoliciesWithPassedRetentionDurationAsync(
                    It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Policy> { new Policy { PolicyNumber = "123456780" } });

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyNumbersForDeletion(It.IsAny<DateTime>()))
                .Returns(new List<string> { "123456789" });

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.DeletePoliciesAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.DeletePolicyHierarchiesAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdateAgentPolicyAccessListAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()))
                .ReturnsAsync(1);

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(accessor => accessor.GetPastDuePolicyNumbers(It.IsAny<int>()))
                .ReturnsAsync(() => null);

            var pbdrvEventManager = new PBDRVEventManager(
                mockLogger.Object,
                mockEventsAccessor.Object,
                mockLifeProAccessor.Object,
                MockHelpers.GetMockMongoClient().Object);

            var pbdrv = new PBDRV
            {
                DESCRIPTION = "YEAREND",
                STATUS_CODE = "B",
                BATCH_START_DATE = 5132023,
                BATCH_STOP_DATE = 5152023
            };

            // Act
            await pbdrvEventManager.ProcessEvent(pbdrv);

            // Assert
            mockLifeProAccessor.Verify(
                accessor => accessor.GetPastDuePolicyNumbers(
                    It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePastDuePoliciesAsync(
                    It.IsAny<List<string>>()),
                Times.Never);

            mockEventsAccessor.Verify(
               eventsAccessor => eventsAccessor.GetInitialPaymentDeclinedPoliciesWithPassedRetentionDurationAsync(
                   It.IsAny<DateTime>()),
               Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyNumbersForDeletion(It.IsAny<DateTime>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.DeletePoliciesAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()),
                Times.Exactly(2));

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.DeletePolicyHierarchiesAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()),
                Times.Exactly(2));

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdateAgentPolicyAccessListAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()),
                Times.Exactly(2));
        }

        [TestMethod]
        public async Task ProcessEvent_Should_UpdatePastDuePolicies_DeletePassedRetentionDuration_And_OlderTerminationDatePolicies()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PBDRVEventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePastDuePoliciesAsync(
                    It.IsAny<List<string>>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetInitialPaymentDeclinedPoliciesWithPassedRetentionDurationAsync(
                    It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Policy> { new Policy { PolicyNumber = "123456780" } });

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyNumbersForDeletion(It.IsAny<DateTime>()))
                .Returns(new List<string> { "123456789" });

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.DeletePoliciesAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.DeletePolicyHierarchiesAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdateAgentPolicyAccessListAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()))
                .ReturnsAsync(1);

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(accessor => accessor.GetPastDuePolicyNumbers(It.IsAny<int>()))
                .ReturnsAsync(new List<string> { "123456789" });

            var pbdrvEventManager = new PBDRVEventManager(
                mockLogger.Object,
                mockEventsAccessor.Object,
                mockLifeProAccessor.Object,
                MockHelpers.GetMockMongoClient().Object);

            var pbdrv = new PBDRV
            {
                DESCRIPTION = "YEAREND",
                STATUS_CODE = "B",
                BATCH_START_DATE = 5132023,
                BATCH_STOP_DATE = 5152023
            };

            // Act
            await pbdrvEventManager.ProcessEvent(pbdrv);

            // Assert
            mockLifeProAccessor.Verify(
                accessor => accessor.GetPastDuePolicyNumbers(
                    It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePastDuePoliciesAsync(
                    It.IsAny<List<string>>()),
                Times.Once);

            mockEventsAccessor.Verify(
               eventsAccessor => eventsAccessor.GetInitialPaymentDeclinedPoliciesWithPassedRetentionDurationAsync(
                   It.IsAny<DateTime>()),
               Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyNumbersForDeletion(It.IsAny<DateTime>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.DeletePoliciesAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()),
                Times.Exactly(2));

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.DeletePolicyHierarchiesAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()),
                Times.Exactly(2));

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdateAgentPolicyAccessListAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<List<string>>()),
                Times.Exactly(2));
        }
    }
}