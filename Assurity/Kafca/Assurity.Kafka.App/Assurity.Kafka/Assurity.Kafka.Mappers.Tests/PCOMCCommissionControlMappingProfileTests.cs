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
    public class PCOMCCommissionControlMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PCOMCCommissionControlMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PCOMCCommissionControlMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_PCOMCCommissionControl_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                     "\"fields\":[{\"name\":\"COMC_ID\",\"type\":\"int\"}, {\"name\":\"POLICY_NUMBER\",\"type\":\"string\"}," +
                    "{\"name\":\"COMPANY_CODE\",\"type\":\"string\"}, {\"name\":\"RECORD_TYPE\",\"type\":\"string\"}," +
                    "{\"name\":\"ISSUE_DATE\",\"type\":\"int\"}]}");

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, "2");
            genericRecord.Add(1, "1234567890");
            genericRecord.Add(2, "01");
            genericRecord.Add(3, "S");
            genericRecord.Add(4, 20000120);

            // Act
            var pcomc = Mapper.Map<PCOMC_COMMISSION_CONTROL>(genericRecord);

            // Assert
            Assert.AreEqual(2, pcomc.COMC_ID);
            Assert.AreEqual("1234567890", pcomc.POLICY_NUMBER);
            Assert.AreEqual("01", pcomc.COMPANY_CODE);
            Assert.AreEqual("S", pcomc.RECORD_TYPE);
            Assert.AreEqual(20000120, pcomc.ISSUE_DATE);
        }
    }
}
