namespace Assurity.Kafka.Managers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Managers.Tests.Extensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PMEDREventManagerTests
    {
        [TestMethod]
        public async Task ProcessEvent_PoliciesAreNull_ShouldLogWarningAndReturnEarly()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PMEDREventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
              .Setup(eventsAccessor => eventsAccessor.GetPoliciesAsync(
                  It.IsAny<short>()))
              .ReturnsAsync(null as List<Policy>);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);

            var pmedrEventManager = new PMEDREventManager(
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockEventsAccessor.Object);

            var pmedr = new PMEDR
            {
                RECORD_TYPE = "R",
                REQ_NUMBER = 1,
                REQ_NAME = "MIB",
                REQ_DESCRIPTION = "MEDICAL INFORMATION BUREAU"
            };

            // Act
            await pmedrEventManager.ProcessEvent(pmedr);

            // Assert
            mockLogger.VerifyLog(LogLevel.Warning, It.IsAny<string>());

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPoliciesAsync(
                    It.IsAny<short>()),
                Times.Once);

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
            var mockLogger = new Mock<ILogger<PMEDREventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);

            var requirement = new Requirement
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
            };

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesAsync(
                    It.IsAny<short>()))
                .ReturnsAsync(new List<Policy>()
                {
                    new Policy()
                    {
                        PolicyNumber = "456788900",
                        Requirements = new List<Requirement>()
                        {
                           requirement
                        }
                    }
                });

            mockEventsAccessor
               .Setup(eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                   It.IsAny<Policy>(),
                   It.IsAny<object>(),
                   It.IsAny<string>()))
               .ReturnsAsync(1);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
               .Setup(policyEngine => policyEngine.UpdateRequirementName(
                   It.IsAny<List<Policy>>(),
                   It.IsAny<short>(),
                   It.IsAny<string>()))
               .Returns(new List<Policy>
               {
                    new Policy
                    {
                        PolicyNumber = "456788900",
                        Requirements = new List<Requirement>()
                        {
                            requirement
                        }
                    }
               });

            var pmedrEventManager = new PMEDREventManager(
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockEventsAccessor.Object);

            var pmedr = new PMEDR
            {
                RECORD_TYPE = "R",
                REQ_NUMBER = 1,
                REQ_NAME = "MIB",
                REQ_DESCRIPTION = "MEDICAL INFORMATION BUREAU"
            };

            // Act
            await pmedrEventManager.ProcessEvent(pmedr);

            // Assert
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPoliciesAsync(
                    It.IsAny<short>()),
                Times.Once);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.UpdateRequirementName(
                    It.IsAny<List<Policy>>(),
                    It.IsAny<short>(),
                    It.IsAny<string>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()),
                Times.Once);
        }
    }
}