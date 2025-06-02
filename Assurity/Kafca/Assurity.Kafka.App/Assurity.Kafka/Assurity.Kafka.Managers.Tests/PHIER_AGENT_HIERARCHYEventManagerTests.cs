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
    using Assurity.Kafka.Utilities.Config;
    using Assurity.Kafka.Utilities.Constants;
    using Assurity.Kafka.Utilities.Extensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MongoDB.Bson;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PHIER_AGENT_HIERARCHYEventManagerTests
    {
        [TestMethod]
        public async Task ProcessEvent_AgentNumExists_ShouldGetAndUpdateAgentPolicyAccess()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PHIER_AGENT_HIERARCHYEventManager>>();

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetAgentPolicyAccessAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new AgentPolicyAccess
                {
                    AgentId = "1234",
                    CompanyCode = "01",
                    PolicyNumbers = new List<string>
                    {
                        "123456789",
                        "9087878786"
                    }
                });

            var agentHierarchies = new List<AgentHierarchy>
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
                }
            };

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyHierarchyAsync(
                    "123456789",
                    "01"))
                .ReturnsAsync(new PolicyHierarchy
                {
                    PolicyNumber = "123456789",
                    CompanyCode = "01",
                    HierarchyBranches = agentHierarchies
                });

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyHierarchyAsync(
                    "9087878786",
                    "01"))
                .ReturnsAsync(new PolicyHierarchy
                {
                    PolicyNumber = "9087878786",
                    CompanyCode = "01",
                    HierarchyBranches = agentHierarchies
                });

            mockEventsAccessor
                .Setup(cache => cache.UpdatePolicyHierarchyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<List<AgentHierarchy>>()))
                .ReturnsAsync("jfgsdjfgs");

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
               .Setup(hierarchyEngine => hierarchyEngine.CompareAgentHierarchies(
                   It.IsAny<string>(),
                   It.IsAny<string>(),
                   It.IsAny<List<AgentHierarchy>>(),
                   It.IsAny<List<AgentHierarchy>>()))
               .Returns(new AgentHierarchiesDTO
               {
                   PolicyNumber = "123456789",
                   CompanyCode = "01",
                   AddedAgents = new List<string>() { "34ZS" },
                   RemovedAgents = new List<string>() { "1234" }
               });

            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.BuildAgentHierarchy(
                    It.IsAny<List<Agent>>(),
                    It.IsAny<DateTime>()))
                .ReturnsAsync(agentHierarchies);

            var mockConfig = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfig
                .Setup(configuration => configuration.KafkaSlowConsumerUpdateThreshold)
                .Returns(50);

            var mockDataStoreAccessor = new Mock<IDataStoreAccessor>(MockBehavior.Strict);
            mockDataStoreAccessor
                .Setup(dataStoreAccessor => dataStoreAccessor.AddSystemDataLoadWithAgentHierarchyChange(
                    It.IsAny<PHIER_AGENT_HIERARCHY>(),
                    It.IsAny<ChangeType>(),
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var phierAgentHierarchyEventManager = new PHIER_AGENT_HIERARCHYEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockConfig.Object,
                mockDataStoreAccessor.Object);

            var phier = new PHIER_AGENT_HIERARCHY
            {
                COMPANY_CODE = "01",
                AGENT_NUM = "2ZVR",
                MARKET_CODE = "JM",
                AGENT_LEVEL = "40",
                STOP_DATE = 20230920,
                START_DATE = 20230120,
                HIERARCHY_AGENT = "2ZVR",
                HIER_MARKET_CODE = "JM",
                HIER_AGENT_LEVEL = "70"
            };

            var changeType = TopicOperations.Create;
            var beforeAgentNum = "2ZVR";

            // Act
            await phierAgentHierarchyEventManager.ProcessEvent(phier, changeType, beforeAgentNum);

            // Assert
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetAgentPolicyAccessAsync(
                    "2ZVR",
                    "01"),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyHierarchyAsync(
                    "123456789",
                    "01"),
                Times.Exactly(1));

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyHierarchyAsync(
                    "9087878786",
                    "01"),
                Times.Exactly(1));

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyHierarchyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<List<AgentHierarchy>>()),
                Times.Exactly(2));

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.InsertAgentPolicyAccessAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Exactly(2));

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.RemoveAgentPolicyAccessAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Exactly(2));

            mockDataStoreAccessor.Verify(
                dataStoreAccessor => dataStoreAccessor.AddSystemDataLoadWithAgentHierarchyChange(
                    It.IsAny<PHIER_AGENT_HIERARCHY>(),
                    It.Is<ChangeType>((actualChangeType) => actualChangeType == ChangeType.Create),
                    It.IsAny<string>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_ChangeTypeEvent_AgentNumExists_ShouldGetAndUpdateAgentPolicyAcess()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PHIER_AGENT_HIERARCHYEventManager>>();

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetAgentPolicyAccessAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new AgentPolicyAccess
                {
                    AgentId = "2ZVR",
                    CompanyCode = "01",
                    PolicyNumbers = new List<string>
                    {
                        "123456789",
                        "9087878786"
                    }
                });

            var agentHierarchies = new List<AgentHierarchy>
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
                }
            };

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyHierarchyAsync(
                    "123456789",
                    "01"))
                .ReturnsAsync(new PolicyHierarchy
                {
                    PolicyNumber = "123456789",
                    CompanyCode = "01",
                    HierarchyBranches = agentHierarchies
                });

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyHierarchyAsync(
                    "9087878786",
                    "01"))
                .ReturnsAsync(new PolicyHierarchy
                {
                    PolicyNumber = "9087878786",
                    CompanyCode = "01",
                    HierarchyBranches = agentHierarchies
                });

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.UpdatePolicyHierarchyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<List<AgentHierarchy>>()))
                .ReturnsAsync("fsdfsdg");

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
               .Setup(hierarchyEngine => hierarchyEngine.CompareAgentHierarchies(
                   It.IsAny<string>(),
                   It.IsAny<string>(),
                   It.IsAny<List<AgentHierarchy>>(),
                   It.IsAny<List<AgentHierarchy>>()))
               .Returns(new AgentHierarchiesDTO
               {
                   PolicyNumber = "123456789",
                   CompanyCode = "01",
                   AddedAgents = new List<string>() { "34ZS" },
                   RemovedAgents = new List<string>() { "1234" }
               });

            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.BuildAgentHierarchy(
                    It.IsAny<List<Agent>>(),
                    It.IsAny<DateTime>()))
                .ReturnsAsync(agentHierarchies);

            var mockConfig = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfig
                .Setup(configuration => configuration.KafkaSlowConsumerUpdateThreshold)
                .Returns(50);

            var mockDataStoreAccessor = new Mock<IDataStoreAccessor>(MockBehavior.Strict);
            mockDataStoreAccessor
                .Setup(dataStoreAccessor => dataStoreAccessor.AddSystemDataLoadWithAgentHierarchyChange(
                    It.IsAny<PHIER_AGENT_HIERARCHY>(),
                    It.IsAny<ChangeType>(),
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var phierAgentHierarchyEventManager = new PHIER_AGENT_HIERARCHYEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockConfig.Object,
                mockDataStoreAccessor.Object);

            var phier = new PHIER_AGENT_HIERARCHY
            {
                COMPANY_CODE = "01",
                AGENT_NUM = "2ZVR",
                MARKET_CODE = "JM",
                AGENT_LEVEL = "40",
                STOP_DATE = 20230920,
                START_DATE = 20230120,
                HIERARCHY_AGENT = "2ZVR",
                HIER_MARKET_CODE = "JM",
                HIER_AGENT_LEVEL = "70"
            };

            var changeType = TopicOperations.Delete;
            var beforeAgentNum = "2ZVR";

            // Act
            await phierAgentHierarchyEventManager.ProcessEvent(phier, changeType, beforeAgentNum);

            // Assert
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetAgentPolicyAccessAsync(
                    "2ZVR",
                    "01"),
                Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.GetPolicyHierarchyAsync(
                    "123456789",
                    "01"),
                Times.Exactly(1));

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdatePolicyHierarchyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<List<AgentHierarchy>>()),
                Times.Exactly(2));

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.InsertAgentPolicyAccessAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Exactly(2));

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.RemoveAgentPolicyAccessAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Exactly(2));

            mockDataStoreAccessor.Verify(
                dataStoreAccessor => dataStoreAccessor.AddSystemDataLoadWithAgentHierarchyChange(
                    It.IsAny<PHIER_AGENT_HIERARCHY>(),
                    It.Is<ChangeType>((actualChangeType) => actualChangeType == ChangeType.Delete),
                    It.IsAny<string>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_PHIER_AGENT_HIERARCHY_Test_Slow_Consumer()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PHIER_AGENT_HIERARCHYEventManager>>();

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetAgentPolicyAccessAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new AgentPolicyAccess
                {
                    AgentId = "1234",
                    CompanyCode = "01",
                    PolicyNumbers = new List<string>
                    {
                        "123456789",
                        "9087878786"
                    }
                });

            var agentHierarchies = new List<AgentHierarchy>
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
                }
            };

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyHierarchyAsync(
                    "123456789",
                    "01"))
                .ReturnsAsync(new PolicyHierarchy
                {
                    PolicyNumber = "123456789",
                    CompanyCode = "01",
                    HierarchyBranches = agentHierarchies
                });

            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetPolicyHierarchyAsync(
                    "9087878786",
                    "01"))
                .ReturnsAsync(new PolicyHierarchy
                {
                    PolicyNumber = "9087878786",
                    CompanyCode = "01",
                    HierarchyBranches = agentHierarchies
                });

            mockEventsAccessor
                .Setup(cache => cache.UpdatePolicyHierarchyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<List<AgentHierarchy>>()))
                .ReturnsAsync("jfgsdjfgs");

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
               .Setup(hierarchyEngine => hierarchyEngine.CompareAgentHierarchies(
                   It.IsAny<string>(),
                   It.IsAny<string>(),
                   It.IsAny<List<AgentHierarchy>>(),
                   It.IsAny<List<AgentHierarchy>>()))
               .Returns(new AgentHierarchiesDTO
               {
                   PolicyNumber = "123456789",
                   CompanyCode = "01",
                   AddedAgents = new List<string>() { "34ZS" },
                   RemovedAgents = new List<string>() { "1234" }
               });

            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.BuildAgentHierarchy(
                    It.IsAny<List<Agent>>(),
                    It.IsAny<DateTime>()))
                .ReturnsAsync(agentHierarchies);

            var mockConfig = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfig
                .Setup(configuration => configuration.KafkaSlowConsumerUpdateThreshold)
                .Returns(-1);

            var mockDataStoreAccessor = new Mock<IDataStoreAccessor>(MockBehavior.Strict);
            mockDataStoreAccessor
                .Setup(dataStoreAccessor => dataStoreAccessor.AddSystemDataLoadWithAgentHierarchyChange(
                    It.IsAny<PHIER_AGENT_HIERARCHY>(),
                    It.IsAny<ChangeType>(),
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var phierAgentHierarchyEventManager = new PHIER_AGENT_HIERARCHYEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockConfig.Object,
                mockDataStoreAccessor.Object);

            var phier = new PHIER_AGENT_HIERARCHY
            {
                COMPANY_CODE = "01",
                AGENT_NUM = "2ZVR",
                MARKET_CODE = "JM",
                AGENT_LEVEL = "40",
                STOP_DATE = 20230920,
                START_DATE = 20230120,
                HIERARCHY_AGENT = "2ZVR",
                HIER_MARKET_CODE = "JM",
                HIER_AGENT_LEVEL = "70"
            };

            var changeType = TopicOperations.Create;
            var beforeAgentNum = "2ZVR";

            // Act
            Task? task = null;
            try
            {
                // Act
                task = phierAgentHierarchyEventManager.ProcessEvent(phier, changeType, beforeAgentNum, false);
                await task;
            }
            catch (SlowConsumerException ex)
            {
                var invo = mockLogger.Invocations;
                var josn = mockLogger.ToJson();

                // Assert
                var expectedMessage = "Slow Consumer - PHIER_AGENT_HIERARCHY event AgentNum: 2ZVR, Number of updates to process: 2. This will be done while disconnected from kafka";

                mockLogger.VerifyLog(LogLevel.Warning, expectedMessage);
                Assert.IsInstanceOfType(ex, typeof(SlowConsumerException));
            }
            catch
            {
                // Assert
                Assert.Fail();
            }

            // Assert
            Assert.IsFalse(task?.IsCompletedSuccessfully);

            mockDataStoreAccessor.Verify(
                dataStoreAccessor => dataStoreAccessor.AddSystemDataLoadWithAgentHierarchyChange(
                    It.IsAny<PHIER_AGENT_HIERARCHY>(),
                    It.IsAny<ChangeType>(),
                    It.IsAny<string>()),
                Times.Never);

            // Now process it off-line with slowConsumer = true.
            try
            {
                // Act
                task = phierAgentHierarchyEventManager.ProcessEvent(phier, changeType, beforeAgentNum, true);
                await task;
            }
            catch (SlowConsumerException)
            {
                Assert.Fail();
            }

            mockDataStoreAccessor.Verify(
                dataStoreAccessor => dataStoreAccessor.AddSystemDataLoadWithAgentHierarchyChange(
                    It.IsAny<PHIER_AGENT_HIERARCHY>(),
                    It.Is<ChangeType>((actualChangeType) => actualChangeType == ChangeType.Create),
                    It.IsAny<string>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_UnexpectedChangeType_ShouldThrowNotImplementedException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PHIER_AGENT_HIERARCHYEventManager>>();

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.GetAgentPolicyAccessAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(null as AgentPolicyAccess);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            var mockConfig = new Mock<IConfigurationManager>(MockBehavior.Strict);

            var mockDataStoreAccessor = new Mock<IDataStoreAccessor>(MockBehavior.Strict);
            mockDataStoreAccessor
                .Setup(dataStoreAccessor => dataStoreAccessor.AddSystemDataLoadWithAgentHierarchyChange(
                    It.IsAny<PHIER_AGENT_HIERARCHY>(),
                    It.IsAny<ChangeType>(),
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var phierAgentHierarchyEventManager = new PHIER_AGENT_HIERARCHYEventManager(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockConfig.Object,
                mockDataStoreAccessor.Object);

            var phier = new PHIER_AGENT_HIERARCHY
            {
                COMPANY_CODE = "01",
                AGENT_NUM = "BBC2"
            };

            var changeType = "z";
            var beforeAgentNum = "BBC2";

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<NotImplementedException>(async () =>
                await phierAgentHierarchyEventManager.ProcessEvent(phier, changeType, beforeAgentNum));

            var expectedMessage = $"{nameof(ChangeType)} '{changeType}' is not handled.";

            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}