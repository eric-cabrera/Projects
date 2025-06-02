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
    public class PNAMEMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PNAMEMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PNAMEMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_PNAME_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                    "\"fields\":[{\"name\":\"BUSINESS_EMAIL_ADR\",\"type\":\"string\"}, {\"name\":\"DATE_OF_BIRTH\",\"type\":\"int\"}," +
                    "{\"name\":\"INDIVIDUAL_PREFIX\",\"type\":\"string\"}, {\"name\":\"INDIVIDUAL_FIRST\",\"type\":\"string\"}, {\"name\":\"INDIVIDUAL_LAST\",\"type\":\"string\"}," +
                    "{\"name\":\"INDIVIDUAL_MIDDLE\", \"type\":\"string\"}, {\"name\":\"INDIVIDUAL_SUFFIX\",\"type\":\"string\"},{\"name\":\"NAME_BUSINESS\",\"type\":\"string\"}, " +
                    "{\"name\":\"NAME_FORMAT_CODE\",\"type\":\"string\"}, {\"name\":\"NAME_ID\",\"type\":\"int\"}, " +
                    "{\"name\":\"PERSONAL_EMAIL_ADR\",\"type\":\"string\"}, {\"name\":\"SEX_CODE\",\"type\":\"string\"}]}");

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, "abc@gmail.com");
            genericRecord.Add(1, 20000120);
            genericRecord.Add(2, "Jr");
            genericRecord.Add(3, "John");
            genericRecord.Add(4, "Smith");
            genericRecord.Add(5, "E");
            genericRecord.Add(6, "S");
            genericRecord.Add(7, "ABC COMPANY");
            genericRecord.Add(8, "212");
            genericRecord.Add(9, 871234);
            genericRecord.Add(10, "ghj@gmail.com");
            genericRecord.Add(11, "M");

            // Act
            var pname = Mapper.Map<PNAME>(genericRecord);

            // Assert
            Assert.AreEqual("abc@gmail.com", pname.BUSINESS_EMAIL_ADR);
            Assert.AreEqual(20000120, pname.DATE_OF_BIRTH);
            Assert.AreEqual("Jr", pname.INDIVIDUAL_PREFIX);
            Assert.AreEqual("John", pname.INDIVIDUAL_FIRST);
            Assert.AreEqual("Smith", pname.INDIVIDUAL_LAST);
            Assert.AreEqual("E", pname.INDIVIDUAL_MIDDLE);
            Assert.AreEqual("S", pname.INDIVIDUAL_SUFFIX);
            Assert.AreEqual("ABC COMPANY", pname.NAME_BUSINESS);
            Assert.AreEqual("212", pname.NAME_FORMAT_CODE);
            Assert.AreEqual(871234, pname.NAME_ID);
            Assert.AreEqual("ghj@gmail.com", pname.PERSONAL_EMAIL_ADR);
            Assert.AreEqual("M", pname.SEX_CODE);
        }
    }
}
