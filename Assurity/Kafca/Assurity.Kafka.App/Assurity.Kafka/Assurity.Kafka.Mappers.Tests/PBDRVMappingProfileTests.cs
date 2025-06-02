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
    public class PBDRVMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PBDRVMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PBDRVMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_PBDRV_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                    "\"fields\":[{\"name\":\"DESCRIPTION\",\"type\":\"string\"}, {\"name\":\"STATUS_CODE\",\"type\":\"string\"}," +
                    "{\"name\":\"BATCH_START_DATE\",\"type\":\"int\"}, {\"name\":\"BATCH_STOP_DATE\",\"type\":\"int\"}]}");

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, "YEAREND");
            genericRecord.Add(1, "B");
            genericRecord.Add(2, 5132023);
            genericRecord.Add(3, 5152023);

            // Act
            var pbdrv = Mapper.Map<PBDRV>(genericRecord);

            // Assert
            Assert.AreEqual("YEAREND", pbdrv.DESCRIPTION);
            Assert.AreEqual("B", pbdrv.STATUS_CODE);
            Assert.AreEqual(5132023, pbdrv.BATCH_START_DATE);
            Assert.AreEqual(5152023, pbdrv.BATCH_STOP_DATE);
        }
    }
}
