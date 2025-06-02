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
    public class SysACAgentMarketCodesMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapperConfiguration = new MapperConfiguration(
                    configuration => configuration.AddProfile(typeof(SysACAgentMarketCodesMappingProfile)));

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
        public void GenericRecord_To_SysACAgentMarketCode_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                 "\"fields\":[{\"name\":\"FOLDERID\",\"type\":\"string\"}, {\"name\":\"MARKETCODE\",\"type\":\"string\"}," +
                "{\"name\":\"CONTRACTLEVEL\",\"type\":\"string\"}, {\"name\":\"UPLINEAGENTID\",\"type\":\"string\"}, " +
                "{\"name\":\"UPLINEMARKETCODE\",\"type\":\"string\"}, {\"name\":\"UPLINECONTRACTLEVEL\",\"type\":\"string\"}, " +
                "{\"name\":\"PENDINGRPTDISABLED\",\"type\":\"int\"}]}");

            const string folderId = "1";
            const string marketCode = "IS";
            const string contractLevel = "50";
            const string uplineAgentId = "1234";
            const string uplineMarketCode = "IS";
            const string uplineContractLevel = "40";
            const int pendingRptDisabled = 0;

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, folderId);
            genericRecord.Add(1, marketCode);
            genericRecord.Add(2, contractLevel);
            genericRecord.Add(3, uplineAgentId);
            genericRecord.Add(4, uplineMarketCode);
            genericRecord.Add(5, uplineContractLevel);
            genericRecord.Add(6, pendingRptDisabled);

            // Act
            var sysACAgentMarketCodes = Mapper.Map<SysACAgentMarketCodes>(genericRecord);

            // Assert
            Assert.AreEqual(folderId, sysACAgentMarketCodes.FOLDERID);
            Assert.AreEqual(marketCode, sysACAgentMarketCodes.MARKETCODE);
            Assert.AreEqual(contractLevel, sysACAgentMarketCodes.CONTRACTLEVEL);
            Assert.AreEqual(uplineAgentId, sysACAgentMarketCodes.UPLINEAGENTID);
            Assert.AreEqual(uplineMarketCode, sysACAgentMarketCodes.UPLINEMARKETCODE);
            Assert.AreEqual(uplineContractLevel, sysACAgentMarketCodes.UPLINECONTRACTLEVEL);
            Assert.AreEqual(pendingRptDisabled, sysACAgentMarketCodes.PENDINGRPTDISABLED);
        }
    }
}
