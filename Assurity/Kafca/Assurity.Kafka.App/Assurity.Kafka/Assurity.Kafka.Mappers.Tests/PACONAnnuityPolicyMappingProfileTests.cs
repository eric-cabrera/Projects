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
    public class PACONAnnuityPolicyMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PACONAnnuityPolicyMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PACONAnnuityPolicyMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_PACONAnnuityPolicy_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                     "\"fields\":[{\"name\":\"COMPANY_CODE\",\"type\":\"string\"}, {\"name\":\"POLICY_NUMBER\",\"type\":\"string\"}," +
                     "{\"name\":\"STATUS_CODE\",\"type\":\"string\"},  {\"name\":\"STATUS_REASON\",\"type\":\"string\"}," +
                     "{\"name\":\"STATUS_DATE\",\"type\":\"int\"},  {\"name\":\"ISSUE_DATE\",\"type\":\"int\"}," +
                     "{\"name\":\"TAX_QUALIFICATION\",\"type\":\"string\"}]}");

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, "01");
            genericRecord.Add(1, "123456789");
            genericRecord.Add(2, "T");
            genericRecord.Add(3, "DC");
            genericRecord.Add(4, 20151202);
            genericRecord.Add(5, 20011201);
            genericRecord.Add(6, "2");

            // Act
            var pacon = Mapper.Map<PACON_ANNUITY_POLICY>(genericRecord);

            // Assert
            Assert.AreEqual("01", pacon.COMPANY_CODE);
            Assert.AreEqual("123456789", pacon.POLICY_NUMBER);
            Assert.AreEqual("T", pacon.STATUS_CODE);
            Assert.AreEqual("DC", pacon.STATUS_REASON);
            Assert.AreEqual(20151202, pacon.STATUS_DATE);
            Assert.AreEqual(20011201, pacon.ISSUE_DATE);
            Assert.AreEqual("2", pacon.TAX_QUALIFICATION);
        }
    }
}
