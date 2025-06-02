namespace Assurity.Kafka.Managers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Hierarchy;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Managers;
    using Assurity.Kafka.Managers.Tests.TestData;
    using Assurity.Kafka.Utilities.Enums;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mongo2Go;
    using MongoDB.Bson;
    using Moq;
    using V1 = Assurity.PolicyInfo.Contracts.V1.Enums;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PACON_ANNUITY_POLICYEventManagerTests
    {
        private static MongoDbRunner? mongoRunner; // In memory mongo DB for unit testing
        private Mock<ILogger<PACON_ANNUITY_POLICYEventManager>> mockLogger = new Mock<ILogger<PACON_ANNUITY_POLICYEventManager>>();

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            mongoRunner = MongoDbRunner.Start(singleNodeReplSet: true);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            mongoRunner.Dispose();
        }

        [TestMethod]
        public async Task ProcessEvent_UpdatesExistingPolicy_IsSuccessful()
        {
            // Arrange
            var cacheId = ObjectId.GenerateNewId().ToString();
            var policy = GetPolicy();
            var pacon = new PACON_ANNUITY_POLICY
            {
                COMPANY_CODE = "01",
                POLICY_NUMBER = "1234567890",
                STATUS_CODE = "T",
                STATUS_REASON = "DC",
                STATUS_DATE = 20151202,
                ISSUE_DATE = 20011201,
                TAX_QUALIFICATION = "2"
            };

            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;

            var distinctAgents = new HashSet<string>
            {
                "1234",
                "4321",
            };

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor.Setup(cache => cache.GetPolicyAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => null);
            mockEventsAccessor.Setup(cache => cache.CreateOrReplacePolicyAsync(It.IsAny<Policy>()))
                .Returns(Task.CompletedTask);
            mockEventsAccessor.Setup(cache => cache.UpdateOrCreatePolicyHierarchyAsync(policyHierarchy))
                .Returns(Task.CompletedTask);
            mockEventsAccessor.Setup(cache => cache.InsertAgentPolicyAccessAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<long>());

            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine.Setup(policyEngine => policyEngine.GetPolicy(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((GetPolicyResult.Found, policy));

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine.Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
              .ReturnsAsync(policyHierarchy);
            mockHierarchyEngine.Setup(hierarchyEngine => hierarchyEngine.GetDistinctAgentIds(It.IsAny<List<AgentHierarchy>>()))
                .Returns(distinctAgents);

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor.Setup(accessor => accessor.GetAnnuityPolicy(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(pacon);

            var manager = new PACON_ANNUITY_POLICYEventManager(
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockLogger.Object);

            // Act
            await manager.ProcessEvent(pacon);

            // Assert
            mockEventsAccessor.Verify(x => x.GetPolicyAsync(pacon.POLICY_NUMBER, pacon.COMPANY_CODE), Times.Once());
            mockEventsAccessor.Verify(x => x.CreateOrReplacePolicyAsync(policy), Times.Once());
            mockEventsAccessor.Verify(x => x.UpdateOrCreatePolicyHierarchyAsync(policyHierarchy), Times.Once());
            foreach (var agent in distinctAgents)
            {
                mockEventsAccessor.Verify(x => x.InsertAgentPolicyAccessAsync(agent, policy.PolicyNumber, policy.CompanyCode), Times.Once);
            }

            mockPolicyEngine.Verify(x => x.GetPolicy(pacon.POLICY_NUMBER, pacon.COMPANY_CODE), Times.Once());

            mockHierarchyEngine.Verify(x => x.GetPolicyHierarchy(policy), Times.Once());
            mockHierarchyEngine.Verify(x => x.GetDistinctAgentIds(policyHierarchy.HierarchyBranches), Times.Once());
        }

        [TestMethod]
        public async Task ProcessEvent_UpdatesExistingPolicy_Active_to_Pending_PolicyStatus_IsSuccessful()
        {
            // Arrange
            var cacheId = ObjectId.GenerateNewId().ToString();
            var policy = GetPolicy();
            var pacon = new PACON_ANNUITY_POLICY
            {
                COMPANY_CODE = "01",
                POLICY_NUMBER = "1234567890",
                STATUS_CODE = "P",
                STATUS_REASON = "IC",
                STATUS_DATE = 20200418,
                ISSUE_DATE = 20200418,
                TAX_QUALIFICATION = "4"
            };

            var policyHierarchy = HierarchyEngineTestData.PolicyHierarchy;

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor.Setup(cache => cache.GetPolicyAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => policy);
            mockEventsAccessor.Setup(cache => cache.GetPolicyHierarchyAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => policyHierarchy);
            mockEventsAccessor.Setup(cache => cache.UpdatePolicyHierarchyAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<List<AgentHierarchy>>()))
                .ReturnsAsync("jfgsdjfgs");
            mockEventsAccessor
                .Setup(cache => cache.UpdatePolicyAsync(
                    policy,
                    It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(1);

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine.Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(It.IsAny<Policy>()))
                .ReturnsAsync(policyHierarchy);

            var manager = new PACON_ANNUITY_POLICYEventManager(
                mockEventsAccessor.Object,
                null,
                mockHierarchyEngine.Object,
                mockLogger.Object);

            // Act
            await manager.ProcessEvent(pacon);

            // Assert
            mockEventsAccessor.Verify(x => x.GetPolicyAsync(pacon.POLICY_NUMBER, pacon.COMPANY_CODE), Times.Once());
            mockEventsAccessor.Verify(x => x.GetPolicyHierarchyAsync(pacon.POLICY_NUMBER, pacon.COMPANY_CODE), Times.Never);
            mockEventsAccessor.Verify(
                x => x.UpdatePolicyAsync(policy, It.IsAny<Dictionary<string, object>>()),
                Times.Once());

            mockHierarchyEngine.Verify(x => x.GetPolicyHierarchy(policy), Times.Never);
        }

        [TestMethod]
        public async Task ProcessEvent_UpdateExistingPolicy_ShouldUpdateRecord()
        {
            var previousEmployer = new Employer
            {
                Business = new Business
                {
                    EmailAddress = "1234@yahoo.com",
                    Name = new Name
                    {
                        BusinessName = "Previous Company"
                    }
                },
                Number = "99887777"
            };

            var previousPolicy = new Policy
            {
                ApplicationDate = new DateTime(2023, 01, 01),
                ModePremium = 5m,
                CompanyCode = "01",
                PolicyNumber = "1234567890",
                ProductCode = "ABCD",
                BillingDay = 10,
                PolicyStatus = Status.Pending,
                PolicyStatusReason = StatusReason.ReadyToIssue,
                PolicyStatusDetail = PolicyStatusDetail.AgentRecommended,
                IssueState = V1.State.NE,
                IssueDate = new DateTime(2023, 03, 01),
                ResidentState = V1.State.NE,
                BillingMode = BillingMode.Ninthly,
                PaidToDate = new DateTime(2023, 03, 01),
                AnnualPremium = 1000m,
                LineOfBusiness = LineOfBusiness.Health,
                BillingStatus = BillingStatus.Unknown,
                BillingReason = BillingReason.Unknown,
                BillingForm = BillingForm.Unknown,
                ReturnPaymentType = ReturnPaymentType.None,
                TaxQualificationStatus = TaxQualificationStatus.Unknown,
                Employer = previousEmployer
            };

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(m => m.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(previousPolicy);

            mockEventsAccessor
                .Setup(m => m.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(1);

            var pacon = new PACON_ANNUITY_POLICY
            {
                COMPANY_CODE = "01",
                POLICY_NUMBER = "1234567890",
                STATUS_CODE = "P",
                STATUS_REASON = "IC",
                STATUS_DATE = 20200418,
                ISSUE_DATE = 20200418,
                TAX_QUALIFICATION = "4"
            };

            var paconAnnuityPolicyEventManager = new PACON_ANNUITY_POLICYEventManager(
                mockEventsAccessor.Object,
                null,
                null,
                mockLogger.Object);

            // Act
            await paconAnnuityPolicyEventManager
                .ProcessEvent(pacon);

            // Assert/Verify
            var expectedPolicy = previousPolicy;
            expectedPolicy.PolicyStatus = Status.Pending;
            expectedPolicy.PolicyStatusReason = StatusReason.Incomplete;
            expectedPolicy.IssueDate = new DateTime(2020, 04, 18);
            expectedPolicy.TaxQualificationStatus = TaxQualificationStatus.SEP;

            mockEventsAccessor
               .Verify(
               m => m.UpdatePolicyAsync(
                   It.Is<Policy>(actualPolicy => VerifyPoliciesMatch(actualPolicy, expectedPolicy)),
                   It.IsAny<Dictionary<string, object>>()),
               Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_DeleteExistingPolicy_ShouldDeleteteRecord()
        {
            var pacon = new PACON_ANNUITY_POLICY
            {
                COMPANY_CODE = "01",
                POLICY_NUMBER = "1234567890",
                STATUS_CODE = "T",
                STATUS_REASON = "ER",
                STATUS_DATE = 20200418,
                ISSUE_DATE = 20200418,
                TAX_QUALIFICATION = "8"
            };

            var existingMongoPolicy = new Policy
            {
                ApplicationDate = new DateTime(2023, 01, 01),
                ModePremium = 5m,
                CompanyCode = "01",
                PolicyNumber = "1234567890",
                ProductCode = "ABCD",
                BillingDay = 10,
                PolicyStatus = Status.Pending,
                PolicyStatusReason = StatusReason.ReadyToIssue,
                IssueState = V1.State.NE,
                IssueDate = new DateTime(2023, 03, 01),
                ResidentState = V1.State.NE,
                BillingMode = BillingMode.Ninthly,
                PaidToDate = new DateTime(2023, 03, 01),
                AnnualPremium = 1000m,
                LineOfBusiness = LineOfBusiness.Health,
                BillingStatus = BillingStatus.Unknown,
                BillingReason = BillingReason.Unknown,
                BillingForm = BillingForm.Unknown,
                ReturnPaymentType = ReturnPaymentType.None,
                TaxQualificationStatus = TaxQualificationStatus.Unknown,
            };

            var existingMongoPolicyHierarchy = new PolicyHierarchy
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
            };

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(m => m.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(existingMongoPolicy);
            mockEventsAccessor
                .Setup(m => m.DeletePolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(1);

            var mockEngineLogger = new Mock<ILogger<ConsumerPolicyEngine>>();
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            mockPolicyEngine
                .Setup(m => m.DeletePolicy(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);
            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.RetrieveAgentIds(
                    It.IsAny<List<AgentHierarchy>>()))
                .Returns(new List<string>());

            var paconAnnuityPolicyEventManager = new PACON_ANNUITY_POLICYEventManager(
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockLogger.Object);

            // Act
            await paconAnnuityPolicyEventManager
                .ProcessEvent(pacon);

            // Assert/Verify
            mockEventsAccessor
                .Verify(
                m => m.GetPolicyAsync(pacon.POLICY_NUMBER, pacon.COMPANY_CODE),
                Times.Once);
            mockPolicyEngine
                .Verify(
                m => m.DeletePolicy(
                    pacon.POLICY_NUMBER,
                    pacon.COMPANY_CODE),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessEvent_ContractCodeIsA_ContractReasonIsSM_ShouldUpdateRecord()
        {
            var currentEmployer = GetEmployer();
            var existingPolicy = GetExistingTerminatedPolicy();
            existingPolicy.LineOfBusiness = LineOfBusiness.ImmediateAnnuity;

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            var mockPolicyEngine = new Mock<IConsumerPolicyEngine>(MockBehavior.Strict);
            var mockHierarchyEngine = new Mock<IConsumerHierarchyEngine>(MockBehavior.Strict);

            SetupMockReturns(
                currentEmployer,
                existingPolicy,
                mockEventsAccessor,
                mockPolicyEngine,
                mockHierarchyEngine);

            var pacon = new PACON_ANNUITY_POLICY
            {
                COMPANY_CODE = "01",
                POLICY_NUMBER = "1234567890",
                STATUS_CODE = "T",
                STATUS_REASON = "LP",
                STATUS_DATE = 20230404,
                ISSUE_DATE = 20200418,
                TAX_QUALIFICATION = "8"
            };

            var paconAnnuityPolicyEventManager = new PACON_ANNUITY_POLICYEventManager(
                mockEventsAccessor.Object,
                mockPolicyEngine.Object,
                mockHierarchyEngine.Object,
                mockLogger.Object);

            // Act
            await paconAnnuityPolicyEventManager
                .ProcessEvent(pacon);

            // Assert/Verify
            var expectedPolicy = new Policy
            {
                ApplicationDate = new DateTime(2023, 01, 01),
                ModePremium = 5m,
                CompanyCode = "01",
                PolicyNumber = "1234567890",
                ProductCode = "ABCD",
                BillingDay = 10,
                PolicyStatus = Status.Terminated,
                PolicyStatusReason = StatusReason.Lapsed,
                PolicyStatusDetail = PolicyStatusDetail.AgentRecommended,
                IssueState = V1.State.NE,
                IssueDate = new DateTime(2020, 04, 18),
                LineOfBusiness = LineOfBusiness.ImmediateAnnuity,
                ResidentState = V1.State.NE,
                BillingMode = BillingMode.Ninthly,
                PaidToDate = new DateTime(2023, 03, 01),
                PastDue = false,
                AnnualPremium = 1000m,
                BillingStatus = BillingStatus.Unknown,
                BillingReason = BillingReason.Unknown,
                BillingForm = BillingForm.Unknown,
                ReturnPaymentType = ReturnPaymentType.None,
                TaxQualificationStatus = TaxQualificationStatus.DefinedBenefit,
                Employer = currentEmployer,
                TerminationDate = new DateTime(2023, 04, 04)
            };

            mockEventsAccessor
                .Verify(
                m => m.UpdatePolicyAsync(
                    It.Is<Policy>(actualPolicy => VerifyPoliciesMatch(actualPolicy, expectedPolicy)),
                    It.IsAny<Dictionary<string, object>>()),
                Times.Once);
        }

        private bool VerifyPoliciesMatch(Policy actualPolicy, Policy expectedPolicy)
        {
            if (actualPolicy.Employer != null && expectedPolicy.Employer != null)
            {
                actualPolicy.Employer.Should().BeEquivalentTo(expectedPolicy.Employer);
            }

            return
                actualPolicy.ApplicationDate == expectedPolicy.ApplicationDate &&
                actualPolicy.ModePremium == expectedPolicy.ModePremium &&
                actualPolicy.CompanyCode == expectedPolicy.CompanyCode &&
                actualPolicy.PolicyNumber == expectedPolicy.PolicyNumber &&
                actualPolicy.ProductCode == expectedPolicy.ProductCode &&
                actualPolicy.BillingDay == expectedPolicy.BillingDay &&
                actualPolicy.PolicyStatus == expectedPolicy.PolicyStatus &&
                actualPolicy.PolicyStatusReason == expectedPolicy.PolicyStatusReason &&
                actualPolicy.IssueState == expectedPolicy.IssueState &&
                actualPolicy.IssueDate == expectedPolicy.IssueDate &&
                actualPolicy.ResidentState == expectedPolicy.ResidentState &&
                actualPolicy.BillingMode == expectedPolicy.BillingMode &&
                actualPolicy.PaidToDate == expectedPolicy.PaidToDate &&
                actualPolicy.AnnualPremium == expectedPolicy.AnnualPremium &&
                actualPolicy.LineOfBusiness == expectedPolicy.LineOfBusiness &&
                actualPolicy.BillingStatus == expectedPolicy.BillingStatus &&
                actualPolicy.BillingReason == expectedPolicy.BillingReason &&
                actualPolicy.BillingForm == expectedPolicy.BillingForm &&
                actualPolicy.ReturnPaymentType == expectedPolicy.ReturnPaymentType &&
                actualPolicy.TaxQualificationStatus == expectedPolicy.TaxQualificationStatus &&
                actualPolicy.TerminationDate == expectedPolicy.TerminationDate;
        }

        private Policy GetPolicy()
        {
            return new Policy
            {
                PolicyNumber = "1234567890",
                CompanyCode = "01",
                BillingForm = BillingForm.CreditCard,
                ProductCode = "AEPUWLNP",
                PolicyStatus = Status.Active,
                PolicyStatusReason = StatusReason.DeathClaim,
                BillingMode = BillingMode.Biweekly,
                PaidToDate = new DateTime(2022, 01, 01),
                ModePremium = 1,
                AnnualPremium = 1,
                LineOfBusiness = LineOfBusiness.TraditionalLife,
                BillingStatus = BillingStatus.Active,
                BillingReason = BillingReason.WaiverDisability,
                ReturnPaymentType = ReturnPaymentType.None,
                TaxQualificationStatus = TaxQualificationStatus.IRA,
                ApplicationDate = new DateTime(2022, 01, 01)
            };
        }

        private Employer GetEmployer()
        {
            return new Employer
            {
                Business = new Business
                {
                    EmailAddress = "1234@yahoo.com",
                    Name = new Name
                    {
                        BusinessName = "Current Company"
                    }
                },
                Number = "99887777"
            };
        }

        private Policy GetExistingTerminatedPolicy()
        {
            var currentEmployer = GetEmployer();

            return new Policy
            {
                ApplicationDate = new DateTime(2023, 01, 01),
                ModePremium = 5m,
                CompanyCode = "01",
                PolicyNumber = "1234567890",
                ProductCode = "ABCD",
                BillingDay = 10,
                PolicyStatus = Status.Terminated,
                PolicyStatusReason = StatusReason.LapsePending,
                PolicyStatusDetail = PolicyStatusDetail.AgentRecommended,
                IssueState = V1.State.NE,
                IssueDate = new DateTime(2023, 03, 01),
                ResidentState = V1.State.NE,
                BillingMode = BillingMode.Ninthly,
                PaidToDate = new DateTime(2023, 03, 01),
                AnnualPremium = 1000m,
                LineOfBusiness = LineOfBusiness.Health,
                BillingStatus = BillingStatus.Unknown,
                BillingReason = BillingReason.Unknown,
                BillingForm = BillingForm.Unknown,
                ReturnPaymentType = ReturnPaymentType.None,
                TaxQualificationStatus = TaxQualificationStatus.TSA,
                Employer = currentEmployer
            };
        }

        private void SetupMockReturns(
            Employer currentEmployer,
            Policy existingPolicy,
            Mock<IEventsAccessor> mockEventsAccessor,
            Mock<IConsumerPolicyEngine> mockPolicyEngine,
            Mock<IConsumerHierarchyEngine> mockHierarchyEngine)
        {
            mockEventsAccessor
                .Setup(m => m.GetPolicyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(existingPolicy);

            mockEventsAccessor
                .Setup(m => m.UpdatePolicyAsync(
                    It.IsAny<Policy>(),
                    It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(cache => cache.UpdatePolicyHierarchyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<List<AgentHierarchy>>()))
                .ReturnsAsync("jfgsdjfgs");

            mockEventsAccessor
                .Setup(m => m.GetPolicyHierarchyAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new PolicyHierarchy());

            mockHierarchyEngine
                .Setup(hierarchyEngine => hierarchyEngine.GetPolicyHierarchy(
                    It.IsAny<Policy>()))
                .ReturnsAsync(new PolicyHierarchy());
        }
    }
}