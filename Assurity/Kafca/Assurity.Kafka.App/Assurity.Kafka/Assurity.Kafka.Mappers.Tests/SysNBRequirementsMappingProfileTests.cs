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
    public class SysNBRequirementsMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(SysNBRequirementsMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void SysNBRequirementsMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_SysNBRequirements_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                    "\"fields\":[{\"name\":\"POLICYNUMBER\",\"type\":\"string\"}, {\"name\":\"REQSEQ\",\"type\":\"int\"}," +
                    "{\"name\":\"IX\",\"type\":\"int\"}, {\"name\":\"REQTYPE\",\"type\":\"string\"}, {\"name\":\"REQNOTE\",\"type\":\"string\"}]}");

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, "4150000016");
            genericRecord.Add(1, 1);
            genericRecord.Add(2, 0);
            genericRecord.Add(3, "MIB");
            genericRecord.Add(4, "We need blood.....");

            // Act
            var sysNBRequirements = Mapper.Map<SysNBRequirements>(genericRecord);

            // Assert
            Assert.AreEqual("4150000016", sysNBRequirements.POLICYNUMBER);
            Assert.AreEqual(1, sysNBRequirements.REQSEQ);
            Assert.AreEqual(0, sysNBRequirements.IX);
            Assert.AreEqual("MIB", sysNBRequirements.REQTYPE);
            Assert.AreEqual("We need blood.....", sysNBRequirements.REQNOTE);
        }
    }
}
