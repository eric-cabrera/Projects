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
    using Assurity.Kafka.Utilities.Config;
    using Assurity.Kafka.Utilities.Extensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using AutoMapper;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PNAMEEventManagerTests
    {
        [TestMethod]
        public async Task ProcessEvent_PName_NotFound_In_Policies_ShouldLogWarningAndReturnEarly()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PNAMEEventManager>>();

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            SetupMockReturns(mockEventsAccessor);

            var pname = new PNAME
            {
                NAME_ID = 822007
            };

            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            mockMapper.Setup(mapper => mapper.Map<Person>(pname))
                .Returns(new Person());

            var mockConfig = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfig
                .Setup(configuration => configuration.KafkaSlowConsumerUpdateThreshold)
                .Returns(50);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);

            var pnameEventManager = new PNAMEEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockMapper.Object,
                mockConfig.Object);

            // Act
            await pnameEventManager.ProcessEvent(pname);

            // Assert
            var expectedMessage = "Processing PNAME event NameId: " + $"{pname.NAME_ID}" +
                " was not found in an existing policy and will be ignored";

            mockLogger.VerifyLog(LogLevel.Warning, expectedMessage);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithInsuredsByNameIdAsync(It.IsAny<int>()),
                Times.Never);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithAssigneeByNameIdAsync(It.IsAny<int>()),
                Times.Never);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithOwnersByNameIdAsync(It.IsAny<int>()),
                Times.Never);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithAnnuitantsByNameIdAsync(It.IsAny<int>()),
                Times.Never);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithPayeeByNameIdAsync(It.IsAny<int>()),
                Times.Never);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithPayorsByNameIdAsync(It.IsAny<int>()),
                Times.Never);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithBeneficiariesByNameIdAsync(It.IsAny<int>()),
                Times.Never);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithAgentsByNameIdAsync(It.IsAny<int>()),
                Times.Never);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithEmployerByNameIdAsync(It.IsAny<int>()),
                Times.Never);
        }

        [TestMethod]
        public async Task ProcessEvent_PName_Found_In_Insureds_Policies_Should_UpdatePolicies()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PNAMEEventManager>>();

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);

            SetupMockReturns(mockEventsAccessor);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithInsuredsByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(new List<Policy>
                {
                    new Policy
                    {
                        Insureds = new List<Insured>
                        {
                            new Insured
                            {
                                Participant = new Participant
                                {
                                    IsBusiness = true,
                                    Business = new Business
                                    {
                                        Name = new Name
                                        {
                                            NameId = 822007,
                                            BusinessName = "ABCD Company"
                                        }
                                    }
                                }
                            }
                        }
                    }
                });

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(1);

            var pname = new PNAME
            {
                NAME_ID = 822007,
                NAME_BUSINESS = "ABC Company",
                BUSINESS_EMAIL_ADR = "abc_company@gmail.com",
                INDIVIDUAL_FIRST = string.Empty,
                INDIVIDUAL_LAST = string.Empty,
                INDIVIDUAL_MIDDLE = string.Empty,
                INDIVIDUAL_PREFIX = string.Empty,
                INDIVIDUAL_SUFFIX = string.Empty,
                NAME_FORMAT_CODE = "B"
            };

            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            mockMapper.Setup(mapper => mapper.Map<Person>(pname))
                .Returns(new Person
                {
                    Name = new Name
                    {
                        NameId = 822007
                    }
                });

            var mockConfig = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfig
                .Setup(configuration => configuration.KafkaSlowConsumerUpdateThreshold)
                .Returns(50);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetRequirements(It.IsAny<Policy>()))
                .ReturnsAsync(() => null);

            var pnameEventManager = new PNAMEEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockMapper.Object,
                mockConfig.Object);

            // Act
            await pnameEventManager.ProcessEvent(pname);

            // Assert
            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithInsuredsByNameIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithAssigneeByNameIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithOwnersByNameIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithAnnuitantsByNameIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithPayeeByNameIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithPayorsByNameIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithBeneficiariesByNameIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithAgentsByNameIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithRequirementsByNameIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithAgentsByNameIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_PName_Found_In_Requirements_Policies_Should_UpdatePolicies()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PNAMEEventManager>>();

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);

            SetupMockReturns(mockEventsAccessor);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithRequirementsByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(new List<Policy>
                {
                    new Policy
                    {
                        Requirements = new List<Requirement>
                        {
                            new Requirement
                            {
                                AppliesTo = new Participant
                                {
                                    IsBusiness = true,
                                    Business = new Business
                                    {
                                        Name = new Name
                                        {
                                            NameId = 822007,
                                            BusinessName = "ABCD Company"
                                        }
                                    },
                                    Address = new Address
                                    {
                                        AddressId = 561234,
                                        Line1 = "123 A St",
                                        City = "Lincoln",
                                        StateAbbreviation = State.NE,
                                        Country = Country.USA
                                    }
                                }
                            }
                        }
                    }
                });

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdateNameAndEmailAddressInPolicyRequirements(
                    It.IsAny<Policy>(),
                    It.IsAny<Person>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(1);

            var pname = new PNAME
            {
                NAME_ID = 822007,
                NAME_BUSINESS = "ABC Company",
                BUSINESS_EMAIL_ADR = "abc_company@gmail.com",
                INDIVIDUAL_FIRST = string.Empty,
                INDIVIDUAL_LAST = string.Empty,
                INDIVIDUAL_MIDDLE = string.Empty,
                INDIVIDUAL_PREFIX = string.Empty,
                INDIVIDUAL_SUFFIX = string.Empty,
                NAME_FORMAT_CODE = "B"
            };

            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            mockMapper.Setup(mapper => mapper.Map<Person>(pname))
                .Returns(new Person
                {
                    Name = new Name
                    {
                        NameId = 822007
                    }
                });

            var mockConfig = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfig
                .Setup(configuration => configuration.KafkaSlowConsumerUpdateThreshold)
                .Returns(50);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(m => m.GetRequirements(It.IsAny<Policy>()))
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

            var pnameEventManager = new PNAMEEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockMapper.Object,
                mockConfig.Object);

            // Act
            await pnameEventManager.ProcessEvent(pname);

            // Assert
            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithInsuredsByNameIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithAssigneeByNameIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithOwnersByNameIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithAnnuitantsByNameIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithPayeeByNameIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithPayorsByNameIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithBeneficiariesByNameIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithAgentsByNameIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithRequirementsByNameIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithAgentsByNameIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>()),
                Times.Exactly(2));
        }

        [TestMethod]
        public async Task ProcessEvent_PNAME_Test_Slow_Consumer()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PNAMEEventManager>>();

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithInsuredsByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(new List<Policy> { new Policy() });
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithAnnuitantsByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(() => null);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithAgentsByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(() => null);
            mockEventsAccessor
               .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithPayorsByNameIdAsync(
                    It.IsAny<int>()))
               .ReturnsAsync(() => null);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithOwnersByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(() => null);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithBeneficiariesByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(() => null);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithPayeeByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(() => null);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithAssigneeByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(() => null);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithRequirementsByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(() => null);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithEmployerByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(() => null);

            var pname = new PNAME
            {
                NAME_ID = 822007,
            };

            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            mockMapper.Setup(mapper => mapper.Map<Person>(pname))
                .Returns(new Person
                {
                    Name = new Name
                    {
                        NameId = 822007,
                        BusinessName = "ABC Company"
                    },
                    EmailAddress = "abc_company@gmail.com"
                });

            var mockConfig = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfig
                .Setup(configuration => configuration.KafkaSlowConsumerUpdateThreshold)
                .Returns(-1);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(m => m.GetRequirements(It.IsAny<Policy>()))
                .ReturnsAsync(new List<Requirement>());

            var pnameEventManager = new PNAMEEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockMapper.Object,
                mockConfig.Object);

            Task? task = null;
            try
            {
                // Act
                task = pnameEventManager.ProcessEvent(pname, false);
                await task;
            }
            catch (SlowConsumerException ex)
            {
                // Assert
                Assert.IsInstanceOfType(ex, typeof(SlowConsumerException));
            }
            catch
            {
                // Assert
                Assert.Fail();
            }

            // Assert
            Assert.IsFalse(task?.IsCompletedSuccessfully);

            // Now process it off-line with slowConsumer = true.
            try
            {
                // Act
                task = pnameEventManager.ProcessEvent(pname, true);
                await task;
            }
            catch (SlowConsumerException)
            {
                Assert.Fail();
            }
        }

        private void SetupMockReturns(Mock<IEventsAccessor> mockEventsAccessor)
        {
            mockEventsAccessor
            .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithInsuredsByNameIdAsync(
                It.IsAny<int>()))
            .ReturnsAsync(() => null);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithAnnuitantsByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(() => null);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithAgentsByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(() => null);
            mockEventsAccessor
               .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithPayorsByNameIdAsync(
                    It.IsAny<int>()))
               .ReturnsAsync(() => null);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithOwnersByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(() => null);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithBeneficiariesByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(() => null);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithPayeeByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(() => null);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithAssigneeByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(() => null);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithRequirementsByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(() => null);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithEmployerByNameIdAsync(
                   It.IsAny<int>()))
                .ReturnsAsync(() => null);
        }
    }
}
