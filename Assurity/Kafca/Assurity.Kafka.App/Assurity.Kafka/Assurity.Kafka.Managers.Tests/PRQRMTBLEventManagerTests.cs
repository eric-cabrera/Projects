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
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PRQRMTBLEventManagerTests
    {
        [TestMethod]
        public async Task ProcessEvent_CompanyCodeAndPolicyNumberIsNull_ShouldLogWarningAndReturnEarly()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PRQRMTBLEventManager>>();
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

            var prqrmEventManager = new PRQRMTBLEventManager(
                mockLogger.Object,
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockLifeProAccessor.Object);

            var prqrmtbl = new PRQRMTBL
            {
                COMPANY_CODE = "01",
                POLICY_NUMBER = "4150299379",
                NAME_ID = 922226,
                REQ_SEQUENCE = 0,
                SEQ = 1,
                UND_REQ_MET = "Y",
                UND_DESC_CODE = 1,
                UND_COMMENTS = string.Empty,
                UND_REQ_DATE = 20081023
            };

            // Act
            await prqrmEventManager.ProcessEvent(prqrmtbl);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPolicyNumber(
                    prqrmtbl.POLICY_NUMBER),
                Times.Once);

            var expectedMessage = "Unable to find policy record in LifePro for " +
                $"'{prqrmtbl.POLICY_NUMBER}' for the " +
                $"{nameof(PRQRMTBL)} event.";

            mockLogger.VerifyLog(LogLevel.Warning, expectedMessage);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public async Task ProcessEvent_PolicyExists_ShouldGetRequirementsAndUpdatePolicy()
        {
            // Arrange
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var mockLogger = new Mock<ILogger<PRQRMTBLEventManager>>();
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

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

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

            var prqrmtblEventManager = new PRQRMTBLEventManager(
                mockLogger.Object,
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockLifeProAccessor.Object);

            var prqrmtbl = new PRQRMTBL
            {
                COMPANY_CODE = "01",
                POLICY_NUMBER = "4150299379",
                NAME_ID = 922226,
                REQ_SEQUENCE = 0,
                SEQ = 1,
                UND_REQ_MET = "Y",
                UND_DESC_CODE = 1,
                UND_COMMENTS = string.Empty,
                UND_REQ_DATE = 20081023
            };

            // Act
            await prqrmtblEventManager.ProcessEvent(prqrmtbl);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByPolicyNumber(
                    prqrmtbl.POLICY_NUMBER),
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