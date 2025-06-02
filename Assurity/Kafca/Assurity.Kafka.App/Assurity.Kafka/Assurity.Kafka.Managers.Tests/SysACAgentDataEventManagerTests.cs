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
    public class SysACAgentDataEventManagerTests
    {
        [TestMethod]
        public async Task ProcessEvent_NoPolicyNumberFound_ShouldNotAttemptPolicyRetrieval()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<SysACAgentDataEventManager>>();

            var mockGlobalDataAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            mockGlobalDataAccessor
                .Setup(globalDataAccessor => globalDataAccessor.GetPolicyNumber(It.IsAny<string>()))
                .ReturnsAsync(string.Empty);

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(It.IsAny<string>()));

            var sysACAgentDataEventManager = new SysACAgentDataEventManager(
             mockGlobalDataAccessor.Object,
             null,
             mockLogger.Object,
             mockEventsAccessor.Object,
             null);

            var sysACAgentData = new SysACAgentData
            {
                FOLDERID = "1",
                AGENTID = "123456",
                FIRSTNAME = "First",
                MIDDLENAME = "Middle",
                LASTNAME = "Last",
                BUSINESSNAME = "Business"
            };

            // Act
            await sysACAgentDataEventManager.ProcessEvent(sysACAgentData);

            // Assert/Verify
            mockEventsAccessor.Verify(
                   eventsAccessor => eventsAccessor.GetPolicyAsync(
                       It.IsAny<string>()),
                   Times.Never);

            var expectedMessage = "No policy numbers were found in Global associated with Folder Id:" + $" {sysACAgentData.FOLDERID}";

            mockLogger.VerifyLog(LogLevel.Warning, expectedMessage);
        }

        [TestMethod]
        public async Task ProcessEvent_NoPolicyFoundInMongo_ShouldNotAttemptToUpdateData()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<SysACAgentDataEventManager>>();

            var policyNumber = "1234567890";
            var mockGlobalDataAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            mockGlobalDataAccessor
                .Setup(globalDataAccessor => globalDataAccessor.GetPolicyNumber(It.IsAny<string>()))
                .ReturnsAsync(policyNumber);

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyAsync(It.IsAny<string>()))
                .ReturnsAsync(null as Policy);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);

            var sysACAgentDataEventManager = new SysACAgentDataEventManager(
                mockGlobalDataAccessor.Object,
                null,
                mockLogger.Object,
                mockEventsAccessor.Object,
                mockPolicyEngine.Object);

            var sysACAgentData = new SysACAgentData
            {
                FOLDERID = "1",
                AGENTID = "123456",
                FIRSTNAME = "First",
                MIDDLENAME = "Middle",
                LASTNAME = "Last",
                BUSINESSNAME = "Business"
            };

            // Act
            await sysACAgentDataEventManager.ProcessEvent(sysACAgentData);

            // Assert
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyAsync(
                    It.IsAny<string>()),
                Times.Once);

            var expectedMessage = $"No policy was found in MongoDB with PolicyNumber: {policyNumber}";

            mockLogger.VerifyLog(LogLevel.Warning, expectedMessage);
        }

        [TestMethod]
        public async Task ProcessEvent_PolicyFound_ShouldRebuildDataAndUpdatePolicy()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<SysACAgentDataEventManager>>();

            var policyNumber = "1234567890";
            var mockGlobalDataAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            mockGlobalDataAccessor
                .Setup(globalDataAccessor => globalDataAccessor.GetPolicyNumber(It.IsAny<string>()))
                .ReturnsAsync(policyNumber);

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
               .ReturnsAsync("fsdff");

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

            var sysACAgentDataEventManager = new SysACAgentDataEventManager(
                mockGlobalDataAccessor.Object,
                mockHierarchyEngine.Object,
                mockLogger.Object,
                mockEventsAccessor.Object,
                mockPolicyEngine.Object);

            var sysACAgentData = new SysACAgentData
            {
                FOLDERID = "1",
                AGENTID = "123456",
                FIRSTNAME = "First",
                MIDDLENAME = "Middle",
                LASTNAME = "Last",
                BUSINESSNAME = "Business"
            };

            // Act
            await sysACAgentDataEventManager.ProcessEvent(sysACAgentData);

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
