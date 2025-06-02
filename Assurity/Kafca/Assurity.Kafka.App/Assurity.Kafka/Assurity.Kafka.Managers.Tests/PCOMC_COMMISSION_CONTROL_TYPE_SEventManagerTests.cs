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
    using Assurity.Kafka.Utilities.Enums;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PCOMC_COMMISSION_CONTROL_TYPE_SEventManagerTests
    {
        [TestMethod]
        public async Task ProcessEvent_CompanyCodeAndPolicyNumberIsNull_ShouldLogWarningAndReturnEarly()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PCOMC_COMMISSION_CONTROL_TYPE_SEventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByCOMCID(
                    It.IsAny<long>()))
                .ReturnsAsync(null as CompanyCodeAndPolicyNumber);

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            var pcomcTypeSEventManager = new PCOMC_COMMISSION_CONTROL_TYPE_SEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockLifeProAccessor.Object);

            var pcomcTypeS = new PCOMC_COMMISSION_CONTROL_TYPE_S
            {
                AGENT = "1111",
                AGENT_LEVEL = "90",
                COMC_ID = 1234,
                MARKET_CODE = "IS",
                SERVICE_AGENT_IND = "X"
            };

            // Act
            await pcomcTypeSEventManager.ProcessEvent(pcomcTypeS);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByCOMCID(
                    pcomcTypeS.COMC_ID),
                Times.Once);

            var expectedMessage = "Unable to find a Company Code and Policy Number " +
                $"associated with the {nameof(pcomcTypeS.COMC_ID)}: {pcomcTypeS.COMC_ID} " +
                $"for the {nameof(PCOMC_COMMISSION_CONTROL_TYPE_S)} event.";

            mockLogger.VerifyLog(LogLevel.Warning, expectedMessage);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetAgents(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>()),
                Times.Never);
        }

        [TestMethod]
        public async Task ProcessEvent_PolicyIsNull_ShouldLogInformationAndCreatePolicyRecordAndReturnEarly()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PCOMC_COMMISSION_CONTROL_TYPE_SEventManager>>();
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine.Setup(
                m => m.GetPolicy(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((GetPolicyResult.Found, new Policy()));
            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);

            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
            {
                CompanyCode = "01",
                PolicyNumber = "1122334455"
            };

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByCOMCID(
                    It.IsAny<long>()))
                .ReturnsAsync(companyCodeAndPolicyNumber);

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(null as Policy);

            var pcomcTypeSEventManager = new PCOMC_COMMISSION_CONTROL_TYPE_SEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockLifeProAccessor.Object);

            var pcomcTypeS = new PCOMC_COMMISSION_CONTROL_TYPE_S
            {
                AGENT = "1111",
                AGENT_LEVEL = "90",
                COMC_ID = 1234,
                MARKET_CODE = "IS",
                SERVICE_AGENT_IND = "X"
            };

            // Act
            await pcomcTypeSEventManager.ProcessEvent(pcomcTypeS);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByCOMCID(
                    pcomcTypeS.COMC_ID),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    companyCodeAndPolicyNumber.PolicyNumber,
                    companyCodeAndPolicyNumber.CompanyCode),
                Times.Once);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetAgents(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>()),
                Times.Never);
        }

        [TestMethod]
        public async Task ProcessEvent_PolicyExists_ShouldGetAgentsAndUpdatePolicy()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PCOMC_COMMISSION_CONTROL_TYPE_SEventManager>>();

            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
            {
                CompanyCode = "01",
                PolicyNumber = "1122334455"
            };

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByCOMCID(
                    It.IsAny<long>()))
                .ReturnsAsync(companyCodeAndPolicyNumber);

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new Policy
                {
                    PolicyNumber = "1122334455",
                    CompanyCode = "01",
                    ApplicationDate = new DateTime(2022, 01, 01)
                });
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyHierarchyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new PolicyHierarchy
                {
                    HierarchyBranches = new List<AgentHierarchy>
                    {
                        new AgentHierarchy
                        {
                            Agent = new Agent
                            {
                                AgentId = "1234",
                            },
                            HierarchyAgents = new List<HierarchyAgent>
                            {
                                new HierarchyAgent
                                {
                                    AgentId = "4321",
                                },
                                new HierarchyAgent
                                {
                                    AgentId = "abc123",
                                },
                            },
                        },
                        new AgentHierarchy
                        {
                            Agent = new Agent
                            {
                                AgentId = "abc321",
                            },
                            HierarchyAgents = new List<HierarchyAgent>
                            {
                                new HierarchyAgent
                                {
                                    AgentId = "abcd123",
                                },
                                new HierarchyAgent
                                {
                                    AgentId = "abc123",
                                },
                            },
                        },
                    }
                });

            var agent = new Agent
            {
                AgentId = "1111",
                IsServicingAgent = true,
                IsWritingAgent = false,
                MarketCode = "IS",
                Level = "90"
            };

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetAgents(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Agent>
                {
                    agent
                });

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.BuildAgentHierarchy(
                    It.IsAny<List<Agent>>(),
                    It.IsAny<DateTime>()))
                .ReturnsAsync(new List<AgentHierarchy>
                {
                    new AgentHierarchy
                    {
                        Agent = new Agent
                        {
                            AgentId = "1234",
                        },
                        HierarchyAgents = new List<HierarchyAgent>
                        {
                            new HierarchyAgent
                            {
                                AgentId = "4321",
                            },
                            new HierarchyAgent
                            {
                                AgentId = "abc123",
                            },
                        },
                    },
                    new AgentHierarchy
                    {
                        Agent = new Agent
                        {
                            AgentId = "abc321",
                        },
                        HierarchyAgents = new List<HierarchyAgent>
                        {
                            new HierarchyAgent
                            {
                                AgentId = "abcd123",
                            },
                            new HierarchyAgent
                            {
                                AgentId = "abc123",
                            },
                        },
                    },
                });

            mockHierarchyEngine
               .Setup(hierarchyEngine => hierarchyEngine.CompareAgentHierarchies(
                   It.IsAny<string>(),
                   It.IsAny<string>(),
                   It.IsAny<List<AgentHierarchy>>(),
                   It.IsAny<List<AgentHierarchy>>()))
               .Returns(new AgentHierarchiesDTO
               {
                   PolicyNumber = "1122334455",
                   CompanyCode = "01",
                   AddedAgents = new List<string>(),
                   RemovedAgents = new List<string>()
               });

            var pcomcTypeSEventManager = new PCOMC_COMMISSION_CONTROL_TYPE_SEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockLifeProAccessor.Object);

            var pcomcTypeS = new PCOMC_COMMISSION_CONTROL_TYPE_S
            {
                AGENT = "1111",
                AGENT_LEVEL = "90",
                COMC_ID = 1234,
                MARKET_CODE = "IS",
                SERVICE_AGENT_IND = "X"
            };

            // Act
            await pcomcTypeSEventManager.ProcessEvent(pcomcTypeS);

            // Assert
            mockLifeProAccessor.Verify(
                lifeProAccessor => lifeProAccessor.GetCompanyCodeAndPolicyNumberByCOMCID(
                    pcomcTypeS.COMC_ID),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    companyCodeAndPolicyNumber.PolicyNumber,
                    companyCodeAndPolicyNumber.CompanyCode),
                Times.Once);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetAgents(
                    companyCodeAndPolicyNumber.PolicyNumber,
                    companyCodeAndPolicyNumber.CompanyCode,
                    It.IsAny<DateTime>()),
                Times.Exactly(2));

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                    It.Is<Policy>(policy => AssertPolicy(policy, agent)),
                    It.IsAny<object>(),
                    It.IsAny<string>()),
                Times.Once);
        }

        private static bool AssertPolicy(
            Policy policy,
            Agent expectedAgent)
        {
            if (policy.Agents == null || policy.Agents.Count != 1)
            {
                return false;
            }

            var agent = policy.Agents[0];

            return agent.AgentId == expectedAgent.AgentId
                && agent.IsJustInTimeAgent == expectedAgent.IsJustInTimeAgent
                && agent.IsServicingAgent == expectedAgent.IsServicingAgent
                && agent.Level == expectedAgent.Level
                && agent.MarketCode == expectedAgent.MarketCode;
        }
    }
}