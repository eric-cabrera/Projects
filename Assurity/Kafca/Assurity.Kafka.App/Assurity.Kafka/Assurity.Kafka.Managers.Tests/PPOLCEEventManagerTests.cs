namespace Assurity.Kafka.Managers.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Hierarchy;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Managers;
    using Assurity.Kafka.Managers.Tests.Extensions;
    using Assurity.Kafka.Utilities.Enums;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PPOLCEEventManagerTests
    {
        private Mock<ILogger<PPOLCEventManager>> mockLogger = new Mock<ILogger<PPOLCEventManager>>();

        [TestMethod]
        public async Task ProcessEvent_ExistingPolicy_InvalidApplicationDateLength_PolicyExists_ShouldDelete()
        {
            // Arrange
            var ppolc = new PPOLC
            {
                POLICY_NUMBER = "abc",
                COMPANY_CODE = "01",
                APPLICATION_DATE = 1
            };

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
            .Setup(m => m.GetPolicyAsync(
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(new Policy());

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>();
            mockPolicyEngine
                .Setup(m => m.DeletePolicy(ppolc.POLICY_NUMBER, ppolc.COMPANY_CODE))
                .Returns(Task.CompletedTask);

            mockPolicyEngine
                .Setup(m => m.GetPolicy(ppolc.POLICY_NUMBER, ppolc.COMPANY_CODE))
                .ReturnsAsync((GetPolicyResult.Found, new Policy()));

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(new PolicyHierarchy());

            var ppolcManager = new PPOLCEventManager(
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockLogger.Object);

            // Act
            await ppolcManager.ProcessEvent(ppolc);

            // Assert
            mockLogger.VerifyLog(LogLevel.Warning, It.IsAny<string>(), Times.Once());
        }

        [TestMethod]
        public async Task ProcessEvent_NotTerminated_ApplicationDate8Characters_ShouldCreate()
        {
            // Arrange
            var ppolc = new PPOLC
            {
                POLICY_NUMBER = "abc",
                COMPANY_CODE = "01",
                APPLICATION_DATE = 12345678
            };

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(m => m.CreateOrReplacePolicyAsync(It.IsAny<Policy>()))
                .Returns(Task.CompletedTask);

            mockEventsAccessor
                .Setup(m => m.UpdatePolicyHierarchyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<List<AgentHierarchy>>()))
                .ReturnsAsync("fssdf");

            mockEventsAccessor
                .Setup(m => m.GetPolicyHierarchyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new PolicyHierarchy());

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>();
            mockPolicyEngine
                .Setup(m => m.GetPolicy(ppolc.POLICY_NUMBER, ppolc.COMPANY_CODE))
                .ReturnsAsync((GetPolicyResult.Found, new Policy()));

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(new PolicyHierarchy());

            var ppolcManager = new PPOLCEventManager(
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockLogger.Object);

            // Act
            await ppolcManager.ProcessEvent(ppolc);
        }

        [TestMethod]
        public async Task ProcessEvent_ExistingPolicy_ContractCodeT_ContractReasonER_ShouldDelete()
        {
            // Arrange
            var ppolc = new PPOLC
            {
                POLICY_NUMBER = "abc",
                COMPANY_CODE = "01",
                CONTRACT_CODE = "T",
                CONTRACT_REASON = "ER"
            };

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
            .Setup(m => m.GetPolicyAsync(
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(new Policy());

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>();
            mockPolicyEngine
                .Setup(m => m.DeletePolicy(ppolc.POLICY_NUMBER, ppolc.COMPANY_CODE))
                .Returns(Task.CompletedTask);

            var ppolcManager = new PPOLCEventManager(
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                null,
                mockLogger.Object);

            // Act
            await ppolcManager.ProcessEvent(ppolc);
        }
    }
}