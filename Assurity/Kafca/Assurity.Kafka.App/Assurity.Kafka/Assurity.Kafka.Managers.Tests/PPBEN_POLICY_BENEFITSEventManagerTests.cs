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
    public class PPBEN_POLICY_BENEFITSEventManagerTests
    {
        [TestMethod]
        public async Task ProcessEvent_BenefitTypeNotRelevant_ShouldLogInformationAndReturnEarly()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PPBEN_POLICY_BENEFITSEventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var ppbenEventManager = new PPBEN_POLICY_BENEFITSEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockHierarchyEngine.Object);

            var ppben = new PPBEN_POLICY_BENEFITS
            {
                BENEFIT_SEQ = 1,
                BENEFIT_TYPE = BenefitTypes.FundValue,
                PBEN_ID = 1234567,
                PLAN_CODE = "G H1107L",
                POLICY_NUMBER = "7777777001",
                STATUS_CODE = "T",
                STATUS_REASON = "LP"
            };

            // Act
            await ppbenEventManager.ProcessEvent(ppben, TopicOperations.Update);

            // Assert
            mockLogger.VerifyLog(LogLevel.Warning, It.IsAny<string>());

            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPolicyNumber(
                    It.IsAny<string>()),
                Times.Never);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetBenefit(
                    It.IsAny<CompanyCodeAndPolicyNumber>(), It.IsAny<LineOfBusiness>(), It.IsAny<PPBEN_POLICY_BENEFITS>()),
                Times.Never);
        }

        [TestMethod]
        public async Task ProcessEvent_CompanyCodeAndPolicyNumberIsNull_ShouldLogWarningAndReturnEarly()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PPBEN_POLICY_BENEFITSEventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPolicyNumber(
                    It.IsAny<string>()))
                .ReturnsAsync(null as CompanyCodeAndPolicyNumber);

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var ppbenEventManager = new PPBEN_POLICY_BENEFITSEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockHierarchyEngine.Object);

            var ppben = new PPBEN_POLICY_BENEFITS
            {
                BENEFIT_SEQ = 1,
                BENEFIT_TYPE = BenefitTypes.Base,
                PBEN_ID = 1234567,
                PLAN_CODE = "G H1107L",
                POLICY_NUMBER = "7777777001",
                STATUS_CODE = "T",
                STATUS_REASON = "LP"
            };

            // Act
            await ppbenEventManager.ProcessEvent(ppben, TopicOperations.Update);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPolicyNumber(
                    "7777777001"),
                Times.Once);

            var expectedMessage = "Unable to find policy record in LifePro for policy number " +
                $"associated with the {nameof(PPBEN_POLICY_BENEFITS)} event." +
                $" {nameof(ppben.POLICY_NUMBER)}: '{ppben.POLICY_NUMBER}', " +
                $"{nameof(ppben.PLAN_CODE)}: '{ppben.PLAN_CODE}', " +
                $"{nameof(ppben.BENEFIT_TYPE)}: '{ppben.BENEFIT_TYPE}', " +
                $"{nameof(ppben.BENEFIT_SEQ)}: {ppben.BENEFIT_SEQ}";

            mockLogger.VerifyLog(LogLevel.Warning, expectedMessage);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetBenefit(
                    It.IsAny<CompanyCodeAndPolicyNumber>(), It.IsAny<LineOfBusiness>(), It.IsAny<PPBEN_POLICY_BENEFITS>()),
                Times.Never);
        }

        [TestMethod]
        public async Task ProcessEvent_PolicyIsNull_ShouldLogInformationAndCreatePolicyRecordAndReturnEarly()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PPBEN_POLICY_BENEFITSEventManager>>();
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(m => m.GetPolicy(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((GetPolicyResult.NotFound, null));

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPolicyNumber(
                    It.IsAny<string>()))
                .ReturnsAsync(new CompanyCodeAndPolicyNumber
                {
                    CompanyCode = "01",
                    PolicyNumber = "7777777001"
                });

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(null as Policy);

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var ppbenEventManager = new PPBEN_POLICY_BENEFITSEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockHierarchyEngine.Object);

            var ppben = new PPBEN_POLICY_BENEFITS
            {
                BENEFIT_SEQ = 1,
                BENEFIT_TYPE = BenefitTypes.Base,
                PBEN_ID = 1234567,
                PLAN_CODE = "G H1107L",
                POLICY_NUMBER = "7777777001",
                STATUS_CODE = "T",
                STATUS_REASON = "LP"
            };

            // Act
            await ppbenEventManager.ProcessEvent(ppben, TopicOperations.Update);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPolicyNumber(
                    "7777777001"),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    "7777777001", "01"),
                Times.Once);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetBenefit(
                    It.IsAny<CompanyCodeAndPolicyNumber>(), It.IsAny<LineOfBusiness>(), It.IsAny<PPBEN_POLICY_BENEFITS>()),
                Times.Never);
        }

        [TestMethod]
        public async Task ProcessEvent_PolicyExistsWithoutBenefit_ShouldUpdatePolicyByAddingBenefit()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PPBEN_POLICY_BENEFITSEventManager>>();

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPolicyNumber(
                    It.IsAny<string>()))
                .ReturnsAsync(new CompanyCodeAndPolicyNumber
                {
                    CompanyCode = "01",
                    PolicyNumber = "7777777001"
                });

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new Policy
                {
                    Benefits = new List<Benefit>(),
                    CompanyCode = "01",
                    PolicyNumber = "7777777001",
                    ProductCode = "G H1107L"
                });

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.InsertPolicyBenefitAsync(
                    It.IsAny<Benefit>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync("42453543");

            var benefit = new Benefit
            {
                BenefitAmount = 144.8M,
                BenefitDescription = "Group PRO Critical Illness",
                BenefitId = 1234567,
                BenefitStatus = Status.Terminated,
                BenefitStatusReason = StatusReason.Lapsed,
                CoverageType = CoverageType.Base,
                PlanCode = "G H1107L",
                BenefitOptions = new List<BenefitOption>(),
                DividendOption = DividendOption.None,
            };

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetBenefit(
                    It.IsAny<CompanyCodeAndPolicyNumber>(), It.IsAny<LineOfBusiness>(), It.IsAny<PPBEN_POLICY_BENEFITS>()))
                .Returns(benefit);

            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetBaseProductDescriptionByPlanCode(
                   It.Is<string>(a => a.Equals("G H1107L"))))
                .ReturnsAsync(new ProductDescription
                {
                    ProdNumber = "G H1107L",
                    ProdCategory = "category",
                    AltProdDesc = "description"
                });

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var ppbenEventManager = new PPBEN_POLICY_BENEFITSEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockHierarchyEngine.Object);

            var ppben = new PPBEN_POLICY_BENEFITS
            {
                BENEFIT_SEQ = 1,
                BENEFIT_TYPE = BenefitTypes.Base,
                PBEN_ID = 1234567,
                PLAN_CODE = "G H1107L",
                POLICY_NUMBER = "7777777001",
                STATUS_CODE = "T",
                STATUS_REASON = "LP"
            };

            // Act
            await ppbenEventManager.ProcessEvent(ppben, TopicOperations.Update);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPolicyNumber(
                    "7777777001"),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    "7777777001", "01"),
                Times.Once);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetBenefit(
                    It.Is<CompanyCodeAndPolicyNumber>(companyCodeAndPolicyNumber =>
                        companyCodeAndPolicyNumber.CompanyCode == "01"
                        && companyCodeAndPolicyNumber.PolicyNumber == "7777777001"),
                    It.IsAny<LineOfBusiness>(),
                    It.Is<PPBEN_POLICY_BENEFITS>(actualPPBEN =>
                        IsExpectedPPBENEquivalentToActual(ppben, actualPPBEN))),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.InsertPolicyBenefitAsync(
                    It.IsAny<Benefit>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_PolicyExistsWithBenefit_ShouldUpdatePolicyByUpdatingBenefit()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PPBEN_POLICY_BENEFITSEventManager>>();

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPolicyNumber(
                    It.IsAny<string>()))
                .ReturnsAsync(new CompanyCodeAndPolicyNumber
                {
                    CompanyCode = "01",
                    PolicyNumber = "7777777001"
                });

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new Policy
                {
                    Benefits = new List<Benefit>
                    {
                        new Benefit
                        {
                            BenefitId = 1234567
                        }
                    },
                    CompanyCode = "01",
                    PolicyNumber = "7777777001",
                    ProductCode = "G H1107L"
                });

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePolicyBenefitsAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>(),
                    It.IsAny<long>()))
                .ReturnsAsync(1);

            var benefit = new Benefit
            {
                BenefitAmount = 144.8M,
                BenefitDescription = "Group PRO Critical Illness",
                BenefitId = 1234567,
                BenefitStatus = Status.Terminated,
                BenefitStatusReason = StatusReason.Lapsed,
                CoverageType = CoverageType.Base,
                PlanCode = "G H1107L",
                BenefitOptions = new List<BenefitOption>(),
                DividendOption = DividendOption.NoDividend,
            };

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetBenefit(
                    It.IsAny<CompanyCodeAndPolicyNumber>(), It.IsAny<LineOfBusiness>(), It.IsAny<PPBEN_POLICY_BENEFITS>()))
                .Returns(benefit);

            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetBaseProductDescriptionByPlanCode(
                   It.Is<string>(a => a.Equals("G H1107L"))))
                .ReturnsAsync(new ProductDescription
                {
                    ProdNumber = "G H1107L",
                    ProdCategory = "category",
                    AltProdDesc = "description",
                });

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var ppbenEventManager = new PPBEN_POLICY_BENEFITSEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockHierarchyEngine.Object);

            var ppben = new PPBEN_POLICY_BENEFITS
            {
                BENEFIT_SEQ = 1,
                BENEFIT_TYPE = BenefitTypes.Base,
                PBEN_ID = 1234567,
                PLAN_CODE = "G H1107L",
                POLICY_NUMBER = "7777777001",
                STATUS_CODE = "T",
                STATUS_REASON = "LP"
            };

            // Act
            await ppbenEventManager.ProcessEvent(ppben, TopicOperations.Update);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPolicyNumber(
                    "7777777001"),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    "7777777001", "01"),
                Times.Once);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetBenefit(
                    It.Is<CompanyCodeAndPolicyNumber>(companyCodeAndPolicyNumber =>
                        companyCodeAndPolicyNumber.CompanyCode == "01"
                        && companyCodeAndPolicyNumber.PolicyNumber == "7777777001"),
                    It.IsAny<LineOfBusiness>(),
                    It.Is<PPBEN_POLICY_BENEFITS>(actualPPBEN =>
                        IsExpectedPPBENEquivalentToActual(ppben, actualPPBEN))),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyBenefitsAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>(),
                    It.IsAny<long>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_PolicyExistsWithBenefit_ShouldUpdatePolicyByRemovingBenefit()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PPBEN_POLICY_BENEFITSEventManager>>();

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPolicyNumber(
                    It.IsAny<string>()))
                .ReturnsAsync(new CompanyCodeAndPolicyNumber
                {
                    CompanyCode = "01",
                    PolicyNumber = "7777777001"
                });

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new Policy
                {
                    Benefits = new List<Benefit>
                    {
                        new Benefit
                        {
                            BenefitId = 1234567
                        }
                    },
                    CompanyCode = "01",
                    PolicyNumber = "7777777001",
                    ProductCode = "G H1107L"
                });
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.RemovePolicyBenefitByBenefitIdAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>()))
                .ReturnsAsync("63e3dbd3d07d44bb50ca3074");

            var benefit = new Benefit
            {
                BenefitAmount = 144.8M,
                BenefitDescription = "Group PRO Critical Illness",
                BenefitId = 1234567,
                BenefitStatus = Status.Terminated,
                BenefitStatusReason = StatusReason.Lapsed,
                CoverageType = CoverageType.Base,
                PlanCode = "G H1107L",
                BenefitOptions = new List<BenefitOption>(),
                DividendOption = DividendOption.NoDividend,
            };

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetBenefit(
                    It.IsAny<CompanyCodeAndPolicyNumber>(), It.IsAny<LineOfBusiness>(), It.IsAny<PPBEN_POLICY_BENEFITS>()))
                .Returns(benefit);

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var ppbenEventManager = new PPBEN_POLICY_BENEFITSEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockHierarchyEngine.Object);

            var ppben = new PPBEN_POLICY_BENEFITS
            {
                BENEFIT_SEQ = 1,
                BENEFIT_TYPE = BenefitTypes.Base,
                PBEN_ID = 1234567,
                PLAN_CODE = "G H1107L",
                POLICY_NUMBER = "7777777001",
                STATUS_CODE = "T",
                STATUS_REASON = "LP"
            };

            // Act
            await ppbenEventManager.ProcessEvent(ppben, TopicOperations.Delete);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPolicyNumber(
                    "7777777001"),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    "7777777001", "01"),
                Times.Once);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetBenefit(
                    It.Is<CompanyCodeAndPolicyNumber>(companyCodeAndPolicyNumber =>
                        companyCodeAndPolicyNumber.CompanyCode == "01"
                        && companyCodeAndPolicyNumber.PolicyNumber == "7777777001"),
                    It.IsAny<LineOfBusiness>(),
                    It.IsAny<PPBEN_POLICY_BENEFITS>()),
                Times.Never);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.RemovePolicyBenefitByBenefitIdAsync(
                   "7777777001",
                   "01",
                   1234567),
                Times.Once);
        }

        private bool IsExpectedPPBENEquivalentToActual(
            PPBEN_POLICY_BENEFITS expectedPPBEN,
            PPBEN_POLICY_BENEFITS actualPPBEN)
        {
            return expectedPPBEN.BENEFIT_SEQ == actualPPBEN.BENEFIT_SEQ
                && expectedPPBEN.BENEFIT_TYPE == actualPPBEN.BENEFIT_TYPE
                && expectedPPBEN.PBEN_ID == actualPPBEN.PBEN_ID
                && expectedPPBEN.PLAN_CODE == actualPPBEN.PLAN_CODE
                && expectedPPBEN.POLICY_NUMBER == actualPPBEN.POLICY_NUMBER
                && expectedPPBEN.STATUS_CODE == actualPPBEN.STATUS_CODE
                && expectedPPBEN.STATUS_REASON == actualPPBEN.STATUS_REASON;
        }

        private bool IsExpectedPolicyBenefitEquivalentToActual(
            Benefit expectedBenefit,
            Policy actualPolicy)
        {
            if (actualPolicy.Benefits == null || actualPolicy.Benefits.Count != 1)
            {
                return false;
            }

            var actualBenefit = actualPolicy.Benefits[0];

            return expectedBenefit.BenefitAmount == actualBenefit.BenefitAmount
                && expectedBenefit.BenefitDescription == actualBenefit.BenefitDescription
                && expectedBenefit.BenefitId == actualBenefit.BenefitId
                && expectedBenefit.BenefitStatus == actualBenefit.BenefitStatus
                && expectedBenefit.BenefitStatusReason == actualBenefit.BenefitStatusReason
                && expectedBenefit.CoverageType == actualBenefit.CoverageType
                && expectedBenefit.PlanCode == actualBenefit.PlanCode
                && actualBenefit.BenefitOptions != null;
        }
    }
}