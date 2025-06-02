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
    public class PADDREventManagerTests
    {
        [TestMethod]
        public async Task ProcessEvent_PADDR_AddressId_NotFound_In_Policies_ShouldLogWarningAndReturnEarly()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PADDREventManager>>();

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            SetupMockReturns(mockEventsAccessor);

            var paddr = new PADDR
            {
                ADDRESS_ID = 678900,
                ADDR_LINE_1 = "1234 A St",
                ADDR_LINE_2 = "Line2",
                ADDR_LINE_3 = "Line3",
                STATE = "NE",
                CITY = "Lincoln",
                ZIP_EXTENSION = "56432",
                BOX_NUMBER = "10",
                COUNTRY = "USA",
                ZIP = "56789"
            };

            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            mockMapper.Setup(mapper => mapper.Map<Address>(paddr))
                .Returns(new Address());

            var mockConfig = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfig
                .Setup(configuration => configuration.KafkaSlowConsumerUpdateThreshold)
                .Returns(50);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);

            var paddrEventManager = new PADDREventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockMapper.Object,
                mockConfig.Object);

            // Act
            await paddrEventManager.ProcessEvent(paddr);

            // Assert
            var expectedMessage = "Processing PADDR event AddressId: " + $"{paddr.ADDRESS_ID}" +
                " was not found in an existing policy and will be ignored";

            mockLogger.VerifyLog(LogLevel.Warning, expectedMessage);

            mockEventsAccessor.Verify(
               cache => cache.GetPoliciesWithInsuredsByAddressIdAsync(It.IsAny<int>()),
               Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithAssigneeByAddressIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithOwnersByAddressIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithAnnuitantsByAddressIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithPayeeByAddressIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithPayorsByAddressIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithBeneficiariesByAddressIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithAgentsByAddressIdAsync(It.IsAny<int>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_PADDR_Found_In_Insureds_Policies_Should_UpdatePolicies()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PADDREventManager>>();

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);

            SetupMockReturns(mockEventsAccessor);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithInsuredsByAddressIdAsync(
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

            var paddr = new PADDR
            {
                ADDRESS_ID = 561234,
                ADDR_LINE_1 = "123 A St",
                CITY = "Lincoln",
                STATE = "NE",
                COUNTRY = "USA"
            };

            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            mockMapper.Setup(mapper => mapper.Map<Address>(paddr))
                .Returns(new Address
                {
                    AddressId = 561234,
                    Line1 = "123 A St",
                    City = "Lincoln",
                    StateAbbreviation = State.NE,
                    Country = Country.USA
                });

            var mockConfig = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfig
                .Setup(configuration => configuration.KafkaSlowConsumerUpdateThreshold)
                .Returns(50);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
               .Setup(policyEngine => policyEngine.GetRequirements(It.IsAny<Policy>()))
               .ReturnsAsync(() => null);

            var paddrEventManager = new PADDREventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockMapper.Object,
                mockConfig.Object);

            // Act
            await paddrEventManager.ProcessEvent(paddr);

            // Assert
            mockEventsAccessor.Verify(
               cache => cache.GetPoliciesWithInsuredsByAddressIdAsync(It.IsAny<int>()),
               Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithAssigneeByAddressIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithOwnersByAddressIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithAnnuitantsByAddressIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithPayeeByAddressIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithPayorsByAddressIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithRequirementsByAddressIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithBeneficiariesByAddressIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithAgentsByAddressIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_PADDR_Found_In_Requirements_Policies_Should_UpdatePolicies()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PADDREventManager>>();

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            SetupMockReturns(mockEventsAccessor);

            mockEventsAccessor
               .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithRequirementsByAddressIdAsync(
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
                .Setup(eventsAccessor => eventsAccessor.UpdateAddressInPolicyRequirements(
                    It.IsAny<Policy>(),
                    It.IsAny<Address>()))
                .ReturnsAsync(1);

            var paddr = new PADDR
            {
                ADDRESS_ID = 561234,
                ADDR_LINE_1 = "123 A St",
                CITY = "Lincoln",
                STATE = "NE",
                COUNTRY = "USA"
            };

            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            mockMapper.Setup(mapper => mapper.Map<Address>(paddr))
                .Returns(new Address
                {
                    AddressId = 561234,
                    Line1 = "123 A St",
                    City = "Lincoln",
                    StateAbbreviation = State.NE,
                    Country = Country.USA
                });

            var mockConfig = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfig
                .Setup(configuration => configuration.KafkaSlowConsumerUpdateThreshold)
                .Returns(50);

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

            var paddrEventManager = new PADDREventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockMapper.Object,
                mockConfig.Object);

            // Act
            await paddrEventManager.ProcessEvent(paddr);

            // Assert
            mockEventsAccessor.Verify(
               cache => cache.GetPoliciesWithInsuredsByAddressIdAsync(It.IsAny<int>()),
               Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithAssigneeByAddressIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithOwnersByAddressIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithAnnuitantsByAddressIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithPayeeByAddressIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithPayorsByAddressIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithBeneficiariesByAddressIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                cache => cache.GetPoliciesWithAgentsByAddressIdAsync(It.IsAny<int>()),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>()),
                Times.Exactly(2));
        }

        [TestMethod]
        public async Task ProcessEvent_PADDR_Test_Slow_Consumer()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PADDREventManager>>();

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            SetupMockReturns(mockEventsAccessor);

            var paddr = new PADDR
            {
                ADDRESS_ID = 678900,
                ADDR_LINE_1 = "1234 A St",
                ADDR_LINE_2 = "Line2",
                ADDR_LINE_3 = "Line3",
                STATE = "NE",
                CITY = "Lincoln",
                ZIP_EXTENSION = "56432",
                BOX_NUMBER = "10",
                COUNTRY = "USA",
                ZIP = "56789"
            };

            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            mockMapper.Setup(mapper => mapper.Map<Address>(paddr))
                .Returns(new Address());

            var mockConfig = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfig
                .Setup(configuration => configuration.KafkaSlowConsumerUpdateThreshold)
                .Returns(-1);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);

            var paddrEventManager = new PADDREventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockMapper.Object,
                mockConfig.Object);

            // Act And Assert
            Task? task = null;
            try
            {
                // Act
                task = paddrEventManager.ProcessEvent(paddr, false);
                await task;
            }
            catch (SlowConsumerException ex)
            {
                // Assert
                var expectedMessage = "Slow Consumer - PADDR event AddressId: 678900, Number of updates to process: 0. This will be done while disconnected from kafka";

                mockLogger.VerifyLog(LogLevel.Warning, expectedMessage);
                Assert.IsInstanceOfType(ex, typeof(SlowConsumerException));
            }
            catch
            {
                Assert.Fail();
            }

            Assert.IsFalse(task?.IsCompletedSuccessfully);
        }

        private void SetupMockReturns(Mock<IEventsAccessor> mockEventsAccessor)
        {
            mockEventsAccessor
              .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithInsuredsByAddressIdAsync(
                  It.IsAny<int>()))
              .ReturnsAsync(() => null);
            mockEventsAccessor
              .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithAnnuitantsByAddressIdAsync(
                  It.IsAny<int>()))
              .ReturnsAsync(() => null);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithAgentsByAddressIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(() => null);
            mockEventsAccessor
               .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithPayorsByAddressIdAsync(
                    It.IsAny<int>()))
               .ReturnsAsync(() => null);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithOwnersByAddressIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(() => null);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithBeneficiariesByAddressIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(() => null);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithPayeeByAddressIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(() => null);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithAssigneeByAddressIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(() => null);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithRequirementsByAddressIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(() => null);
        }
    }
}
