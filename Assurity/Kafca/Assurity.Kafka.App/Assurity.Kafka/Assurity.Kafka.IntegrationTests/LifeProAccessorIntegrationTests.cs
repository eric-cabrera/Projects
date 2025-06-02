namespace Assurity.Kafka.IntegrationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Context;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.IntegrationTests.Config;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [Ignore]
    [ExcludeFromCodeCoverage]
    public class LifeProAccessorIntegrationTests
    {
        private IServiceProvider serviceProvider;

        [TestInitialize]
        public void InitializeTests()
        {
            var config = TestConfigurationManager.GetConfigurationRoot("appsettings.test.json");
            var lifeProConnectionString = config["ConnectionStrings:LifeProConnectionString"];

            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddDbContextFactory<LifeProContext>(dbContextOptionBuilder =>
                    dbContextOptionBuilder.UseSqlServer(lifeProConnectionString));

            serviceCollection.AddTransient<ILifeProAccessor, LifeProAccessor>();

            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        public async Task QueryIntegrationTests()
        {
            // Arrange
            var lifeProAccessor = serviceProvider.GetRequiredService<ILifeProAccessor>();

            // Act
            var agentRecords = await lifeProAccessor
                .GetAgents("01", "4750612114");

            // Assert
            Assert.IsNotNull(agentRecords);
        }

        [TestMethod]
        public async Task GetPolicyRelationships_Integration()
        {
            // Arrange
            var lifeProAccessor = serviceProvider.GetRequiredService<ILifeProAccessor>();

            // Act
            var policyRelationships = await lifeProAccessor
                .GetPolicyRelationships(1241566, 1598279);

            // Assert
            Assert.IsNotNull(policyRelationships);
        }

        [TestMethod]
        public async Task GetPRELA_RELATIONSHIP_MASTER_BENEFIT_ForBenefitUpdates_Integration()
        {
            // Arrange
            var lifeProAccessor = serviceProvider.GetRequiredService<ILifeProAccessor>();

            // Act
            var prela = await lifeProAccessor
                .GetPRELA_RELATIONSHIP_MASTER_BENEFIT(
                    "4150823859", 2218183, "ML", 4);

            // Assert
            Assert.IsNotNull(prela);
            Assert.AreEqual("014150823859", prela.IDENTIFYING_ALPHA.Trim());
            Assert.AreEqual(2218183, prela.NAME_ID);
            Assert.AreEqual("ML", prela.RELATE_CODE);
            Assert.AreEqual(4, prela.BENEFIT_SEQ_NUMBER);
        }

        [TestMethod]
        public async Task GetPolicyRequirements_Integration()
        {
            // Arrange
            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
            {
                CompanyCode = "01",
                PolicyNumber = "4150823859"
            };

            var lifeProAccessor = serviceProvider.GetRequiredService<ILifeProAccessor>();

            // Act
            var policyRequirements = await lifeProAccessor
                .GetPolicyRequirementsForHealth(companyCodeAndPolicyNumber);

            // Assert
            Assert.IsNotNull(policyRequirements);

            var hasRequirementsWithEmptyDescriptions = policyRequirements
                .Any(policyRequirement => string.IsNullOrWhiteSpace(policyRequirement.Description));

            Assert.IsFalse(hasRequirementsWithEmptyDescriptions);
        }
    }
}