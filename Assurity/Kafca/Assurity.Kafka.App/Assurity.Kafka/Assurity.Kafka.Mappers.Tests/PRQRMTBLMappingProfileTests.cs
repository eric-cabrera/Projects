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
    public class PRQRMTBLMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PRQRMTBLMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PRQRMTBLMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_PRQRMTBL_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                    "\"fields\":[{\"name\":\"REQ_SEQUENCE\",\"type\":\"int\"}, {\"name\":\"UND_DESC_CODE\",\"type\":\"string\"}," +
                    "{\"name\":\"COMPANY_CODE\",\"type\":\"string\"}, {\"name\":\"UND_REQ_MET\",\"type\":\"string\"}, {\"name\":\"UND_REQ_DATE\",\"type\":\"int\"}," +
                    "{\"name\":\"UND_OBTAIN_DATE\", \"type\":\"int\"}, {\"name\":\"SEQ\",\"type\":\"int\"},{\"name\":\"NAME_ID\",\"type\":\"int\"}, " +
                    "{\"name\":\"POLICY_NUMBER\",\"type\":\"string\"}, {\"name\":\"UND_COMMENTS\",\"type\":\"string\"}, {\"name\":\"UND_REQ_NOTE_SEQ\",\"type\":\"int\"}]}");

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, 0);
            genericRecord.Add(1, 1);
            genericRecord.Add(2, "01");
            genericRecord.Add(3, "Y");
            genericRecord.Add(4, 20190908);
            genericRecord.Add(5, 20221129);
            genericRecord.Add(6, 1);
            genericRecord.Add(7, 456788);
            genericRecord.Add(8, "8767890012");
            genericRecord.Add(9, "OTHER EXAM");
            genericRecord.Add(10, 0);

            // Act
            var prqrmtbl = Mapper.Map<PRQRMTBL>(genericRecord);

            // Assert
            Assert.AreEqual(0, prqrmtbl.REQ_SEQUENCE);
            Assert.AreEqual(1, prqrmtbl.UND_DESC_CODE);
            Assert.AreEqual("01", prqrmtbl.COMPANY_CODE);
            Assert.AreEqual("8767890012", prqrmtbl.POLICY_NUMBER);
            Assert.AreEqual(456788, prqrmtbl.NAME_ID);
            Assert.AreEqual("Y", prqrmtbl.UND_REQ_MET);
            Assert.AreEqual(1, prqrmtbl.SEQ);
            Assert.AreEqual(20190908, prqrmtbl.UND_REQ_DATE);
            Assert.AreEqual(20221129, prqrmtbl.UND_OBTAIN_DATE);
            Assert.AreEqual("OTHER EXAM", prqrmtbl.UND_COMMENTS);
            Assert.AreEqual(0, prqrmtbl.UND_REQ_NOTE_SEQ);
        }
    }
}
