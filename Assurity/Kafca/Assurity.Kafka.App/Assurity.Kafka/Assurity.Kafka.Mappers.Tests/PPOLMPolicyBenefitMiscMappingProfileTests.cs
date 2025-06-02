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
    public class PPOLMPolicyBenefitMiscMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PPOLMPolicyBenefitMiscMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PPOLMPolicyBenefitMiscMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_PPOLMPolicyBenefitMisc_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                     "\"fields\":[{\"name\":\"POLICY_NUMBER\",\"type\":\"string\"}, {\"name\":\"CANCEL_REASON\",\"type\":\"string\"}," +
                    "{\"name\":\"CANCEL_DESC\",\"type\":\"string\"}, {\"name\":\"COMPANY_CODE\",\"type\":\"string\"}," +
                    "{\"name\":\"POLM_ID\",\"type\":\"int\"}, {\"name\":\"SEQ\",\"type\":\"int\"}]}");

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, "1234567890");
            genericRecord.Add(1, "17");
            genericRecord.Add(2, "FREE LOOK PERIOD – BAD CHECK OR CREDIT CARD");
            genericRecord.Add(3, "01");
            genericRecord.Add(4, 1234);
            genericRecord.Add(5, 1);

            // Act
            var ppolm = Mapper.Map<PPOLM_POLICY_BENEFIT_MISC>(genericRecord);

            // Assert
            Assert.AreEqual("1234567890", ppolm.POLICY_NUMBER);
            Assert.AreEqual("01", ppolm.COMPANY_CODE);
            Assert.AreEqual("17", ppolm.CANCEL_REASON);
            Assert.AreEqual("FREE LOOK PERIOD – BAD CHECK OR CREDIT CARD", ppolm.CANCEL_DESC);
            Assert.AreEqual(1234, ppolm.POLM_ID);
            Assert.AreEqual(1, ppolm.SEQ);
        }
    }
}
