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
    public class PPBENPolicyBenefitsTypesBAORMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PPBENPolicyBenefitsTypesBAORMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PPBENPolicyBenefitsTypesBAORMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_PPBENPolicyBenefitsTypesBA_OR_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                    "\"fields\":[{\"name\":\"ANN_PREM_PER_UNIT\",\"type\":\"float\"}, {\"name\":\"NUMBER_OF_UNITS\",\"type\":\"string\"}," +
                    "{\"name\":\"PBEN_ID\",\"type\":\"int\"}, {\"name\":\"DIVIDEND\",\"type\":\"string\"}, {\"name\":\"VALUE_PER_UNIT\",\"type\":\"float\"}]}");

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, 7.24m);
            genericRecord.Add(1, 20m);
            genericRecord.Add(2, 1234567);
            genericRecord.Add(3, "5");
            genericRecord.Add(4, 200m);

            // Act
            var ppben = Mapper.Map<PPBEN_POLICY_BENEFITS_TYPES_BA_OR>(genericRecord);

            // Assert
            Assert.AreEqual(7.24m, ppben.ANN_PREM_PER_UNIT);
            Assert.AreEqual(20m, ppben.NUMBER_OF_UNITS);
            Assert.AreEqual(1234567, ppben.PBEN_ID);
            Assert.AreEqual("5", ppben.DIVIDEND);
            Assert.AreEqual(200m, ppben.VALUE_PER_UNIT);
        }
    }
}
