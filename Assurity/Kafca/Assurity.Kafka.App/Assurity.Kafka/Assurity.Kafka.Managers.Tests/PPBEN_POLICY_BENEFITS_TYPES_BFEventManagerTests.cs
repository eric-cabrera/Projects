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
    using Assurity.Kafka.Utilities.Enums;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PPBEN_POLICY_BENEFITS_TYPES_BFEventManagerTests
    {
        [TestMethod]
        public async Task ProcessEvent_CompanyCodeAndPolicyNumberIsNull_ShouldLogWarningAndReturnEarly()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PPBEN_POLICY_BENEFITS_TYPES_BFEventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPBENID(
                    It.IsAny<long>()))
                .ReturnsAsync(null as CompanyCodeAndPolicyNumber);

            var ppbenTypesBFEventManager = new PPBEN_POLICY_BENEFITS_TYPES_BFEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockLifeProAccessor.Object);

            var ppbenTypesBF = new PPBEN_POLICY_BENEFITS_TYPES_BF
            {
                PBEN_ID = 277772,
                ANN_PREM_PER_UNIT = 100M,
                NUMBER_OF_UNITS = 9M
            };

            // Act
            await ppbenTypesBFEventManager.ProcessEvent(ppbenTypesBF);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPBENID(
                    277772),
                Times.Once);

            var expectedMessage = "Unable to find policy record in LifePro for " +
                $"{nameof(ppbenTypesBF.PBEN_ID)} {ppbenTypesBF.PBEN_ID} associated " +
                $"with the {nameof(PPBEN_POLICY_BENEFITS_TYPES_BF)} event.";

            mockLogger.VerifyLog(LogLevel.Warning, expectedMessage);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public async Task ProcessEvent_PolicyIsNull_ShouldLogInformationAndReturnEarly()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PPBEN_POLICY_BENEFITS_TYPES_BFEventManager>>();

            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
            {
                CompanyCode = "01",
                PolicyNumber = "5555566661"
            };

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(m => m.GetPolicy(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((GetPolicyResult.NotFound, null));

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPBENID(
                    It.IsAny<long>()))
                .ReturnsAsync(companyCodeAndPolicyNumber);

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(null as Policy);

            var ppbenTypesBFEventManager = new PPBEN_POLICY_BENEFITS_TYPES_BFEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockLifeProAccessor.Object);

            var ppbenTypesBF = new PPBEN_POLICY_BENEFITS_TYPES_BF
            {
                PBEN_ID = 277772,
                ANN_PREM_PER_UNIT = 100M,
                NUMBER_OF_UNITS = 9M
            };

            // Act
            await ppbenTypesBFEventManager.ProcessEvent(ppbenTypesBF);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPBENID(
                    277772),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_BenefitNotFoundInPolicy_ShouldLogInformationAndReturnEarly()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PPBEN_POLICY_BENEFITS_TYPES_BFEventManager>>();

            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
            {
                CompanyCode = "01",
                PolicyNumber = "5555566661"
            };

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPBENID(
                    It.IsAny<long>()))
                .ReturnsAsync(companyCodeAndPolicyNumber);

            var policy = new Policy
            {
                PolicyNumber = companyCodeAndPolicyNumber.PolicyNumber,
                Benefits = new List<Benefit>()
            };

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(policy);

            var ppbenTypesBFEventManager = new PPBEN_POLICY_BENEFITS_TYPES_BFEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockLifeProAccessor.Object);

            var ppbenTypesBF = new PPBEN_POLICY_BENEFITS_TYPES_BF
            {
                PBEN_ID = 277772,
                ANN_PREM_PER_UNIT = 100M,
                NUMBER_OF_UNITS = 9M,
                BF_DATE_NEGATIVE = 0
            };

            // Act
            await ppbenTypesBFEventManager.ProcessEvent(ppbenTypesBF);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPBENID(
                    277772),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    companyCodeAndPolicyNumber.PolicyNumber,
                    companyCodeAndPolicyNumber.CompanyCode),
                Times.Once);

            mockLogger.VerifyLog(LogLevel.Warning, It.IsAny<string>());
        }

        [TestMethod]
        public async Task ProcessEvent_BenefitExistsInPolicy_ShouldUpdatePolicyByUpdatingBenefitAmount_PastDueTrue()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PPBEN_POLICY_BENEFITS_TYPES_BFEventManager>>();

            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
            {
                CompanyCode = "01",
                PolicyNumber = "5555566661"
            };

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPBENID(
                    It.IsAny<long>()))
                .ReturnsAsync(companyCodeAndPolicyNumber);

            var policy = new Policy
            {
                PolicyNumber = companyCodeAndPolicyNumber.PolicyNumber,
                Benefits = new List<Benefit>
                {
                    new Benefit
                    {
                        BenefitAmount = 125000M,
                        BenefitId = 277772
                    }
                }
            };

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(policy);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePolicyBenefitsAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>(),
                    It.IsAny<long>()))
                .ReturnsAsync(1);

            var ppbenTypesBFEventManager = new PPBEN_POLICY_BENEFITS_TYPES_BFEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockLifeProAccessor.Object);

            var ppbenTypesBF = new PPBEN_POLICY_BENEFITS_TYPES_BF
            {
                PBEN_ID = 277772,
                VALUE_PER_UNIT = 100M,
                NUMBER_OF_UNITS = 9M,
                BF_DATE_NEGATIVE = 20220721,
                BF_CURRENT_DB = 125000m
            };

            // Act
            await ppbenTypesBFEventManager.ProcessEvent(ppbenTypesBF);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPBENID(
                    277772),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    companyCodeAndPolicyNumber.PolicyNumber,
                    companyCodeAndPolicyNumber.CompanyCode),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.Is<Policy>(actualPolicy =>
                        IsPolicyBenefitUpdatedWithExpectedBenefitAmount(125000M, actualPolicy)),
                    It.IsAny<bool>(),
                    It.IsAny<string>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.Is<Policy>(actualPolicy => actualPolicy.PastDue == true),
                    It.IsAny<bool>(),
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
        public async Task ProcessEvent_BenefitExistsInPolicy_ShouldUpdatePolicyByUpdatingBenefitAmount_PastDueFalse()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PPBEN_POLICY_BENEFITS_TYPES_BFEventManager>>();

            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
            {
                CompanyCode = "01",
                PolicyNumber = "5555566661"
            };

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPBENID(
                    It.IsAny<long>()))
                .ReturnsAsync(companyCodeAndPolicyNumber);

            var policy = new Policy
            {
                PolicyNumber = companyCodeAndPolicyNumber.PolicyNumber,
                Benefits = new List<Benefit>
                {
                    new Benefit
                    {
                        BenefitAmount = 125000M,
                        BenefitId = 277772
                    }
                }
            };

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(policy);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePolicyBenefitsAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>(),
                    It.IsAny<long>()))
                .ReturnsAsync(1);

            var ppbenTypesBFEventManager = new PPBEN_POLICY_BENEFITS_TYPES_BFEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockLifeProAccessor.Object);

            var ppbenTypesBF = new PPBEN_POLICY_BENEFITS_TYPES_BF
            {
                PBEN_ID = 277772,
                VALUE_PER_UNIT = 100M,
                NUMBER_OF_UNITS = 9M,
                BF_DATE_NEGATIVE = 0,
                BF_CURRENT_DB = 125000m
            };

            // Act
            await ppbenTypesBFEventManager.ProcessEvent(ppbenTypesBF);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPBENID(
                    277772),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    companyCodeAndPolicyNumber.PolicyNumber,
                    companyCodeAndPolicyNumber.CompanyCode),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.Is<Policy>(actualPolicy =>
                        IsPolicyBenefitUpdatedWithExpectedBenefitAmount(125000M, actualPolicy)),
                    It.IsAny<bool>(),
                    It.IsAny<string>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.Is<Policy>(actualPolicy => actualPolicy.PastDue == false),
                    It.IsAny<bool>(),
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
        public async Task ProcessEvent_BenefitExistsInPolicy_ShouldUpdatePolicyByUpdatingBenefitAmount_DeathBenefitOption()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PPBEN_POLICY_BENEFITS_TYPES_BFEventManager>>();

            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
            {
                CompanyCode = "01",
                PolicyNumber = "5555566661"
            };

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPBENID(
                    It.IsAny<long>()))
                .ReturnsAsync(companyCodeAndPolicyNumber);

            var policy = new Policy
            {
                PolicyNumber = companyCodeAndPolicyNumber.PolicyNumber,
                Benefits = new List<Benefit>
                {
                    new Benefit
                    {
                        BenefitAmount = 125000M,
                        BenefitId = 277772,
                        DeathBenefitOption = DeathBenefitOption.FaceAmountOption
                    }
                }
            };

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(policy);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePolicyBenefitsAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>(),
                    It.IsAny<long>()))
                .ReturnsAsync(1);

            var ppbenTypesBFEventManager = new PPBEN_POLICY_BENEFITS_TYPES_BFEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockLifeProAccessor.Object);

            var ppbenTypesBF = new PPBEN_POLICY_BENEFITS_TYPES_BF
            {
                PBEN_ID = 277772,
                VALUE_PER_UNIT = 100M,
                NUMBER_OF_UNITS = 9M,
                BF_DATE_NEGATIVE = 20220721,
                BF_CURRENT_DB = 125000m,
                BF_DB_OPTION = "2"
            };

            // Act
            await ppbenTypesBFEventManager.ProcessEvent(ppbenTypesBF);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPBENID(
                    277772),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    companyCodeAndPolicyNumber.PolicyNumber,
                    companyCodeAndPolicyNumber.CompanyCode),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.Is<Policy>(actualPolicy =>
                        IsPolicyBenefitUpdatedWithExpectedBenefitAmount(125000M, actualPolicy)),
                    It.IsAny<bool>(),
                    It.IsAny<string>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.Is<Policy>(actualPolicy => actualPolicy.Benefits[0].DeathBenefitOption == DeathBenefitOption.FaceAmountPlusCashValueOption),
                    It.IsAny<bool>(),
                    It.IsAny<string>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyBenefitsAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>(),
                    It.IsAny<long>()),
                Times.Once);
        }

        private bool IsPolicyBenefitUpdatedWithExpectedBenefitAmount(
            decimal expectedBenefitAmount,
            Policy actualPolicy)
        {
            if (actualPolicy.Benefits == null || actualPolicy.Benefits.Count != 1)
            {
                return false;
            }

            return expectedBenefitAmount == actualPolicy.Benefits[0].BenefitAmount;
        }
    }
}