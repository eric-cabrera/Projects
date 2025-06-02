namespace Assurity.Kafka.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Context;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Hierarchy;
    using Assurity.Kafka.Engines.Mapping;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.IntegrationTests.Config;
    using Assurity.Kafka.Utilities.Constants;
    using Assurity.Kafka.Utilities.Enums;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MongoDB.Driver;
    using Moq;

    [TestClass]
    [Ignore]
    [ExcludeFromCodeCoverage]
    public class PolicyEngineIntegrationTests
    {
        private IServiceProvider serviceProvider;
        private Mock<ILogger<ConsumerPolicyEngine>> mockLogger = new Mock<ILogger<ConsumerPolicyEngine>>();

        [TestInitialize]
        public void InitializeTests()
        {
            var config = TestConfigurationManager.GetConfigurationRoot("appsettings.test.json");
            var lifeproConnectionString = config["ConnectionStrings:LifeProConnectionString"];
            var dataStoreConnectionString = config["ConnectionStrings:DataStoreConnectionString"];
            var globalDataConnectionString = config["ConnectionStrings:GlobalDataConnectionString"];
            var supportDataConnectionString = config["ConnectionStrings:SupportDataConnectionString"];
            var eventsConnectionString = config["Cache:MongoDbConnectionString"];

            var mongoSettings = MongoClientSettings.FromConnectionString(eventsConnectionString);
            var dbClient = new MongoClient(mongoSettings);
            var database = dbClient.GetDatabase(config["Cache:MongoDbDatabaseName"]);
            var requirementMappingCollection = database.GetCollection<RequirementMapping>(config["Cache:MongoDbRequirementMappingCollectionName"]);

            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddDbContextFactory<LifeProContext>(dbContextOptionBuilder =>
                    dbContextOptionBuilder.UseSqlServer(lifeproConnectionString));

            serviceCollection
                .AddDbContextFactory<DataStoreContext>(dbContextOptionBuilder =>
                    dbContextOptionBuilder.UseSqlServer(dataStoreConnectionString));

            serviceCollection
                .AddDbContextFactory<GlobalDataContext>(dbContextOptionBuilder =>
                    dbContextOptionBuilder.UseSqlServer(globalDataConnectionString));

            serviceCollection
                .AddDbContextFactory<SupportDataContext>(dbContextOptionBuilder =>
                    dbContextOptionBuilder.UseSqlServer(supportDataConnectionString));

            serviceCollection.AddTransient<ILifeProAccessor, LifeProAccessor>();
            serviceCollection.AddTransient<IDataStoreAccessor, DataStoreAccessor>();
            serviceCollection.AddTransient<IGlobalDataAccessor, GlobalDataAccessor>();
            serviceCollection.AddTransient<ISupportDataAccessor, SupportDataAccessor>();
            serviceCollection.AddTransient<IBenefitMapper, BenefitMapper>();
            serviceCollection.AddTransient<IParticipantMapper, ParticipantMapper>();
            serviceCollection.AddTransient<IRequirementsMapper, RequirementsMapper>();
            serviceCollection.AddTransient<IPolicyMapper, PolicyMapper>();

            serviceProvider = serviceCollection.AddLogging().BuildServiceProvider();
        }

        [TestMethod]
        public async Task TestGetPolicy_Given_PolicyNumber_Found()
        {
            var lifeProAccessor = serviceProvider.GetRequiredService<ILifeProAccessor>();
            var globalDataAccessor = serviceProvider.GetRequiredService<IGlobalDataAccessor>();
            var benefitMapper = serviceProvider.GetRequiredService<IBenefitMapper>();
            var requirementsMapper = serviceProvider.GetRequiredService<IRequirementsMapper>();
            var participantMapper = new ParticipantMapper();
            var policyMapper = new PolicyMapper(new BenefitMapper(), participantMapper, new RequirementsMapper(participantMapper));

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(m => m.GetRequirementMappings(It.IsAny<List<int>>()))
                .Returns(new List<RequirementMapping> { new RequirementMapping() });

            var policyEngine = new ConsumerPolicyEngine(
                lifeProAccessor,
                globalDataAccessor,
                null,
                mockEventsAccessor.Object,
                null,
                mockLogger.Object,
                policyMapper,
                null);

            (var result, var policy) = await policyEngine.GetPolicy("4750252332", "01");

            Assert.IsNotNull(policy);
            var expected = GetExpectedPolicy();

            Assert.AreEqual(result, GetPolicyResult.Found);
        }

        [TestMethod]
        public async Task TestGetPolicy_Given_PolicyNumber_NotFound()
        {
            var lifeProAccessor = serviceProvider.GetRequiredService<ILifeProAccessor>();

            var policyEngine = new ConsumerPolicyEngine(
                lifeProAccessor,
                null,
                null,
                null,
                null,
                mockLogger.Object,
                null,
                null);

            (var result, var policy) = await policyEngine.GetPolicy("001", "01");

            Assert.AreEqual(result, GetPolicyResult.NotFound);
            Assert.IsNull(policy);
        }

        [TestMethod]
        public async Task PolicyEngine_GetPolicyHierarchyWithJustInTimeAgents()
        {
            // This test pulls live data from GlobalData and SupportData.  It is possible this data could change and cause this test to fail.
            var policy = new Policy
            {
                PolicyNumber = "4150000090",
                CompanyCode = "01",
                ApplicationDate = new DateTime(2004, 09, 28),
                Agents = new List<Agent>
                {
                    new Agent
                    {
                        AgentId = AgentType.Z9Agent,
                        IsServicingAgent = true,
                        IsWritingAgent = false,
                    },
                },
            };

            var dataStoreAccessor = serviceProvider.GetRequiredService<IDataStoreAccessor>();
            var globalDataAccessor = serviceProvider.GetRequiredService<IGlobalDataAccessor>();
            var supportDataAccessor = serviceProvider.GetRequiredService<ISupportDataAccessor>();

            var hierarchyEngine = new MigrateHierarchyEngine(
                dataStoreAccessor,
                globalDataAccessor,
                supportDataAccessor);

            var hierarchy = await hierarchyEngine.GetPolicyHierarchy(policy);

            Assert.IsNotNull(hierarchy);
        }

        private Policy GetExpectedPolicy()
        {
            return new Policy
            {
                CompanyCode = "01",
                PolicyNumber = "4750612114",
                ProductCode = "W H1101   ",
                PolicyStatus = Status.Terminated,
                PolicyStatusReason = StatusReason.Lapsed,
                IssueState = State.LA,
                ResidentState = State.LA,
                BillingMode = BillingMode.FiftyTwoPay,
                BillingForm = BillingForm.Direct,
                IssueDate = new DateTime(2014, 06, 01),
                PaidToDate = new DateTime(2014, 10, 19),
                SubmitDate = DateTime.Now,
                ModePremium = 66.16M,
                AnnualPremium = 264.63M,
                BillingDay = 0,
                ProductCategory = "Accident Expense",
                ProductDescription = "Ind. PRO 24-hour Accident Expense",
                Owners = new List<Owner>()
                {
                    new Owner()
                    {
                        OwnerType = OwnerType.Primary,
                        Participant = new Participant()
                        {
                            IsBusiness = false,
                            Person = new Person()
                            {
                                Gender = Gender.Male,
                                EmailAddress = string.Empty,
                                DateOfBirth = new DateTime(1993, 05, 07),
                                Name = new Name()
                                {
                                    NameId = 1775381,
                                    BusinessName = null,
                                    IndividualPrefix = string.Empty,
                                    IndividualFirst = "JUAN",
                                    IndividualMiddle = "A",
                                    IndividualLast = "SILVA",
                                    IndividualSuffix = string.Empty,
                                },
                            },
                            Address = new Address()
                            {
                                AddressId = 8675309,
                                Line1 = "8815 G S R I AVE APT 1",
                                Line2 = string.Empty,
                                Line3 = string.Empty,
                                City = "BATON ROUGE",
                                StateAbbreviation = State.LA,
                                ZipCode = "70810",
                                ZipExtension = string.Empty,
                                BoxNumber = "6275",
                                Country = Country.USA,
                            },
                        },
                    },
                },
                Insureds = new List<Insured>()
                {
                    new Insured()
                    {
                        RelationshipToPrimaryInsured = RelationshipToPrimaryInsured.Self,
                        Participant = new Participant()
                        {
                            IsBusiness = false,
                            Person = new Person()
                            {
                                Gender = Gender.Male,
                                EmailAddress = string.Empty,
                                DateOfBirth = new DateTime(1993, 05, 07),
                                Name = new Name()
                                {
                                    NameId = 1775381,
                                    BusinessName = null,
                                    IndividualPrefix = string.Empty,
                                    IndividualFirst = "JUAN",
                                    IndividualMiddle = "A",
                                    IndividualLast = "SILVA",
                                    IndividualSuffix = string.Empty,
                                },
                            },
                            Address = new Address()
                            {
                                AddressId = 8675309,
                                Line1 = "8815 G S R I AVE APT 1",
                                Line2 = string.Empty,
                                Line3 = string.Empty,
                                City = "BATON ROUGE",
                                StateAbbreviation = State.LA,
                                ZipCode = "70810",
                                ZipExtension = string.Empty,
                                BoxNumber = "6275",
                                Country = Country.USA,
                            },
                        },
                    },
                },
                Payors = new List<Payor>()
                {
                    new Payor()
                    {
                        PayorType = PayorType.Primary,
                        Participant = new Participant()
                        {
                            IsBusiness = false,
                            Person = new Person()
                            {
                                Gender = Gender.Male,
                                EmailAddress = string.Empty,
                                DateOfBirth = new DateTime(1993, 05, 07),
                                Name = new Name()
                                {
                                    NameId = 1775381,
                                    BusinessName = null,
                                    IndividualPrefix = string.Empty,
                                    IndividualFirst = "JUAN",
                                    IndividualMiddle = "A",
                                    IndividualLast = "SILVA",
                                    IndividualSuffix = string.Empty,
                                },
                            },
                            Address = new Address()
                            {
                                AddressId = 8675309,
                                Line1 = "8815 G S R I AVE APT 1",
                                Line2 = string.Empty,
                                Line3 = string.Empty,
                                City = "BATON ROUGE",
                                StateAbbreviation = State.LA,
                                ZipCode = "70810",
                                ZipExtension = string.Empty,
                                BoxNumber = "6275",
                                Country = Country.USA,
                            },
                        },
                    },
                },
                Agents = new List<Agent>()
                {
                    // NOTE - at runtime this may change order
                    // and cause a failure of test checking on it.
                    new Agent()
                    {
                        IsServicingAgent = false,
                        IsWritingAgent = true,
                        AgentId = "2ZVR",
                        MarketCode = "WSR5",
                        Level = "01",
                    },
                    new Agent()
                    {
                        IsServicingAgent = true,
                        IsWritingAgent = false,
                        AgentId = "7H80",
                        MarketCode = "WS-1",
                        Level = "90",
                    },
                },
                Benefits = new List<Benefit>()
                {
                    new Benefit()
                    {
                        CoverageType = CoverageType.Base,
                        BenefitId = 2580405,
                        PlanCode = "W H1101   ",
                        BenefitDescription = "Ind. PRO 24-hour Accident Expense",
                        BenefitStatus = Status.Terminated,
                        BenefitStatusReason = StatusReason.Lapsed,
                        BenefitAmount = 223.6300000000M,
                        BenefitOptions = new List<BenefitOption>()
                    },
                    new Benefit()
                    {
                        CoverageType = CoverageType.Rider,
                        BenefitId = 2580406,
                        PlanCode = "W H1101CC ",
                        BenefitDescription = "Ind. PRO 24-hour Accident Expense",
                        BenefitStatus = Status.Terminated,
                        BenefitStatusReason = StatusReason.Lapsed,
                        BenefitAmount = 0.0000000000M,
                        BenefitOptions = new List<BenefitOption>()
                    },
                    new Benefit()
                    {
                        CoverageType = CoverageType.Rider,
                        BenefitId = 2580407,
                        PlanCode = "R W1110   ",
                        BenefitDescription = "Ind. PRO 24-hour Accident Expense",
                        BenefitStatus = Status.Terminated,
                        BenefitStatusReason = StatusReason.Lapsed,
                        BenefitAmount = 41.0000000000M,
                        BenefitOptions = new List<BenefitOption>()
                    },
                },
            };
        }
    }
}