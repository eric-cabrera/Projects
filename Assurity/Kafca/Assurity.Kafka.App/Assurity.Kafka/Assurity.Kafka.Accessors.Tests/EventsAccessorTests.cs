namespace Assurity.Kafka.Accessors.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Accessors.Extensions;
    using Assurity.Kafka.Accessors.Tests.TestData;
    using Assurity.Kafka.Utilities;
    using Assurity.Kafka.Utilities.Config;
    using Assurity.Kafka.Utilities.Constants;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mongo2Go;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using Moq;
    using State = Assurity.PolicyInfo.Contracts.V1.Enums.State;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class EventsAccessorTests
    {
        private const string DatabaseName = "Events";
        private const string PolicyCollectionName = "Policies";
        private const string PolicyHierarchyCollectionName = "PolicyHierarchy";
        private const string AgentPolicyAccessCollectionName = "AgentPolicyAccess";
        private const string RequirementMappingCollectionName = "RequirementMapping";
        private const string BenefitOptionsCollectionName = "BenefitOptionsMapping";
        private const string PPOLCCollectionName = "PPOLC_Events";

        private Utilities.Config.ConfigurationManager? cacheConfig;
        private MongoDbRunner? mongoRunner; // In memory mongo DB for unit testing
        private MongoClient client;
        private Mock<ILogger<EventsAccessor>> mockLogger = new Mock<ILogger<EventsAccessor>>();
        private IMongoCollection<Policy> policyCollection;
        private IMongoCollection<PolicyHierarchy> policyHierarchyCollection;
        private IMongoCollection<AgentPolicyAccess> agentPolicyAccessCollection;
        private IMongoCollection<PPOLCEvent> ppolcEventsCollection;
        private IMongoCollection<RequirementMapping> requirementMappingCollection;

        [TestInitialize]
        public void Initialize()
        {
            mongoRunner = MongoDbRunner.Start();

            var inMemorySettings = new Dictionary<string, string>
            {
                { "Environment", "LOCAL" },
                { "Cache:MongoDbConnectionString", mongoRunner.ConnectionString },
                { "Cache:MongoDbDatabaseName", DatabaseName },
                { "Cache:MongoDBPolicyCollectionName", PolicyCollectionName },
                { "Cache:MongoDbPolicyHierarchyCollectionName", PolicyHierarchyCollectionName },
                { "Cache:MongoDbAgentPolicyAccessCollectionName", AgentPolicyAccessCollectionName },
                { "Cache:MongoDbRequirementMappingCollectionName", RequirementMappingCollectionName },
                { "Cache:MongoDbBenefitOptionsMappingCollectionName", BenefitOptionsCollectionName },
                { "Cache:MongoDbPPOLCEventsCollectionName", PPOLCCollectionName }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            cacheConfig = new Utilities.Config.ConfigurationManager(configuration);
            CreateConnection();

            SeedData();

            client = new MongoClient(mongoRunner.ConnectionString);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mongoRunner.Dispose();
        }

        [TestMethod]
        public void IsMongo2GoWorking()
        {
            mongoRunner.Should().NotBeNull();
            mongoRunner.ConnectionString.Should().NotBeNull();
            mongoRunner.State.Should().Be(Mongo2Go.State.Running);
        }

        [TestMethod]
        public async Task CreatePolicy_Returns()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();
            var application = new Policy
            {
                PolicyNumber = "12345567",
                CompanyCode = "01",
            };

            // Act
            var cacheId = await eventsAccessor.CreatePolicyAsync(application);

            // Assert
            cacheId.Should().NotBeNull();
        }

        [TestMethod]
        public async Task CreateOrUpdatePolicy_PolicyDoesNotExist_ShouldBeCreated()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();
            var policy = new Policy
            {
                PolicyNumber = "98764132",
                CompanyCode = "01",
            };

            Assert.IsFalse(await eventsAccessor.CheckIfPolicyExists(policy.PolicyNumber, policy.CompanyCode));

            // Act
            await eventsAccessor.CreateOrReplacePolicyAsync(policy);

            // Assert
            Assert.IsTrue(await eventsAccessor.CheckIfPolicyExists(policy.PolicyNumber, policy.CompanyCode));
        }

        [TestMethod]
        public async Task CreateOrUpdatePolicy_PolicyExists_ShouldBeReplaced()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();
            var objectId = ObjectId.GenerateNewId().ToString();
            var policy = new Policy
            {
                Id = objectId,
                PolicyNumber = "12345567",
                CompanyCode = "01",
                Flagged = false
            };

            var updatePolicy = new Policy
            {
                Id = objectId,
                PolicyNumber = "12345567",
                CompanyCode = "01",
                Flagged = true
            };

            await eventsAccessor.CreatePolicyAsync(policy);
            var policyRecord = await eventsAccessor.GetPolicyAsync(policy.PolicyNumber);
            Assert.IsNotNull(policyRecord);
            Assert.IsFalse(policyRecord.Flagged);

            // Act
            await eventsAccessor.CreateOrReplacePolicyAsync(updatePolicy);

            // Assert
            var updatedPolicyRecord = await eventsAccessor.GetPolicyAsync(policy.PolicyNumber);
            Assert.IsTrue(updatedPolicyRecord.Flagged);
        }

        [TestMethod]
        public async Task GetAllRequirementMappingsAsync_ShouldReturn_RequirementMappingList()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();

            // Act
            var requirementMappingsList = await eventsAccessor.GetAllRequirementMappingsAsync();

            // Assert
            Assert.IsNotNull(requirementMappingsList);
            Assert.IsInstanceOfType(requirementMappingsList, typeof(List<RequirementMapping>));
            Assert.AreEqual(115, requirementMappingsList.Count);

            Assert.AreEqual("HomeOffice", requirementMappingsList[0].FulfillingParty);
            Assert.IsFalse(requirementMappingsList[0].Display);

            Assert.AreEqual("Agent", requirementMappingsList[20].FulfillingParty);
            Assert.AreEqual("UploadFile", requirementMappingsList[20].AgentAction);
            Assert.IsTrue(requirementMappingsList[20].Display);
        }

        [TestMethod]
        public async Task GetRequirementMappingAsync_ShouldReturn_RequirementMapping_HomeOffice()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();

            // Act
            var requirementMapping = await eventsAccessor.GetRequirementMappingAsync(6);

            // Assert
            Assert.IsNotNull(requirementMapping);
            Assert.IsInstanceOfType(requirementMapping, typeof(RequirementMapping));
            Assert.AreEqual("HomeOffice", requirementMapping.FulfillingParty);
            Assert.AreEqual(true, requirementMapping.Display);
        }

        [TestMethod]
        public async Task GetRequirementMappingAsync_ShouldReturn_RequirementMapping_Agent()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();

            // Act
            var requirementMapping = await eventsAccessor.GetRequirementMappingAsync(42);

            // Assert
            Assert.IsNotNull(requirementMapping);
            Assert.IsInstanceOfType(requirementMapping, typeof(RequirementMapping));
            Assert.AreEqual("Agent", requirementMapping.FulfillingParty);
            Assert.AreEqual(nameof(RequirementActionType.UploadFile), requirementMapping.AgentAction);
            Assert.AreEqual(true, requirementMapping.Display);
        }

        [TestMethod]
        public async Task GetAllRequirementMappingsAsync_ShouldDeleteAndInsert_RequirementMappingDocuments()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();

            // Assert
            var requirementMappingList = await eventsAccessor.GetAllRequirementMappingsAsync();
            Assert.IsNotNull(requirementMappingList);
            Assert.AreEqual(115, requirementMappingList.Count);
        }

        [TestMethod]
        public async Task GetRequirementMappingAsync_ShouldReturn_Null()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();

            // Act
            var requirementMapping = await eventsAccessor.GetRequirementMappingAsync(999);

            // Assert
            Assert.IsNull(requirementMapping);
        }

        [TestMethod]
        public async Task CheckIfPolicyExists_ShouldExist()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.CheckIfPolicyExists("123456789", "01");

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task CheckIfPolicyExists_ShouldNot()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.CheckIfPolicyExists("234992359129345", "01");

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void GetAllPolicyNumbersAsync_ShouldReturnAllPolicyNumbers()
        {
            // Arrange
            var allPolicyNumbers = GetPolicies().Select(p => new CompanyCodeAndPolicyNumber(p.CompanyCode, p.PolicyNumber)).ToList();
            allPolicyNumbers.AddRange(MongoDbTestData.PoliciesForGetPoliciesAsyncTests.Select(p => new CompanyCodeAndPolicyNumber(p.CompanyCode, p.PolicyNumber)));
            var eventsAccessor = GetEventsAccessor();

            // Act
            var policyNumbers = eventsAccessor.GetAllCompanyCodesAndPolicyNumbers().ToList();

            // Assert
            CollectionAssert.AreEquivalent(allPolicyNumbers, policyNumbers);
        }

        [TestMethod]
        public async Task UpdatePolicyAsync_With_Beneficiaries_ReturnsPolicy_With_UpdatedBeneficiaries()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCode = "01";
            var beneficiaries = new List<Beneficiary>()
            {
                new Beneficiary
                {
                    Participant = new Participant
                    {
                        Person = new Person
                        {
                            Name = new Name
                            {
                                IndividualFirst = "Rachel",
                                IndividualLast = "Willams"
                            }
                        }
                    },
                    BeneficiaryType = BeneficiaryType.Primary
                }
            };

            var eventsAccessor = GetEventsAccessor();

            // Act
            var policy = await eventsAccessor.GetPolicyAsync(policyNumber, companyCode);
            Assert.IsNotNull(policy);
            policy.Beneficiaries = beneficiaries;

            await eventsAccessor.UpdatePolicyAsync(policy, policy.Beneficiaries, "Beneficiaries");

            // Assert
            Assert.IsNotNull(policy);
            Assert.AreEqual(policyNumber, policy.PolicyNumber);
            Assert.AreEqual(companyCode, policy.CompanyCode);
            Assert.AreEqual(beneficiaries, policy.Beneficiaries);
        }

        [TestMethod]
        public async Task UpdatePolicyAsync_With_ObjectDictionary_ReturnsPolicy_With_UpdatedProperties()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCode = "01";

            var taxQualificationStatus = TaxQualificationStatus.SEP;
            var issueDate = new DateTime(2023, 02, 12);
            var terminationDate = new DateTime(2023, 04, 20);

            var objDictionary = new Dictionary<string, object>
            {
                { "TaxQualificationStatus", taxQualificationStatus },
                { "IssueDate", issueDate },
                { "TerminationDate", terminationDate }
            };

            var eventsAccessor = GetEventsAccessor();

            // Act
            var policy = await eventsAccessor.GetPolicyAsync(policyNumber, companyCode);
            Assert.IsNotNull(policy);
            policy.TaxQualificationStatus = taxQualificationStatus;
            policy.IssueDate = issueDate;
            policy.TerminationDate = terminationDate;

            await eventsAccessor.UpdatePolicyAsync(policy, objDictionary);

            // Assert
            Assert.IsNotNull(policy);
            Assert.AreEqual(policyNumber, policy.PolicyNumber);
            Assert.AreEqual(companyCode, policy.CompanyCode);
            Assert.AreEqual(taxQualificationStatus, policy.TaxQualificationStatus);
            Assert.AreEqual(issueDate, policy.IssueDate);
            Assert.AreEqual(terminationDate, policy.TerminationDate);
        }

        [TestMethod]
        public async Task UpdatePolicyBenefitsAsync_ReturnsPolicyBenefits_WithUpdatedProperties()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCode = "01";

            var benefitAmount = 1001m;
            var dividendOption = DividendOption.CashDividend;
            var benefitDescription = "Waiver on Base Test";
            var benefitStatus = Status.Pending;
            var benefitStatusReason = StatusReason.Restored;
            var planCode = "ADIR-E1";

            var objDictionary = new Dictionary<string, object>
            {
                { BenefitProperties.BenefitAmount, benefitAmount },
                { BenefitProperties.BenefitDescription, benefitDescription },
                { BenefitProperties.BenefitStatus, benefitStatus },
                { BenefitProperties.BenefitStatusReason, benefitStatusReason },
                { BenefitProperties.PlanCode, planCode },
                { BenefitProperties.DividendOption, dividendOption }
            };

            var eventsAccessor = GetEventsAccessor();

            // Act
            var policy = await eventsAccessor.GetPolicyAsync(policyNumber, companyCode);

            await eventsAccessor.UpdatePolicyBenefitsAsync(policy, objDictionary, 1234);

            var updatedPolicy = await eventsAccessor.GetPolicyAsync(policyNumber, companyCode);
            var index = policy.Benefits.FindIndex(benefit => benefit.BenefitId == 1234);
            var updatedBenefit = updatedPolicy.Benefits[index];

            // Assert
            Assert.IsNotNull(policy);
            Assert.AreEqual(policyNumber, policy.PolicyNumber);
            Assert.AreEqual(companyCode, policy.CompanyCode);

            Assert.AreEqual(1001, updatedBenefit.BenefitAmount);
            Assert.AreEqual(DividendOption.CashDividend, updatedBenefit.DividendOption);
            Assert.AreEqual(Status.Pending, updatedBenefit.BenefitStatus);
            Assert.AreEqual(StatusReason.Restored, updatedBenefit.BenefitStatusReason);
            Assert.AreEqual("Waiver on Base Test", updatedBenefit.BenefitDescription);
            Assert.AreEqual("ADIR-E1", updatedBenefit.PlanCode);
        }

        [TestMethod]
        public async Task UpdateNameAndEmailAddressInPolicyRequirements_ReturnsPolicy_With_UpdatedPersonName()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCode = "01";

            var eventsAccessor = GetEventsAccessor();

            // Act
            var policy = await eventsAccessor.GetPolicyAsync(policyNumber, companyCode);

            var newPerson = new Person
            {
                Name = new Name
                {
                    IndividualFirst = "First1",
                    IndividualLast = "Last1",
                    IndividualMiddle = "N",
                    NameId = 451234
                },
                EmailAddress = "wow@gmail.com"
            };

            var isBusiness = false;
            await eventsAccessor.UpdateNameAndEmailAddressInPolicyRequirements(policy, newPerson, isBusiness);

            var updatedPolicy = await eventsAccessor.GetPolicyAsync(policyNumber, companyCode);

            // Assert
            Assert.IsNotNull(updatedPolicy);
            Assert.AreEqual(newPerson.Name.IndividualFirst, updatedPolicy.Requirements[0].AppliesTo.Person.Name.IndividualFirst);
            Assert.AreEqual(newPerson.Name.IndividualLast, updatedPolicy.Requirements[0].AppliesTo.Person.Name.IndividualLast);
            Assert.AreEqual(newPerson.Name.IndividualMiddle, updatedPolicy.Requirements[0].AppliesTo.Person.Name.IndividualMiddle);
            Assert.AreEqual(newPerson.EmailAddress, updatedPolicy.Requirements[0].AppliesTo.Person.EmailAddress);
        }

        [TestMethod]
        public async Task UpdateNameInPolicyRequirements_ReturnsPolicy_With_UpdatedBusinessName()
        {
            // Arrange
            var policyNumber = "556789123";
            var companyCode = "01";

            var eventsAccessor = GetEventsAccessor();

            // Act
            var policy = await eventsAccessor.GetPolicyAsync(policyNumber, companyCode);
            var newPerson = new Person
            {
                Name = new Name
                {
                    BusinessName = "ABCD Company",
                    NameId = 1234
                },
                EmailAddress = "abcd@gmail.com"
            };

            var isBusiness = true;

            await eventsAccessor.UpdateNameAndEmailAddressInPolicyRequirements(policy, newPerson, isBusiness);

            var updatedPolicy = await eventsAccessor.GetPolicyAsync(policyNumber, companyCode);

            // Assert
            Assert.IsNotNull(updatedPolicy);
            Assert.AreEqual(newPerson.Name.BusinessName, updatedPolicy.Requirements[0].AppliesTo.Business.Name.BusinessName);
            Assert.AreEqual(newPerson.EmailAddress, updatedPolicy.Requirements[0].AppliesTo.Business.EmailAddress);
        }

        [TestMethod]
        public async Task UpdateAddressInPolicyRequirements_ReturnsPolicy_With_UpdatedAddress()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCode = "01";

            var eventsAccessor = GetEventsAccessor();

            // Act
            var policy = await eventsAccessor.GetPolicyAsync(policyNumber, companyCode);

            var newAddress = new Address
            {
                AddressId = 561234,
                Line1 = "123 B St",
                City = "Omaha",
                StateAbbreviation = State.NE,
                Country = Country.USA
            };

            await eventsAccessor.UpdateAddressInPolicyRequirements(policy, newAddress);

            var updatedPolicy = await eventsAccessor.GetPolicyAsync(policyNumber, companyCode);

            // Assert
            Assert.IsNotNull(updatedPolicy);
            Assert.AreEqual(policyNumber, updatedPolicy.PolicyNumber);
            Assert.AreEqual(companyCode, updatedPolicy.CompanyCode);
            Assert.AreEqual("123 B St", updatedPolicy.Requirements[0].AppliesTo.Address.Line1);
            Assert.AreEqual("Omaha", updatedPolicy.Requirements[0].AppliesTo.Address.City);
            Assert.AreEqual(State.NE, updatedPolicy.Requirements[0].AppliesTo.Address.StateAbbreviation);
        }

        [TestMethod]
        public async Task UpdatePhoneNumberInPolicyRequirements_ReturnsPolicy_With_UpdatedPhoneNumber()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCode = "01";

            var eventsAccessor = GetEventsAccessor();

            // Act
            var policy = await eventsAccessor.GetPolicyAsync(policyNumber, companyCode);

            var nameId = 451234;
            var phoneNumber = "456-789-0000";

            await eventsAccessor.UpdatePhoneNumberInPolicyRequirements(policy, nameId, phoneNumber);

            var updatedPolicy = await eventsAccessor.GetPolicyAsync(policyNumber, companyCode);

            // Assert
            Assert.IsNotNull(updatedPolicy);
            Assert.AreEqual(policyNumber, updatedPolicy.PolicyNumber);
            Assert.AreEqual(companyCode, updatedPolicy.CompanyCode);
            Assert.AreEqual("456-789-0987", policy.Requirements[0].AppliesTo.PhoneNumber);
            Assert.AreEqual(phoneNumber, updatedPolicy.Requirements[0].AppliesTo.PhoneNumber);
        }

        [TestMethod]
        public async Task UpdatePhoneNumberInPolicyRequirements_Business_UpdatesPhoneNumber()
        {
            // Arrange
            var policyNumber = "556789123";
            var companyCode = "01";

            var eventsAccessor = GetEventsAccessor();

            // Act
            var policy = await eventsAccessor.GetPolicyAsync(policyNumber, companyCode);

            var nameId = 1234;
            var phoneNumber = "456-789-0000";

            await eventsAccessor.UpdatePhoneNumberInPolicyRequirements(policy, nameId, phoneNumber);

            var updatedPolicy = await eventsAccessor.GetPolicyAsync(policyNumber, companyCode);

            // Assert
            Assert.IsNotNull(updatedPolicy);
            Assert.AreEqual(policyNumber, updatedPolicy.PolicyNumber);
            Assert.AreEqual(companyCode, updatedPolicy.CompanyCode);
            Assert.AreEqual(phoneNumber, updatedPolicy.Requirements[0].AppliesTo.PhoneNumber);
        }

        [TestMethod]
        public async Task GetPolicyAsync_CompanyCodeAndPolicyNumber_PolicyNotFound_ReturnsNull()
        {
            // Arrange
            var policyNumber = "987654321";
            var companyCode = "01";

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPolicyAsync(policyNumber, companyCode);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetPolicyAsync_ByPolicyNumber_ShouldReturnPolicy()
        {
            // Arrange
            const string policyNumber = "123456789";

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPolicyAsync(policyNumber);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetPolicyAsync_ByPolicyNumber_NoPolicyFound_ShouldReturnNull()
        {
            // Arrange
            const string policyNumber = "Bogus";

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPolicyAsync(policyNumber);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetPoliciesAsync_ByRequirementId_ShouldReturnPoliciesWithRequirementId()
        {
            // Arrange
            short requirementId = 999;
            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPoliciesAsync(requirementId);

            // Assert
            Assert.IsTrue(result.Exists(p => p.Requirements.Exists(r => r.Id == requirementId)));
        }

        [TestMethod]
        public async Task GetPoliciesByGroupNumber_MatchExists_ShouldReturnPolicy()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();
            const string groupNumber = "999";

            // Act
            var policies = await eventsAccessor.GetPoliciesByGroupNumber(groupNumber);

            // Assert
            Assert.AreEqual(1, policies.Count);
        }

        [TestMethod]
        public async Task GetPoliciesByGroupNumber_NoMatchFound_ShouldReturnEmptyList()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();

            // Act
            var policies = await eventsAccessor.GetPoliciesByGroupNumber("Bogus Group Number");

            // Assert
            Assert.AreEqual(0, policies.Count);
        }

        [TestMethod]
        public void GetCompanyCodeAndPolicyNumberOfFlaggedPolicies_ShouldReturnExpectedValues()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();
            var expectedCompanyCodesAndFlaggedPolicies = new List<CompanyCodeAndPolicyNumber>
            {
                new CompanyCodeAndPolicyNumber
                {
                    CompanyCode = "02",
                    PolicyNumber = "456789123",
                },
                new CompanyCodeAndPolicyNumber
                {
                    CompanyCode = "01",
                    PolicyNumber = "556789123",
                }
            };

            // Act
            var flaggedCompanyCodeAndPolicyNumbers = eventsAccessor.GetCompanyCodeAndPolicyNumberOfFlaggedPolicies();

            // Assert
            Assert.AreEqual(2, flaggedCompanyCodeAndPolicyNumbers.Count);
            CollectionAssert.AreEquivalent(expectedCompanyCodesAndFlaggedPolicies, flaggedCompanyCodeAndPolicyNumbers);
        }

        [TestMethod]
        public async Task GetPolicyHierarchyAsync_PolicyNumberFound_ShouldReturnPolicyHierarchy()
        {
            // Arrange
            var companyCode = "01";
            var policyNumber = "123456789";

            var eventsAccessor = GetEventsAccessor();

            // Act
            var policyHierarchy = await eventsAccessor.GetPolicyHierarchyAsync(policyNumber, companyCode);

            // Assert
            Assert.IsNotNull(policyHierarchy);
            Assert.AreEqual(companyCode, policyHierarchy.CompanyCode);
            Assert.AreEqual(policyNumber, policyHierarchy.PolicyNumber);
            Assert.AreEqual(1, policyHierarchy.HierarchyBranches.Count);
            Assert.AreEqual("1234", policyHierarchy.HierarchyBranches[0].Agent.AgentId);
            Assert.AreEqual("4321", policyHierarchy.HierarchyBranches[0].HierarchyAgents[0].AgentId);
        }

        [TestMethod]
        public async Task UpdatePolicyHierarchyAsync_PolicyNumberFound_ShouldReturnPolicyHierarchy()
        {
            // Arrange
            var companyCode = "01";
            var policyNumber = "123456789";
            var agentId = "4001";
            var hierarchyAgentId = "4002";

            var eventsAccessor = GetEventsAccessor();
            var newPolicyHierarchy = await eventsAccessor.GetPolicyHierarchyAsync(policyNumber, companyCode);
            if (newPolicyHierarchy != null)
            {
                newPolicyHierarchy.HierarchyBranches = new List<AgentHierarchy>
                {
                    new AgentHierarchy
                    {
                        Agent = new Agent
                        {
                            AgentId = agentId,
                        },
                        HierarchyAgents = new List<HierarchyAgent>
                        {
                            new HierarchyAgent
                            {
                                AgentId = hierarchyAgentId,
                            }
                        }
                    }
                };
            }

            // Act
            var result = await eventsAccessor.UpdatePolicyHierarchyAsync(companyCode, policyNumber, newPolicyHierarchy.HierarchyBranches);

            // Assert
            Assert.IsNotNull(result);

            Assert.AreEqual(hierarchyAgentId, newPolicyHierarchy.HierarchyBranches[0].HierarchyAgents[0].AgentId);
            Assert.AreEqual(agentId, newPolicyHierarchy.HierarchyBranches[0].Agent.AgentId);
        }

        [TestMethod]
        public async Task UpdateOrCreatePolicyHierarchyAsync_ExistingPolicyHierarchy_ShouldUpdate()
        {
            // Arrange
            var companyCode = "01";
            var policyNumber = "123456789";
            var newPolicyHierarchy = new PolicyHierarchy
            {
                CompanyCode = companyCode,
                PolicyNumber = policyNumber,
                HierarchyBranches = null
            };

            var eventsAccessor = GetEventsAccessor();

            var existingPolicyHierarchy = await eventsAccessor.GetPolicyHierarchyAsync(policyNumber, companyCode);

            Assert.IsNotNull(existingPolicyHierarchy.HierarchyBranches);

            // Act
            await eventsAccessor.UpdateOrCreatePolicyHierarchyAsync(newPolicyHierarchy);

            // Assert
            var updatedPolicyHierarchy = await eventsAccessor.GetPolicyHierarchyAsync(policyNumber, companyCode);
            Assert.IsNull(updatedPolicyHierarchy.HierarchyBranches);
        }

        [TestMethod]
        public async Task UpdateOrCreatePolicyHierarchyAsync_NonExistentPolicyHierarchy_ShouldCreate()
        {
            // Arrange
            var companyCode = "01";
            var policyNumber = "5514321241";
            var newPolicyHierarchy = new PolicyHierarchy
            {
                CompanyCode = companyCode,
                PolicyNumber = policyNumber,
                HierarchyBranches = null
            };

            var eventsAccessor = GetEventsAccessor();

            var existingPolicyHierarchy = await eventsAccessor.GetPolicyHierarchyAsync(policyNumber, companyCode);

            Assert.IsNull(existingPolicyHierarchy);

            // Act
            await eventsAccessor.UpdateOrCreatePolicyHierarchyAsync(newPolicyHierarchy);

            // Assert
            var updatedPolicyHierarchy = await eventsAccessor.GetPolicyHierarchyAsync(policyNumber, companyCode);
            Assert.IsNotNull(updatedPolicyHierarchy);
        }

        [TestMethod]
        public async Task UpdatePastDuePoliciesAsync_ShouldReturn_UpdatedCount()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();
            var policyNumbers = new List<string> { "123456789" };

            // Act
            var updatedCount = await eventsAccessor.UpdatePastDuePoliciesAsync(policyNumbers);

            // Assert
            Assert.AreEqual(1, updatedCount);
        }

        [TestMethod]
        public async Task GetInitialPaymentDeclinedPoliciesWithPassedRetentionDurationAsync_ShouldReturn_Passed_RetentionDuration_Policies()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();

            // Act
            var results = await eventsAccessor.GetInitialPaymentDeclinedPoliciesWithPassedRetentionDurationAsync(DateTime.Now);

            // Assert
            Assert.IsTrue(results?.Any());
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("987654321", results[0].PolicyNumber);
        }

        [TestMethod]
        public void GetPolicyNumbersForDeletion_ShouldReturn_Policies()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = eventsAccessor.GetPolicyNumbersForDeletion(DateTime.Now);

            // Assert
            // No date is provided in configuration above for the batch start time.
            // Therefore it defaults to now.
            var now = DateTime.Now;
            var policiesForGetPoliciesAsyncTestsPolicyCount =
                MongoDbTestData
                .PoliciesForGetPoliciesAsyncTests
                .Where(policy => policy.Employer == null && policy.TerminationDate < now)
                .Count();

            var additionalStagesPoliciesCount =
                GetPolicies()
                .Where(p => p.Employer == null && p.TerminationDate < now)
                .Count();

            policiesForGetPoliciesAsyncTestsPolicyCount += additionalStagesPoliciesCount;
            Assert.AreEqual(policiesForGetPoliciesAsyncTestsPolicyCount, result.Count());
            Assert.IsTrue(result.Count() > 0);
        }

        [TestMethod]
        public async Task DeletePolicyAsync_PolicyNumberAndCompanyCode_ShouldDelete()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCode = "01";
            var eventsAccessor = GetEventsAccessor();

            // Act
            var deletedCount = await eventsAccessor.DeletePolicyAsync(policyNumber, companyCode);

            // Assert
            Assert.AreEqual(1, deletedCount);
        }

        [TestMethod]
        public async Task DeletePoliciesAsync_WithSession_ShouldReturn_DeletedCount()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();
            var policyNumbers = new List<string> { "123456789" };

            using (var session = client.StartSession())
            {
                // Act
                var deletedCount = await eventsAccessor.DeletePoliciesAsync(session, policyNumbers);

                // Assert
                Assert.AreEqual(1, deletedCount);
            }
        }

        [TestMethod]
        public async Task DeletePoliciesAsync_WithSession_ShouldReturn_DeletedCount_Equals_Zero()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();
            var policyNumbers = new List<string> { "123456780" };
            using (var session = client.StartSession())
            {
                // Act
                var deletedCount = await eventsAccessor.DeletePoliciesAsync(session, policyNumbers);

                // Assert
                Assert.AreEqual(0, deletedCount);
            }
        }

        [TestMethod]
        public async Task DeletePolicyHierarchies_ShouldDeletePolicyHierarchies()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();
            var records = new List<CompanyCodeAndPolicyNumber> { new CompanyCodeAndPolicyNumber("01", "123456789") };

            Assert.IsNotNull(await eventsAccessor.GetPolicyHierarchyAsync(records.First().PolicyNumber, records.First().CompanyCode));

            // Act
            eventsAccessor.DeletePolicyHierarchies(records);

            // Assert
            Assert.IsNull(await eventsAccessor.GetPolicyHierarchyAsync(records.First().PolicyNumber, records.First().CompanyCode));
        }

        [TestMethod]
        public async Task DeletePolicyHierarchiesAsync_ShouldReturn_DeletedCount()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();
            var policyNumbers = new List<string> { "123456789" };

            using (var session = client.StartSession())
            {
                // Act
                var deletedCount = await eventsAccessor.DeletePolicyHierarchiesAsync(session, policyNumbers);

                // Assert
                Assert.AreEqual(1, deletedCount);
            }
        }

        [TestMethod]
        public async Task DeletePoliciesAsync_WithSession_ShouldDeleteDocuments()
        {
            // Arrange
            var policy1 = "123456789";
            var policy2 = "987654321";
            var eventsAccessor = GetEventsAccessor();
            var policyNumbers = new List<string> { policy1, policy2 };

            Assert.IsNotNull(await eventsAccessor.GetPolicyAsync(policy1));
            Assert.IsNotNull(await eventsAccessor.GetPolicyAsync(policy2));

            using (var session = client.StartSession())
            {
                // Act
                await eventsAccessor.DeletePoliciesAsync(policyNumbers);

                // Assert
                Assert.IsNull(await eventsAccessor.GetPolicyAsync(policy1));
                Assert.IsNull(await eventsAccessor.GetPolicyAsync(policy2));
            }
        }

        [TestMethod]
        public async Task DeletePolicyHierarchiesAsync_ShouldReturn_DeletedCount_Equals_Zero()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();
            var policyNumbers = new List<string> { "123456780" };

            using (var session = client.StartSession())
            {
                // Act
                var deletedCount = await eventsAccessor.DeletePolicyHierarchiesAsync(session, policyNumbers);

                // Assert
                Assert.AreEqual(0, deletedCount);
            }
        }

        [TestMethod]
        public async Task UpdateAgentPolicyAccessListAsync_ShouldReturn_DeletedCount()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();
            var policyNumbers = new List<string> { "123456789" };

            using (var session = client.StartSession())
            {
                // Act
                var updatedCount = await eventsAccessor.UpdateAgentPolicyAccessListAsync(session, policyNumbers);

                // Assert
                Assert.AreEqual(2, updatedCount);
            }
        }

        [TestMethod]
        public async Task UpdatePastDuePoliciesAsync_ShouldReturn_ZeroCount()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();
            var policyNumbers = new List<string>();

            // Act
            var updatedCount = await eventsAccessor.UpdatePastDuePoliciesAsync(policyNumbers);

            // Assert
            Assert.AreEqual(0, updatedCount);
        }

        [TestMethod]
        public async Task GetAgentPolicyAccess_ShouldReturn_AgentPolicyAccess()
        {
            // Arrange
            var agentId = "1234";
            var companyCode = "01";

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetAgentPolicyAccessAsync(agentId, companyCode);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(AgentPolicyAccess));
        }

        [TestMethod]
        public async Task GetPoliciesWithInsuredsByNameIdAsync_ShouldReturn_Policies()
        {
            // Arrange
            var nameId = 561234;

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPoliciesWithInsuredsByNameIdAsync(nameId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<Policy>));
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task GetPoliciesWithOwnersByNameIdAsync_ShouldReturn_Policies()
        {
            // Arrange
            var nameId = 451234;

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPoliciesWithOwnersByNameIdAsync(nameId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<Policy>));
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task GetPoliciesWithPayorsByNameIdAsync_ShouldReturn_Policies()
        {
            // Arrange
            var nameId = 451234;

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPoliciesWithPayorsByNameIdAsync(nameId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<Policy>));
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task GetPoliciesWithBeneficiariesByNameIdAsync_ShouldReturn_Policies()
        {
            // Arrange
            var nameId = 451234;

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPoliciesWithBeneficiariesByNameIdAsync(nameId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<Policy>));
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task GetPoliciesWithAnnuitantsByNameIdAsync_ShouldReturn_Policies()
        {
            // Arrange
            var nameId = 451234;

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPoliciesWithAnnuitantsByNameIdAsync(nameId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<Policy>));
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task GetPoliciesWithAgentsByNameIdAsync_ShouldReturn_Policies()
        {
            // Arrange
            var nameId = 451234;

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPoliciesWithAgentsByNameIdAsync(nameId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<Policy>));
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task GetPoliciesWithPayeeByNameIdAsync_ShouldReturn_Policies()
        {
            // Arrange
            var nameId = 451234;

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPoliciesWithPayeeByNameIdAsync(nameId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<Policy>));
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task GetPoliciesWithAssigneeByNameIdAsync_ShouldReturn_Policies()
        {
            // Arrange
            var nameId = 451234;

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPoliciesWithAssigneeByNameIdAsync(nameId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<Policy>));
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task GetPoliciesWithRequirementsByNameIdAsync_ShouldReturn_Policies()
        {
            // Arrange
            var nameId = 451234;

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPoliciesWithRequirementsByNameIdAsync(nameId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<Policy>));
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task InsertAgentPolicyAccess_NewRecord()
        {
            // Arrange
            var agentId = "abc123";
            var policyNumber = "abc123456";
            var companyCode = "01";

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.InsertAgentPolicyAccessAsync(agentId, policyNumber, companyCode);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(long));
        }

        [TestMethod]
        public async Task InsertAgentPolicyAccess_NewAgentId()
        {
            // Arrange
            var agentId = "abc124";
            var policyNumber = "abc123456";
            var companyCode = "01";

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.InsertAgentPolicyAccessAsync(agentId, policyNumber, companyCode);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(long));
        }

        [TestMethod]
        public async Task InsertPolicyBenefitAsync_NewRecord()
        {
            // Arrange
            var policyNumber = "abc123456";
            var companyCode = "01";

            var eventsAccessor = GetEventsAccessor();

            var benefit = new Benefit
            {
                BenefitId = 1234,
                BenefitAmount = 500,
                BenefitDescription = "Waiver on Base",
                BenefitStatus = Status.Active,
                BenefitStatusReason = StatusReason.None,
                CoverageType = CoverageType.Rider,
                DividendOption = DividendOption.AccumulateAtInterestOrAddToCashValue,
                PlanCode = "ADIR-E",
                BenefitOptions = new List<BenefitOption>
                {
                    new BenefitOption
                    {
                        BenefitOptionName = BenefitOptionName.BenefitPeriod,
                        BenefitOptionValue = BenefitOptionValue.TwentyFourMonths,
                        RelationshipToPrimaryInsured = RelationshipToPrimaryInsured.Self
                    },
                    new BenefitOption
                    {
                        BenefitOptionName = BenefitOptionName.EliminationPeriod,
                        BenefitOptionValue = BenefitOptionValue.NinetyDays,
                        RelationshipToPrimaryInsured = RelationshipToPrimaryInsured.Self
                    },
                    new BenefitOption
                    {
                        BenefitOptionName = BenefitOptionName.UnderwritingClass,
                        BenefitOptionValue = BenefitOptionValue.NonTobacco,
                        RelationshipToPrimaryInsured = RelationshipToPrimaryInsured.Self
                    },
                }
            };

            // Act
            var result = await eventsAccessor.InsertPolicyBenefitAsync(benefit, policyNumber, companyCode);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(string));
        }

        [TestMethod]
        public async Task RemovePolicyBenefit_Removes_PolicyBenefitRecord()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCode = "01";
            var benefitId = 1234;

            var eventsAccessor = GetEventsAccessor();

            var existingPolicy = await eventsAccessor.GetPolicyAsync(policyNumber);
            Assert.IsTrue(existingPolicy.Benefits.Exists(b => b.BenefitId == benefitId));

            // Act
            var result = await eventsAccessor.RemovePolicyBenefitByBenefitIdAsync(policyNumber, companyCode, benefitId);

            // Assert
            var updatedPolicy = await eventsAccessor.GetPolicyAsync(policyNumber);
            Assert.IsFalse(updatedPolicy.Benefits.Exists(b => b.BenefitId == benefitId));
        }

        [TestMethod]
        public async Task GetPoliciesWithEmployerByNameIdAsync_ShouldReturn_Policies()
        {
            // Arrange
            var nameId = 871234;

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPoliciesWithEmployerByNameIdAsync(nameId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<Policy>));
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task GetPoliciesWithInsuredsByAddressIdAsync_ShouldReturn_Policies()
        {
            // Arrange
            var addressId = 561234;

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPoliciesWithInsuredsByAddressIdAsync(addressId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<Policy>));
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task GetPoliciesWithAnnuitantsByAddressIdAsync_ShouldReturn_Policies()
        {
            // Arrange
            var addressId = 561234;

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPoliciesWithAnnuitantsByAddressIdAsync(addressId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<Policy>));
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task GetPoliciesWithOwnersByAddressIdAsync_ShouldReturn_Policies()
        {
            // Arrange
            var addressId = 561234;

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPoliciesWithOwnersByAddressIdAsync(addressId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<Policy>));
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task GetPoliciesWithBeneficiariesByAddressIdAsync_ShouldReturn_Policies()
        {
            // Arrange
            var addressId = 561234;

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPoliciesWithBeneficiariesByAddressIdAsync(addressId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<Policy>));
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task GetPoliciesWithPayorsByAddressIdAsync_ShouldReturn_Policies()
        {
            // Arrange
            var addressId = 561234;

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPoliciesWithPayorsByAddressIdAsync(addressId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<Policy>));
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task GetPoliciesWithPayeeByAddressIdAsync_ShouldReturn_Policies()
        {
            // Arrange
            var addressId = 561234;

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPoliciesWithPayeeByAddressIdAsync(addressId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<Policy>));
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task GetPoliciesWithAssigneeByAddressIdAsync_ShouldReturn_Policies()
        {
            // Arrange
            var addressId = 561234;

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPoliciesWithAssigneeByAddressIdAsync(addressId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<Policy>));
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task GetPoliciesWithAgentsByAddressIdAsync_ShouldReturn_Policies()
        {
            // Arrange
            var addressId = 561234;

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPoliciesWithAgentsByAddressIdAsync(addressId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<Policy>));
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task GetPoliciesWithRequirementsByAddressIdAsync_ShouldReturn_Policies()
        {
            // Arrange
            var addressId = 561234;

            var eventsAccessor = GetEventsAccessor();

            // Act
            var result = await eventsAccessor.GetPoliciesWithRequirementsByAddressIdAsync(addressId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<Policy>));
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task SetChangedPoliciesFlagged_ShouldReturn_ChangedPolicyCount()
        {
            // Arrange
            var eventsAccessor = GetEventsAccessor();
            var changedPolicies = new List<string>() { "123456789", "987654321" };

            // Act
            var updatedPolicyCount = await eventsAccessor.SetChangedPoliciesFlagged(changedPolicies);

            // Assert
            Assert.AreEqual(2, updatedPolicyCount);
        }

        private void CreateConnection()
        {
            var client = new MongoClient(mongoRunner.ConnectionString);
            var database = client.GetDatabase(DatabaseName);
            policyCollection = database.GetCollection<Policy>(PolicyCollectionName);
            policyHierarchyCollection = database.GetCollection<PolicyHierarchy>(PolicyHierarchyCollectionName);
            agentPolicyAccessCollection = database.GetCollection<AgentPolicyAccess>(AgentPolicyAccessCollectionName);
            ppolcEventsCollection = database.GetCollection<PPOLCEvent>(PPOLCCollectionName);
            requirementMappingCollection = database.GetCollection<RequirementMapping>(RequirementMappingCollectionName);
        }

        private Address GetAddress()
        {
            return new Address
            {
                AddressId = 561234,
                Line1 = "123 A St",
                City = "Lincoln",
                StateAbbreviation = State.NE,
                Country = Country.USA
            };
        }

        private List<Policy> GetPolicies()
        {
            return new List<Policy>
            {
                new Policy
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Agents = new List<Agent>
                    {
                        new Agent
                        {
                            Participant = new Participant
                            {
                                Address = GetAddress(),
                                Person = new Person
                                {
                                    Name = new Name
                                    {
                                        NameId = 451234
                                    }
                                }
                            }
                        }
                    },
                    Annuitants = new List<Annuitant>
                    {
                        new Annuitant
                        {
                            Participant = new Participant
                            {
                                Address = GetAddress(),
                                Person = new Person
                                {
                                    Name = new Name
                                    {
                                        NameId = 451234
                                    }
                                }
                            }
                        }
                    },
                    AnnualPremium = 12.69m,
                    Assignee = new Assignee
                    {
                        Participant = new Participant
                        {
                            Address = GetAddress(),
                            Person = new Person
                            {
                                Name = new Name
                                {
                                    NameId = 451234
                                }
                            }
                        }
                    },
                    Beneficiaries = new List<Beneficiary>
                    {
                        new Beneficiary
                        {
                            Participant = new Participant
                            {
                                Address = GetAddress(),
                                Person = new Person
                                {
                                    Name = new Name
                                    {
                                        NameId = 451234
                                    }
                                }
                            }
                        }
                    },
                    Benefits = new List<Benefit>
                    {
                        new Benefit
                        {
                            BenefitId = 1234,
                            BenefitAmount = 500,
                            BenefitDescription = "Waiver on Base",
                            BenefitStatus = Status.Active,
                            BenefitStatusReason = StatusReason.None,
                            CoverageType = CoverageType.Rider,
                            DividendOption = DividendOption.AccumulateAtInterestOrAddToCashValue,
                            PlanCode = "ADIR-E",
                            BenefitOptions = new List<BenefitOption>
                            {
                                new BenefitOption
                                {
                                    BenefitOptionName = BenefitOptionName.BenefitPeriod,
                                    BenefitOptionValue = BenefitOptionValue.TwentyFourMonths,
                                    RelationshipToPrimaryInsured = RelationshipToPrimaryInsured.Self,
                                    StartDate = DateTime.Now,
                                    StopDate = new DateTime(9999, 12, 31)
                                },
                                new BenefitOption
                                {
                                    BenefitOptionName = BenefitOptionName.EliminationPeriod,
                                    BenefitOptionValue = BenefitOptionValue.NinetyDays,
                                    RelationshipToPrimaryInsured = RelationshipToPrimaryInsured.Self,
                                    StartDate = DateTime.Now,
                                    StopDate = new DateTime(9999, 12, 31)
                                },
                                new BenefitOption
                                {
                                    BenefitOptionName = BenefitOptionName.UnderwritingClass,
                                    BenefitOptionValue = BenefitOptionValue.NonTobacco,
                                    RelationshipToPrimaryInsured = RelationshipToPrimaryInsured.Self,
                                    StartDate = DateTime.Now,
                                    StopDate = new DateTime(9999, 12, 31)
                                },
                            }
                        }
                    },
                    BillingDay = 0,
                    BillingForm = BillingForm.Direct,
                    CompanyCode = "01",
                    CreateDate = DateTime.Now,
                    Flagged = false,
                    Insureds = new List<Insured>
                    {
                        new Insured
                        {
                            Participant = new Participant
                            {
                                Address = GetAddress(),
                                IsBusiness = true,
                                Business = new Business
                                {
                                    Name = new Name
                                    {
                                        NameId = 561234,
                                        BusinessName = "ABC Company"
                                    }
                                }
                            }
                        }
                    },
                    IssueDate = DateTime.Now,
                    LastModified = DateTime.Now,
                    ModePremium = 12.69m,
                    Owners = new List<Owner>()
                    {
                        new Owner
                        {
                            Participant = new Participant
                            {
                                Address = GetAddress(),
                                Person = new Person
                                {
                                    Name = new Name
                                    {
                                        NameId = 451234
                                    }
                                }
                            }
                        }
                    },
                    PaidToDate = DateTime.Now,
                    Payors = new List<Payor>
                    {
                        new Payor
                        {
                            Participant = new Participant
                            {
                                Address = GetAddress(),
                                Person = new Person
                                {
                                    Name = new Name
                                    {
                                        NameId = 451234
                                    }
                                }
                            }
                        }
                    },
                    Payee = new Payee
                    {
                        Participant = new Participant
                        {
                            Address = GetAddress(),
                            Person = new Person
                            {
                                Name = new Name
                                {
                                    NameId = 451234
                                }
                            }
                        }
                    },
                    PolicyNumber = "123456789",
                    PolicyStatus = Status.Active,
                    PolicyStatusReason = StatusReason.None,
                    ProductCategory = null,
                    ProductCode = "41OLPUB8",
                    ProductDescription = null,
                    Requirements = new List<Requirement>
                    {
                        new Requirement
                        {
                            AppliesTo = new Participant
                            {
                                Address = GetAddress(),
                                IsBusiness = false,
                                Person = new Person
                                {
                                    Name = new Name
                                    {
                                        NameId = 451234,
                                        IndividualFirst = "First",
                                        IndividualLast = "Last",
                                        IndividualMiddle = "M"
                                    }
                                },
                                PhoneNumber = "456-789-0987"
                            }
                        },
                    },
                    ResidentState = State.NE,
                    ReturnPaymentType = ReturnPaymentType.None,
                    SubmitDate = DateTime.Now,
                    ApplicationDate = DateTime.Now,
                    ApplicationReceivedDate = DateTime.Now,
                    Employer = new Employer
                    {
                        Number = "999",
                        Business = new Business
                        {
                            EmailAddress = "company@yahoo.com",
                            Name = new Name
                            {
                                BusinessName = "Company Inc.",
                                NameId = 871234
                            }
                        }
                    }
                },
                new Policy
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    PolicyNumber = "987654321",
                    ReturnPaymentType = ReturnPaymentType.InitialPaymentCheckDraftDeclined,
                    ReturnPaymentDate = DateTime.Now.AddDays(-50),
                },
                new Policy
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    PolicyNumber = "887654321",
                    TerminationDate = DateTime.Now.AddYears(-4)
                },
                new Policy
                {
                     Id = ObjectId.GenerateNewId().ToString(),
                     CompanyCode = "02",
                     PolicyNumber = "456789123",
                     Flagged = true
                },
                new Policy
                {
                     Id = ObjectId.GenerateNewId().ToString(),
                     PolicyNumber = "556789123",
                     CompanyCode = "01",
                     Flagged = true,
                     Requirements = new List<Requirement>
                     {
                         new Requirement
                         {
                             Id = 999,
                             AppliesTo = new Participant
                             {
                                 IsBusiness = true,
                                 Business = new Business
                                 {
                                     Name = new Name
                                     {
                                         BusinessName = "ABC Company",
                                         NameId = 1234
                                     },
                                     EmailAddress = "abc@gmail.com"
                                 }
                             }
                         }
                     }
                }
            };
        }

        private void SeedData()
        {
            var policies = GetPolicies();

            var policyHierarchy = new List<PolicyHierarchy>
            {
                new PolicyHierarchy
                {
                    CompanyCode = "01",
                    PolicyNumber = "123456789",
                    ApplicationDate = DateTime.Now,
                    HierarchyBranches = new List<AgentHierarchy>
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
                                }
                            }
                        }
                    }
                }
            };

            var agentPolicyAccess = new List<AgentPolicyAccess>
            {
                new AgentPolicyAccess
                {
                    AgentId = "1234",
                    CompanyCode = "01",
                    PolicyNumbers = new List<string>
                    {
                        "123456789",
                    },
                },
                new AgentPolicyAccess
                {
                    AgentId = "4321",
                    CompanyCode = "01",
                    PolicyNumbers = new List<string>
                    {
                        "123456789",
                    }
                }
            };

            var pPOLCEvents = new List<PPOLCEvent>
            {
                new PPOLCEvent
                {
                    POLICY_NUMBER = "123456789",
                    COMPANY_CODE = "01",
                    PRODUCT_CODE = "IH2004",
                    CONTRACT_REASON = "IC",
                    CONTRACT_CODE = "P",
                    CONTRACT_DATE = 20240901,
                    ISSUE_STATE = "NE",
                    RES_STATE = "NJ",
                    BILLING_MODE = 1,
                    BILLING_FORM = "DIR",
                    BILLING_DATE = 0,
                    ISSUE_DATE = 20240901,
                    PAID_TO_DATE = 0,
                    MODE_PREMIUM = 0.00m,
                    ANNUAL_PREMIUM = 0.00m,
                    POLICY_BILL_DAY = 0,
                    POLC_SPECIAL_MODE = " ",
                    LINE_OF_BUSINESS = "H",
                    BILLING_CODE = string.Empty,
                    BILLING_REASON = string.Empty,
                    PAYMENT_REASON = string.Empty,
                    TAX_QUALIFY_CODE = "0",
                    APPLICATION_DATE = 20240829,
                    GROUP_NUMBER = string.Empty,
                    CHANGETIME_DB = DateTimeUtility.DateTimeNowCentral().AddDays(-1).ToString("MM-dd-yyyy hh:mm:ss:fff"), // "09-05-2024 09:43:06.970",
                    APP_RECEIVED_DATE = 20240905,
                },
                new PPOLCEvent
                {
                    POLICY_NUMBER = "123456789",
                    COMPANY_CODE = "01",
                    PRODUCT_CODE = "IH2004",
                    CONTRACT_REASON = "IC",
                    CONTRACT_CODE = "P",
                    CONTRACT_DATE = 20240901,
                    ISSUE_STATE = "NE",
                    RES_STATE = "NJ",
                    BILLING_MODE = 1,
                    BILLING_FORM = "DIR",
                    BILLING_DATE = 20240901,
                    ISSUE_DATE = 20240901,
                    PAID_TO_DATE = 20240901,
                    MODE_PREMIUM = 104.99m,
                    ANNUAL_PREMIUM = 1206.63m,
                    POLICY_BILL_DAY = 0,
                    POLC_SPECIAL_MODE = string.Empty,
                    LINE_OF_BUSINESS = "H",
                    BILLING_CODE = string.Empty,
                    BILLING_REASON = "MD",
                    PAYMENT_REASON = string.Empty,
                    TAX_QUALIFY_CODE = "0",
                    APPLICATION_DATE = 20240829,
                    GROUP_NUMBER = string.Empty,
                    CHANGETIME_DB = DateTimeUtility.DateTimeNowCentral().AddDays(-1).ToString("MM-dd-yyyy hh:mm:ss:fff"), // "09-05-2024 09:43:17.787",
                    APP_RECEIVED_DATE = 20240905,
                },
                new PPOLCEvent
                {
                    POLICY_NUMBER = "987654321",
                    COMPANY_CODE = "01",
                    PRODUCT_CODE = "IH2004",
                    CONTRACT_REASON = "IC",
                    CONTRACT_CODE = "P",
                    CONTRACT_DATE = 20240901,
                    ISSUE_STATE = "NE",
                    RES_STATE = "NJ",
                    BILLING_MODE = 1,
                    BILLING_FORM = "DIR",
                    BILLING_DATE = 20240901,
                    ISSUE_DATE = 20240901,
                    PAID_TO_DATE = 20240901,
                    MODE_PREMIUM = 104.99m,
                    ANNUAL_PREMIUM = 1206.63m,
                    POLICY_BILL_DAY = 0,
                    POLC_SPECIAL_MODE = string.Empty,
                    LINE_OF_BUSINESS = "H",
                    BILLING_CODE = string.Empty,
                    BILLING_REASON = "MD",
                    PAYMENT_REASON = string.Empty,
                    TAX_QUALIFY_CODE = "0",
                    APPLICATION_DATE = 20240829,
                    GROUP_NUMBER = string.Empty,
                    CHANGETIME_DB = DateTimeUtility.DateTimeNowCentral().AddDays(-1).ToString("MM-dd-yyyy hh:mm:ss:fff"), // "09-05-2024 11:36:27.457",
                    APP_RECEIVED_DATE = 20240905
                }
            };

            policies.AddRange(MongoDbTestData.PoliciesForGetPoliciesAsyncTests);
            policyCollection.InsertMany(policies);
            policyHierarchyCollection.InsertMany(policyHierarchy);
            agentPolicyAccessCollection.InsertMany(agentPolicyAccess);
            ppolcEventsCollection.InsertMany(pPOLCEvents);
            requirementMappingCollection.InsertMany(RequirementMappingSeedData.GetRequirementMapping);
        }

        private Mock<IConfigurationManager> GetMockConfigurationManager()
        {
            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager
                .Setup(m => m.InitialPaymentDeclinedRetentionDays)
                .Returns(45);

            mockConfigurationManager
                .Setup(m => m.TerminationRetentionYears)
                .Returns(3);

            mockConfigurationManager
                .Setup(m => m.MongoDbDatabaseName)
                .Returns(DatabaseName);

            mockConfigurationManager
                .Setup(m => m.MongoPolicyCollectionName)
                .Returns(PolicyCollectionName);

            mockConfigurationManager
                .Setup(m => m.MongoPolicyHierarchyCollectionName)
                .Returns(PolicyHierarchyCollectionName);

            mockConfigurationManager
                .Setup(m => m.MongoPPOLCEventsCollectionName)
                .Returns(PPOLCCollectionName);

            mockConfigurationManager
                .Setup(m => m.MongoAgentPolicyAccessCollectionName)
                .Returns(AgentPolicyAccessCollectionName);

            mockConfigurationManager
                .Setup(m => m.MongoBenefitOptionsMappingCollectionName)
                .Returns(BenefitOptionsCollectionName);

            mockConfigurationManager
                .Setup(m => m.MongoRequirementMappingCollectionName)
                .Returns(RequirementMappingCollectionName);

            return mockConfigurationManager;
        }

        private EventsAccessor GetEventsAccessor()
        {
            return new EventsAccessor(mockLogger.Object, GetMockConfigurationManager().Object, client);
        }
    }
}