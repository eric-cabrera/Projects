namespace Assurity.Kafka.Managers.Tests
{
    using System;
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
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PPEND_NEW_BUSINESS_PENDINGEventManagerTests
    {
        [TestMethod]
        public async Task ProcessEvent_CompanyCodeAndPolicyNumberIsNull_ShouldLogWarningAndReturnEarly()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PPEND_NEW_BUSINESS_PENDINGEventManager>>();
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

            var ppendEventManager = new PPEND_NEW_BUSINESS_PENDINGEventManager(
                mockLogger.Object,
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockHierarchyEngine.Object);

            var ppend = new PPEND_NEW_BUSINESS_PENDING
            {
                POLICY_NUMBER = "2150941812",
                COMPANY_CODE = "01",
                REDEF_TYPE = RedefTypes.Underwriting,
                LAST_CHANGE_DATE = 20190520,
                UND_NAME_ID = 2454411,
                PEND_ID = 562595
            };

            // Act
            await ppendEventManager.ProcessEvent(ppend);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPolicyNumber(
                    ppend.POLICY_NUMBER),
                Times.Once);

            var expectedMessage = "Unable to find policy record in LifePro for " +
                $"'{ppend.POLICY_NUMBER}' for the " +
                $"{nameof(PPEND_NEW_BUSINESS_PENDING)} event.";

            mockLogger.VerifyLog(LogLevel.Warning, expectedMessage);

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
        public async Task ProcessEvent_PolicyExists_ShouldGetRequirementsAndUpdatePolicy()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PPEND_NEW_BUSINESS_PENDINGEventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new Policy());

            mockEventsAccessor
               .Setup(eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                   It.IsAny<Policy>(),
                   It.IsAny<object>(),
                   It.IsAny<string>()))
               .ReturnsAsync(1);

            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
            {
                CompanyCode = "01",
                PolicyNumber = "4150299379"
            };

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetRequirements(It.IsAny<Policy>()))
                .ReturnsAsync(new List<Requirement>
                {
                    new Requirement
                    {
                        AppliesTo = new Participant
                        {
                            Person = new Person
                            {
                                Name = new Name
                                {
                                    NameId = 922226
                                }
                            }
                        },
                        Name = "MEDICAL INFORMATION BUREAU",
                        LifeProComment = string.Empty,
                        Status = RequirementStatus.Met,
                        AddedDate = new DateTime(2008, 10, 23),
                        ObtainedDate = new DateTime(2008, 11, 18),
                        Id = 1
                    }
                });

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPolicyNumber(
                    It.IsAny<string>()))
                .ReturnsAsync(companyCodeAndPolicyNumber);

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var ppendEventManager = new PPEND_NEW_BUSINESS_PENDINGEventManager(
                mockLogger.Object,
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockHierarchyEngine.Object);

            var ppend = new PPEND_NEW_BUSINESS_PENDING
            {
                POLICY_NUMBER = "2150941812",
                COMPANY_CODE = "01",
                REDEF_TYPE = RedefTypes.Underwriting,
                LAST_CHANGE_DATE = 20190520,
                UND_NAME_ID = 2454411,
                PEND_ID = 562595
            };

            // Act
            await ppendEventManager.ProcessEvent(ppend);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPolicyNumber(
                    ppend.POLICY_NUMBER),
                Times.Once);

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
                policyEngine => policyEngine.GetRequirements(It.IsAny<Policy>()),
                Times.Once);
        }
    }
}
