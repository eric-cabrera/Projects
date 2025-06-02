namespace Assurity.Kafka.Mappers.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Consumer.Mappers;
    using Assurity.PolicyInfo.Contracts.V1;
    using AutoMapper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PNAMEToPersonMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PNAMEToPersonMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PNAMEToPersonMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void PNAMEToPersonMappingProfile_Should_Return_Business_Name()
        {
            // Arrange
            var pname = new PNAME
            {
                NAME_ID = 822007,
                NAME_BUSINESS = "ABC Company",
                BUSINESS_EMAIL_ADR = "abc_company@gmail.com",
                INDIVIDUAL_FIRST = string.Empty,
                INDIVIDUAL_LAST = string.Empty,
                INDIVIDUAL_MIDDLE = string.Empty,
                INDIVIDUAL_PREFIX = string.Empty,
                INDIVIDUAL_SUFFIX = string.Empty,
                DATE_OF_BIRTH = 0,
                NAME_FORMAT_CODE = string.Empty,
                SEX_CODE = string.Empty,
                PERSONAL_EMAIL_ADR = string.Empty
            };

            // Act
            var person = Mapper.Map<Person>(pname);

            // Assert
            Assert.IsNotNull(person);
            Assert.AreEqual(822007, person.Name.NameId);
            Assert.AreEqual("ABC Company", person.Name.BusinessName);
        }

        [TestMethod]
        public void PNAMEToPersonMappingProfile_Should_Return_Person()
        {
            // Arrange
            var pname = new PNAME
            {
                NAME_ID = 123456,
                INDIVIDUAL_PREFIX = "Mr.",
                INDIVIDUAL_FIRST = "John",
                INDIVIDUAL_MIDDLE = "A",
                INDIVIDUAL_LAST = "Wick",
                INDIVIDUAL_SUFFIX = "Sr.",
            };

            // Act
            var person = Mapper.Map<Person>(pname);

            // Assert
            Assert.IsNotNull(person);
            Assert.AreEqual(123456, person.Name.NameId);
            Assert.AreEqual("John", person.Name.IndividualFirst);
            Assert.AreEqual("Wick", person.Name.IndividualLast);
            Assert.AreEqual("A", person.Name.IndividualMiddle);
            Assert.AreEqual("Mr.", person.Name.IndividualPrefix);
            Assert.AreEqual("Sr.", person.Name.IndividualSuffix);
        }
    }
}
