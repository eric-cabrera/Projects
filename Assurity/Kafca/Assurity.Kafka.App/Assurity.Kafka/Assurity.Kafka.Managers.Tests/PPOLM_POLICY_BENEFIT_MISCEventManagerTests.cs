namespace Assurity.Kafka.Managers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Hierarchy;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Managers.Tests.TestData;
    using Assurity.Kafka.Utilities.Enums;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PPOLM_POLICY_BENEFIT_MISCEventManagerTests
    {
        [TestMethod]
        public async Task ProcessEvent_PolicyIsNull_ShouldLogInformationAndCreatePolicyRecordAndReturnEarly()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PPOLM_POLICY_BENEFIT_MISCEventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(null as Policy);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(m => m.GetPolicy(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((GetPolicyResult.NotFound, null));

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var ppolmEventManager = new PPOLM_POLICY_BENEFIT_MISCEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object);

            var ppolm = new PPOLM_POLICY_BENEFIT_MISC
            {
                POLM_ID = 1,
                COMPANY_CODE = "01",
                POLICY_NUMBER = "1122334455",
                CANCEL_REASON = "05",
                SEQ = 12
            };

            // Act
            await ppolmEventManager.ProcessEvent(ppolm);

            // Assert
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    ppolm.POLICY_NUMBER,
                    ppolm.COMPANY_CODE),
                Times.Once);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetPolicyStatusDetail(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public async Task ProcessEvent_PolicyExists_ShouldGetPolicyStatusDetailAndUpdatePolicy()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PPOLM_POLICY_BENEFIT_MISCEventManager>>();

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new Policy());
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(1);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetPolicyStatusDetail(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(PolicyStatusDetail.InsuredRequested);

            mockPolicyEngine
               .Setup(policyEngine => policyEngine.GetReturnPaymentData(
                   It.IsAny<string>(),
                   It.IsAny<string>()))
               .Returns((ReturnPaymentType.CardDeclined, DateTime.Now));

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var ppolmEventManager = new PPOLM_POLICY_BENEFIT_MISCEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object);

            var ppolm = new PPOLM_POLICY_BENEFIT_MISC
            {
                POLM_ID = 1,
                COMPANY_CODE = "01",
                POLICY_NUMBER = "1122334455",
                CANCEL_REASON = "05",
                SEQ = 12
            };

            // Act
            await ppolmEventManager.ProcessEvent(ppolm);

            // Assert
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    ppolm.POLICY_NUMBER,
                    ppolm.COMPANY_CODE),
                Times.Once);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetPolicyStatusDetail(
                    ppolm.POLICY_NUMBER,
                    ppolm.COMPANY_CODE),
                Times.Once);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetReturnPaymentData(
                    ppolm.POLICY_NUMBER,
                    ppolm.COMPANY_CODE),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.Is<Policy>(policy => policy.PolicyStatusDetail == PolicyStatusDetail.InsuredRequested),
                    It.IsAny<Dictionary<string, object>>()),
                Times.Once);
        }
    }
}