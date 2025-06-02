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
    public class PPENDNewBusPendUnderwritingMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PPENDNewBusPendUnderwritingMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PPENDNewBusPendUnderwritingMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_PPENDNewBusPendUnderwriting_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                    "\"fields\":[{\"name\":\"PEND_ID\",\"type\":\"int\"}, {\"name\":\"IDX\",\"type\":\"int\"}," +
                    "{\"name\":\"UND_FLAG\",\"type\":\"string\"}, {\"name\":\"UND_DATE\",\"type\":\"int\"}, {\"name\":\"UND_O_DATE\",\"type\":\"int\"}," +
                    "{\"name\":\"UND_CODE\",\"type\":\"int\"}, {\"name\":\"COMMENTS\",\"type\":\"string\"}, {\"name\":\"NOTE_SEQ\",\"type\":\"string\"}]}");

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, 117200);
            genericRecord.Add(1, 1);
            genericRecord.Add(2, "N");
            genericRecord.Add(3, 20190908);
            genericRecord.Add(4, 20220329);
            genericRecord.Add(5, 66);
            genericRecord.Add(6, "SEE NOTE");
            genericRecord.Add(7, 0);

            // Act
            var ppend = Mapper.Map<PPEND_NEW_BUS_PEND_UNDERWRITING>(genericRecord);

            // Assert
            Assert.AreEqual(117200, ppend.PEND_ID);
            Assert.AreEqual(1, ppend.IDX);
            Assert.AreEqual("N", ppend.UND_FLAG);
            Assert.AreEqual(20190908, ppend.UND_DATE);
            Assert.AreEqual(20220329, ppend.UND_O_DATE);
            Assert.AreEqual(66, ppend.UND_CODE);
            Assert.AreEqual("SEE NOTE", ppend.COMMENTS);
            Assert.AreEqual(0, ppend.NOTE_SEQ);
        }
    }
}
