namespace Assurity.Kafka.Mappers.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Consumer.Mappers;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using AutoMapper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PADDRToAddressMappingProfileTests
    {
        private IMapper Mapper
        {
            get
            {
                var mapper = new MapperConfiguration(configuration =>
                    configuration.AddProfile(typeof(PADDRToAddressMappingProfile)));

                return mapper.CreateMapper();
            }
        }

        [TestMethod]
        public void PADDRToAddressMappingProfile_AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void PADDRToAddressMappingProfile_Should_Return_Address()
        {
            // Arrange
            var paddr = new PADDR
            {
                ADDRESS_ID = 678900,
                ADDR_LINE_1 = "1234 A St",
                ADDR_LINE_2 = "Line2",
                ADDR_LINE_3 = "Line3",
                STATE = "NE",
                CITY = "Lincoln",
                ZIP_EXTENSION = "56432",
                BOX_NUMBER = "10",
                COUNTRY = "USA",
                ZIP = "56789"
            };

            // Act
            var address = Mapper.Map<Address>(paddr);

            // Assert
            Assert.IsNotNull(address);
            Assert.AreEqual(678900, address.AddressId);
            Assert.AreEqual("1234 A St", address.Line1);
            Assert.AreEqual("Line2", address.Line2);
            Assert.AreEqual("Line3", address.Line3);
            Assert.AreEqual(State.NE, address.StateAbbreviation);
            Assert.AreEqual("Lincoln", address.City);
            Assert.AreEqual("56432", address.ZipExtension);
            Assert.AreEqual("10", address.BoxNumber);
            Assert.AreEqual(Country.USA, address.Country);
            Assert.AreEqual("56789", address.ZipCode);
        }
    }
}