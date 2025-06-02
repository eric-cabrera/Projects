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
    public class PACTGMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PACTGMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PACTGMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_PACTG_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                    "\"fields\":[{\"name\":\"COMPANY_CODE\",\"type\":\"string\"}, {\"name\":\"POLICY_NUMBER\",\"type\":\"string\"}," +
                    "{\"name\":\"BENEFIT_SEQ\",\"type\":\"int\"}, {\"name\":\"DATE_ADDED\",\"type\":\"int\"}, {\"name\":\"TIME_ADDED\",\"type\":\"int\"}," +
                    "{\"name\":\"RECORD_SEQUENCE\", \"type\":\"int\"}, {\"name\":\"LIFEPRO_ID\",\"type\":\"int\"},{\"name\":\"EFFECTIVE_DATE\",\"type\":\"int\"}, " +
                    "{\"name\":\"DEBIT_CODE\",\"type\":\"int\"}, {\"name\":\"CREDIT_CODE\",\"type\":\"int\"}, {\"name\":\"REVERSAL_CODE\",\"type\":\"string\"}]}");

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, "01");
            genericRecord.Add(1, "1234567890");
            genericRecord.Add(2, 7);
            genericRecord.Add(3, 20150119);
            genericRecord.Add(4, 17094539);
            genericRecord.Add(5, 7895);
            genericRecord.Add(6, 8);
            genericRecord.Add(7, 20150220);
            genericRecord.Add(8, 520);
            genericRecord.Add(9, 13);
            genericRecord.Add(10, "Y");

            // Act
            var pactg = Mapper.Map<PACTG>(genericRecord);

            // Assert
            Assert.AreEqual("01", pactg.COMPANY_CODE);
            Assert.AreEqual("1234567890", pactg.POLICY_NUMBER);
            Assert.AreEqual(7, pactg.BENEFIT_SEQ);
            Assert.AreEqual(20150119, pactg.DATE_ADDED);
            Assert.AreEqual(17094539, pactg.TIME_ADDED);
            Assert.AreEqual(7895, pactg.RECORD_SEQUENCE);
            Assert.AreEqual(8, pactg.LIFEPRO_ID);
            Assert.AreEqual(20150220, pactg.EFFECTIVE_DATE);
            Assert.AreEqual(520, pactg.DEBIT_CODE);
            Assert.AreEqual(13, pactg.CREDIT_CODE);
            Assert.AreEqual("Y", pactg.REVERSAL_CODE);
        }
    }
}