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
    public class PHIERAgentHierarchyMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PHIERAgentHierarchyMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PHIERAgentHierarchyMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_PHIERAgentHierarchy_ShouldMap()
        {
            // Arrange
            var schema = SetupSchema();

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, "01");
            genericRecord.Add(1, "2ZVR");
            genericRecord.Add(2, "JM");
            genericRecord.Add(3, "40");
            genericRecord.Add(4, 20230907);
            genericRecord.Add(5, 20230107);
            genericRecord.Add(6, "2ZVR");
            genericRecord.Add(7, "JM");
            genericRecord.Add(8, "70");
            genericRecord.Add(9, "u");
            genericRecord.Add(10, "098J");

            // Act
            var phier = Mapper.Map<PHIER_AGENT_HIERARCHY>(genericRecord);

            // Assert
            Assert.AreEqual("01", phier.COMPANY_CODE);
            Assert.AreEqual("2ZVR", phier.AGENT_NUM);
            Assert.AreEqual("JM", phier.MARKET_CODE);
            Assert.AreEqual("40", phier.AGENT_LEVEL);
            Assert.AreEqual(20230907, phier.STOP_DATE);
            Assert.AreEqual(20230107, phier.START_DATE);
            Assert.AreEqual("2ZVR", phier.HIERARCHY_AGENT);
            Assert.AreEqual("JM", phier.HIER_MARKET_CODE);
            Assert.AreEqual("70", phier.HIER_AGENT_LEVEL);
        }

        public RecordSchema SetupSchema()
        {
            return (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                "\"fields\":[{\"name\":\"COMPANY_CODE\",\"type\":\"string\"}, {\"name\":\"AGENT_NUM\",\"type\":\"string\"}," +
                "{\"name\":\"MARKET_CODE\",\"type\":\"string\"}, {\"name\":\"AGENT_LEVEL\",\"type\":\"string\"}," +
                "{\"name\":\"STOP_DATE\",\"type\":\"int\"}, {\"name\":\"START_DATE\",\"type\":\"int\"}," +
                "{\"name\":\"HIERARCHY_AGENT\",\"type\":\"string\"}, {\"name\":\"HIER_MARKET_CODE\",\"type\":\"string\"}," +
                "{\"name\":\"HIER_AGENT_LEVEL\",\"type\":\"string\"}, {\"name\":\"OP\",\"type\":\"string\"}," +
                "{\"name\":\"BEFORE_AGENT_NUM\",\"type\":\"string\"}]}");
        }
    }
}
