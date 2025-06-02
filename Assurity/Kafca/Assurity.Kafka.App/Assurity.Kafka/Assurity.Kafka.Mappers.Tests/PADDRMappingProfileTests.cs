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
    public class PADDRMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PADDRMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PADDRMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_PADDR_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                    "\"fields\":[{\"name\":\"ADDRESS_ID\",\"type\":\"string\"}, {\"name\":\"ADDR_LINE_1\",\"type\":\"string\"}," +
                    "{\"name\":\"ADDR_LINE_2\",\"type\":\"string\"}, {\"name\":\"ADDR_LINE_3\",\"type\":\"string\"}, {\"name\":\"CITY\",\"type\":\"string\"}," +
                    "{\"name\":\"STATE\", \"type\":\"string\"}, {\"name\":\"ZIP\",\"type\":\"string\"},{\"name\":\"ZIP_EXTENSION\",\"type\":\"string\"}, " +
                    "{\"name\":\"BOX_NUMBER\",\"type\":\"string\"}, {\"name\":\"COUNTRY\",\"type\":\"string\"}]}");

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, 123456);
            genericRecord.Add(1, "123 Fake Street");
            genericRecord.Add(2, "Apt 22");
            genericRecord.Add(3, "Line3");
            genericRecord.Add(4, "Lincoln");
            genericRecord.Add(5, "NE");
            genericRecord.Add(6, "68522");
            genericRecord.Add(7, "1234");
            genericRecord.Add(8, "68522");
            genericRecord.Add(9, "USA");

            // Act
            var paddr = Mapper.Map<PADDR>(genericRecord);

            // Assert
            Assert.AreEqual(123456, paddr.ADDRESS_ID);
            Assert.AreEqual("123 Fake Street", paddr.ADDR_LINE_1);
            Assert.AreEqual("Apt 22", paddr.ADDR_LINE_2);
            Assert.AreEqual("Line3", paddr.ADDR_LINE_3);
            Assert.AreEqual("Lincoln", paddr.CITY);
            Assert.AreEqual("NE", paddr.STATE);
            Assert.AreEqual("68522", paddr.ZIP);
            Assert.AreEqual("1234", paddr.ZIP_EXTENSION);
            Assert.AreEqual("68522", paddr.BOX_NUMBER);
            Assert.AreEqual("USA", paddr.COUNTRY);
        }
    }
}