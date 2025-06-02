namespace Assurity.Kafka.Mappers.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Consumer.Mappers;
    using Assurity.Kafka.Utilities.Constants;
    using AutoMapper;
    using Avro;
    using Avro.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PPBENPolicyBenefitsMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PPBENPolicyBenefitsMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PPBENPolicyBenefitsMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_PPBENPolicyBenefits_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                     "\"fields\":[{\"name\":\"PBEN_ID\",\"type\":\"int\"}, {\"name\":\"POLICY_NUMBER\",\"type\":\"string\"}," +
                     "{\"name\":\"BENEFIT_SEQ\",\"type\":\"int\"},  {\"name\":\"BENEFIT_TYPE\",\"type\":\"string\"}," +
                     "{\"name\":\"PLAN_CODE\",\"type\":\"string\"},  {\"name\":\"STATUS_CODE\",\"type\":\"string\"}," +
                     "{\"name\":\"STATUS_REASON\",\"type\":\"string\"}, {\"name\":\"STATUS_DATE\",\"type\":\"int\"}, " +
                     "{\"name\":\"COMPANY_CODE\",\"type\":\"string\"}]}");

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, 54321);
            genericRecord.Add(1, "8819851955");
            genericRecord.Add(2, 2);
            genericRecord.Add(3, BenefitTypes.OtherRider);
            genericRecord.Add(4, "ADIR_E");
            genericRecord.Add(5, "A");
            genericRecord.Add(6, string.Empty);
            genericRecord.Add(7, 20231220);
            genericRecord.Add(8, "01");

            // Act
            var ppben = Mapper.Map<PPBEN_POLICY_BENEFITS>(genericRecord);

            // Assert
            Assert.AreEqual(54321, ppben.PBEN_ID);
            Assert.AreEqual("8819851955", ppben.POLICY_NUMBER);
            Assert.AreEqual(2, ppben.BENEFIT_SEQ);
            Assert.AreEqual(BenefitTypes.OtherRider, ppben.BENEFIT_TYPE);
            Assert.AreEqual("ADIR_E", ppben.PLAN_CODE);
            Assert.AreEqual("A", ppben.STATUS_CODE);
            Assert.AreEqual(string.Empty, ppben.STATUS_REASON);
            Assert.AreEqual(20231220, ppben.STATUS_DATE);
        }
    }
}
