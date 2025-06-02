namespace Assurity.Kafka.Mappers.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Consumer.Mappers;
    using AutoMapper;
    using Avro;
    using Avro.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PPOLCMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PPOLCMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PPOLCMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_PPOLC_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," +
                "\"name\":\"PPOLCSchema\"," +
                "\"fields\":[{\"name\":\"COMPANY_CODE\",\"type\":\"string\"}," +
                "{\"name\":\"POLICY_NUMBER\",\"type\":\"string\"}," +
                "{\"name\":\"GROUP_NUMBER\",\"type\":\"string\"}," +
                "{\"name\":\"LINE_OF_BUSINESS\",\"type\":\"string\"}," +
                "{\"name\":\"PRODUCT_CODE\",\"type\":\"string\"}," +
                "{\"name\":\"CONTRACT_CODE\",\"type\":\"string\"}," +
                "{\"name\":\"CONTRACT_REASON\",\"type\":\"string\"}," +
                "{\"name\":\"CONTRACT_DATE\",\"type\":\"int\"}," +
                "{\"name\":\"BILLING_DATE\",\"type\":\"int\"}," +
                "{\"name\":\"ISSUE_DATE\",\"type\":\"int\"}," +
                "{\"name\":\"ISSUE_STATE\",\"type\":\"string\"}," +
                "{\"name\":\"RES_STATE\",\"type\":\"string\"}," +
                "{\"name\":\"BILLING_CODE\",\"type\":\"string\"}," +
                "{\"name\":\"BILLING_MODE\",\"type\":\"int\"}," +
                "{\"name\":\"BILLING_FORM\",\"type\":\"string\"}," +
                "{\"name\":\"BILLING_REASON\",\"type\":\"string\"}," +
                "{\"name\":\"PAID_TO_DATE\",\"type\":\"int\"}," +
                "{\"name\":\"POLICY_BILL_DAY\",\"type\":\"int\"}," +
                "{\"name\":\"MODE_PREMIUM\",\"type\":\"double\"}," +
                "{\"name\":\"ANNUAL_PREMIUM\",\"type\":\"double\"}," +
                "{\"name\":\"APPLICATION_DATE\",\"type\":\"int\"}," +
                "{\"name\":\"APP_RECEIVED_DATE\",\"type\":\"int\"}," +
                "{\"name\":\"POLC_SPECIAL_MODE\",\"type\":\"string\"}," +
                "{\"name\":\"PAYMENT_REASON\",\"type\":\"string\"}," +
                "{\"name\":\"TAX_QUALIFY_CODE\",\"type\":\"string\"}]}");

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add("COMPANY_CODE", "abc");
            genericRecord.Add("POLICY_NUMBER", "123");
            genericRecord.Add("GROUP_NUMBER", "456");
            genericRecord.Add("LINE_OF_BUSINESS", "A");
            genericRecord.Add("PRODUCT_CODE", "GH1715CI");
            genericRecord.Add("CONTRACT_CODE", "T");
            genericRecord.Add("CONTRACT_REASON", "CR");
            genericRecord.Add("CONTRACT_DATE", 20220101);
            genericRecord.Add("BILLING_DATE", 20220101);
            genericRecord.Add("ISSUE_DATE", 20220101);
            genericRecord.Add("ISSUE_STATE", "NY");
            genericRecord.Add("RES_STATE", "NY");
            genericRecord.Add("BILLING_CODE", "A");
            genericRecord.Add("BILLING_MODE", 1);
            genericRecord.Add("BILLING_FORM", "F1");
            genericRecord.Add("BILLING_REASON", "PC");
            genericRecord.Add("PAID_TO_DATE", 20220101);
            genericRecord.Add("POLICY_BILL_DAY", 1);
            genericRecord.Add("MODE_PREMIUM", 100.50);
            genericRecord.Add("ANNUAL_PREMIUM", 1200.00);
            genericRecord.Add("APPLICATION_DATE", 20220101);
            genericRecord.Add("APP_RECEIVED_DATE", 20220101);
            genericRecord.Add("POLC_SPECIAL_MODE", "A");
            genericRecord.Add("PAYMENT_REASON", "Q");
            genericRecord.Add("TAX_QUALIFY_CODE", "Z");

            // Act
            var ppolc = Mapper.Map<PPOLC>(genericRecord);

            // Assert
            Assert.AreEqual("abc", genericRecord["COMPANY_CODE"].ToString());
            Assert.AreEqual("123", genericRecord["POLICY_NUMBER"].ToString());
            Assert.AreEqual("456", genericRecord["GROUP_NUMBER"].ToString());
            Assert.AreEqual("A", genericRecord["LINE_OF_BUSINESS"].ToString());
            Assert.AreEqual("GH1715CI", genericRecord["PRODUCT_CODE"].ToString());
            Assert.AreEqual("T", genericRecord["CONTRACT_CODE"].ToString());
            Assert.AreEqual("CR", genericRecord["CONTRACT_REASON"].ToString());
            Assert.AreEqual(20220101, genericRecord["CONTRACT_DATE"]);
            Assert.AreEqual(20220101, genericRecord["BILLING_DATE"]);
            Assert.AreEqual(20220101, genericRecord["ISSUE_DATE"]);
            Assert.AreEqual("NY", genericRecord["ISSUE_STATE"].ToString());
            Assert.AreEqual("NY", genericRecord["RES_STATE"].ToString());
            Assert.AreEqual("A", genericRecord["BILLING_CODE"].ToString());
            Assert.AreEqual(1, genericRecord["BILLING_MODE"]);
            Assert.AreEqual("F1", genericRecord["BILLING_FORM"].ToString());
            Assert.AreEqual("PC", genericRecord["BILLING_REASON"].ToString());
            Assert.AreEqual(20220101, genericRecord["PAID_TO_DATE"]);
            Assert.AreEqual(1, genericRecord["POLICY_BILL_DAY"]);
            Assert.AreEqual(100.50, genericRecord["MODE_PREMIUM"]);
            Assert.AreEqual(1200.00, genericRecord["ANNUAL_PREMIUM"]);
            Assert.AreEqual(20220101, genericRecord["APPLICATION_DATE"]);
            Assert.AreEqual(20220101, genericRecord["APP_RECEIVED_DATE"]);
            Assert.AreEqual("A", genericRecord["POLC_SPECIAL_MODE"].ToString());
        }
    }
}
