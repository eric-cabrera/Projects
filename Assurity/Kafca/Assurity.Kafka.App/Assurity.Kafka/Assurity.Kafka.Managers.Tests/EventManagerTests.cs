namespace Assurity.Kafka.Managers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Engines.Hierarchy;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Managers.Tests.Extensions;
    using Assurity.Kafka.Managers.Tests.TestData;
    using Assurity.Kafka.Utilities.Enums;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MongoDB.Bson;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class EventManagerTests
    {
        [TestMethod]
        public async Task CreatePolicyAndAccess_FailedToGeneratePolicy_Failed()
        {
            // Arrange
            Policy? policy = null;
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;

            var mockLogger = new Mock<ILogger<BaseClassTester>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetPolicy(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((GetPolicyResult.NotFound, policy));

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);

            var tester = new BaseClassTester(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object);

            // Act
            var success = await tester.GeneratePolicyWithHierarchyAndAgentAccess("1234567890", "01", "BaseClassTester");

            // Assert
            Assert.AreEqual(false, success);

            mockPolicyEngine.Verify(
                mockPolicyEngine => mockPolicyEngine.GetPolicy("1234567890", "01"), Times.Once);

            var expectedMessage = "Policy Number: 1234567890 for the 'BaseClassTester' event -- failed to get or generate policy from Lifepro. The policy was not found in the database (due to rules of policies we care about or it does not exist).";
            mockLogger.VerifyLog(LogLevel.Warning, expectedMessage);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.CreateOrReplacePolicyAsync(new Policy()), Times.Never);
            mockHierarchyEngine.Verify(
                mockHierarchyEngine => mockHierarchyEngine.GetPolicyHierarchy(new Policy()), Times.Never);
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdateOrCreatePolicyHierarchyAsync(new PolicyHierarchy()), Times.Never);
            mockHierarchyEngine.Verify(
                mockHierarchyEngine => mockHierarchyEngine.GetDistinctAgentIds(new List<AgentHierarchy>()), Times.Never);
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.InsertAgentPolicyAccessAsync(string.Empty, string.Empty, string.Empty), Times.Never);
        }

        [TestMethod]
        public async Task CreatePolicyAndAccess_FailedToStorePolicy_Failure()
        {
            // Arrange
            Policy? policy = new Policy();
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;

            var mockLogger = new Mock<ILogger<BaseClassTester>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(cache => cache.CreateOrReplacePolicyAsync(It.IsAny<Policy>()))
                .Throws(new Exception());

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetPolicy(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((GetPolicyResult.Found, policy));

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var tester = new BaseClassTester(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object);

            // Act
            var success = await tester.GeneratePolicyWithHierarchyAndAgentAccess("1234567890", "01", "BaseClassTester");

            // Assert
            Assert.AreEqual(false, success);

            mockPolicyEngine.Verify(
                mockPolicyEngine => mockPolicyEngine.GetPolicy("1234567890", "01"), Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.CreateOrReplacePolicyAsync(policy), Times.Once);

            var expectedMessage = "Policy '1234567890' failed to be stored by the 'BaseClassTester' event manager.";
            mockLogger.VerifyLog(LogLevel.Error, expectedMessage);

            mockHierarchyEngine.Verify(
                mockHierarchyEngine => mockHierarchyEngine.GetPolicyHierarchy(new Policy()), Times.Never);
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdateOrCreatePolicyHierarchyAsync(new PolicyHierarchy()), Times.Never);
            mockHierarchyEngine.Verify(
                mockHierarchyEngine => mockHierarchyEngine.GetDistinctAgentIds(new List<AgentHierarchy>()), Times.Never);
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.InsertAgentPolicyAccessAsync(string.Empty, string.Empty, string.Empty), Times.Never);
        }

        [TestMethod]
        public async Task CreatePolicyAndAccess_NoApplicationDate_Success()
        {
            // Arrange
            Policy? policy = new Policy();
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var cacheId = ObjectId.GenerateNewId().ToString();

            var mockLogger = new Mock<ILogger<BaseClassTester>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(cache => cache.CreateOrReplacePolicyAsync(It.IsAny<Policy>()))
                .Returns(Task.CompletedTask);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetPolicy(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((GetPolicyResult.Found, policy));

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var tester = new BaseClassTester(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object);

            // Act
            var success = await tester.GeneratePolicyWithHierarchyAndAgentAccess("1234567890", "01", "BaseClassTester");

            // Assert
            Assert.AreEqual(true, success);

            mockPolicyEngine.Verify(
                mockPolicyEngine => mockPolicyEngine.GetPolicy("1234567890", "01"), Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.CreateOrReplacePolicyAsync(policy), Times.Once);

            var expectedMessage = "Policy '1234567890' was stored by the 'BaseClassTester' event manager.";
            mockLogger.VerifyLog(LogLevel.Information, expectedMessage);

            mockHierarchyEngine.Verify(
                mockHierarchyEngine => mockHierarchyEngine.GetPolicyHierarchy(new Policy()), Times.Never);
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdateOrCreatePolicyHierarchyAsync(new PolicyHierarchy()), Times.Never);
            mockHierarchyEngine.Verify(
                mockHierarchyEngine => mockHierarchyEngine.GetDistinctAgentIds(new List<AgentHierarchy>()), Times.Never);
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.InsertAgentPolicyAccessAsync(string.Empty, string.Empty, string.Empty), Times.Never);
        }

        [TestMethod]
        public async Task CreatePolicyAndAccess_FailedToGenerateHierarchy_Failed()
        {
            // Arrange
            Policy? policy = new Policy
            {
                ApplicationDate = new DateTime(1980, 1, 1)
            };
            PolicyHierarchy policyHierarchy = null;
            var cacheId = ObjectId.GenerateNewId().ToString();

            var mockLogger = new Mock<ILogger<BaseClassTester>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(cache => cache.CreateOrReplacePolicyAsync(It.IsAny<Policy>()))
                .Returns(Task.CompletedTask);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetPolicy(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((GetPolicyResult.Found, policy));

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var tester = new BaseClassTester(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object);

            // Act
            var success = await tester.GeneratePolicyWithHierarchyAndAgentAccess("1234567890", "01", "BaseClassTester");

            // Assert
            Assert.AreEqual(false, success);

            mockPolicyEngine.Verify(
                mockPolicyEngine => mockPolicyEngine.GetPolicy("1234567890", "01"), Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.CreateOrReplacePolicyAsync(policy), Times.Once);

            var expectedMessage = "Policy '1234567890' was stored by the 'BaseClassTester' event manager.";
            mockLogger.VerifyLog(LogLevel.Information, expectedMessage);

            mockHierarchyEngine.Verify(
                mockHierarchyEngine => mockHierarchyEngine.GetPolicyHierarchy(policy), Times.Once);

            expectedMessage = "Hierarchy for policy: '1234567890' has not been created. Failed to generate policy hierarchy from Lifepro.";
            mockLogger.VerifyLog(LogLevel.Error, expectedMessage);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdateOrCreatePolicyHierarchyAsync(new PolicyHierarchy()), Times.Never);
            mockHierarchyEngine.Verify(
                mockHierarchyEngine => mockHierarchyEngine.GetDistinctAgentIds(new List<AgentHierarchy>()), Times.Never);
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.InsertAgentPolicyAccessAsync(string.Empty, string.Empty, string.Empty), Times.Never);
        }

        [TestMethod]
        public async Task CreatePolicyAndAccess_FailedToStoreHierarchy_Failed()
        {
            // Arrange
            Policy? policy = new Policy
            {
                ApplicationDate = new DateTime(1980, 1, 1)
            };
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var cacheId = ObjectId.GenerateNewId().ToString();

            var mockLogger = new Mock<ILogger<BaseClassTester>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(cache => cache.CreateOrReplacePolicyAsync(It.IsAny<Policy>()))
                .Returns(Task.CompletedTask);

            mockEventsAccessor.Setup(cache => cache.UpdateOrCreatePolicyHierarchyAsync(It.IsAny<PolicyHierarchy>()))
                .Throws(new Exception());

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetPolicy(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((GetPolicyResult.Found, policy));

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var tester = new BaseClassTester(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object);

            // Act
            var success = await tester.GeneratePolicyWithHierarchyAndAgentAccess("1234567890", "01", "BaseClassTester");

            // Assert
            Assert.AreEqual(false, success);

            mockPolicyEngine.Verify(
                mockPolicyEngine => mockPolicyEngine.GetPolicy("1234567890", "01"), Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.CreateOrReplacePolicyAsync(policy), Times.Once);

            var expectedMessage = "Policy '1234567890' was stored by the 'BaseClassTester' event manager.";
            mockLogger.VerifyLog(LogLevel.Information, expectedMessage);

            mockHierarchyEngine.Verify(
                mockHierarchyEngine => mockHierarchyEngine.GetPolicyHierarchy(policy), Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdateOrCreatePolicyHierarchyAsync(policyHierarchy), Times.Once);

            expectedMessage = "Hierarchy for policy '1234567890' failed to be stored by the 'BaseClassTester' event manager.";
            mockLogger.VerifyLog(LogLevel.Error, expectedMessage);

            mockHierarchyEngine.Verify(
                mockHierarchyEngine => mockHierarchyEngine.GetDistinctAgentIds(new List<AgentHierarchy>()), Times.Never);
            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.InsertAgentPolicyAccessAsync(string.Empty, string.Empty, string.Empty), Times.Never);
        }

        [TestMethod]
        public async Task CreatePolicyAndAccess_FailedToStoreAgentPolicyAccess_Failed()
        {
            // Arrange
            Policy? policy = new Policy
            {
                CompanyCode = "01",
                PolicyNumber = "1234567890",
                ApplicationDate = new DateTime(1980, 1, 1)
            };
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var cacheId = ObjectId.GenerateNewId().ToString();
            var distinctAgents = new HashSet<string>
            {
                "1234"
            };

            var mockLogger = new Mock<ILogger<BaseClassTester>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(cache => cache.CreateOrReplacePolicyAsync(It.IsAny<Policy>()))
                .Returns(Task.CompletedTask);
            mockEventsAccessor.Setup(cache => cache.UpdateOrCreatePolicyHierarchyAsync(It.IsAny<PolicyHierarchy>()))
                .Returns(Task.CompletedTask);
            mockEventsAccessor.Setup(cache => cache.InsertAgentPolicyAccessAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetPolicy(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((GetPolicyResult.Found, policy));

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetDistinctAgentIds(It.IsAny<List<AgentHierarchy>>()))
                .Returns(distinctAgents);

            var tester = new BaseClassTester(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object);

            // Act
            var success = await tester.GeneratePolicyWithHierarchyAndAgentAccess("1234567890", "01", "BaseClassTester");

            // Assert
            Assert.AreEqual(false, success);

            mockPolicyEngine.Verify(
                mockPolicyEngine => mockPolicyEngine.GetPolicy("1234567890", "01"), Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.CreateOrReplacePolicyAsync(policy), Times.Once);

            var expectedMessage = "Policy '1234567890' was stored by the 'BaseClassTester' event manager.";
            mockLogger.VerifyLog(LogLevel.Information, expectedMessage);

            mockHierarchyEngine.Verify(
               mockHierarchyEngine => mockHierarchyEngine.GetPolicyHierarchy(policy), Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdateOrCreatePolicyHierarchyAsync(policyHierarchy), Times.Once);

            expectedMessage = "Hierarchy for policy '1234567890' was stored by the 'BaseClassTester' event manager.";
            mockLogger.VerifyLog(LogLevel.Information, expectedMessage);

            mockHierarchyEngine.Verify(
                mockHierarchyEngine => mockHierarchyEngine.GetDistinctAgentIds(policyHierarchy.HierarchyBranches), Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.InsertAgentPolicyAccessAsync("1234", "1234567890", "01"), Times.Once);

            expectedMessage = "Failure while attempting to add '1234567890' to AgentPolicyAccess for agent '1234'. The call originated in the 'BaseClassTester' event manager.";
            mockLogger.VerifyLog(LogLevel.Error, expectedMessage);
        }

        [TestMethod]
        public async Task CreatePolicyAndAccess_Success()
        {
            // Arrange
            Policy? policy = new Policy
            {
                CompanyCode = "01",
                PolicyNumber = "1234567890",
                ApplicationDate = new DateTime(1980, 1, 1)
            };
            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;
            var cacheId = ObjectId.GenerateNewId().ToString();
            var distinctAgents = new HashSet<string>
            {
                "1234"
            };

            var mockLogger = new Mock<ILogger<BaseClassTester>>();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(cache => cache.CreateOrReplacePolicyAsync(It.IsAny<Policy>()))
                .Returns(Task.CompletedTask);
            mockEventsAccessor.Setup(cache => cache.UpdateOrCreatePolicyHierarchyAsync(It.IsAny<PolicyHierarchy>()))
                .Returns(Task.CompletedTask);
            mockEventsAccessor.Setup(cache => cache.InsertAgentPolicyAccessAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(1);

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(policyEngine => policyEngine.GetPolicy(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((GetPolicyResult.Found, policy));

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetDistinctAgentIds(It.IsAny<List<AgentHierarchy>>()))
                .Returns(distinctAgents);

            var tester = new BaseClassTester(
                mockEventsAccessor.Object,
                mockLogger.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object);

            // Act
            var success = await tester.GeneratePolicyWithHierarchyAndAgentAccess("1234567890", "01", "BaseClassTester");

            // Assert
            Assert.AreEqual(true, success);

            mockPolicyEngine.Verify(
                mockPolicyEngine => mockPolicyEngine.GetPolicy("1234567890", "01"), Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.CreateOrReplacePolicyAsync(policy), Times.Once);

            var expectedMessage = "Policy '1234567890' was stored by the 'BaseClassTester' event manager.";
            mockLogger.VerifyLog(LogLevel.Information, expectedMessage);

            mockHierarchyEngine.Verify(
               mockHierarchyEngine => mockHierarchyEngine.GetPolicyHierarchy(policy), Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.UpdateOrCreatePolicyHierarchyAsync(policyHierarchy), Times.Once);

            expectedMessage = "Hierarchy for policy '1234567890' was stored by the 'BaseClassTester' event manager.";
            mockLogger.VerifyLog(LogLevel.Information, expectedMessage);

            mockHierarchyEngine.Verify(
                mockHierarchyEngine => mockHierarchyEngine.GetDistinctAgentIds(policyHierarchy.HierarchyBranches), Times.Once);

            mockEventsAccessor.Verify(
                eventsAccessor => eventsAccessor.InsertAgentPolicyAccessAsync("1234", "1234567890", "01"), Times.Once);

            expectedMessage = "Policy access updated for all agents for policy '1234567890'.";
            mockLogger.VerifyLog(LogLevel.Information, expectedMessage);
        }

        public class BaseClassTester : EventManager
        {
            public BaseClassTester(
               IEventsAccessor eventsAccessor,
               ILogger<BaseClassTester> logger,
               IConsumerPolicyEngine policyEngine,
               IConsumerHierarchyEngine hierarchyEngine)
               : base(eventsAccessor, logger, policyEngine, hierarchyEngine)
            {
                EventsAccessor = eventsAccessor;
                Logger = logger;
                PolicyEngine = policyEngine;
                HierarchyEngine = hierarchyEngine;
            }

            private ILogger Logger { get; }

            private IEventsAccessor EventsAccessor { get; }

            private IPolicyEngine PolicyEngine { get; }

            private IHierarchyEngine HierarchyEngine { get; }
        }
    }
}