namespace Assurity.Kafka.Accessors.Configuration
{
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Accessors.Extensions;
    using Assurity.Kafka.Utilities.Config;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.Extensions.DependencyInjection;
    using MongoDB.Driver;

    public static class MongoDbConfiguration
    {
        public static MongoClient ConfigureAndPopulateMongoDB(
            IServiceCollection services,
            IConfigurationManager configurationManager)
        {
            string connString = configurationManager.MongoDbConnectionString;

            var mongoSettings = MongoClientSettings.FromConnectionString(connString);

            if (configurationManager.UseTls == "true")
            {
                mongoSettings.UseTls = true;
                SetSslSettings(mongoSettings, configurationManager);
            }

            var dbClient = new MongoClient(mongoSettings);
            var database = dbClient.GetDatabase(configurationManager.MongoDbDatabaseName);

            var policyCollection = database.GetCollection<Policy>(configurationManager.MongoPolicyCollectionName);
            CreateMongoDbPoliciesIndexesBulk(policyCollection);

            var policyHierarchyCollection = database.GetCollection<PolicyHierarchy>(configurationManager.MongoPolicyHierarchyCollectionName);
            CreateMongoDbPolicyHierarchyIndexes(policyHierarchyCollection);

            var agentPolicyAccessCollection = database.GetCollection<AgentPolicyAccess>(configurationManager.MongoAgentPolicyAccessCollectionName);
            CreateMongoDbAgentPolicyAccessIndexes(agentPolicyAccessCollection);

            var requirementMappingCollection = database.GetCollection<RequirementMapping>(configurationManager.MongoRequirementMappingCollectionName);
            requirementMappingCollection.DeleteMany(Builders<RequirementMapping>.Filter.Empty);
            requirementMappingCollection.InsertMany(RequirementMappingSeedData.GetRequirementMapping);

            var benefitOptionsMappingCollection = database.GetCollection<BenefitOptionsMapping>(configurationManager.MongoBenefitOptionsMappingCollectionName);
            benefitOptionsMappingCollection.DeleteMany(Builders<BenefitOptionsMapping>.Filter.Empty);
            benefitOptionsMappingCollection.InsertMany(BenefitOptionsMappingSeedData.GetBenefitOptionsMapping);

            services.AddSingleton<IMongoClient>(dbClient);

            return new MongoClient(mongoSettings);
        }

        private static void SetSslSettings(MongoClientSettings mongoSettings, IConfigurationManager configurationManager)
        {
            var clientCerts = new List<X509Certificate2>();
            try
            {
                SetWindowsCertificate(clientCerts);
            }
            catch (Exception)
            {
                clientCerts = SetLinuxCertificate(configurationManager);
            }

            mongoSettings.SslSettings = new SslSettings
            {
                CheckCertificateRevocation = false,
                EnabledSslProtocols = SslProtocols.Tls12,
                ClientCertificates = clientCerts
            };
        }

        private static void SetWindowsCertificate(List<X509Certificate2> clientCerts)
        {
            using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine, OpenFlags.ReadOnly))
            {
                // Add all certificates from Personal certificate store. MongoDB selects the right one based on the database server
                clientCerts.AddRange(store.Certificates.OfType<X509Certificate2>());
            }
        }

        private static List<X509Certificate2> SetLinuxCertificate(IConfigurationManager configurationManager)
        {
            return new List<X509Certificate2>()
            {
                new X509Certificate2("/etc/pki/tls/private/mongoClient.pfx", configurationManager.MongoDbClientCertificatePassword)
            };
        }

        private static void CreateMongoDbPoliciesIndexesBulk(IMongoCollection<Policy> policyCollection)
        {
            var policyIndexes = new List<CreateIndexModel<Policy>>();

            var policyNumberIndexModel = new CreateIndexModel<Policy>(
                Builders<Policy>.IndexKeys.Ascending(i => i.PolicyNumber),
                new CreateIndexOptions
                {
                    Unique = true,
                    Name = "PolicyNumber"
                });
            policyIndexes.Add(policyNumberIndexModel);

            var employerNumberIndexModel = new CreateIndexModel<Policy>(
                Builders<Policy>.IndexKeys.Ascending(i => i.Employer.Number),
                new CreateIndexOptions
                {
                    Sparse = true,
                    Name = "EmployerNumber"
                });
            policyIndexes.Add(employerNumberIndexModel);

            policyCollection.Indexes.CreateMany(policyIndexes);
        }

        private static void CreateMongoDbPolicyHierarchyIndexes(IMongoCollection<PolicyHierarchy> policyHierarchyCollection)
        {
            var policyHierarchyPolicyNumberIndexModel =
                new CreateIndexModel<PolicyHierarchy>(Builders<PolicyHierarchy>.IndexKeys.Ascending(i => i.PolicyNumber), new CreateIndexOptions { Unique = true, Name = "PolicyNumber" });

            policyHierarchyCollection.Indexes.CreateOne(policyHierarchyPolicyNumberIndexModel);
        }

        private static void CreateMongoDbAgentPolicyAccessIndexes(IMongoCollection<AgentPolicyAccess> agentPolicyAccessCollection)
        {
            var agentPolicyAccessAgentIdIndexModel =
                new CreateIndexModel<AgentPolicyAccess>(Builders<AgentPolicyAccess>.IndexKeys.Ascending(i => i.AgentId), new CreateIndexOptions { Unique = true, Name = "AgentId" });

            agentPolicyAccessCollection.Indexes.CreateOne(agentPolicyAccessAgentIdIndexModel);
        }
    }
}
