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
    public class PPBENPolicyBenefitsTypesSUMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PPBENPolicyBenefitsTypesSUMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PPBENPolicyBenefitsTypesSUMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_PPBENPolicyBenefitsTypesSU_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                    "\"fields\":[{\"name\":\"ANN_PREM_PER_UNIT\",\"type\":\"float\"}, {\"name\":\"NUMBER_OF_UNITS\",\"type\":\"string\"}," +
                    "{\"name\":\"PBEN_ID\",\"type\":\"int\"}, {\"name\":\"VALUE_PER_UNIT\",\"type\":\"float\"}]}");

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, 7.24m);
            genericRecord.Add(1, 20m);
            genericRecord.Add(2, 1234567);
            genericRecord.Add(3, 200m);

            // Act
            var ppben = Mapper.Map<PPBEN_POLICY_BENEFITS_TYPES_SU>(genericRecord);

            // Assert
            Assert.AreEqual(7.24m, ppben.ANN_PREM_PER_UNIT);
            Assert.AreEqual(20m, ppben.NUMBER_OF_UNITS);
            Assert.AreEqual(1234567, ppben.PBEN_ID);
            Assert.AreEqual(200m, ppben.VALUE_PER_UNIT);
        }
    }
}
