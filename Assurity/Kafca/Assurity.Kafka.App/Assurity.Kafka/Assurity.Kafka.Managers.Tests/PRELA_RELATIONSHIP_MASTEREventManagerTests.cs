namespace Assurity.Kafka.Managers.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Hierarchy;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Managers.Tests.Extensions;
    using Assurity.Kafka.Managers.Tests.TestData;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PRELA_RELATIONSHIP_MASTEREventManagerTests
    {
        [TestMethod]
        public async Task ProcessEvent_IdentifyingAlphaNotValidLength_ShouldLogWarningAndReturnEarly()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PRELA_RELATIONSHIP_MASTER>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var prelaEventManager = new PRELA_RELATIONSHIP_MASTEREventManager(
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                mockLogger.Object,
                mockHierarchyEngine.Object);

            var prela = new PRELA_RELATIONSHIP_MASTER
            {
                IDENTIFYING_ALPHA = "01",
                NAME_ID = 500001,
                RELATE_CODE = "IN",
                BENEFIT_SEQ_NUMBER = 0
            };

            // Act
            await prelaEventManager.ProcessEvent(prela);

            // Assert
            mockLogger.VerifyLog(LogLevel.Warning, It.IsAny<string>());

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public async Task ProcessEvent_PRELA_With_Insured_RelateCode_ShouldAddInsuredsAndUpdatePolicy()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PRELA_RELATIONSHIP_MASTER>>();
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            var prela = new PRELA_RELATIONSHIP_MASTER();

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            SetMockUpData(mockPolicyEngine, mockEventsAccessor, prela);
            prela.RELATE_CODE = "IN";

            var prelaEventManager = new PRELA_RELATIONSHIP_MASTEREventManager(
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                mockLogger.Object,
                mockHierarchyEngine.Object);

            // Act
            await prelaEventManager.ProcessEvent(prela);

            // Assert
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()),
                Times.Once);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetInsureds(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_PRELA_With_Annuitant_RelateCode_ShouldAddAnnuitantsAndUpdatePolicy()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PRELA_RELATIONSHIP_MASTER>>();
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            var prela = new PRELA_RELATIONSHIP_MASTER();
            prela.RELATE_CODE = "A2";

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            SetMockUpData(mockPolicyEngine, mockEventsAccessor, prela);

            var prelaEventManager = new PRELA_RELATIONSHIP_MASTEREventManager(
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                mockLogger.Object,
                mockHierarchyEngine.Object);

            // Act
            await prelaEventManager.ProcessEvent(prela);

            // Assert
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()),
                Times.Once);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetAnnuitants(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_PRELA_With_Assignee_RelateCode_ShouldAddAssigneeAndUpdatePolicy()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PRELA_RELATIONSHIP_MASTER>>();
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            var prela = new PRELA_RELATIONSHIP_MASTER();

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            SetMockUpData(mockPolicyEngine, mockEventsAccessor, prela);
            prela.RELATE_CODE = "AS";

            var prelaEventManager = new PRELA_RELATIONSHIP_MASTEREventManager(
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                mockLogger.Object,
                mockHierarchyEngine.Object);

            // Act
            await prelaEventManager.ProcessEvent(prela);

            // Assert
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()),
                Times.Once);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetAssignee(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_PRELA_With_Beneficiary_RelateCode_ShouldAddBeneficiariesAndUpdatePolicy()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PRELA_RELATIONSHIP_MASTER>>();
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            var prela = new PRELA_RELATIONSHIP_MASTER();

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            SetMockUpData(mockPolicyEngine, mockEventsAccessor, prela);
            prela.RELATE_CODE = "B2";

            var prelaEventManager = new PRELA_RELATIONSHIP_MASTEREventManager(
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                mockLogger.Object,
                mockHierarchyEngine.Object);

            // Act
            await prelaEventManager.ProcessEvent(prela);

            // Assert
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()),
                Times.Once);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetBeneficiaries(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_PRELA_With_Owner_RelateCode_ShouldAddOwnersAndUpdatePolicy()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PRELA_RELATIONSHIP_MASTER>>();
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            var prela = new PRELA_RELATIONSHIP_MASTER();

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            SetMockUpData(mockPolicyEngine, mockEventsAccessor, prela);
            prela.RELATE_CODE = "PO";

            var prelaEventManager = new PRELA_RELATIONSHIP_MASTEREventManager(
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                mockLogger.Object,
                mockHierarchyEngine.Object);

            // Act
            await prelaEventManager.ProcessEvent(prela);

            // Assert
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()),
                Times.Once);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetOwners(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_PRELA_With_Payor_RelateCode_ShouldAddPayorsAndUpdatePolicy()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PRELA_RELATIONSHIP_MASTER>>();
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            var prela = new PRELA_RELATIONSHIP_MASTER();

            SetMockUpData(mockPolicyEngine, mockEventsAccessor, prela);
            prela.RELATE_CODE = "PA";

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var prelaEventManager = new PRELA_RELATIONSHIP_MASTEREventManager(
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                mockLogger.Object,
                mockHierarchyEngine.Object);

            // Act
            await prelaEventManager.ProcessEvent(prela);

            // Assert
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()),
                Times.Once);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetPayors(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_PRELA_With_Payee_RelateCode_ShouldAddPayeeAndUpdatePolicy()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PRELA_RELATIONSHIP_MASTER>>();
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            var prela = new PRELA_RELATIONSHIP_MASTER();

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            SetMockUpData(mockPolicyEngine, mockEventsAccessor, prela);
            prela.RELATE_CODE = "PE";

            var prelaEventManager = new PRELA_RELATIONSHIP_MASTEREventManager(
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                mockLogger.Object,
                mockHierarchyEngine.Object);

            // Act
            await prelaEventManager.ProcessEvent(prela);

            // Assert
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()),
                Times.Once);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetPayee(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);
        }

        private void SetMockUpData(
            Mock<IConsumerPolicyEngine> mockPolicyEngine,
            Mock<IEventsAccessor> mockEventsAccessor,
            PRELA_RELATIONSHIP_MASTER prela)
        {
            prela.IDENTIFYING_ALPHA = "015150198401";
            prela.NAME_ID = 500001;
            prela.BENEFIT_SEQ_NUMBER = 0;

            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetInsureds(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new List<Insured>());

            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetAnnuitants(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new List<Annuitant>());

            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetAssignee(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new Assignee());

            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetBeneficiaries(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new List<Beneficiary>());

            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetOwners(
                   It.IsAny<string>(),
                   It.IsAny<string>()))
                .Returns(new List<Owner>());

            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetPayors(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new List<Payor>());

            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetPayee(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new Payee());

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new Policy());

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()))
                .ReturnsAsync(1);
        }
    }
}
