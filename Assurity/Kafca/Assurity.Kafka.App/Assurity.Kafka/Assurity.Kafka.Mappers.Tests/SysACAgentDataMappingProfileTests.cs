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
    public class SysACAgentDataMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapperConfiguration = new MapperConfiguration(
                    configuration => configuration.AddProfile(typeof(SysACAgentDataMappingProfile)));

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
        public void GenericRecord_To_SysACAgentData_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                 "\"fields\":[{\"name\":\"FOLDERID\",\"type\":\"string\"}, {\"name\":\"AGENTID\",\"type\":\"string\"}," +
                "{\"name\":\"FIRSTNAME\",\"type\":\"string\"}, {\"name\":\"MIDDLENAME\",\"type\":\"string\"}," +
                "{\"name\":\"LASTNAME\",\"type\":\"string\"}, {\"name\":\"BUSINESSNAME\",\"type\":\"string\"}]}");

            const string folderId = "1";
            const string agentId = "1234";
            const string firstName = "Test";
            const string middleName = "T";
            const string lastName = "Testerson";
            const string businessName = "Company LLC";

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, folderId);
            genericRecord.Add(1, agentId);
            genericRecord.Add(2, firstName);
            genericRecord.Add(3, middleName);
            genericRecord.Add(4, lastName);
            genericRecord.Add(5, businessName);

            // Act
            var sysACAgentData = Mapper.Map<SysACAgentData>(genericRecord);

            // Assert
            Assert.AreEqual(folderId, sysACAgentData.FOLDERID);
            Assert.AreEqual(agentId, sysACAgentData.AGENTID);
            Assert.AreEqual(firstName, sysACAgentData.FIRSTNAME);
            Assert.AreEqual(middleName, sysACAgentData.MIDDLENAME);
            Assert.AreEqual(lastName, sysACAgentData.LASTNAME);
            Assert.AreEqual(businessName, sysACAgentData.BUSINESSNAME);
        }
    }
}
