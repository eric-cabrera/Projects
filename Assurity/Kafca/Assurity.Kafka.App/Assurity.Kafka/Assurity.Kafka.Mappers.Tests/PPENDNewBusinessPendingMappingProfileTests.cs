namespace Assurity.Kafka.Mappers.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Consumer.Mappers;
    using Assurity.Kafka.Utilities.Constants;
    using AutoMapper;
    using Avro;
    using Avro.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PPENDNewBusinessPendingMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PPENDNewBusinessPendingMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PPENDNewBusinessPendingMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void GenericRecord_To_PPENDNewBusinessPending_ShouldMap()
        {
            // Arrange
            var schema = (RecordSchema)Avro.Schema.Parse("{\"type\":\"record\"," + "\"name\":\"KsqlDataSourceSchema\"," +
                    "\"fields\":[{\"name\":\"PEND_ID\",\"type\":\"int\"}, {\"name\":\"POLICY_NUMBER\",\"type\":\"string\"}," +
                    "{\"name\":\"COMPANY_CODE\",\"type\":\"string\"}, {\"name\":\"FACE_AMOUNT\",\"type\":\"double\"}, {\"name\":\"REQUIREMENT_DATE\",\"type\":\"int\"}," +
                    "{\"name\":\"LAST_CHANGE_DATE\",\"type\":\"int\"}, {\"name\":\"UND_NAME_ID\",\"type\":\"int\"}, {\"name\":\"REDEF_TYPE\",\"type\":\"string\"}," +
                    "{\"name\":\"REQ_SEQUENCE\", \"type\":\"string\"}]}");

            var genericRecord = new GenericRecord(schema);
            genericRecord.Add(0, 117200);
            genericRecord.Add(1, "3456712345");
            genericRecord.Add(2, "01");
            genericRecord.Add(3, 200.50);
            genericRecord.Add(4, 20190908);
            genericRecord.Add(5, 20200103);
            genericRecord.Add(6, 567123);
            genericRecord.Add(7, RedefTypes.Underwriting);
            genericRecord.Add(8, 2);

            // Act
            var ppend = Mapper.Map<PPEND_NEW_BUSINESS_PENDING>(genericRecord);

            // Assert
            Assert.AreEqual(117200, ppend.PEND_ID);
            Assert.AreEqual("3456712345", ppend.POLICY_NUMBER);
            Assert.AreEqual("01", ppend.COMPANY_CODE);
            Assert.AreEqual(200.5M, ppend.FACE_AMOUNT);
            Assert.AreEqual(20190908, ppend.REQUIREMENT_DATE);
            Assert.AreEqual(20200103, ppend.LAST_CHANGE_DATE);
            Assert.AreEqual(567123, ppend.UND_NAME_ID);
            Assert.AreEqual(RedefTypes.Underwriting, ppend.REDEF_TYPE);
            Assert.AreEqual(2, ppend.REQ_SEQUENCE);
        }
    }
}
