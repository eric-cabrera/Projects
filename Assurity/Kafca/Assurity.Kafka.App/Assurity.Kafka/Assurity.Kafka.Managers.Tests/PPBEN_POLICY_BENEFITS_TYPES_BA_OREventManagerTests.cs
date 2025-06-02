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
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PPBEN_POLICY_BENEFITS_TYPES_BA_OREventManagerTests
    {
        [TestMethod]
        public async Task ProcessEvent_CompanyCodeAndPolicyNumberIsNull_ShouldLogWarningAndReturnEarly()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPBENID(
                    It.IsAny<long>()))
                .ReturnsAsync(null as CompanyCodeAndPolicyNumber);

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var ppbenTypesBAOREventManager = new PPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockHierarchyEngine.Object);

            var ppbenTypesBAOR = new PPBEN_POLICY_BENEFITS_TYPES_BA_OR
            {
                PBEN_ID = 277772,
                ANN_PREM_PER_UNIT = 100M,
                NUMBER_OF_UNITS = 9M,
                DIVIDEND = "0",
            };

            // Act
            await ppbenTypesBAOREventManager.ProcessEvent(ppbenTypesBAOR);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPBENID(
                    277772),
                Times.Once);

            var expectedMessage = "Unable to find policy record in LifePro for " +
                $"{nameof(ppbenTypesBAOR.PBEN_ID)} {ppbenTypesBAOR.PBEN_ID} associated " +
                $"with the {nameof(PPBEN_POLICY_BENEFITS_TYPES_BA_OR)} event.";

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
            var mockLogger = new Mock<ILogger<PPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager>>(MockBehavior.Loose);

            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
            {
                CompanyCode = "01",
                PolicyNumber = "5555566661"
            };

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetPolicy(companyCodeAndPolicyNumber.PolicyNumber, companyCodeAndPolicyNumber.CompanyCode))
                .ReturnsAsync((GetPolicyResult.NotFound, null));

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

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var ppbenTypesBAOREventManager = new PPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockHierarchyEngine.Object);

            var ppbenTypesBAOR = new PPBEN_POLICY_BENEFITS_TYPES_BA_OR
            {
                PBEN_ID = 277772,
                ANN_PREM_PER_UNIT = 100M,
                NUMBER_OF_UNITS = 9M,
                DIVIDEND = "1"
            };

            // Act
            await ppbenTypesBAOREventManager.ProcessEvent(ppbenTypesBAOR);

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
        }

        [TestMethod]
        public async Task ProcessEvent_BenefitNotFoundInPolicy_ShouldLogInformationAndReturnEarly()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager>>();

            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
            {
                CompanyCode = "01",
                PolicyNumber = "5555566661"
            };

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPBENID(
                    It.IsAny<long>()))
                .ReturnsAsync(companyCodeAndPolicyNumber);

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

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

            var ppbenTypesBAOREventManager = new PPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockHierarchyEngine.Object);

            var ppbenTypesBAOR = new PPBEN_POLICY_BENEFITS_TYPES_BA_OR
            {
                PBEN_ID = 277772,
                ANN_PREM_PER_UNIT = 100M,
                NUMBER_OF_UNITS = 9M,
                DIVIDEND = "2"
            };

            // Act
            await ppbenTypesBAOREventManager.ProcessEvent(ppbenTypesBAOR);

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

            var expectedMessage = "Benefit not found in Mongo policy for policy number " +
                $"'{policy.PolicyNumber}' and {nameof(ppbenTypesBAOR.PBEN_ID)} {ppbenTypesBAOR.PBEN_ID} " +
                $"for the {nameof(PPBEN_POLICY_BENEFITS_TYPES_BA_OR)} event. No updates will be made.";

            mockLogger.VerifyLog(LogLevel.Warning, expectedMessage);
        }

        [TestMethod]
        public async Task ProcessEvent_BenefitExistsInPolicy_ShouldUpdatePolicyByUpdatingBenefitAmount()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager>>();

            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
            {
                CompanyCode = "01",
                PolicyNumber = "5555566661"
            };

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
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
                        BenefitAmount = 123M,
                        BenefitId = 277772,
                        DividendOption = PolicyInfo.Contracts.V1.Enums.DividendOption.ReducePremium
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
                .Setup(eventsAccessor => eventsAccessor.UpdatePolicyBenefitsAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>(),
                    It.IsAny<long>()))
                .ReturnsAsync(1);

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var ppbenTypesBAOREventManager = new PPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockHierarchyEngine.Object);

            var ppbenTypesBAOR = new PPBEN_POLICY_BENEFITS_TYPES_BA_OR
            {
                PBEN_ID = 277772,
                VALUE_PER_UNIT = 100M,
                NUMBER_OF_UNITS = 9M,
                DIVIDEND = "4"
            };

            // Act
            await ppbenTypesBAOREventManager.ProcessEvent(ppbenTypesBAOR);

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
                eventsAccessor => eventsAccessor.UpdatePolicyBenefitsAsync(
                    It.Is<Policy>(actualPolicy =>
                        IsPolicyBenefitUpdatedWithExpectedBenefitAmount(900M, actualPolicy)),
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