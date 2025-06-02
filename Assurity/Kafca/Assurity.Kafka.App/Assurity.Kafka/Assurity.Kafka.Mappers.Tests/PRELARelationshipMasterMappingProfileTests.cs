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
    public class PRELARelationshipMasterMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PRELARelationshipMasterMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PRELARelationshipMasterMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_PRELARelationshipMaster_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                     "\"fields\":[{\"name\":\"IDENTIFYING_ALPHA\",\"type\":\"string\"}, {\"name\":\"NAME_ID\",\"type\":\"int\"}," +
                    "{\"name\":\"RELATE_CODE\",\"type\":\"string\"}, {\"name\":\"BENEFIT_SEQ_NUMBER\",\"type\":\"int\"}]}");

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, "011234567890");
            genericRecord.Add(1, 56895);
            genericRecord.Add(2, "IN");
            genericRecord.Add(3, 1);

            // Act
            var prela = Mapper.Map<PRELA_RELATIONSHIP_MASTER>(genericRecord);

            // Assert
            Assert.AreEqual("011234567890", prela.IDENTIFYING_ALPHA);
            Assert.AreEqual(56895, prela.NAME_ID);
            Assert.AreEqual("IN", prela.RELATE_CODE);
            Assert.AreEqual(1, prela.BENEFIT_SEQ_NUMBER);
        }
    }
}
