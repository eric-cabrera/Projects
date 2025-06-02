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
    public class PACTGEventManagerTests
    {
        [TestMethod]
        public async Task ProcessEvent_PolicyIsNull_ShouldLogInformationAndCreatePolicyRecordAndReturnEarly()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PACTGEventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(null as Policy);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine.Setup(
                m => m.GetPolicy(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((GetPolicyResult.Found, new Policy()));

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var pactgEventManager = new PACTGEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object);

            var pactg = new PACTG
            {
                COMPANY_CODE = "01",
                POLICY_NUMBER = "5150198401",
                LIFEPRO_ID = 172,
                DATE_ADDED = 20070409,
                TIME_ADDED = 8073374,
                BENEFIT_SEQ = 1,
                RECORD_SEQUENCE = 0,
                CREDIT_CODE = 2,
                DEBIT_CODE = 10,
                EFFECTIVE_DATE = 20181214,
                REVERSAL_CODE = "Y"
            };

            // Act
            await pactgEventManager.ProcessEvent(pactg);

            // Assert
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    pactg.POLICY_NUMBER,
                    pactg.COMPANY_CODE),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_PolicyExists_ShouldGetReturnPaymentTypeAndUpdatePolicy()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PACTGEventManager>>();
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
               .Setup(policyEngine => policyEngine.GetReturnPaymentData(
                   It.IsAny<string>(),
                   It.IsAny<string>()))
               .Returns((ReturnPaymentType.CardDeclined, DateTime.Now));

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var pactgEventManager = new PACTGEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object);

            var pactg = new PACTG
            {
                COMPANY_CODE = "01",
                POLICY_NUMBER = "5150198401",
                LIFEPRO_ID = 172,
                DATE_ADDED = 20070409,
                TIME_ADDED = 8073374,
                BENEFIT_SEQ = 1,
                RECORD_SEQUENCE = 0,
                CREDIT_CODE = 2,
                DEBIT_CODE = 10,
                EFFECTIVE_DATE = 20181214,
                REVERSAL_CODE = "Y"
            };

            // Act
            await pactgEventManager.ProcessEvent(pactg);

            // Assert
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    pactg.POLICY_NUMBER,
                    pactg.COMPANY_CODE),
                Times.Once);

            mockPolicyEngine.Verify(
               policyEngine => policyEngine.GetReturnPaymentData(
                   pactg.POLICY_NUMBER,
                   pactg.COMPANY_CODE),
               Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>()),
                Times.Once);
        }
    }
}
