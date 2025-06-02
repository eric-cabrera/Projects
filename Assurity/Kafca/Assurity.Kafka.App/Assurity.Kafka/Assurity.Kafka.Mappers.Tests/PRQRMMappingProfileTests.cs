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
    public class PRQRMMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PRQRMMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PRQRMMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_PRQRM_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                    "\"fields\":[{\"name\":\"COMPANY_CODE\",\"type\":\"string\"}, {\"name\":\"REQ_SEQUENCE\",\"type\":\"int\"}," +
                    "{\"name\":\"LAST_CHG_DATE\",\"type\":\"int\"}, {\"name\":\"POLICY_NUMBER\",\"type\":\"string\"}, {\"name\":\"NAME_ID\",\"type\":\"int\"}]}");

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, "01");
            genericRecord.Add(1, 1);
            genericRecord.Add(2, 20190908);
            genericRecord.Add(3, "3456712345");
            genericRecord.Add(4, 567221);

            // Act
            var prqrm = Mapper.Map<PRQRM>(genericRecord);

            // Assert
            Assert.AreEqual("01", prqrm.COMPANY_CODE);
            Assert.AreEqual(1, prqrm.REQ_SEQUENCE);
            Assert.AreEqual(20190908, prqrm.LAST_CHG_DATE);
            Assert.AreEqual("3456712345", prqrm.POLICY_NUMBER);
            Assert.AreEqual(567221, prqrm.NAME_ID);
        }
    }
}
