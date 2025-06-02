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
    public class PPBENPolicyBenefitsTypesBFMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PPBENPolicyBenefitsTypesBFMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PPBENPolicyBenefitsTypesBFMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_PPBENPolicyBenefitsTypesBF_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                    "\"fields\":[{\"name\":\"PBEN_ID\",\"type\":\"int\"}, {\"name\":\"ANN_PREM_PER_UNIT\",\"type\":\"float\"}," +
                    "{\"name\":\"NUMBER_OF_UNITS\",\"type\":\"float\"}, {\"name\":\"BF_DB_OPTION\",\"type\":\"string\"}, " +
                    "{\"name\":\"VALUE_PER_UNIT\",\"type\":\"float\"}, {\"name\":\"BF_DATE_NEGATIVE\",\"type\":\"int\"}, " +
                    "{\"name\":\"BF_CURRENT_DB\",\"type\":\"float\"}]}");

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, 277772);
            genericRecord.Add(1, 100m);
            genericRecord.Add(2, 9m);
            genericRecord.Add(3, "1");
            genericRecord.Add(4, 200m);
            genericRecord.Add(5, 20180119);
            genericRecord.Add(6, 150000m);

            // Act
            var ppben = Mapper.Map<PPBEN_POLICY_BENEFITS_TYPES_BF>(genericRecord);

            // Assert
            Assert.AreEqual(277772, ppben.PBEN_ID);
            Assert.AreEqual(100m, ppben.ANN_PREM_PER_UNIT);
            Assert.AreEqual(9m, ppben.NUMBER_OF_UNITS);
            Assert.AreEqual("1", ppben.BF_DB_OPTION);
            Assert.AreEqual(200m, ppben.VALUE_PER_UNIT);
            Assert.AreEqual(20180119, ppben.BF_DATE_NEGATIVE);
            Assert.AreEqual(150000m, ppben.BF_CURRENT_DB);
        }
    }
}
