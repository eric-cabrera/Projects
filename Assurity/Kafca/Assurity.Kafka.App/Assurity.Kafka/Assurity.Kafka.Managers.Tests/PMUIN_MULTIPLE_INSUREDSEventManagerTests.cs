namespace Assurity.Kafka.Managers.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Hierarchy;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Managers.Tests.Extensions;
    using Assurity.Kafka.Managers.Tests.TestData;
    using Assurity.Kafka.Utilities.Constants;
    using Assurity.Kafka.Utilities.Enums;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PMUIN_MULTIPLE_INSUREDSEventManagerTests
    {
        [TestMethod]
        public async Task ProcessEvent_BenefitSeq_0_ShouldLogInformationAndReturnEarly()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PMUIN_MULTIPLE_INSUREDSEventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var pmuin = new PMUIN_MULTIPLE_INSUREDS
            {
                POLICY_NUMBER = "8819851955",
                NAME_ID = 880001,
                RELATIONSHIP_CODE = "ML",
                BENEFIT_SEQ = 0,
                MULT_RELATE = "SELF",
                KD_BEN_EXTEND_KEYS = string.Empty,
                KD_DEF_SEGT_ID = string.Empty
            };

            var pmuinEventManager = new PMUIN_MULTIPLE_INSUREDSEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockHierarchyEngine.Object);

            // Act
            await pmuinEventManager.ProcessEvent(pmuin, TopicOperations.Update);

            // Assert
            var expectedMessage = $"ProcessEvent for {nameof(PMUIN_MULTIPLE_INSUREDS)} " +
                $"has been ignored. Currently only handling {pmuin.BENEFIT_SEQ} greater than 0. " +
                $"{nameof(pmuin.POLICY_NUMBER)}: {pmuin.POLICY_NUMBER}, " +
                $"{nameof(pmuin.NAME_ID)}: {pmuin.NAME_ID}, " +
                $"{nameof(pmuin.MULT_RELATE)}: {pmuin.MULT_RELATE}, " +
                $"{nameof(pmuin.BENEFIT_SEQ)}: {pmuin.BENEFIT_SEQ}";

            mockLogger.VerifyLog(LogLevel.Warning, expectedMessage);

            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetPRELA_RELATIONSHIP_MASTER_BENEFIT(
                    It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<short>()),
                Times.Never);
        }

        [TestMethod]
        public async Task ProcessEvent_PRELA_Null_ShouldLogWarningAndReturnEarly()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PMUIN_MULTIPLE_INSUREDSEventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetPRELA_RELATIONSHIP_MASTER_BENEFIT(
                    It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<short>()))
                .ReturnsAsync(null as PRELA_RELATIONSHIP_MASTER);

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var pmuin = new PMUIN_MULTIPLE_INSUREDS
            {
                POLICY_NUMBER = "8819851955",
                NAME_ID = 880001,
                RELATIONSHIP_CODE = "ML",
                BENEFIT_SEQ = 1,
                MULT_RELATE = "SELF",
                KD_BEN_EXTEND_KEYS = "0101",
                KD_DEF_SEGT_ID = "024M090D"
            };

            var pmuinEventManager = new PMUIN_MULTIPLE_INSUREDSEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockHierarchyEngine.Object);

            // Act
            await pmuinEventManager.ProcessEvent(pmuin, TopicOperations.Update);

            // Assert
            var expectedMessage = "Unable to find Policy and Relationship data to process " +
                    $"the {nameof(PMUIN_MULTIPLE_INSUREDS)} event. " +
                $"{nameof(pmuin.POLICY_NUMBER)}: {pmuin.POLICY_NUMBER}, " +
                $"{nameof(pmuin.NAME_ID)}: {pmuin.NAME_ID}, " +
                $"{nameof(pmuin.MULT_RELATE)}: {pmuin.MULT_RELATE}, " +
                $"{nameof(pmuin.BENEFIT_SEQ)}: {pmuin.BENEFIT_SEQ}";

            mockLogger.VerifyLog(LogLevel.Warning, expectedMessage);

            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetPRELA_RELATIONSHIP_MASTER_BENEFIT(
                    "8819851955", 880001, "ML", 1),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public async Task ProcessEvent_Policy_Null_ShouldLogInformationAndReturnEarly()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PMUIN_MULTIPLE_INSUREDSEventManager>>();
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine.Setup(
                m => m.GetPolicy(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((GetPolicyResult.Found, new Policy()));

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetPRELA_RELATIONSHIP_MASTER_BENEFIT(
                    It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<short>()))
                .ReturnsAsync(new PRELA_RELATIONSHIP_MASTER
                {
                    IDENTIFYING_ALPHA = "018819851955",
                    NAME_ID = 880001,
                    RELATE_CODE = "ML",
                    BENEFIT_SEQ_NUMBER = 1
                });

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(null as Policy);

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var pmuin = new PMUIN_MULTIPLE_INSUREDS
            {
                POLICY_NUMBER = "8819851955",
                NAME_ID = 880001,
                RELATIONSHIP_CODE = "ML",
                BENEFIT_SEQ = 1,
                MULT_RELATE = "SELF",
                KD_BEN_EXTEND_KEYS = "0101",
                KD_DEF_SEGT_ID = "024M090D"
            };

            var pmuinEventManager = new PMUIN_MULTIPLE_INSUREDSEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockHierarchyEngine.Object);

            // Act
            await pmuinEventManager.ProcessEvent(pmuin, TopicOperations.Update);

            // Assert
            var expectedMessage = "Policy not found in Mongo for policy number " +
                $"'{pmuin.POLICY_NUMBER}' for the {nameof(PMUIN_MULTIPLE_INSUREDS)} event.";

            mockLogger.VerifyLog(LogLevel.Warning, expectedMessage);

            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetPRELA_RELATIONSHIP_MASTER_BENEFIT(
                    "8819851955", 880001, "ML", 1),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    "8819851955", "01"),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_PolicyExists_ShouldUpdateBenefitsTwiceAndPolicy()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);

            SetupMockReturns(mockEventsAccessor, mockPolicyEngine, mockLifeProAccessor, mockHierarchyEngine, 1234565);

            var pmuin = new PMUIN_MULTIPLE_INSUREDS
            {
                POLICY_NUMBER = "8819851955",
                NAME_ID = 880001,
                RELATIONSHIP_CODE = "ML",
                BENEFIT_SEQ = 1,
                MULT_RELATE = "SELF",
                KD_BEN_EXTEND_KEYS = "0101",
                KD_DEF_SEGT_ID = "024M090D"
            };
            var pmuinEventManager = new PMUIN_MULTIPLE_INSUREDSEventManager(
                mockEventsAccessor.Object,
                null,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockHierarchyEngine.Object);

            // Act
            await pmuinEventManager.ProcessEvent(pmuin, TopicOperations.Update);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetPRELA_RELATIONSHIP_MASTER_BENEFIT(
                    "8819851955", 880001, "ML", 1),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    "8819851955", "01"),
                Times.Once);

            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetPPBEN_POLICY_BENEFITS(
                    It.IsAny<string>(), It.IsAny<short>(), It.IsAny<long>(), It.IsAny<string>()),
                Times.Once);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetBenefit(
                    It.IsAny<CompanyCodeAndPolicyNumber>(), It.IsAny<LineOfBusiness>(), It.IsAny<PPBEN_POLICY_BENEFITS>()),
                Times.Exactly(2));

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyBenefitsAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>(),
                    It.IsAny<long>()),
                Times.Exactly(2));
        }

        [TestMethod]
        public async Task ProcessEvent_PolicyExists_ShouldUpdateBenefitsOnceAndPolicy()
        {
            // Arrange
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);

            SetupMockReturns(mockEventsAccessor, mockPolicyEngine, mockLifeProAccessor, mockHierarchyEngine, 1234566);

            var pmuin = new PMUIN_MULTIPLE_INSUREDS
            {
                POLICY_NUMBER = "8819851955",
                NAME_ID = 880001,
                RELATIONSHIP_CODE = "ML",
                BENEFIT_SEQ = 1,
                MULT_RELATE = "SELF",
                KD_BEN_EXTEND_KEYS = "0101",
                KD_DEF_SEGT_ID = "024M090D"
            };
            var pmuinEventManager = new PMUIN_MULTIPLE_INSUREDSEventManager(
                mockEventsAccessor.Object,
                null,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockHierarchyEngine.Object);

            // Act
            await pmuinEventManager.ProcessEvent(pmuin, TopicOperations.Update);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetPRELA_RELATIONSHIP_MASTER_BENEFIT(
                    "8819851955", 880001, "ML", 1),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    "8819851955", "01"),
                Times.Once);

            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetPPBEN_POLICY_BENEFITS(
                    It.IsAny<string>(),
                    It.IsAny<short>(),
                    It.IsAny<long>(),
                    It.IsAny<string>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyBenefitsAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>(),
                    It.IsAny<long>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_PolicyExists_ShouldCreateBenefitAndNotRemoveCreatedBenefit()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);

            SetupMockReturns(mockEventsAccessor, mockPolicyEngine, mockLifeProAccessor, mockHierarchyEngine, 1234569);

            var pmuin = new PMUIN_MULTIPLE_INSUREDS
            {
                POLICY_NUMBER = "8819851955",
                NAME_ID = 880001,
                RELATIONSHIP_CODE = "ML",
                BENEFIT_SEQ = 1,
                MULT_RELATE = "SELF",
                KD_BEN_EXTEND_KEYS = "0101",
                KD_DEF_SEGT_ID = "024M090D"
            };

            var pmuinEventManager = new PMUIN_MULTIPLE_INSUREDSEventManager(
                mockEventsAccessor.Object,
                null,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockHierarchyEngine.Object);

            // Act
            await pmuinEventManager.ProcessEvent(pmuin, TopicOperations.Update);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetPRELA_RELATIONSHIP_MASTER_BENEFIT(
                    "8819851955", 880001, "ML", 1),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    "8819851955", "01"),
                Times.Once);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetBenefit(
                    It.IsAny<CompanyCodeAndPolicyNumber>(), It.IsAny<LineOfBusiness>(), It.IsAny<PPBEN_POLICY_BENEFITS>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.InsertPolicyBenefitAsync(
                    It.IsAny<Benefit>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.RemovePolicyBenefitByBenefitIdAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<long>()),
                Times.Never);
        }

        [TestMethod]
        public async Task ProcessEvent_PolicyExists_ShouldCreateBenefitAndRemoveCreatedBenefit()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);

            SetupMockReturns(mockEventsAccessor, mockPolicyEngine, mockLifeProAccessor, mockHierarchyEngine, 1234568);

            var pmuin = new PMUIN_MULTIPLE_INSUREDS
            {
                POLICY_NUMBER = "8819851955",
                NAME_ID = 880001,
                RELATIONSHIP_CODE = "ML",
                BENEFIT_SEQ = 1,
                MULT_RELATE = "SELF",
                KD_BEN_EXTEND_KEYS = "0101",
                KD_DEF_SEGT_ID = "024M090D"
            };

            var pmuinEventManager = new PMUIN_MULTIPLE_INSUREDSEventManager(
                mockEventsAccessor.Object,
                null,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockHierarchyEngine.Object);

            // Act
            await pmuinEventManager.ProcessEvent(pmuin, TopicOperations.Update);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetPRELA_RELATIONSHIP_MASTER_BENEFIT(
                    "8819851955", 880001, "ML", 1),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    "8819851955", "01"),
                Times.Once);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetBenefit(
                    It.IsAny<CompanyCodeAndPolicyNumber>(), It.IsAny<LineOfBusiness>(), It.IsAny<PPBEN_POLICY_BENEFITS>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.InsertPolicyBenefitAsync(
                    It.IsAny<Benefit>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.RemovePolicyBenefitByBenefitIdAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<long>()),
                Times.Once);
        }

        private void SetupMockReturns(
            Mock<IEventsAccessor> mockEventsAccessor,
            Mock<IConsumerPolicyEngine> mockPolicyEngine,
            Mock<ILifeProAccessor> mockLifeProAccessor,
            Mock<IConsumerHierarchyEngine> mockHierarchyEngine,
            long benefitId)
        {
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;

            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new Policy
                {
                    CompanyCode = "01",
                    PolicyNumber = "8819851955",
                    Benefits = new List<Benefit>
                    {
                        new Benefit
                        {
                            CoverageType = CoverageType.Base,
                            BenefitStatus = Status.Active,
                            BenefitStatusReason = StatusReason.None,
                            PlanCode = "ADIR-E",
                            BenefitId = 1234565
                        },
                        new Benefit()
                        {
                            CoverageType = CoverageType.Rider,
                            BenefitId = 1234566,
                            PlanCode = "R W1110",
                            BenefitDescription = "Ind. PRO 24-hour Accident Expense",
                            BenefitStatus = Status.Terminated,
                            BenefitStatusReason = StatusReason.Lapsed
                        }
                    }
                });

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.InsertPolicyBenefitAsync(
                    It.IsAny<Benefit>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync("42453543");

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.RemovePolicyBenefitByBenefitIdAsync(
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<long>()))
                .ReturnsAsync("757578");

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePolicyBenefitsAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>(),
                    It.IsAny<long>()))
                .ReturnsAsync(1);

            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetPRELA_RELATIONSHIP_MASTER_BENEFIT(
                    It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<short>()))
                .ReturnsAsync(new PRELA_RELATIONSHIP_MASTER
                {
                    IDENTIFYING_ALPHA = "018819851955",
                    NAME_ID = 880001,
                    RELATE_CODE = "ML",
                    BENEFIT_SEQ_NUMBER = 1
                });

            var ppben = new PPBEN_POLICY_BENEFITS
            {
                BENEFIT_SEQ = 2,
                BENEFIT_TYPE = BenefitTypes.SpecifiedAmountIncrease,
                PBEN_ID = benefitId,
                PLAN_CODE = "R H0930",
                POLICY_NUMBER = "7777777001",
                STATUS_CODE = "T",
                STATUS_REASON = "LP",
                STATUS_DATE = 20230909
            };

            mockLifeProAccessor
                .SetupSequence(lifeProAccessor => lifeProAccessor.GetPPBEN_POLICY_BENEFITS(
                    It.IsAny<string>(), It.IsAny<short>(), It.IsAny<string>()))
                .ReturnsAsync(ppben);

            mockLifeProAccessor
                .SetupSequence(lifeProAccessor => lifeProAccessor.GetPPBEN_POLICY_BENEFITS(
                    It.IsAny<string>(), It.IsAny<short>(), It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(ppben);

            mockLifeProAccessor
                .SetupSequence(lifeProAccessor => lifeProAccessor.GetPPBEN_POLICY_BENEFITS(
                    It.IsAny<string>(), It.IsAny<short>(), 1234568, It.IsAny<string>()))
                .ReturnsAsync(() => null);

            var benefit = new Benefit
            {
                BenefitAmount = 144.8M,
                BenefitDescription = "Group PRO Critical Illness",
                BenefitId = benefitId,
                BenefitStatus = Status.Active,
                BenefitStatusReason = StatusReason.Lapsed,
                CoverageType = CoverageType.Base,
                PlanCode = "G H1107L",
                BenefitOptions = new List<BenefitOption>(),
                DividendOption = DividendOption.None
            };

            var updatedBenefit = new Benefit
            {
                BenefitAmount = 150M,
                BenefitDescription = "Group PRO Critical Illness",
                BenefitId = benefitId,
                BenefitStatus = Status.Terminated,
                BenefitStatusReason = StatusReason.Lapsed,
                CoverageType = CoverageType.Base,
                PlanCode = "G H1107L",
                BenefitOptions = new List<BenefitOption>(),
                DividendOption = DividendOption.None
            };

            var queue = new Queue<Benefit>(new[] { benefit, updatedBenefit });

            if (benefitId == 1234565)
            {
                mockPolicyEngine
                    .Setup(policyEngine => policyEngine.GetBenefit(
                        It.IsAny<CompanyCodeAndPolicyNumber>(), It.IsAny<LineOfBusiness>(), It.IsAny<PPBEN_POLICY_BENEFITS>()))
                    .Returns(queue.Dequeue);
            }
            else
            {
                mockPolicyEngine
                    .Setup(policyEngine => policyEngine.GetBenefit(
                        It.IsAny<CompanyCodeAndPolicyNumber>(), It.IsAny<LineOfBusiness>(), It.IsAny<PPBEN_POLICY_BENEFITS>()))
                    .Returns(benefit);
            }

            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetInsureds(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<Insured>());
        }
    }
}