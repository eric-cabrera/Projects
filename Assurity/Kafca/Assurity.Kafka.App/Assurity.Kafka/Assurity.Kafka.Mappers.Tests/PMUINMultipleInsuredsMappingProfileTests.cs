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
    public class PMUINMultipleInsuredsMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PMUINMultipleInsuredsMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PMUINMultipleInsuredsMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_PMUINMultipleInsureds_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                     "\"fields\":[{\"name\":\"COMPANY_CODE\",\"type\":\"string\"}, {\"name\":\"POLICY_NUMBER\",\"type\":\"string\"}," +
                     "{\"name\":\"NAME_ID\",\"type\":\"int\"},  {\"name\":\"RELATIONSHIP_CODE\",\"type\":\"string\"}," +
                     "{\"name\":\"MULT_RELATE\",\"type\":\"string\"},  {\"name\":\"BENEFIT_SEQ\",\"type\":\"int\"}," +
                     "{\"name\":\"KD_DEF_SEGT_ID\",\"type\":\"string\"}, {\"name\":\"KD_BEN_EXTEND_KEYS\",\"type\":\"string\"}," +
                     "{\"name\":\"UWCLS\",\"type\":\"string\"}, {\"name\":\"START_DATE\",\"type\":\"int\"}," +
                     "{\"name\":\"STOP_DATE\",\"type\":\"int\"}]}");

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, "01");
            genericRecord.Add(1, "123456789");
            genericRecord.Add(2, 317726);
            genericRecord.Add(3, "ML");
            genericRecord.Add(4, "SELF");
            genericRecord.Add(5, 1);
            genericRecord.Add(6, "IFB 236");
            genericRecord.Add(7, "040203");
            genericRecord.Add(8, "N");
            genericRecord.Add(9, 20220401);
            genericRecord.Add(10, 20230401);

            // Act
            var pmuin = Mapper.Map<PMUIN_MULTIPLE_INSUREDS>(genericRecord);

            // Assert
            Assert.AreEqual("01", pmuin.COMPANY_CODE);
            Assert.AreEqual("123456789", pmuin.POLICY_NUMBER);
            Assert.AreEqual(317726, pmuin.NAME_ID);
            Assert.AreEqual("ML", pmuin.RELATIONSHIP_CODE);
            Assert.AreEqual("SELF", pmuin.MULT_RELATE);
            Assert.AreEqual(1, pmuin.BENEFIT_SEQ);
            Assert.AreEqual("IFB 236", pmuin.KD_DEF_SEGT_ID);
            Assert.AreEqual("040203", pmuin.KD_BEN_EXTEND_KEYS);
            Assert.AreEqual("N", pmuin.UWCLS);
            Assert.AreEqual(20220401, pmuin.START_DATE);
            Assert.AreEqual(20230401, pmuin.STOP_DATE);
        }
    }
}
