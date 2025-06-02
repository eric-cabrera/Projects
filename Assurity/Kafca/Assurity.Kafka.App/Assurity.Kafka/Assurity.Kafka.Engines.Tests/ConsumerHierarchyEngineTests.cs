namespace Assurity.Kafka.Engines.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Engines.Hierarchy;
    using Assurity.Kafka.Utilities;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Xunit.Sdk;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ConsumerHierarchyEngineTests
    {
        [TestMethod]
        public async Task GetPolicyHierarchy_BuildSuccessfulHierarchy()
        {
            // Arrange
            var policy = new Policy
            {
                CompanyCode = "01",
                PolicyNumber = "123456",
                Agents = new List<Agent>
                {
                    new Agent
                    {
                        AgentId = "1234",
                        IsServicingAgent = true,
                        IsWritingAgent = false,
                        Level = "12",
                        MarketCode = "IS",
                    },
                    new Agent
                    {
                        AgentId = "4321",
                        IsServicingAgent = false,
                        IsWritingAgent = true,
                        Level = "13",
                        MarketCode = "IS",
                    },
                },
                ApplicationDate = DateTime.Now,
            };

            var firstUpLine = new Agent
            {
                AgentId = "abc123",
                IsServicingAgent = false,
                IsWritingAgent = true,
                Level = "50",
                MarketCode = "IS",
            };
            var secondUpLine = new Agent
            {
                AgentId = "123abc",
                IsServicingAgent = false,
                IsWritingAgent = true,
                Level = "99",
                MarketCode = "IS",
            };

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() => null);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline("4321", "IS", "13", It.IsAny<DateTime>()))
                .ReturnsAsync(firstUpLine);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline("abc123", "IS", "50", It.IsAny<DateTime>()))
                .ReturnsAsync(secondUpLine);

            var mockGlobalAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            mockGlobalAccessor.Setup(accessor => accessor.GetAgentFolderIdsFromAttributes(It.IsAny<string>()))
                .ReturnsAsync(new List<string> { });

            var hierarchyEngine = new ConsumerHierarchyEngine(mockLifeProAccessor.Object, mockGlobalAccessor.Object, null);

            // Act
            var result = await hierarchyEngine.GetPolicyHierarchy(policy);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.HierarchyBranches[0].HierarchyAgents.Count);
            Assert.AreEqual(2, result.HierarchyBranches[1].HierarchyAgents.Count);
            Assert.AreEqual(firstUpLine.AgentId, result.HierarchyBranches[1].HierarchyAgents[0].AgentId);
            Assert.AreEqual(secondUpLine.AgentId, result.HierarchyBranches[1].HierarchyAgents[1].AgentId);
            Assert.AreEqual(1, result.HierarchyBranches[1].HierarchyAgents[0].Sequence);
            Assert.AreEqual(2, result.HierarchyBranches[1].HierarchyAgents[1].Sequence);
        }

        [TestMethod]
        public async Task GetPolicyHierarchy_BuildSuccessfulHierarchy_JITAgents_UpLine_From_Global()
        {
            // Arrange
            var policy = new Policy
            {
                CompanyCode = "01",
                PolicyNumber = "123456",
                Agents = new List<Agent>
                {
                    new Agent
                    {
                        AgentId = "1234",
                        IsServicingAgent = true,
                        IsWritingAgent = false,
                        Level = "12",
                        MarketCode = "IS",
                    },
                    new Agent
                    {
                        AgentId = "654321",
                        IsServicingAgent = false,
                        IsJustInTimeAgent = true,
                        IsWritingAgent = true,
                        Level = "13",
                        MarketCode = "IS",
                    }
                },
                ApplicationDate = DateTime.Now,
            };

            var secondUpLine = new Agent
            {
                AgentId = "123abc",
                IsServicingAgent = false,
                IsWritingAgent = true,
                Level = "99",
                MarketCode = "IS",
            };

            var justInTimeAgent = new JustInTimeAgentDTO
            {
                AgentId = "654321",
                Level = "13",
                MarketCode = "IS",
                UplineAgentId = "abc123",
                UplineLevel = "55",
                UplineMarketCode = "IS",
            };

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() => null);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline("abc123", "IS", "55", It.IsAny<DateTime>()))
                .ReturnsAsync(secondUpLine);

            var mockGlobalAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            mockGlobalAccessor.Setup(accessor => accessor.GetNewBusinessFolderIds(policy.PolicyNumber))
                .ReturnsAsync(new List<string> { "HowdyThere" });
            mockGlobalAccessor.Setup(accessor => accessor.GetJustInTimeAgentIds(It.IsAny<List<string>>()))
                .ReturnsAsync(new List<AgentDTO>
                {
                    new AgentDTO
                    {
                        AGENT = "654321",
                        AGENT_LEVEL = "13",
                        MARKET_CODE = "IS"
                    }
                });
            mockGlobalAccessor.Setup(accessor => accessor.GetAgentFolderIdsFromAttributes(It.IsAny<string>()))
                .ReturnsAsync(new List<string> { "agentFolderId" });
            mockGlobalAccessor.Setup(accessor => accessor.GetQueueFromFolderId("agentFolderId"))
                .ReturnsAsync("AC AdvChgDocRecv".ToLower());
            mockGlobalAccessor.Setup(accessor => accessor.GetJitAgentInfoFromFolderId("654321", "agentFolderId", "IS", "13"))
                .ReturnsAsync(justInTimeAgent);
            mockGlobalAccessor.Setup(accessor => accessor.GetJitAgentInfoFromFolderId("abc123", "agentFolderId", "IS", "55"))
                .ReturnsAsync(() => null);
            mockGlobalAccessor.Setup(accessor => accessor.GetJitAgentInfoFromFolderId("123abc", "agentFolderId", "IS", "99"))
               .ReturnsAsync(() => null);

            var mockSupportAccessor = new Mock<ISupportDataAccessor>(MockBehavior.Strict);
            mockSupportAccessor.Setup(accessor => accessor.GetQueueDescriptions())
                .ReturnsAsync(new List<string?> { "AC AdvChgDocRecv".ToLower(), "AC AdvChgFollowUp".ToLower() });

            var hierarchyEngine = new ConsumerHierarchyEngine(mockLifeProAccessor.Object, mockGlobalAccessor.Object, mockSupportAccessor.Object);

            // Act
            var result = await hierarchyEngine.GetPolicyHierarchy(policy);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.HierarchyBranches[0].HierarchyAgents.Count);
            Assert.AreEqual(2, result.HierarchyBranches[1].HierarchyAgents.Count);
            Assert.IsFalse(result.HierarchyBranches[0].Agent.IsJustInTimeAgent);
            Assert.IsTrue(result.HierarchyBranches[1].Agent.IsJustInTimeAgent);
            Assert.AreEqual(justInTimeAgent.AgentId, result.HierarchyBranches[1].Agent.AgentId);
            Assert.AreEqual(justInTimeAgent.UplineAgentId, result.HierarchyBranches[1].HierarchyAgents[0].AgentId);
            Assert.AreEqual(secondUpLine.AgentId, result.HierarchyBranches[1].HierarchyAgents[1].AgentId);
            Assert.AreEqual(1, result.HierarchyBranches[1].HierarchyAgents[0].Sequence);
            Assert.AreEqual(2, result.HierarchyBranches[1].HierarchyAgents[1].Sequence);
        }

        [TestMethod]
        public async Task GetPolicyHierarchy_BuildSuccessfulHierarchy_JITAgents_UpLine_From_LifePro()
        {
            // Arrange
            var policy = new Policy
            {
                CompanyCode = "01",
                PolicyNumber = "123456",
                Agents = new List<Agent>
                {
                    new Agent
                    {
                        AgentId = "1234",
                        IsServicingAgent = true,
                        IsWritingAgent = false,
                        Level = "12",
                        MarketCode = "IS",
                    },
                    new Agent
                    {
                        AgentId = "654321",
                        IsServicingAgent = false,
                        IsJustInTimeAgent = true,
                        IsWritingAgent = true,
                        Level = "13",
                        MarketCode = "IS",
                    }
                },
                ApplicationDate = DateTime.Now,
            };

            var secondUpLine = new Agent
            {
                AgentId = "123abc",
                IsServicingAgent = false,
                IsWritingAgent = true,
                Level = "99",
                MarketCode = "IS",
            };

            var secondUpLineJIT = new Agent
            {
                AgentId = "123456",
                IsServicingAgent = false,
                IsWritingAgent = true,
                Level = "50",
                MarketCode = "IS",
            };

            var thirdUpLineJIT = new Agent
            {
                AgentId = "223456",
                IsServicingAgent = false,
                IsWritingAgent = true,
                Level = "51",
                MarketCode = "IS",
            };

            var justInTimeAgent = new JustInTimeAgentDTO
            {
                AgentId = "654321",
                Level = "13",
                MarketCode = "IS",
                UplineAgentId = "abc123",
                UplineLevel = "55",
                UplineMarketCode = "IS",
            };

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() => null);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline("abc123", "IS", "55", It.IsAny<DateTime>()))
                .ReturnsAsync(secondUpLine);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline("654321", "IS", "13", It.IsAny<DateTime>()))
               .ReturnsAsync(secondUpLineJIT);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline("123456", "IS", "50", It.IsAny<DateTime>()))
               .ReturnsAsync(thirdUpLineJIT);

            var mockGlobalAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            mockGlobalAccessor.Setup(accessor => accessor.GetNewBusinessFolderIds(policy.PolicyNumber))
                .ReturnsAsync(new List<string> { "HowdyThere" });
            mockGlobalAccessor.Setup(accessor => accessor.GetJustInTimeAgentIds(It.IsAny<List<string>>()))
                .ReturnsAsync(new List<AgentDTO>
                {
                    new AgentDTO
                    {
                        AGENT = "654321",
                        AGENT_LEVEL = "13",
                        MARKET_CODE = "IS"
                    }
                });
            mockGlobalAccessor.Setup(accessor => accessor.GetAgentFolderIdsFromAttributes(It.IsAny<string>()))
                .ReturnsAsync(new List<string> { "agentFolderId" });
            mockGlobalAccessor.Setup(accessor => accessor.GetQueueFromFolderId("agentFolderId"))
                .ReturnsAsync("AC AdvChgDocRecv".ToLower());
            mockGlobalAccessor.Setup(accessor => accessor.GetJitAgentInfoFromFolderId("654321", "agentFolderId", "IS", "13"))
                .ReturnsAsync(() => null);
            mockGlobalAccessor.Setup(accessor => accessor.GetJitAgentInfoFromFolderId("123456", "agentFolderId", "IS", "50"))
               .ReturnsAsync(() => null);
            mockGlobalAccessor.Setup(accessor => accessor.GetJitAgentInfoFromFolderId("223456", "agentFolderId", "IS", "51"))
              .ReturnsAsync(() => null);

            var mockSupportAccessor = new Mock<ISupportDataAccessor>(MockBehavior.Strict);
            mockSupportAccessor.Setup(accessor => accessor.GetQueueDescriptions())
                .ReturnsAsync(new List<string?> { "AC AdvChgDocRecv".ToLower(), "AC AdvChgFollowUp".ToLower() });

            var hierarchyEngine = new ConsumerHierarchyEngine(mockLifeProAccessor.Object, mockGlobalAccessor.Object, mockSupportAccessor.Object);

            // Act
            var result = await hierarchyEngine.GetPolicyHierarchy(policy);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.HierarchyBranches[0].HierarchyAgents.Count);
            Assert.AreEqual(2, result.HierarchyBranches[1].HierarchyAgents.Count);
            Assert.IsFalse(result.HierarchyBranches[0].Agent.IsJustInTimeAgent);
            Assert.IsTrue(result.HierarchyBranches[1].Agent.IsJustInTimeAgent);
            Assert.AreEqual(justInTimeAgent.AgentId, result.HierarchyBranches[1].Agent.AgentId);
            Assert.AreEqual(secondUpLineJIT.AgentId, result.HierarchyBranches[1].HierarchyAgents[0].AgentId);
            Assert.AreEqual(thirdUpLineJIT.AgentId, result.HierarchyBranches[1].HierarchyAgents[1].AgentId);
            Assert.AreEqual(1, result.HierarchyBranches[1].HierarchyAgents[0].Sequence);
            Assert.AreEqual(2, result.HierarchyBranches[1].HierarchyAgents[1].Sequence);
        }

        [TestMethod]
        public async Task GetPolicyHierarchy_BuildSuccessfulHierarchy_JITAgent_From_Global_And_UpLine_From_LifePro()
        {
            // Arrange
            var policy = new Policy
            {
                CompanyCode = "01",
                PolicyNumber = "9090909456",
                Agents = new List<Agent>
                {
                    new Agent
                    {
                        AgentId = "654321",
                        IsServicingAgent = false,
                        IsJustInTimeAgent = true,
                        IsWritingAgent = true,
                        Level = "13",
                        MarketCode = "IS",
                    }
                },
                ApplicationDate = DateTime.Now,
            };

            var secondUpLine = new Agent
            {
                AgentId = "123abc",
                IsServicingAgent = false,
                IsWritingAgent = true,
                Level = "99",
                MarketCode = "IS",
            };

            var secondUpLineLifePro = new Agent
            {
                AgentId = "1234",
                IsServicingAgent = false,
                IsWritingAgent = false,
                Level = "50",
                MarketCode = "IS",
            };

            var thirdUpLineLifePro = new Agent
            {
                AgentId = "223456",
                IsServicingAgent = false,
                IsWritingAgent = false,
                Level = "51",
                MarketCode = "IS",
            };

            var justInTimeAgent = new JustInTimeAgentDTO
            {
                AgentId = "654321",
                Level = "13",
                MarketCode = "IS",
                UplineAgentId = "abc123",
                UplineLevel = "55",
                UplineMarketCode = "IS",
            };

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() => null);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline("abc123", "IS", "55", It.IsAny<DateTime>()))
                .ReturnsAsync(secondUpLine);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline("654321", "IS", "13", It.IsAny<DateTime>()))
               .ReturnsAsync(secondUpLineLifePro);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline("1234", "IS", "50", It.IsAny<DateTime>()))
               .ReturnsAsync(thirdUpLineLifePro);

            var mockGlobalAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            mockGlobalAccessor.Setup(accessor => accessor.GetNewBusinessFolderIds(policy.PolicyNumber))
                .ReturnsAsync(new List<string> { "HowdyThere" });
            mockGlobalAccessor.Setup(accessor => accessor.GetJustInTimeAgentIds(It.IsAny<List<string>>()))
                .ReturnsAsync(new List<AgentDTO>
                {
                    new AgentDTO
                    {
                        AGENT = "654321",
                        AGENT_LEVEL = "13",
                        MARKET_CODE = "IS"
                    }
                });
            mockGlobalAccessor.Setup(accessor => accessor.GetAgentFolderIdsFromAttributes(It.IsAny<string>()))
                .ReturnsAsync(new List<string> { "agentFolderId" });
            mockGlobalAccessor.Setup(accessor => accessor.GetQueueFromFolderId("agentFolderId"))
                .ReturnsAsync("AC AdvChgDocRecv".ToLower());
            mockGlobalAccessor.Setup(accessor => accessor.GetJitAgentInfoFromFolderId("654321", "agentFolderId", "IS", "13"))
                .ReturnsAsync(() => null);
            mockGlobalAccessor.Setup(accessor => accessor.GetJitAgentInfoFromFolderId("1234", "agentFolderId", "IS", "50"))
               .ReturnsAsync(() => null);
            mockGlobalAccessor.Setup(accessor => accessor.GetJitAgentInfoFromFolderId("223456", "agentFolderId", "IS", "51"))
              .ReturnsAsync(() => null);

            var mockSupportAccessor = new Mock<ISupportDataAccessor>(MockBehavior.Strict);
            mockSupportAccessor.Setup(accessor => accessor.GetQueueDescriptions())
                .ReturnsAsync(new List<string?> { "AC AdvChgDocRecv".ToLower(), "AC AdvChgFollowUp".ToLower() });

            var hierarchyEngine = new ConsumerHierarchyEngine(mockLifeProAccessor.Object, mockGlobalAccessor.Object, mockSupportAccessor.Object);

            // Act
            var result = await hierarchyEngine.GetPolicyHierarchy(policy);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.HierarchyBranches[0].HierarchyAgents.Count);
            Assert.IsTrue(result.HierarchyBranches[0].Agent.IsJustInTimeAgent);
            Assert.AreEqual(justInTimeAgent.AgentId, result.HierarchyBranches[0].Agent.AgentId);
            Assert.AreEqual(secondUpLineLifePro.AgentId, result.HierarchyBranches[0].HierarchyAgents[0].AgentId);
            Assert.AreEqual(thirdUpLineLifePro.AgentId, result.HierarchyBranches[0].HierarchyAgents[1].AgentId);
            Assert.AreEqual(1, result.HierarchyBranches[0].HierarchyAgents[0].Sequence);
            Assert.AreEqual(2, result.HierarchyBranches[0].HierarchyAgents[1].Sequence);
        }

        [TestMethod]
        public void GetDistinctAgentIds_ReturnsDistinctAgents()
        {
            // Arrange
            var agents = new List<AgentHierarchy>
            {
                new AgentHierarchy
                {
                    Agent = new Agent
                    {
                        AgentId = "1234",
                        Participant = new Participant
                        {
                            Person = new Person
                            {
                                Name = new Name
                                {
                                    IndividualFirst = "John",
                                    IndividualLast = "Smith"
                                }
                            }
                        }
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
                        Participant = new Participant
                        {
                            Person = new Person
                            {
                                Name = new Name
                                {
                                    IndividualFirst = "Jacob",
                                    IndividualLast = "Smith"
                                }
                            }
                        }
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
            };

            var hierarchyEngine = new ConsumerHierarchyEngine(null, null, null);

            // Act
            var result = hierarchyEngine.GetDistinctAgentIds(agents);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);
        }

        [TestMethod]
        public void CompareAgentHierarchies_NoChanges_ShouldReturn_EmptyAddedAndRemovedAgentIds()
        {
            // Arrange
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            var mockGlobalDataAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            var mockSupportDataAccessor = new Mock<ISupportDataAccessor>(MockBehavior.Strict);

            var hierarchyEngine = new ConsumerHierarchyEngine(
                mockLifeProAccessor.Object,
                mockGlobalDataAccessor.Object,
                mockSupportDataAccessor.Object);

            var oldAgentHierarchies = new List<AgentHierarchy>
            {
                new AgentHierarchy
                {
                    Agent = new Agent
                    {
                        AgentId = "1234",
                        IsServicingAgent = true,
                        IsWritingAgent = false,
                        Level = "13",
                        MarketCode = "WA",
                    },
                    HierarchyAgents = new List<HierarchyAgent>
                    {
                        new HierarchyAgent
                        {
                            Sequence = 1,
                            AgentId = "4321",
                            Level = "14",
                            MarketCode = "WA",
                        },
                    }
                },
                new AgentHierarchy
                {
                    Agent = new Agent
                    {
                        AgentId = "4234",
                        IsServicingAgent = false,
                        IsWritingAgent = true,
                        Level = "11",
                        MarketCode = "WA",
                    },
                    HierarchyAgents = new List<HierarchyAgent>
                    {
                        new HierarchyAgent
                        {
                            Sequence = 1,
                            AgentId = "4001",
                            Level = "14",
                            MarketCode = "WA",
                        },
                    }
                }
            };

            var newAgentHierarchies = new List<AgentHierarchy>
            {
                new AgentHierarchy
                {
                    Agent = new Agent
                    {
                        AgentId = "1234",
                        IsServicingAgent = true,
                        IsWritingAgent = false,
                        Level = "13",
                        MarketCode = "WA",
                    },
                    HierarchyAgents = new List<HierarchyAgent>
                    {
                        new HierarchyAgent
                        {
                            Sequence = 1,
                            AgentId = "4321",
                            Level = "14",
                            MarketCode = "WA",
                        },
                    }
                },
                new AgentHierarchy
                {
                    Agent = new Agent
                    {
                        AgentId = "4234",
                        IsServicingAgent = false,
                        IsWritingAgent = true,
                        Level = "11",
                        MarketCode = "WA",
                    },
                    HierarchyAgents = new List<HierarchyAgent>
                    {
                        new HierarchyAgent
                        {
                            Sequence = 1,
                            AgentId = "4001",
                            Level = "14",
                            MarketCode = "WA",
                        },
                    }
                }
            };

            var companyCode = "01";
            var policyNumber = "1234567890";

            var result = hierarchyEngine.CompareAgentHierarchies(companyCode, policyNumber, oldAgentHierarchies, newAgentHierarchies);
            Assert.AreEqual(companyCode, result.CompanyCode);
            Assert.AreEqual(policyNumber, result.PolicyNumber);

            Assert.AreEqual(0, result.AddedAgents.Count);
            Assert.AreEqual(0, result.RemovedAgents.Count);
        }

        [TestMethod]
        public void CompareAgentHierarchies_AddedAgents_ShouldReturn_AddedAgentIds()
        {
            // Arrange
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            var mockGlobalDataAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            var mockSupportDataAccessor = new Mock<ISupportDataAccessor>(MockBehavior.Strict);

            var hierarchyEngine = new ConsumerHierarchyEngine(
                mockLifeProAccessor.Object,
                mockGlobalDataAccessor.Object,
                mockSupportDataAccessor.Object);

            var oldAgentHierarchies = new List<AgentHierarchy>
            {
                new AgentHierarchy
                {
                    Agent = new Agent
                    {
                        AgentId = "1234",
                        IsServicingAgent = true,
                        IsWritingAgent = false,
                        Level = "13",
                        MarketCode = "WA",
                    },
                    HierarchyAgents = new List<HierarchyAgent>
                    {
                        new HierarchyAgent
                        {
                            Sequence = 1,
                            AgentId = "4321",
                            Level = "14",
                            MarketCode = "WA",
                        },
                    }
                }
            };

            var newAgentHierarchies = new List<AgentHierarchy>
            {
                new AgentHierarchy
                {
                    Agent = new Agent
                    {
                        AgentId = "1234",
                        IsServicingAgent = true,
                        IsWritingAgent = false,
                        Level = "13",
                        MarketCode = "WA",
                    },
                    HierarchyAgents = new List<HierarchyAgent>
                    {
                        new HierarchyAgent
                        {
                            Sequence = 1,
                            AgentId = "4321",
                            Level = "14",
                            MarketCode = "WA",
                        },
                    }
                },
                new AgentHierarchy
                {
                    Agent = new Agent
                    {
                        AgentId = "4234",
                        IsServicingAgent = false,
                        IsWritingAgent = true,
                        Level = "11",
                        MarketCode = "WA",
                    },
                    HierarchyAgents = new List<HierarchyAgent>
                    {
                        new HierarchyAgent
                        {
                            Sequence = 1,
                            AgentId = "4321",
                            Level = "14",
                            MarketCode = "WA",
                        },
                    }
                }
            };

            var companyCode = "01";
            var policyNumber = "1234567890";
            var result = hierarchyEngine.CompareAgentHierarchies(companyCode, policyNumber, oldAgentHierarchies, newAgentHierarchies);
            Assert.AreEqual(companyCode, result.CompanyCode);
            Assert.AreEqual(policyNumber, result.PolicyNumber);

            Assert.AreEqual(1, result.AddedAgents.Count);
            Assert.AreEqual("4234", result.AddedAgents[0]);

            Assert.AreEqual(0, result.RemovedAgents.Count);
        }

        [TestMethod]
        public void CompareAgentHierarchies_DeletedAgents_ShouldReturn_DeletedAgentIds()
        {
            // Arrange
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            var mockGlobalDataAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            var mockSupportDataAccessor = new Mock<ISupportDataAccessor>(MockBehavior.Strict);

            var hierarchyEngine = new ConsumerHierarchyEngine(
                mockLifeProAccessor.Object,
                mockGlobalDataAccessor.Object,
                mockSupportDataAccessor.Object);

            var inAgentHierarchies = new List<AgentHierarchy>
            {
                new AgentHierarchy
                {
                    Agent = new Agent
                    {
                        AgentId = "1234",
                        IsServicingAgent = true,
                        IsWritingAgent = false,
                        Level = "13",
                        MarketCode = "WA",
                    },
                    HierarchyAgents = new List<HierarchyAgent>
                    {
                        new HierarchyAgent
                        {
                            Sequence = 1,
                            AgentId = "4321",
                            Level = "14",
                            MarketCode = "WA",
                        },
                    }
                }
            };

            var newAgentHierarchies = new List<AgentHierarchy>
            {
                new AgentHierarchy
                {
                    Agent = new Agent
                    {
                        AgentId = "1234",
                        IsServicingAgent = true,
                        IsWritingAgent = false,
                        Level = "13",
                        MarketCode = "WA",
                    },
                    HierarchyAgents = new List<HierarchyAgent>
                    {
                        new HierarchyAgent
                        {
                            Sequence = 1,
                            AgentId = "4321",
                            Level = "14",
                            MarketCode = "WA",
                        },
                    }
                },
                new AgentHierarchy
                {
                    Agent = new Agent
                    {
                        AgentId = "3001",
                        IsServicingAgent = false,
                        IsWritingAgent = true,
                        Level = "11",
                        MarketCode = "WA",
                    },
                    HierarchyAgents = new List<HierarchyAgent>
                    {
                        new HierarchyAgent
                        {
                            Sequence = 1,
                            AgentId = "3401",
                            Level = "14",
                            MarketCode = "WA",
                        },
                    }
                }
            };

            var companyCode = "01";
            var policyNumber = "1234567890";
            var result = hierarchyEngine.CompareAgentHierarchies(companyCode, policyNumber, inAgentHierarchies, newAgentHierarchies);
            Assert.AreEqual(companyCode, result.CompanyCode);
            Assert.AreEqual(policyNumber, result.PolicyNumber);

            Assert.AreEqual(2, result.AddedAgents.Count);
            Assert.AreEqual("3001", result.AddedAgents[0]);
            Assert.AreEqual("3401", result.AddedAgents[1]);

            Assert.AreEqual(0, result.RemovedAgents.Count);
        }

        [TestMethod]
        public void CompareAgentHierarchies_UpdatedAgents_ShouldReturn_AddedAndRemovedAgentIds()
        {
            // Arrange
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            var mockGlobalDataAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            var mockSupportDataAccessor = new Mock<ISupportDataAccessor>(MockBehavior.Strict);

            var hierarchyEngine = new ConsumerHierarchyEngine(
              mockLifeProAccessor.Object,
              mockGlobalDataAccessor.Object,
              mockSupportDataAccessor.Object);

            var oldAgentHierarchies = new List<AgentHierarchy>
            {
                new AgentHierarchy
                {
                    Agent = new Agent
                    {
                        AgentId = "1234",
                        IsServicingAgent = true,
                        IsWritingAgent = false,
                        Level = "13",
                        MarketCode = "WA",
                    },
                    HierarchyAgents = new List<HierarchyAgent>
                    {
                        new HierarchyAgent
                        {
                            Sequence = 1,
                            AgentId = "4321",
                            Level = "14",
                            MarketCode = "WA",
                        },
                    }
                },
                new AgentHierarchy
                {
                    Agent = new Agent
                    {
                        AgentId = "2034",
                        IsServicingAgent = false,
                        IsWritingAgent = true,
                        Level = "11",
                        MarketCode = "WA",
                    },
                    HierarchyAgents = new List<HierarchyAgent>
                    {
                        new HierarchyAgent
                        {
                            Sequence = 1,
                            AgentId = "0321",
                            Level = "14",
                            MarketCode = "WA",
                        },
                    }
                }
            };

            var newAgentHierarchies = new List<AgentHierarchy>
            {
                new AgentHierarchy
                {
                    Agent = new Agent
                    {
                        AgentId = "1234",
                        IsServicingAgent = true,
                        IsWritingAgent = false,
                        Level = "13",
                        MarketCode = "WA",
                    },
                    HierarchyAgents = new List<HierarchyAgent>
                    {
                        new HierarchyAgent
                        {
                            Sequence = 1,
                            AgentId = "4321",
                            Level = "14",
                            MarketCode = "WA",
                        },
                    }
                },
                new AgentHierarchy
                {
                    Agent = new Agent
                    {
                        AgentId = "4234",
                        IsServicingAgent = false,
                        IsWritingAgent = true,
                        Level = "11",
                        MarketCode = "WA",
                    },
                    HierarchyAgents = new List<HierarchyAgent>
                    {
                        new HierarchyAgent
                        {
                            Sequence = 1,
                            AgentId = "4321",
                            Level = "14",
                            MarketCode = "WA",
                        },
                    }
                }
            };

            var companyCode = "01";
            var policyNumber = "1234567890";

            var result = hierarchyEngine.CompareAgentHierarchies(companyCode, policyNumber, oldAgentHierarchies, newAgentHierarchies);
            Assert.AreEqual(companyCode, result.CompanyCode);
            Assert.AreEqual(policyNumber, result.PolicyNumber);

            Assert.AreEqual(1, result.AddedAgents.Count);
            Assert.AreEqual("4234", result.AddedAgents[0]);

            Assert.AreEqual(2, result.RemovedAgents.Count);
            Assert.AreEqual("2034", result.RemovedAgents[0]);
            Assert.AreEqual("0321", result.RemovedAgents[1]);
        }

        [TestMethod]
        public async Task BuildAgentHierarchy_BuildSuccessfulHierarchy_WritingAgent_With_AdditionalAgentUplines()
        {
            // Arrange
            var policy = new Policy
            {
                CompanyCode = "01",
                PolicyNumber = "123456",
                Agents = new List<Agent>
                {
                    new Agent
                    {
                        AgentId = "1234",
                        IsServicingAgent = false,
                        IsWritingAgent = true,
                        Level = "12",
                        MarketCode = "IS",
                    },
                    new Agent
                    {
                        AgentId = "4321",
                        IsServicingAgent = false,
                        IsWritingAgent = true,
                        Level = "13",
                        MarketCode = "IS",
                    },
                },
                ApplicationDate = DateTime.Now,
            };

            var firstUpLine1 = new Agent
            {
                AgentId = "abc123",
                IsServicingAgent = true,
                IsWritingAgent = false,
                Level = "50",
                MarketCode = "IS",
            };
            var secondUpLine1 = new Agent
            {
                AgentId = "def567",
                IsServicingAgent = false,
                IsWritingAgent = true,
                Level = "99",
                MarketCode = "IS",
            };
            var firstUpLine2 = new Agent
            {
                AgentId = "ghi123",
                IsServicingAgent = true,
                IsWritingAgent = false,
                Level = "50",
                MarketCode = "IS",
            };
            var secondUpLine2 = new Agent
            {
                AgentId = "jkl567",
                IsServicingAgent = false,
                IsWritingAgent = true,
                Level = "99",
                MarketCode = "IS",
            };

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() => null);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline("1234", "IS", "12", It.IsAny<DateTime>()))
                .ReturnsAsync(firstUpLine1);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline("abc123", "IS", "50", It.IsAny<DateTime>()))
                .ReturnsAsync(secondUpLine1);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline("4321", "IS", "13", It.IsAny<DateTime>()))
                .ReturnsAsync(firstUpLine2);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline("ghi123", "IS", "50", It.IsAny<DateTime>()))
               .ReturnsAsync(secondUpLine2);

            var mockGlobalAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            mockGlobalAccessor.Setup(accessor => accessor.GetAgentFolderIdsFromAttributes(It.IsAny<string>()))
               .ReturnsAsync(new List<string> { });

            var hierarchyEngine = new ConsumerHierarchyEngine(mockLifeProAccessor.Object, mockGlobalAccessor.Object, null);
            var isMigrationWorker = false;

            // Act
            var agentHierarchy = await hierarchyEngine.BuildAgentHierarchy(isMigrationWorker, policy.Agents, policy.ApplicationDate.Value);

            // Assert
            Assert.IsNotNull(agentHierarchy);
            Assert.AreEqual(2, agentHierarchy.Count);
            Assert.AreEqual("1234", agentHierarchy[0].Agent.AgentId);
            Assert.AreEqual("4321", agentHierarchy[1].Agent.AgentId);

            Assert.AreEqual(2, agentHierarchy[0].HierarchyAgents.Count);
            Assert.AreEqual("abc123", agentHierarchy[0].HierarchyAgents[0].AgentId);
            Assert.AreEqual("def567", agentHierarchy[0].HierarchyAgents[1].AgentId);

            Assert.AreEqual(2, agentHierarchy[1].HierarchyAgents.Count);
            Assert.AreEqual("ghi123", agentHierarchy[1].HierarchyAgents[0].AgentId);
            Assert.AreEqual("jkl567", agentHierarchy[1].HierarchyAgents[1].AgentId);
        }

        [TestMethod]
        public async Task BuildAgentHierarchy_BuildSuccessfulHierarchy_WritingAgent_Without_HierarchyAgentsAndAdditionalAgentUplines()
        {
            // Arrange
            var policy = new Policy
            {
                CompanyCode = "01",
                PolicyNumber = "123456",
                Agents = new List<Agent>
                {
                    new Agent
                    {
                        AgentId = "1234",
                        IsServicingAgent = false,
                        IsWritingAgent = true,
                        Level = "12",
                        MarketCode = "IS",
                    },
                    new Agent
                    {
                        AgentId = "4321",
                        IsServicingAgent = false,
                        IsWritingAgent = true,
                        Level = "13",
                        MarketCode = "IS",
                    },
                },
                ApplicationDate = DateTime.Now,
            };

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() => null);

            var hierarchyEngine = new ConsumerHierarchyEngine(mockLifeProAccessor.Object, null, null);
            var isMigrationWorker = false;

            // Act
            var agentHierarchy = await hierarchyEngine.BuildAgentHierarchy(isMigrationWorker, policy.Agents, policy.ApplicationDate.Value);

            // Assert
            Assert.IsNotNull(agentHierarchy);
            Assert.AreEqual(2, agentHierarchy.Count);
            Assert.AreEqual("1234", agentHierarchy[0].Agent.AgentId);
            Assert.AreEqual("4321", agentHierarchy[1].Agent.AgentId);

            Assert.AreEqual(0, agentHierarchy[0].HierarchyAgents.Count);
            Assert.AreEqual(0, agentHierarchy[1].HierarchyAgents.Count);
        }

        [TestMethod]
        public async Task BuildAgentHierarchy_BuildSuccessfulHierarchy_ServicingAgent_Without_AdditionalAgentUplines()
        {
            // Arrange
            var policy = new Policy
            {
                CompanyCode = "01",
                PolicyNumber = "123456",
                Agents = new List<Agent>
                {
                    new Agent
                    {
                        AgentId = "1234",
                        IsServicingAgent = true,
                        IsWritingAgent = false,
                        Level = "12",
                        MarketCode = "IS",
                    },
                    new Agent
                    {
                        AgentId = "4321",
                        IsServicingAgent = true,
                        IsWritingAgent = false,
                        Level = "13",
                        MarketCode = "IS",
                    },
                },
                ApplicationDate = DateTime.Now,
            };

            var firstUpLine1 = new Agent
            {
                AgentId = "abc123",
                IsServicingAgent = true,
                IsWritingAgent = false,
                Level = "50",
                MarketCode = "IS",
            };
            var secondUpLine1 = new Agent
            {
                AgentId = "def567",
                IsServicingAgent = false,
                IsWritingAgent = true,
                Level = "99",
                MarketCode = "IS",
            };
            var firstUpLine2 = new Agent
            {
                AgentId = "ghi123",
                IsServicingAgent = true,
                IsWritingAgent = false,
                Level = "50",
                MarketCode = "IS",
            };
            var secondUpLine2 = new Agent
            {
                AgentId = "jkl567",
                IsServicingAgent = false,
                IsWritingAgent = true,
                Level = "99",
                MarketCode = "IS",
            };

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() => null);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline("1234", "IS", "12", It.IsAny<DateTime>()))
                .ReturnsAsync(firstUpLine1);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline("abc123", "IS", "50", It.IsAny<DateTime>()))
                .ReturnsAsync(secondUpLine1);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline("4321", "IS", "13", It.IsAny<DateTime>()))
                .ReturnsAsync(firstUpLine2);
            mockLifeProAccessor.Setup(accessor => accessor.GetAgentUpline("ghi123", "IS", "50", It.IsAny<DateTime>()))
              .ReturnsAsync(secondUpLine2);

            var hierarchyEngine = new ConsumerHierarchyEngine(mockLifeProAccessor.Object, null, null);

            // Act
            var agentHierarchy = await hierarchyEngine.BuildAgentHierarchy(policy.Agents, policy.ApplicationDate.Value);

            // Assert
            Assert.IsNotNull(agentHierarchy);
            Assert.AreEqual(2, agentHierarchy.Count);
            Assert.AreEqual("1234", agentHierarchy[0].Agent.AgentId);
            Assert.AreEqual("4321", agentHierarchy[1].Agent.AgentId);
            Assert.AreEqual(1, agentHierarchy[0].HierarchyAgents.Count);
            Assert.AreEqual("abc123", agentHierarchy[0].HierarchyAgents[0].AgentId);
            Assert.AreEqual(1, agentHierarchy[1].HierarchyAgents.Count);
            Assert.AreEqual("ghi123", agentHierarchy[1].HierarchyAgents[0].AgentId);
        }
    }
}