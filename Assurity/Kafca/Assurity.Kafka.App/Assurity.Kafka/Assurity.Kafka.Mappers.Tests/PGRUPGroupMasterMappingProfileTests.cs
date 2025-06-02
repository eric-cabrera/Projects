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
    public class PGRUPGroupMasterMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapperConfiguration = new MapperConfiguration(
                    configuration => configuration.AddProfile(typeof(PGRUP_GroupMasterMappingProfile)));

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
        public void GenericRecord_To_PGRUPGroupMaster_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Schema.Parse(
                "{\"type\":\"record\"," +
                "\"name\":\"KsqlDataSourceSchema\"," +
                "\"fields\":" +
                    "[{\"name\":\"NAME_ID\",\"type\":\"int\"}, " +
                    "{\"name\":\"GROUP_NUMBER\",\"type\":\"string\"}," +
                    "{\"name\":\"COMPANY_CODE\",\"type\":\"string\"}," +
                    "{\"name\":\"STATUS_CODE\",\"type\":\"string\"}]}");

            var genericRecord = new GenericRecord(schema);

            genericRecord.Add(0, 123456);
            genericRecord.Add(1, "GroupNumber");
            genericRecord.Add(2, "01");
            genericRecord.Add(3, "A");

            // Act
            var pgrupGroupMaster = Mapper.Map<PGRUP_GROUP_MASTER>(genericRecord);

            // Assert
            Assert.AreEqual(123456, pgrupGroupMaster.NAME_ID);
            Assert.AreEqual("GroupNumber", pgrupGroupMaster.GROUP_NUMBER);
            Assert.AreEqual("01", pgrupGroupMaster.COMPANY_CODE);
            Assert.AreEqual('A', pgrupGroupMaster.STATUS_CODE);
        }
    }
}
