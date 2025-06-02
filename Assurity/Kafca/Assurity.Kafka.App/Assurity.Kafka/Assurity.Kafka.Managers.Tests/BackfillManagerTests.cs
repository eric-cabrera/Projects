namespace Assurity.Kafka.Managers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.DataTransferObjects;
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
    public class BackfillManagerTests
    {
        private const int BatchProcessingSize = 100;

        private Mock<ILogger<BackfillManager>> mockLogger = new Mock<ILogger<BackfillManager>>();

        [TestMethod]
        public async Task MigrateSinglePolicy_PolicyDoesNotExist_SuccessfulMigration()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
            {
                PolicyNumber = policyNumber,
                CompanyCode = "01"
            };

            var policy1 = GetTestPolicy();
            var policyHierarchy = GetPolicyHierarchy();
            var distinctAgents = new HashSet<string>
            {
                "1234",
                "4321",
            };

            var mockMongoAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockMongoAccessor
                .Setup(cache => cache.CheckIfPolicyExists(companyCodeAndPolicyNumber.PolicyNumber, companyCodeAndPolicyNumber.CompanyCode))
                .ReturnsAsync(false);

            var mockPolicyEngine = new Mock<IMigratePolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(m => m.GetPolicy(companyCodeAndPolicyNumber.PolicyNumber, companyCodeAndPolicyNumber.CompanyCode))
                .ReturnsAsync((GetPolicyResult.Found, policy1));

            mockMongoAccessor.Setup(cache => cache.CreatePolicyAsync(policy1))
                .ReturnsAsync(Guid.NewGuid().ToString());

            var mockHierarchyEngine = new Mock<IMigrateHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(policy1))
                .ReturnsAsync(policyHierarchy);

            mockMongoAccessor
                .Setup(cache => cache.UpdateOrCreatePolicyHierarchyAsync(policyHierarchy))
                .Returns(Task.CompletedTask);

            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetDistinctAgentIds(policyHierarchy.HierarchyBranches))
                .Returns(distinctAgents);

            mockMongoAccessor
                .Setup(cache => cache.InsertAgentPolicyAccessAsync(
                    It.Is<string>(str => distinctAgents.Contains(str)),
                    policy1.PolicyNumber,
                    policy1.CompanyCode))
                .ReturnsAsync(It.IsAny<long>());

            var manager = new BackfillManager(
                null,
                mockMongoAccessor.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockLogger.Object);

            // Act
            await manager.MigrateSinglePolicy(companyCodeAndPolicyNumber);
        }

        [TestMethod]
        public async Task MigrateSinglePolicy_PolicyAlreadyExists_ShouldNotCreatePolicy()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
            {
                PolicyNumber = policyNumber,
                CompanyCode = "01"
            };

            var mockMongoAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockMongoAccessor
                .Setup(cache => cache.CheckIfPolicyExists(companyCodeAndPolicyNumber.PolicyNumber, companyCodeAndPolicyNumber.CompanyCode))
                .ReturnsAsync(true);

            var manager = new BackfillManager(
                null,
                mockMongoAccessor.Object,
                null,
                null,
                mockLogger.Object);

            // Act
            await manager.MigrateSinglePolicy(companyCodeAndPolicyNumber);
        }

        [TestMethod]
        public async Task MigrateSinglePolicy_PolicyDoesNotExist_PolicyEngineDoesNotReturnFound_ShouldStop()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
            {
                PolicyNumber = policyNumber,
                CompanyCode = "01"
            };

            var mockMongoAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockMongoAccessor
                .Setup(cache => cache.CheckIfPolicyExists(companyCodeAndPolicyNumber.PolicyNumber, companyCodeAndPolicyNumber.CompanyCode))
                .ReturnsAsync(true);

            var mockPolicyEngine = new Mock<IMigratePolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(m => m.GetPolicy(companyCodeAndPolicyNumber.PolicyNumber, companyCodeAndPolicyNumber.CompanyCode))
                .ReturnsAsync((GetPolicyResult.ExceptionThrown, null));

            var manager = new BackfillManager(
                null,
                mockMongoAccessor.Object,
                mockPolicyEngine.Object,
                null,
                mockLogger.Object);

            // Act
            await manager.MigrateSinglePolicy(companyCodeAndPolicyNumber);
        }

        [TestMethod]
        public async Task MigrateSinglePolicy_PolicyDoesNotExist_PolicyEngineReturnsFoundButNoApplicationDate_ShouldStop()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
            {
                PolicyNumber = policyNumber,
                CompanyCode = "01"
            };

            var policy1 = GetTestPolicy();
            policy1.ApplicationDate = null;

            var mockMongoAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockMongoAccessor
                .Setup(cache => cache.CheckIfPolicyExists(companyCodeAndPolicyNumber.PolicyNumber, companyCodeAndPolicyNumber.CompanyCode))
                .ReturnsAsync(true);

            var mockPolicyEngine = new Mock<IMigratePolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(m => m.GetPolicy(companyCodeAndPolicyNumber.PolicyNumber, companyCodeAndPolicyNumber.CompanyCode))
                .ReturnsAsync((GetPolicyResult.Found, policy1));

            var manager = new BackfillManager(
                null,
                mockMongoAccessor.Object,
                mockPolicyEngine.Object,
                null,
                mockLogger.Object);

            // Act
            await manager.MigrateSinglePolicy(companyCodeAndPolicyNumber);
        }

        [TestMethod]
        public async Task BackfillPolicies_Migrate_ShouldMigrateAllPolicies()
        {
            // Arrange
            var recordsToProcess = GenerateRandomCompanyCodeAndPolicyNumber(BatchProcessingSize);
            var expectedPolicyNumbers = recordsToProcess.Select(r => r.PolicyNumber).ToList();
            var policyHierarchy = GetPolicyHierarchy();
            var distinctAgents = new HashSet<string>
            {
                "1234",
                "4321",
            };

            var mockDataStoreAccessor = new Mock<IDataStoreAccessor>(MockBehavior.Strict);
            mockDataStoreAccessor
                .Setup(m => m.GetMigratablePPOLCRecords())
                .Returns(new HashSet<CompanyCodeAndPolicyNumber>(recordsToProcess));

            var mockMongoAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockMongoAccessor
                .Setup(cache => cache.GetAllCompanyCodesAndPolicyNumbers())
                .Returns(new HashSet<CompanyCodeAndPolicyNumber>());

            var generatedPolicy = new Policy { ApplicationDate = DateTime.Now };
            var mockPolicyEngine = new Mock<IMigratePolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(m => m.GetPolicy(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((policyNumber, companyCode) =>
                {
                    generatedPolicy.PolicyNumber = policyNumber;
                    generatedPolicy.CompanyCode = companyCode;
                })
                .ReturnsAsync((GetPolicyResult.Found, generatedPolicy));

            mockMongoAccessor
                .Setup(m => m.CreatePolicyAsync(It.IsAny<Policy>()))
                .ReturnsAsync(string.Empty);

            var mockHierarchyEngine = new Mock<IMigrateHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            mockMongoAccessor
                .Setup(cache => cache.UpdateOrCreatePolicyHierarchyAsync(It.IsAny<PolicyHierarchy>()))
                .Returns(Task.CompletedTask);

            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetDistinctAgentIds(It.IsAny<List<AgentHierarchy>>()))
                .Returns(distinctAgents);

            mockMongoAccessor
                .Setup(cache => cache.InsertAgentPolicyAccessAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(1000);

            var manager = new BackfillManager(
                mockDataStoreAccessor.Object,
                mockMongoAccessor.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockLogger.Object);

            // Act
            await manager.BackFillPolicies("Migrate");

            // Assert
            mockPolicyEngine.Verify(
                x => x.GetPolicy(It.IsAny<string>(), It.IsAny<string>()),
                Times.Exactly(BatchProcessingSize));

            mockMongoAccessor.Verify(
                x => x.CreatePolicyAsync(It.IsAny<Policy>()),
                Times.Exactly(BatchProcessingSize));

            mockMongoAccessor.Verify(
                x => x.UpdateOrCreatePolicyHierarchyAsync(It.IsAny<PolicyHierarchy>()),
                Times.Exactly(BatchProcessingSize));

            mockHierarchyEngine.Verify(
                x => x.GetDistinctAgentIds(policyHierarchy.HierarchyBranches),
                Times.Exactly(BatchProcessingSize));

            foreach (var agent in distinctAgents)
            {
                mockMongoAccessor.Verify(
                    x => x.InsertAgentPolicyAccessAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                    Times.Exactly(BatchProcessingSize * 2));
            }

            mockLogger
                .VerifyLog(LogLevel.Error, It.IsAny<string>(), Times.Never());
        }

        [TestMethod]
        public async Task BackfillPolicies_Update_ShouldDeleteAllPolicies_ThenBackfill()
        {
            // Arrange
            var recordsToProcess = GenerateRandomCompanyCodeAndPolicyNumber(BatchProcessingSize);
            var expectedPolicyNumbers = recordsToProcess.Select(r => r.PolicyNumber).ToList();
            var policyHierarchy = GetPolicyHierarchy();
            var distinctAgents = new HashSet<string>
            {
                "1234",
                "4321",
            };

            var mockMongoAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockMongoAccessor
                .Setup(m => m.GetCompanyCodeAndPolicyNumberOfFlaggedPolicies())
                .Returns(recordsToProcess);

            var generatedPolicy = new Policy { ApplicationDate = DateTime.Now };
            var mockPolicyEngine = new Mock<IMigratePolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(m => m.DeletePolicy(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            mockPolicyEngine
                .Setup(m => m.GetPolicy(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((policyNumber, companyCode) =>
                {
                    generatedPolicy.PolicyNumber = policyNumber;
                    generatedPolicy.CompanyCode = companyCode;
                })
                .ReturnsAsync((GetPolicyResult.Found, generatedPolicy));

            mockMongoAccessor
                .Setup(m => m.CreatePolicyAsync(It.IsAny<Policy>()))
                .ReturnsAsync(string.Empty);

            var mockHierarchyEngine = new Mock<IMigrateHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            mockMongoAccessor
                .Setup(cache => cache.UpdateOrCreatePolicyHierarchyAsync(It.IsAny<PolicyHierarchy>()))
                .Returns(Task.CompletedTask);

            mockMongoAccessor
                .Setup(m => m.InsertAgentPolicyAccessAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(1000);

            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetDistinctAgentIds(It.IsAny<List<AgentHierarchy>>()))
                .Returns(distinctAgents);

            var manager = new BackfillManager(
                null,
                mockMongoAccessor.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockLogger.Object);

            // Act
            await manager.BackFillPolicies("Update");

            // Assert
            mockPolicyEngine.Verify(
                x => x.DeletePolicy(It.IsAny<string>(), It.IsAny<string>()),
                Times.Exactly(BatchProcessingSize));

            mockPolicyEngine.Verify(
                x => x.GetPolicy(It.IsAny<string>(), It.IsAny<string>()),
                Times.Exactly(BatchProcessingSize));

            mockMongoAccessor.Verify(
                x => x.CreatePolicyAsync(It.IsAny<Policy>()),
                Times.Exactly(BatchProcessingSize));

            mockMongoAccessor.Verify(
                x => x.UpdateOrCreatePolicyHierarchyAsync(It.IsAny<PolicyHierarchy>()),
                Times.Exactly(BatchProcessingSize));

            mockHierarchyEngine.Verify(
                x => x.GetDistinctAgentIds(policyHierarchy.HierarchyBranches),
                Times.Exactly(BatchProcessingSize));

            foreach (var agent in distinctAgents)
            {
                mockMongoAccessor.Verify(
                    x => x.InsertAgentPolicyAccessAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                    Times.Exactly(BatchProcessingSize * 2));
            }

            mockLogger
                .VerifyLog(LogLevel.Error, It.IsAny<string>(), Times.Never());
        }

        [TestMethod]
        public async Task BackfillPolicies_UpdateAndMigrate_ShouldCallDeleteAndGetForAllPolicyNumbers()
        {
            // Arrange
            var recordsToProcess = GenerateRandomCompanyCodeAndPolicyNumber(BatchProcessingSize);
            var expectedPolicyNumbers = recordsToProcess.Select(r => r.PolicyNumber).ToList();
            var policyHierarchy = GetPolicyHierarchy();
            var distinctAgents = new HashSet<string>
            {
                "1234",
                "4321",
            };

            var mockMongoAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockMongoAccessor
                .Setup(cache => cache.GetAllCompanyCodesAndPolicyNumbers())
                .Returns(new HashSet<CompanyCodeAndPolicyNumber>());

            mockMongoAccessor
                .Setup(m => m.GetCompanyCodeAndPolicyNumberOfFlaggedPolicies())
                .Returns(recordsToProcess);

            var generatedPolicy = new Policy { ApplicationDate = DateTime.Now };
            var mockPolicyEngine = new Mock<IMigratePolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(m => m.DeletePolicy(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            mockPolicyEngine
                .Setup(m => m.GetPolicy(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((policyNumber, companyCode) =>
                {
                    generatedPolicy.PolicyNumber = policyNumber;
                    generatedPolicy.CompanyCode = companyCode;
                })
                .ReturnsAsync((GetPolicyResult.Found, generatedPolicy));

            mockMongoAccessor
                .Setup(m => m.CreatePolicyAsync(It.IsAny<Policy>()))
                .ReturnsAsync(string.Empty);

            var mockHierarchyEngine = new Mock<IMigrateHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            mockMongoAccessor
                .Setup(cache => cache.UpdateOrCreatePolicyHierarchyAsync(It.IsAny<PolicyHierarchy>()))
                .Returns(Task.CompletedTask);

            mockMongoAccessor
                .Setup(m => m.InsertAgentPolicyAccessAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(1000);

            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetDistinctAgentIds(It.IsAny<List<AgentHierarchy>>()))
                .Returns(distinctAgents);

            var mockDataStoreAccessor = new Mock<IDataStoreAccessor>(MockBehavior.Strict);
            mockDataStoreAccessor
                .Setup(m => m.GetMigratablePPOLCRecords())
                .Returns(new HashSet<CompanyCodeAndPolicyNumber>(recordsToProcess));

            var manager = new BackfillManager(
                mockDataStoreAccessor.Object,
                mockMongoAccessor.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockLogger.Object);

            // Act
            await manager.BackFillPolicies("UpdateAndMigrate");

            // Assert
            mockPolicyEngine.Verify(
                x => x.DeletePolicy(It.IsAny<string>(), It.IsAny<string>()),
                Times.Exactly(BatchProcessingSize * 2));

            mockPolicyEngine.Verify(
                x => x.GetPolicy(It.IsAny<string>(), It.IsAny<string>()),
                Times.Exactly(BatchProcessingSize * 2));

            mockMongoAccessor.Verify(
                x => x.CreatePolicyAsync(It.IsAny<Policy>()),
                Times.Exactly(BatchProcessingSize * 2));

            mockMongoAccessor.Verify(
                x => x.UpdateOrCreatePolicyHierarchyAsync(It.IsAny<PolicyHierarchy>()),
                Times.Exactly(BatchProcessingSize * 2));

            mockMongoAccessor.Verify(
                x => x.UpdateOrCreatePolicyHierarchyAsync(It.IsAny<PolicyHierarchy>()),
                Times.Exactly(BatchProcessingSize * 2));

            mockHierarchyEngine.Verify(
                x => x.GetDistinctAgentIds(policyHierarchy.HierarchyBranches),
                Times.Exactly(BatchProcessingSize * 2));

            foreach (var agent in distinctAgents)
            {
                mockMongoAccessor.Verify(
                    x => x.InsertAgentPolicyAccessAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                    Times.Exactly(BatchProcessingSize * 2 * 2));
            }

            mockLogger
                .VerifyLog(LogLevel.Error, It.IsAny<string>(), Times.Never());
        }

        [TestMethod]
        public async Task BackfillPolicies_InvalidExecutionMode_ShouldThrow()
        {
            // Arrange
            var manager = new BackfillManager(
                null,
                null,
                null,
                null,
                mockLogger.Object);

            // Act
            var result = await Assert.ThrowsExceptionAsync<NotImplementedException>(async () => await manager.BackFillPolicies("taco"));
        }

        [TestMethod]
        public void FlagPastDuePolicies_SuccessfulUpdate()
        {
            // Arrange
            long updatedCount = 500;
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.FlagPastDuePolicies())
                .Returns(updatedCount);

            var manager = new BackfillManager(
                null,
                mockEventsAccessor.Object,
                null,
                null,
                mockLogger.Object);

            // Act
            manager.FlagPastDuePolicies();

            // Assert
            mockEventsAccessor.Verify(x => x.FlagPastDuePolicies(), Times.Once());
        }

        [TestMethod]
        public void FlagPendingPolicies_SuccessfulUpdate()
        {
            // Arrange
            var updatedCount = 500;
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(eventsAccessor => eventsAccessor.FlagPendingPolicies())
                .Returns(updatedCount);

            var manager = new BackfillManager(
                null,
                mockEventsAccessor.Object,
                null,
                null,
                mockLogger.Object);

            // Act
            manager.FlagPendingPolicies();

            // Assert
            mockEventsAccessor.Verify(x => x.FlagPendingPolicies(), Times.Once());
        }

        private Policy GetTestPolicy()
        {
            return new Policy
            {
                PolicyNumber = "123456789",
                CompanyCode = "01"
            };
        }

        private PolicyHierarchy GetPolicyHierarchy()
        {
            return new PolicyHierarchy
            {
                CompanyCode = "01",
                ApplicationDate = DateTime.Now,
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
                            }
                        }
                    }
                },
                PolicyNumber = "123456789"
            };
        }

        private List<CompanyCodeAndPolicyNumber> GenerateRandomCompanyCodeAndPolicyNumber(int numberToCreate)
        {
            var bogus = new Bogus.Faker();
            var policyNumberMin = 10000000;
            var policyNumberMax = 99999999;

            return
                Enumerable.Range(0, numberToCreate)
                .Select(i =>
                    new CompanyCodeAndPolicyNumber(
                        "01",
                        bogus.Random.Number(policyNumberMin, policyNumberMax).ToString()))
                .ToList();
        }
    }
}