namespace Assurity.Kafka.Managers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PGRUP_GROUP_MASTEREventManagerTests
    {
        [TestMethod]
        public async Task ProcessEvent_ExceptionThrownRetrievingPolicies_ShouldNotUpdatePolicies()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PGRUP_GROUP_MASTEREventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesByGroupNumber(
                    It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetEmployer(
                    It.IsAny<string>(),
                    It.IsAny<string>()));

            var pgrupGroupMasterEventManager = new PGRUP_GROUP_MASTEREventManager(
                null,
                mockEventsAccessor.Object,
                mockLogger.Object);

            // Act
            await pgrupGroupMasterEventManager.ProcessEvent(new PGRUP_GROUP_MASTER
            {
                GROUP_NUMBER = "1234"
            });

            // Assert/Verify
            mockPolicyEngine
                .Verify(
                    policyEngine => policyEngine.GetEmployer(
                        It.IsAny<string>(),
                        It.IsAny<string>()),
                    Times.Never);

            mockEventsAccessor
                .Verify(
                    eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                        It.IsAny<Policy>(),
                        It.IsAny<object>(),
                        It.IsAny<string>()),
                    Times.Never);
        }

        [TestMethod]
        public async Task ProcessEvent_NoPoliciesReturnedFromMongo_ShouldNotUpdatePolicies()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PGRUP_GROUP_MASTEREventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesByGroupNumber(
                    It.IsAny<string>()))
                .ReturnsAsync(new List<Policy>());

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()));

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetEmployer(
                    It.IsAny<string>(),
                    It.IsAny<string>()));

            var pgrupGroupMasterEventManager = new PGRUP_GROUP_MASTEREventManager(
                null,
                mockEventsAccessor.Object,
                mockLogger.Object);

            // Act
            await pgrupGroupMasterEventManager.ProcessEvent(new PGRUP_GROUP_MASTER
            {
                GROUP_NUMBER = "1234"
            });

            // Assert/Verify
            mockPolicyEngine
                .Verify(
                    policyEngine => policyEngine.GetEmployer(
                        It.IsAny<string>(),
                        It.IsAny<string>()),
                    Times.Never);

            mockEventsAccessor
                .Verify(
                    eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                        It.IsAny<Policy>(),
                        It.IsAny<object>(),
                        It.IsAny<string>()),
                    Times.Never);
        }

        [TestMethod]
        public async Task ProcessEvent_PoliciesReturned_ShouldUpdateRecords()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PGRUP_GROUP_MASTEREventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesByGroupNumber(
                    It.IsAny<string>()))
                .ReturnsAsync(new List<Policy>
                {
                    new Policy
                    {
                        PolicyNumber = "1234567890",
                        CompanyCode = "01"
                    }
                });

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()))
                .ReturnsAsync(1);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetEmployer(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new Employer());

            var pgrupGroupMasterEventMangager = new PGRUP_GROUP_MASTEREventManager(
                mockPolicyEngine.Object,
                mockEventsAccessor.Object,
                mockLogger.Object);

            // Act
            await pgrupGroupMasterEventMangager.ProcessEvent(new PGRUP_GROUP_MASTER
            {
                GROUP_NUMBER = "1234"
            });

            // Assert/Verify
            mockEventsAccessor
                .Verify(
                    eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                        It.IsAny<Policy>(),
                        It.IsAny<object>(),
                        It.IsAny<string>()),
                    Times.Once);

            mockPolicyEngine
                .Verify(
                    policyEngine => policyEngine.GetEmployer(
                        It.IsAny<string>(),
                        It.IsAny<string>()),
                    Times.Once);
        }
    }
}
