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
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class QUEUESEventManagerTests
    {
        [TestMethod]
        public async Task ProcessEvent_NoPolicyNumberFound_ShouldNotAttemptPolicyRetrieval()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<QUEUESEventManager>>();

            var mockGlobalDataAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            mockGlobalDataAccessor
                .Setup(globalDataAccessor => globalDataAccessor.GetPolicyNumber(It.IsAny<string>()))
                .ReturnsAsync(string.Empty);

            var mockSupportDataAccessor = new Mock<ISupportDataAccessor>(MockBehavior.Strict);
            mockSupportDataAccessor
                .Setup(supportDataAccessor => supportDataAccessor.IsJustInTimeQueue("abc123"))
                .ReturnsAsync(true);

            mockSupportDataAccessor
                .Setup(supportDataAccessor => supportDataAccessor.IsJustInTimeQueue("abc1234"))
                .ReturnsAsync(false);

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(It.IsAny<string>()));

            var queuesEventManager = new QUEUESEventManager(
                mockGlobalDataAccessor.Object,
                mockSupportDataAccessor.Object,
                mockEventsAccessor.Object,
                mockLogger.Object,
                null,
                null);

            var queues = new QUEUES
            {
                ID = "13044DEV1000000",
                QUEUE = "abc123"
            };

            var beforeQueue = "abc1234";

            // Act
            await queuesEventManager.ProcessEvent(queues, beforeQueue);

            // Assert/Verify
            mockEventsAccessor.Verify(
                   eventsAccessor => eventsAccessor.GetPolicyAsync(
                       It.IsAny<string>()),
                   Times.Never);

            var expectedMessage = "No policy numbers were found in Global associated with Folder Id:" + $" {queues.ID}";

            mockLogger.VerifyLog(LogLevel.Warning, expectedMessage);
        }

        [TestMethod]
        public async Task ProcessEvent_NoPolicyFoundInMongo_ShouldNotAttemptToUpdateData()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<QUEUESEventManager>>();

            var policyNumber = "1234567890";
            var mockGlobalDataAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            mockGlobalDataAccessor
                .Setup(globalDataAccessor => globalDataAccessor.GetPolicyNumber(It.IsAny<string>()))
                .ReturnsAsync(policyNumber);

            var mockSupportDataAccessor = new Mock<ISupportDataAccessor>(MockBehavior.Strict);
            mockSupportDataAccessor
                .Setup(supportDataAccessor => supportDataAccessor.IsJustInTimeQueue("abc123"))
                .ReturnsAsync(true);

            mockSupportDataAccessor
                .Setup(supportDataAccessor => supportDataAccessor.IsJustInTimeQueue("abc1234"))
                .ReturnsAsync(false);

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(It.IsAny<string>()))
                .ReturnsAsync(null as Policy);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);

            var queuesEventManager = new QUEUESEventManager(
                mockGlobalDataAccessor.Object,
                mockSupportDataAccessor.Object,
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                null);

            var queues = new QUEUES
            {
                ID = "13044DEV1000000",
                QUEUE = "abc123"
            };

            var beforeQueue = "abc1234";

            // Act
            await queuesEventManager.ProcessEvent(queues, beforeQueue);

            // Assert
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>()),
                Times.Once);

            var expectedMessage = "Policy not found in Mongo for policy number " +
                $"'{policyNumber}' for the {nameof(QUEUES)} event.";

            mockLogger.VerifyLog(LogLevel.Warning, expectedMessage);
        }

        [TestMethod]
        public async Task ProcessEvent_PolicyFound_ShouldRebuildDataAndUpdatePolicy()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<QUEUESEventManager>>();

            var policyNumber = "1234567890";
            var mockGlobalDataAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            mockGlobalDataAccessor
                .Setup(globalDataAccessor => globalDataAccessor.GetPolicyNumber(It.IsAny<string>()))
                .ReturnsAsync(policyNumber);

            var mockSupportDataAccessor = new Mock<ISupportDataAccessor>(MockBehavior.Strict);
            mockSupportDataAccessor
                .Setup(supportDataAccessor => supportDataAccessor.IsJustInTimeQueue("abc123"))
                .ReturnsAsync(true);

            mockSupportDataAccessor
                .Setup(supportDataAccessor => supportDataAccessor.IsJustInTimeQueue("abc1234"))
                .ReturnsAsync(false);

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(
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
               .Setup(eventsAccessor => eventsAccessor.UpdatePolicyHierarchyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<List<AgentHierarchy>>()))
               .ReturnsAsync("fsdf");

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
                    AddedAgents = new List<string>() { "34ZS" },
                    RemovedAgents = new List<string>() { "1234" }
                });

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.RemoveAgentPolicyAccessAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.InsertAgentPolicyAccessAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(1);

            var queuesEventManager = new QUEUESEventManager(
               mockGlobalDataAccessor.Object,
               mockSupportDataAccessor.Object,
               mockEventsAccessor.Object,
               mockLogger.Object,
               mockPolicyEngine.Object,
               mockHierarchyEngine.Object);

            var queues = new QUEUES
            {
                ID = "13044DEV1000000",
                QUEUE = "abc123"
            };

            var beforeQueue = "abc1234";

            // Act
            await queuesEventManager.ProcessEvent(queues, beforeQueue);

            // Assert
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>()),
                Times.Once);

            mockEventsAccessor
                .Verify(
                    eventsAccessor => eventsAccessor.UpdatePolicyAsync(
                        It.IsAny<Policy>(),
                        It.IsAny<object>(),
                        It.IsAny<string>()),
                    Times.Once());

            mockEventsAccessor
                .Verify(
                    eventsAccessor => eventsAccessor.UpdatePolicyHierarchyAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<List<AgentHierarchy>>()),
                    Times.Once());

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.InsertAgentPolicyAccessAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Exactly(1));

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.RemoveAgentPolicyAccessAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Exactly(1));
        }
    }
}
