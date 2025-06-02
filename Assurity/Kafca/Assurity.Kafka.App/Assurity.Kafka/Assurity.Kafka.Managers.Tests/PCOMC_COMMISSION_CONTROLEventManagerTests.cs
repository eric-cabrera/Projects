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
    using Assurity.Kafka.Utilities.Enums;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PCOMC_COMMISSION_CONTROLEventManagerTests
    {
        [TestMethod]
        public async Task ProcessEvent_PolicyIsNull_ShouldLogInformationAndCreatePolicyRecordAndReturnEarly()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PCOMC_COMMISSION_CONTROLEventManager>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(null as Policy);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine.Setup(
                m => m.GetPolicy(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((GetPolicyResult.Found, new Policy()));

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);

            var pcomcEventManager = new PCOMC_COMMISSION_CONTROLEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object);

            var pcomc = new PCOMC_COMMISSION_CONTROL
            {
                COMC_ID = 1234,
                COMPANY_CODE = "01",
                POLICY_NUMBER = "1122334455",
                RECORD_TYPE = "S"
            };

            // Act
            await pcomcEventManager.ProcessEvent(pcomc);

            // Assert
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    pcomc.POLICY_NUMBER,
                    pcomc.COMPANY_CODE),
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
            var mockLogger = new Mock<ILogger<PCOMC_COMMISSION_CONTROLEventManager>>();

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

            mockEventsAccessor
                .Setup(cache => cache.UpdatePolicyHierarchyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<List<AgentHierarchy>>()))
                .ReturnsAsync("jfgsdjfgs");

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

            var pcomcEventManager = new PCOMC_COMMISSION_CONTROLEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object);

            var pcomc = new PCOMC_COMMISSION_CONTROL
            {
                COMC_ID = 1234,
                COMPANY_CODE = "01",
                POLICY_NUMBER = "1122334455",
                RECORD_TYPE = "S"
            };

            // Act
            await pcomcEventManager.ProcessEvent(pcomc);

            // Assert
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    pcomc.POLICY_NUMBER,
                    pcomc.COMPANY_CODE),
                Times.Once);

            mockPolicyEngine.Verify(
                policyEngine => policyEngine.GetAgents(
                    pcomc.POLICY_NUMBER,
                    pcomc.COMPANY_CODE,
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