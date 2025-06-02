namespace Assurity.Kafka.Managers.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Utilities.Config;
    using Assurity.Kafka.Utilities.Extensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PNALKEventManagerTests
    {
        [TestMethod]
        public async Task ProcessEvent_PNALK_NameId_NotFound_In_Policies_ShouldLogWarningAndReturn()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PNALKEventManager>>();

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            SetupMockReturns(mockEventsAccessor);

            mockEventsAccessor
                   .Setup(cache => cache.UpdatePolicyAsync(
                       It.IsAny<Policy>(),
                       It.IsAny<Dictionary<string, object>>()))
                   .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithInsuredsByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(new List<Policy>
                {
                    new Policy
                    {
                        PolicyNumber = "123456789",
                        Insureds = new List<Insured>
                        {
                            new Insured
                            {
                                Participant = new Participant
                                {
                                    IsBusiness = true,
                                    Person = new Person
                                    {
                                        Name = new Name
                                        {
                                            NameId = 822007,
                                        }
                                    }
                                }
                            }
                        }
                    }
                });

            var pnalk = new PNALK
            {
                NAME_ID = 822007,
                CANCEL_DATE = 0,
                ADDRESS_CODE = string.Empty,
                ADDRESS_ID = 1,
                TELE_NUM = "3125882300"
            };

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetPADDR(It.IsAny<int>()))
                .ReturnsAsync(new PADDR
                {
                    ADDRESS_ID = 1,
                    ADDR_LINE_1 = "123 Fake Street   ",
                    ADDR_LINE_2 = "APT 22    ",
                    ADDR_LINE_3 = "Line 3   ",
                    CITY = "Lincoln    ",
                    STATE = "NE",
                    ZIP = "68522",
                    ZIP_EXTENSION = "1234    ",
                    BOX_NUMBER = "68522",
                    COUNTRY = "USA    "
                });

            var mockConfig = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfig
                .Setup(configuration => configuration.KafkaSlowConsumerUpdateThreshold)
                .Returns(50);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
            .Setup(policyEngine => policyEngine.GetRequirements(It.IsAny<Policy>()))
            .ReturnsAsync(() => null);

            var pnalkEventManager = new PNALKEventManager(
                mockLogger.Object,
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockConfig.Object);

            // Act
            await pnalkEventManager.ProcessEvent(pnalk);

            // Assert
            var expectedMessage = $"PNALK event - Participant has no address. Attempted to update " +
                    $"AddressId: {pnalk.ADDRESS_ID} " +
                    $"NameId: {pnalk.NAME_ID} " +
                    $"TelephoneNumner {pnalk.TELE_NUM.ToPhoneNumber()}: " +
                    "for PolicyNumber: 123456789";

            mockLifeProAccessor.Verify(
                db => db.GetPADDR(1),
                Times.Never);

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
        }

        [TestMethod]
        public async Task ProcessEvent_PNALK_Found_In_Policies_Should_UpdatePolicies_InsuredIsBusiness()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PNALKEventManager>>();

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

            var pnalk = new PNALK
            {
                NAME_ID = 822007,
                CANCEL_DATE = 0,
                ADDRESS_CODE = string.Empty,
                ADDRESS_ID = 1,
                TELE_NUM = "3125882300"
            };

            var mockConfig = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfig
                .Setup(configuration => configuration.KafkaSlowConsumerUpdateThreshold)
                .Returns(50);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
            .Setup(policyEngine => policyEngine.GetRequirements(It.IsAny<Policy>()))
            .ReturnsAsync(() => null);

            var pnameEventManager = new PNALKEventManager(
                mockLogger.Object,
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                null,
                mockConfig.Object);

            // Act
            await pnameEventManager.ProcessEvent(pnalk);

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

        // This is for the  case when a Policy is found to have a Participant
        // with a nameID that matches the nameID in the PNALK event. Also, The addressID in the
        // PNALK matches the addressID in Participant. No change in relationship between a Participants
        // Name and Address was made and no changes to the Policy should be made.
        // If the underlying address is changed PADDR event will handle it.
        [TestMethod]
        public async Task ProcessEvent_PNALK_NameIdFound_In_Policies_SameAddr_ShouldNotUpdatePolicy_InsuredIsPerson_NoPhone()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PNALKEventManager>>();

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            SetupMockReturns(mockEventsAccessor);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithInsuredsByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(new List<Policy>
                {
                    new Policy
                    {
                        PolicyNumber = "123456789",
                        Insureds = new List<Insured>
                        {
                            new Insured
                            {
                                Participant = new Participant
                                {
                                    IsBusiness = true,
                                    Person = new Person
                                    {
                                        Name = new Name
                                        {
                                            NameId = 822006,
                                            IndividualFirst = "First",
                                            IndividualLast = "Last"
                                        },
                                    },
                                    Address = new Address
                                    {
                                        AddressId = 1,
                                        Line1 = "123 A St",
                                        City = "Lincoln",
                                        StateAbbreviation = State.NE,
                                        Country = Country.USA
                                    }
                                }
                            },
                        },
                    }
                });

            var pnalk = new PNALK
            {
                NAME_ID = 822006,
                CANCEL_DATE = 0,
                ADDRESS_CODE = string.Empty,
                ADDRESS_ID = 1,
            };

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetPADDR(It.IsAny<int>()))
                .ReturnsAsync(new PADDR
                {
                    ADDRESS_ID = 1,
                    ADDR_LINE_1 = "123 Fake Street   ",
                    ADDR_LINE_2 = "APT 22    ",
                    ADDR_LINE_3 = "Line 3   ",
                    CITY = "Lincoln    ",
                    STATE = "NE",
                    ZIP = "68522",
                    ZIP_EXTENSION = "1234    ",
                    BOX_NUMBER = "68522",
                    COUNTRY = "USA    "
                });

            var mockConfig = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfig
                .Setup(configuration => configuration.KafkaSlowConsumerUpdateThreshold)
                .Returns(50);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetRequirements(It.IsAny<Policy>()))
                .ReturnsAsync(() => null);

            var pnameEventManager = new PNALKEventManager(
                mockLogger.Object,
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockConfig.Object);

            // Act
            await pnameEventManager.ProcessEvent(pnalk);

            // Assert
            mockLifeProAccessor.Verify(
                db => db.GetPADDR(1),
                Times.Never);

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
                Times.Never);
        }

        // This is for the  case when a Policy is found to have a Participant
        // with a nameID that matches the nameID in the PNALK event. Also, The addressID in the
        // PNALK matches the addressID in Participant. No change in relationship between a Participants
        // Name and Address was made and no changes to the Policy should be made.
        // If the underlying address is changed PADDR event will handle it.
        [TestMethod]
        public async Task ProcessEvent_PNALK_NameIdFound_In_Policies_DiffAddr_ShouldUpdatePolicy_InsuredIsPerson_NoPhone()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PNALKEventManager>>();

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            SetupMockReturns(mockEventsAccessor);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithInsuredsByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(new List<Policy>
                {
                    new Policy
                    {
                        PolicyNumber = "123456789",
                        Insureds = new List<Insured>
                        {
                            new Insured
                            {
                                Participant = new Participant
                                {
                                    IsBusiness = true,
                                    Person = new Person
                                    {
                                        Name = new Name
                                        {
                                            NameId = 822006,
                                            IndividualFirst = "First",
                                            IndividualLast = "Last"
                                        },
                                    },
                                    Address = new Address
                                    {
                                        AddressId = 1,
                                        Line1 = "123 A St",
                                        City = "Lincoln",
                                        StateAbbreviation = State.NE,
                                        Country = Country.USA
                                    }
                                }
                            },
                        },
                    }
                });

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(1);

            var pnalk = new PNALK
            {
                NAME_ID = 822006,
                CANCEL_DATE = 0,
                ADDRESS_CODE = string.Empty,
                ADDRESS_ID = 2,
            };

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetPADDR(It.IsAny<int>()))
                .ReturnsAsync(new PADDR
                {
                    ADDRESS_ID = 2,
                    ADDR_LINE_1 = "123 Fake Street   ",
                    ADDR_LINE_2 = "APT 22    ",
                    ADDR_LINE_3 = "Line 3   ",
                    CITY = "Lincoln    ",
                    STATE = "NE",
                    ZIP = "68522",
                    ZIP_EXTENSION = "1234    ",
                    BOX_NUMBER = "68522",
                    COUNTRY = "USA    "
                });

            var mockConfig = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfig
                .Setup(configuration => configuration.KafkaSlowConsumerUpdateThreshold)
                .Returns(50);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetRequirements(It.IsAny<Policy>()))
                .ReturnsAsync(() => null);

            var pnameEventManager = new PNALKEventManager(
                mockLogger.Object,
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockConfig.Object);

            // Act
            await pnameEventManager.ProcessEvent(pnalk);

            // Assert
            mockLifeProAccessor.Verify(
                db => db.GetPADDR(1),
                Times.Never);

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
        public async Task ProcessEvent_PNALK_NameIdFound_In_Policies_DiffAddr_ShouldUpdatePolicy_RequirementHasPerson_NoPhone()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PNALKEventManager>>();

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            SetupMockReturns(mockEventsAccessor);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPoliciesWithRequirementsByNameIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(new List<Policy>
                {
                    new Policy
                    {
                        PolicyNumber = "123456789",
                        Requirements = new List<Requirement>
                        {
                            new Requirement
                            {
                                AppliesTo = new Participant
                                {
                                    IsBusiness = true,
                                    Person = new Person
                                    {
                                        Name = new Name
                                        {
                                            NameId = 822006,
                                            IndividualFirst = "First",
                                            IndividualLast = "Last"
                                        },
                                    },
                                    Address = new Address
                                    {
                                        AddressId = 1,
                                        Line1 = "123 A St",
                                        City = "Lincoln",
                                        StateAbbreviation = State.NE,
                                        Country = Country.USA
                                    }
                                }
                            },
                        },
                    }
                });

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdateAddressInPolicyRequirements(
                    It.IsAny<Policy>(),
                    It.IsAny<Address>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePhoneNumberInPolicyRequirements(
                    It.IsAny<Policy>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()))
                .ReturnsAsync(1);

            var pnalk = new PNALK
            {
                NAME_ID = 822006,
                CANCEL_DATE = 0,
                ADDRESS_CODE = string.Empty,
                ADDRESS_ID = 2,
            };

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetPADDR(It.IsAny<int>()))
                .ReturnsAsync(new PADDR
                {
                    ADDRESS_ID = 2,
                    ADDR_LINE_1 = "123 Fake Street   ",
                    ADDR_LINE_2 = "APT 22    ",
                    ADDR_LINE_3 = "Line 3   ",
                    CITY = "Lincoln    ",
                    STATE = "NE",
                    ZIP = "68522",
                    ZIP_EXTENSION = "1234    ",
                    BOX_NUMBER = "68522",
                    COUNTRY = "USA    "
                });

            var mockConfig = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfig
                .Setup(configuration => configuration.KafkaSlowConsumerUpdateThreshold)
                .Returns(50);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetRequirements(It.IsAny<Policy>()))
                .ReturnsAsync(() => null);

            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetRequirements(It.IsAny<Policy>()))
                .ReturnsAsync(new List<Requirement>
                {
                    new Requirement
                        {
                            AppliesTo = new Participant
                            {
                                IsBusiness = true,
                                Person = new Person
                                {
                                    Name = new Name
                                    {
                                        NameId = 822006,
                                        IndividualFirst = "First",
                                        IndividualLast = "Last"
                                    },
                                },
                                Address = new Address
                                {
                                    AddressId = 2,
                                    Line1 = "123 A St",
                                    City = "Lincoln",
                                    StateAbbreviation = State.NE,
                                    Country = Country.USA
                                }
                            }
                        },
                });

            var pnameEventManager = new PNALKEventManager(
                mockLogger.Object,
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                mockLifeProAccessor.Object,
                mockConfig.Object);

            // Act
            await pnameEventManager.ProcessEvent(pnalk);

            // Assert
            mockLifeProAccessor.Verify(
                db => db.GetPADDR(2),
                Times.Once);

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
        public async Task ProcessEvent_PNALK_Test_Slow_Consumer()
        {
            // Arrange
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

            var pnalk = new PNALK
            {
                NAME_ID = 822007,
            };

            var mockLogger = new Mock<ILogger<PNALKEventManager>>();
            var mockConfig = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfig
                .Setup(configuration => configuration.KafkaSlowConsumerUpdateThreshold)
                .Returns(-1);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetRequirements(It.IsAny<Policy>()))
                .ReturnsAsync(() => null);

            var pnameEventManager = new PNALKEventManager(
                mockLogger.Object,
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                null,
                mockConfig.Object);

            Task? task = null;
            try
            {
                // Act
                task = pnameEventManager.ProcessEvent(pnalk, false);
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
                task = pnameEventManager.ProcessEvent(pnalk, true);
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