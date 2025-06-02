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
    public class PCOMCCommissionControlTypeSMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PCOMCCommissionControlTypeSMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PCOMCCommissionControlTypeSMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_PCOMCCommissionControlTypeS_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                    "\"fields\":[{\"name\":\"AGENT\",\"type\":\"string\"}, {\"name\":\"AGENT_LEVEL\",\"type\":\"string\"}," +
                    "{\"name\":\"COMC_ID\",\"type\":\"int\"}, {\"name\":\"MARKET_CODE\",\"type\":\"string\"}," +
                    "{\"name\":\"SERVICE_AGENT_IND\",\"type\":\"string\"}, {\"name\":\"COMM_PCNT\",\"type\":\"float\"}]}");

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, "0781");
            genericRecord.Add(1, "99");
            genericRecord.Add(2, "2");
            genericRecord.Add(3, "HO");
            genericRecord.Add(4, "X");
            genericRecord.Add(5, 100.00m);

            // Act
            var pcomcs = Mapper.Map<PCOMC_COMMISSION_CONTROL_TYPE_S>(genericRecord);

            // Assert
            Assert.AreEqual("0781", pcomcs.AGENT);
            Assert.AreEqual("99", pcomcs.AGENT_LEVEL);
            Assert.AreEqual(2, pcomcs.COMC_ID);
            Assert.AreEqual("HO", pcomcs.MARKET_CODE);
            Assert.AreEqual("X", pcomcs.SERVICE_AGENT_IND);
            Assert.AreEqual(100.00m, pcomcs.COMM_PCNT);
        }
    }
}
