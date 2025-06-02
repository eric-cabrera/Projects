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
    public class SysZ9MappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapperConfiguration = new MapperConfiguration(
                    configuration => configuration.AddProfile(typeof(SysZ9ProcessMappingProfile)));

                return mapperConfiguration.CreateMapper();
            }
        }

        [TestMethod]
        public void AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_SysZ9Process_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                 "\"fields\":[{\"name\":\"AGENTID\",\"type\":\"string\"}, {\"name\":\"AGENTMARKETCODE\",\"type\":\"string\"}," +
                "{\"name\":\"AGENTLEVEL\",\"type\":\"string\"}, {\"name\":\"NBFOLDEROBJID\",\"type\":\"string\"}]}");

            const string agentId = "1234";
            const string agentMarketCode = "IS";
            const string agentLevel = "50";
            const string nbFolderObjId = "NBFolderObjID";

            var genericRecord = new GenericRecord(schema);

            genericRecord.Add(0, agentId);
            genericRecord.Add(1, agentMarketCode);
            genericRecord.Add(2, agentLevel);
            genericRecord.Add(3, nbFolderObjId);

            // Act
            var sysZ9Process = Mapper.Map<SysZ9Process>(genericRecord);

            // Assert
            Assert.AreEqual(agentId, sysZ9Process.AGENTID);
            Assert.AreEqual(agentMarketCode, sysZ9Process.AGENTMARKETCODE);
            Assert.AreEqual(agentLevel, sysZ9Process.AGENTLEVEL);
            Assert.AreEqual(nbFolderObjId, sysZ9Process.NBFOLDEROBJID);
        }
    }
}
