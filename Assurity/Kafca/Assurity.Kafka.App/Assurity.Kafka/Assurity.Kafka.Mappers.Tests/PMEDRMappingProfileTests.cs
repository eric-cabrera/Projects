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
    public class PMEDRMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PMEDRMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PMEDRMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_PMEDR_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                    "\"fields\":[{\"name\":\"RECORD_TYPE\",\"type\":\"string\"}, {\"name\":\"REQ_NAME\",\"type\":\"string\"}," +
                    "{\"name\":\"REQ_DESCRIPTION\",\"type\":\"string\"}, {\"name\":\"REQ_NUMBER\",\"type\":\"int\"}]}");

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, "R");
            genericRecord.Add(1, "MIB");
            genericRecord.Add(2, "MEDICAL INFORMATION BUREAU");
            genericRecord.Add(3, 1);

            // Act
            var pmedr = Mapper.Map<PMEDR>(genericRecord);

            // Assert
            Assert.AreEqual("R", pmedr.RECORD_TYPE);
            Assert.AreEqual("MIB", pmedr.REQ_NAME);
            Assert.AreEqual("MEDICAL INFORMATION BUREAU", pmedr.REQ_DESCRIPTION);
            Assert.AreEqual(1, pmedr.REQ_NUMBER);
        }
    }
}
