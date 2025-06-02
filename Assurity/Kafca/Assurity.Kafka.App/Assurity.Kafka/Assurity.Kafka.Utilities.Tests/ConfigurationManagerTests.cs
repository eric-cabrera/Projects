namespace Assurity.Kafka.Utilities.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ConfigurationManagerTests
    {
        private const string PPOLCValue = "d1yn1PPOLC_EVENT";
        private const string PNAMEValue = "d1yn1PNAME_EVENT";
        private const string PADDRValue = "d1yn1PADDR_EVENT";
        private const string PPEND_NEW_BUSINESS_PENDINGValue = "d1yn1PPEND_NEW_BUSINESS_PENDING_EVENT";
        private const string PRELA_RELATIONSHIP_MASTERValue = "d1yn1PRELA_RELATIONSHIP_MASTER_EVENT";
        private const string PCOMC_COMMISSION_CONTROL_TYPE_SValue = "d1yn1PCOMC_COMMISSION_CONTROL_TYPE_S_EVENT";
        private const string PCOMC_COMMISSION_CONTROLValue = "d1yn1PCOMC_COMMISSION_CONTROL_EVENT";
        private const string PNALKValue = "d1yn1PNALK_EVENT";
        private const string PPBEN_POLICY_BENEFITSValue = "d1yn1PPBEN_POLICY_BENEFITS_EVENT";
        private const string PPBEN_POLICY_BENEFITS_TYPES_BA_ORValue = "d1yn1PPBEN_POLICY_BENEFITS_TYPES_BA_OR_EVENT";
        private const string PPBEN_POLICY_BENEFITS_TYPES_BFValue = "d1yn1PPBEN_POLICY_BENEFITS_TYPES_BF_EVENT";
        private const string ProductDescriptionValue = "d1yn1PProductDescription_EVENT";

        [TestMethod]
        public void Test_GroupId_Basic()
        {
            // Arrange
            const string groupName = "groupIdName";

            var configurationBuilder =
                new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "Kafka:GroupId", groupName }
                    });

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.KafkaGroupId;

            // Assert
            Assert.AreEqual(groupName, value);
        }

        [TestMethod]
        public void Test_GetTopics_All()
        {
            // Arrange
            const string topicsString = "Topic.PPOLC,"
                + "Topic.PNAME,"
                + "Topic.PADDR,"
                + "Topic.PPEND_NEW_BUSINESS_PENDING,"
                + "Topic.PRELA_RELATIONSHIP_MASTER,"
                + "Topic.PCOMC_COMMISSION_CONTROL_TYPE_S,"
                + "Topic.PCOMC_COMMISSION_CONTROL,"
                + "Topic.PNALK,"
                + "Topic.PPBEN_POLICY_BENEFITS,"
                + "Topic.PPBEN_POLICY_BENEFITS_TYPES_BA_OR,"
                + "Topic.PPBEN_POLICY_BENEFITS_TYPES_BF,"
                + "Topic.ProductDescription";

            var configurationBuilder = SetConfigedValues(topicsString);

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.GetTopics();

            // Assert
            Assert.AreEqual(12, value.Count);
            Assert.IsTrue(value.Contains(PPOLCValue));
            Assert.IsTrue(value.Contains(PNAMEValue));
            Assert.IsTrue(value.Contains(PADDRValue));
            Assert.IsTrue(value.Contains(PPEND_NEW_BUSINESS_PENDINGValue));
            Assert.IsTrue(value.Contains(PRELA_RELATIONSHIP_MASTERValue));
            Assert.IsTrue(value.Contains(PCOMC_COMMISSION_CONTROL_TYPE_SValue));
            Assert.IsTrue(value.Contains(PCOMC_COMMISSION_CONTROLValue));
            Assert.IsTrue(value.Contains(PNALKValue));
            Assert.IsTrue(value.Contains(PPBEN_POLICY_BENEFITSValue));
            Assert.IsTrue(value.Contains(PPBEN_POLICY_BENEFITS_TYPES_BA_ORValue));
            Assert.IsTrue(value.Contains(PPBEN_POLICY_BENEFITS_TYPES_BFValue));
            Assert.IsTrue(value.Contains(ProductDescriptionValue));
        }

        [TestMethod]
        public void Test_GetTopics_InvalidTopic()
        {
            // Arrange
            const string topicsString = "Topic.PPOLC,"
                + "Topic.PNAME,"
                + "Topic.PADDR,"
                + "Topic.Invalid,"
                + "Topic.PRELA_RELATIONSHIP_MASTER,"
                + "Topic.PCOMC_COMMISSION_CONTROL_TYPE_S,"
                + "Topic.PCOMC_COMMISSION_CONTROL,"
                + "Topic.PNALK,"
                + "Topic.PPBEN_POLICY_BENEFITS,"
                + "Topic.PPBEN_POLICY_BENEFITS_TYPES_BA_OR,"
                + "Topic.PPBEN_POLICY_BENEFITS_TYPES_BF,"
                + "Topic.ProductDescription";

            var configurationBuilder = SetConfigedValues(topicsString);

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.GetTopics();

            // Assert
            Assert.AreEqual(11, value.Count);
            Assert.IsTrue(value.Contains(PPOLCValue));
            Assert.IsTrue(value.Contains(PNAMEValue));
            Assert.IsTrue(value.Contains(PADDRValue));
            Assert.IsFalse(value.Contains(PPEND_NEW_BUSINESS_PENDINGValue));
            Assert.IsTrue(value.Contains(PRELA_RELATIONSHIP_MASTERValue));
            Assert.IsTrue(value.Contains(PCOMC_COMMISSION_CONTROL_TYPE_SValue));
            Assert.IsTrue(value.Contains(PCOMC_COMMISSION_CONTROLValue));
            Assert.IsTrue(value.Contains(PNALKValue));
            Assert.IsTrue(value.Contains(PPBEN_POLICY_BENEFITSValue));
            Assert.IsTrue(value.Contains(PPBEN_POLICY_BENEFITS_TYPES_BA_ORValue));
            Assert.IsTrue(value.Contains(PPBEN_POLICY_BENEFITS_TYPES_BFValue));
            Assert.IsTrue(value.Contains(ProductDescriptionValue));
        }

        [TestMethod]
        public void Test_GetTopics_PPBEN_Mixed()
        {
            // Arrange
            const string topicsString = "Topic.PPBEN_POLICY_BENEFITS_TYPES_BA_OR,"
                + "Topic.PPBEN_POLICY_BENEFITS,";

            var configurationBuilder = SetConfigedValues(topicsString);

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.GetTopics();

            // Assert
            Assert.AreEqual(2, value.Count);
            Assert.IsFalse(value.Contains(PPOLCValue));
            Assert.IsFalse(value.Contains(PNAMEValue));
            Assert.IsFalse(value.Contains(PADDRValue));
            Assert.IsFalse(value.Contains(PPEND_NEW_BUSINESS_PENDINGValue));
            Assert.IsFalse(value.Contains(PRELA_RELATIONSHIP_MASTERValue));
            Assert.IsFalse(value.Contains(PCOMC_COMMISSION_CONTROL_TYPE_SValue));
            Assert.IsFalse(value.Contains(PCOMC_COMMISSION_CONTROLValue));
            Assert.IsFalse(value.Contains(PNALKValue));
            Assert.IsTrue(value.Contains(PPBEN_POLICY_BENEFITSValue));
            Assert.IsTrue(value.Contains(PPBEN_POLICY_BENEFITS_TYPES_BA_ORValue));
            Assert.IsFalse(value.Contains(PPBEN_POLICY_BENEFITS_TYPES_BFValue));
            Assert.IsFalse(value.Contains(ProductDescriptionValue));
        }

        [TestMethod]
        public void Test_GetTopics_PCOM_Mixed()
        {
            // Arrange
            const string topicsString = "Topic.PCOMC_COMMISSION_CONTROL,"
                + "Topic.PCOMC_COMMISSION_CONTROL_TYPE_S,";
            IConfigurationBuilder configurationBuilder = SetConfigedValues(topicsString);

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.GetTopics();

            // Assert
            Assert.AreEqual(2, value.Count);
            Assert.IsFalse(value.Contains(PPOLCValue));
            Assert.IsFalse(value.Contains(PNAMEValue));
            Assert.IsFalse(value.Contains(PADDRValue));
            Assert.IsFalse(value.Contains(PPEND_NEW_BUSINESS_PENDINGValue));
            Assert.IsFalse(value.Contains(PRELA_RELATIONSHIP_MASTERValue));
            Assert.IsTrue(value.Contains(PCOMC_COMMISSION_CONTROL_TYPE_SValue));
            Assert.IsTrue(value.Contains(PCOMC_COMMISSION_CONTROLValue));
            Assert.IsFalse(value.Contains(PNALKValue));
            Assert.IsFalse(value.Contains(PPBEN_POLICY_BENEFITSValue));
            Assert.IsFalse(value.Contains(PPBEN_POLICY_BENEFITS_TYPES_BA_ORValue));
            Assert.IsFalse(value.Contains(PPBEN_POLICY_BENEFITS_TYPES_BFValue));
            Assert.IsFalse(value.Contains(ProductDescriptionValue));
        }

        [TestMethod]
        public void Test_KafkaDebug_VarMissing_ShouldBeNull()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>());

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.KafkaDebug;

            // Assert
            Assert.IsNull(value);
        }

        [TestMethod]
        public void Test_KafkaDebug_DefaultValue()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                   { "Kafka:KafkaDebug", string.Empty },
                });

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.KafkaDebug;

            // Assert
            Assert.IsNull(value);
        }

        [TestMethod]
        public void Test_KafkaDebug_CustomValue()
        {
            // Arrange
            var customValue = "debug";

            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Kafka:KafkaDebug", customValue },
                });

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.KafkaDebug;

            // Assert
            Assert.AreEqual(customValue, value);
        }

        [TestMethod]
        public void Test_KafkaFetchMaxBytes_VarMissing_ShouldBeDefault()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                { });

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.KafkaFetchMaxBytes;

            // Assert
            Assert.AreEqual(52428800, value);
        }

        [TestMethod]
        public void Test_KafkaFetchMaxBytes_DefaultValue()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Kafka:KafkaFetchMaxBytes", null },
                });

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.KafkaFetchMaxBytes;

            // Assert
            Assert.AreEqual(52428800, value);
        }

        [TestMethod]
        public void Test_KafkaFetchMaxBytes_CustomValue()
        {
            // Arrange
            var customValue = "1000000";

            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Kafka:KafkaFetchMaxBytes", customValue },
                });

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.KafkaFetchMaxBytes;

            // Assert
            Assert.AreEqual(int.Parse(customValue), value);
        }

        [TestMethod]
        public void Test_KafkaMaxPartitionFetchBytes_VarMissing_ShouldBeDefault()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>());

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.KafkaMaxPartitionFetchBytes;

            // Assert
            Assert.AreEqual(1048576, value);
        }

        [TestMethod]
        public void Test_KafkaMaxPartitionFetchBytes_DefaultValue()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Kafka:KafkaMaxPartitionFetchBytes", null },
                });

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.KafkaMaxPartitionFetchBytes;

            // Assert
            Assert.AreEqual(1048576, value);
        }

        [TestMethod]
        public void Test_KafkaMaxPartitionFetchBytes_CustomValue()
        {
            // Arrange
            var customValue = "500000";

            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Kafka:KafkaMaxPartitionFetchBytes", customValue },
                });

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.KafkaMaxPartitionFetchBytes;

            // Assert
            Assert.AreEqual(int.Parse(customValue), value);
        }

        [TestMethod]
        public void Test_KafkaQueuedMaxMessagesKilobytes_VarMissing_ShouldBeDefault()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>());

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.KafkaQueuedMaxMessagesKilobytes;

            // Assert
            Assert.AreEqual(65536, value);
        }

        [TestMethod]
        public void Test_KafkaQueuedMaxMessagesKilobytes_DefaultValue()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Kafka:KafkaQueuedMaxMessagesKilobytes", null },
                });

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.KafkaQueuedMaxMessagesKilobytes;

            // Assert
            Assert.AreEqual(65536, value);
        }

        [TestMethod]
        public void Test_KafkaQueuedMaxMessagesKilobytes_CustomValue()
        {
            // Arrange
            var customValue = "100";

            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Kafka:KafkaQueuedMaxMessagesKilobytes", customValue },
                });

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.KafkaQueuedMaxMessagesKilobytes;

            // Assert
            Assert.AreEqual(int.Parse(customValue), value);
        }

        [TestMethod]
        public void Test_KafkaMessageMaxBytes_VarMissing_ShouldBeDefault()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>());

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.KafkaMessageMaxBytes;

            // Assert
            Assert.AreEqual(1000000, value);
        }

        [TestMethod]
        public void Test_KafkaMessageMaxBytes_DefaultValue()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Kafka:KafkaQueuedMaxMessagesKilobytes", null },
                });

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.KafkaMessageMaxBytes;

            // Assert
            Assert.AreEqual(1000000, value);
        }

        [TestMethod]
        public void Test_KafkaMessageMaxBytes_CustomValue()
        {
            // Arrange
            var customValue = "500000";

            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Kafka:KafkaMessageMaxBytes", customValue },
                });

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.KafkaMessageMaxBytes;

            // Assert
            Assert.AreEqual(int.Parse(customValue), value);
        }

        [TestMethod]
        public void Test_LogOffsetInfoIntervalSec_VarMissing_ShouldBeDefault()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>());

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.LogOffsetInfoIntervalSec;

            // Assert
            Assert.AreEqual(120, value);
        }

        [TestMethod]
        public void Test_LogOffsetInfoIntervalSec_DefaultValue()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Kafka:LogOffsetInfoIntervalSec", null },
                });

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.LogOffsetInfoIntervalSec;

            // Assert
            Assert.AreEqual(120, value);
        }

        [TestMethod]
        public void Test_LogOffsetInfoIntervalSec_CustomValue()
        {
            // Arrange
            var customValue = "60";

            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Kafka:LogOffsetInfoIntervalSec", customValue },
                });

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.LogOffsetInfoIntervalSec;

            // Assert
            Assert.AreEqual(double.Parse(customValue), value);
        }

        [TestMethod]
        public void MongoDbConnectionString_DEV_ShouldDecrypt()
        {
            // Arrange
            const string encryptedConnectionString = "EAAAAM8KIknIGThG0t775a5C6YPS4bnBr4DZs6HPvnYDSGz1bGLraCUD7dnmDCWkko0otQ==";
            const string plainConnectionString = "PlainConnectionString";

            var configurationBuilder =
                new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "Cache:MongoDbConnectionString", encryptedConnectionString },
                        { "Environment", "Dev" }
                    });

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.MongoDbConnectionString;

            // Assert
            Assert.AreNotEqual(encryptedConnectionString, value);
            Assert.AreEqual(plainConnectionString, value);
        }

        [TestMethod]
        public void MongoDbConnectionString_LOCAL_ShouldDecrypt()
        {
            // Arrange
            const string plainConnectionString = "PlainConnectionString";

            var configurationBuilder =
                new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "Cache:MongoDbConnectionString", plainConnectionString },
                        { "Environment", "local" }
                    });

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.MongoDbConnectionString;

            // Assert
            Assert.AreEqual(plainConnectionString, value);
        }

        [TestMethod]
        public void MongoDbClientCertificatePassword_LOCAL_ShouldDecrypt()
        {
            // Arrange
            const string plainPasswordString = "PlainPasswordString";

            var configurationBuilder =
                new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "Cache:MongoDbClientCertificatePassword", plainPasswordString },
                        { "Environment", "local" }
                    });

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.MongoDbClientCertificatePassword;

            // Assert
            Assert.AreEqual(plainPasswordString, value);
        }

        [TestMethod]
        public void MongoDbClientCertificatePassword_DEV_ShouldDecrypt()
        {
            // Arrange
            const string encryptedPasswordString = "EAAAAGPwQkbYXopLQ8lv5QH6VtLI01mFdRLjOyFXDJ1oA4MPgHlTSXhjzNb4LhJN1RVHdQ==";
            const string plainPasswordString = "PlainPasswordString";

            var configurationBuilder =
                new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "Cache:MongoDbClientCertificatePassword", encryptedPasswordString },
                        { "Environment", "Dev" }
                    });

            var configurationManager = new Utilities.Config.ConfigurationManager(configurationBuilder.Build());

            // Act
            var value = configurationManager.MongoDbClientCertificatePassword;

            // Assert
            Assert.AreNotEqual(encryptedPasswordString, value);
            Assert.AreEqual(plainPasswordString, value);
        }

        private static IConfigurationBuilder SetConfigedValues(string topicsString)
        {
            return new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "Kafka:Topics", topicsString },
                        { "Kafka:Topic.PPOLC", PPOLCValue },
                        { "Kafka:Topic.PNAME", PNAMEValue },
                        { "Kafka:Topic.PADDR", PADDRValue },
                        { "Kafka:Topic.PPEND_NEW_BUSINESS_PENDING", PPEND_NEW_BUSINESS_PENDINGValue },
                        { "Kafka:Topic.PRELA_RELATIONSHIP_MASTER", PRELA_RELATIONSHIP_MASTERValue },
                        { "Kafka:Topic.PCOMC_COMMISSION_CONTROL_TYPE_S", PCOMC_COMMISSION_CONTROL_TYPE_SValue },
                        { "Kafka:Topic.PCOMC_COMMISSION_CONTROL", PCOMC_COMMISSION_CONTROLValue },
                        { "Kafka:Topic.PNALK", PNALKValue },
                        { "Kafka:Topic.PPBEN_POLICY_BENEFITS", PPBEN_POLICY_BENEFITSValue },
                        { "Kafka:Topic.PPBEN_POLICY_BENEFITS_TYPES_BA_OR", PPBEN_POLICY_BENEFITS_TYPES_BA_ORValue },
                        { "Kafka:Topic.PPBEN_POLICY_BENEFITS_TYPES_BF", PPBEN_POLICY_BENEFITS_TYPES_BFValue },
                        { "Kafka:Topic.ProductDescription", ProductDescriptionValue },
                    });
        }
    }
}