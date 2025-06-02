namespace Assurity.AgentPortal.Managers.Tests.Mapping
{
    using System.Diagnostics.CodeAnalysis;
    using Assurity.AgentPortal.Managers.PolicyInfo.Mapping;
    using Assurity.AgentPortal.Managers.Tests.Mapping.TestData;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using AutoMapper;
    using KellermanSoftware.CompareNetObjects;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class AddressMappingProfileTests
    {
        public AddressMappingProfileTests()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.AddProfile<AddressMappingProfile>();
            }).CreateMapper();
        }

        private IMapper Mapper { get; }

        [Fact]
        public void AssertConfigurationIsValid()
        {
            // Assert
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [Fact]
        public void Address_To_Address_ShouldMap()
        {
            // Arrange
            var address = PolicySourceTestData.ActivePolicy.Insureds
                .Single(insured => insured.RelationshipToPrimaryInsured == RelationshipToPrimaryInsured.Self)
                .Participant.Address;

            // Act
            var mappedAddress = Mapper.Map<Address>(address);

            // Assert
            var expectedAddress = PolicyDestinationTestData.ActivePolicy.Insureds
                .Single(insured => insured.RelationshipToPrimaryInsured == RelationshipToPrimaryInsured.Self)
                .Participant.Address;

            var compareResult = new CompareLogic()
                .Compare(expectedAddress, mappedAddress);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
        }

        [Fact]
        public void Address_To_Address_ZipExtensionIsNull_BoxNumberIsNull_ShouldMapNullZipExtension()
        {
            // Arrange & Act
            var mappedAddress = GetMappedAddressForZipExtensionTests(null, null);

            // Assert
            Assert.Null(mappedAddress.ZipExtension);
        }

        [Fact]
        public void Address_To_Address_ZipExtensionIsEmpty_BoxNumberIsNull_ShouldMapNullZipExtension()
        {
            // Arrange & Act
            var mappedAddress = GetMappedAddressForZipExtensionTests(string.Empty, null);

            // Assert
            Assert.Null(mappedAddress.ZipExtension);
        }

        [Fact]
        public void Address_To_Address_ZipExtensionIsNull_BoxNumberIsEmpty_ShouldMapNullZipExtension()
        {
            // Arrange & Act
            var mappedAddress = GetMappedAddressForZipExtensionTests(null, string.Empty);

            // Assert
            Assert.Null(mappedAddress.ZipExtension);
        }

        [Fact]
        public void Address_To_Address_ZipExtensionIsEmpty_BoxNumberIsEmpty_ShouldMapNullZipExtension()
        {
            // Arrange & Act
            var mappedAddress = GetMappedAddressForZipExtensionTests(string.Empty, string.Empty);

            // Assert
            Assert.Null(mappedAddress.ZipExtension);
        }

        [Fact]
        public void Address_To_Address_ZipExtensionIsLessThan4Digits_BoxNumberIs4Digits_ShouldMapBoxNumberToZipExtension()
        {
            // Arrange & Act
            var mappedAddress = GetMappedAddressForZipExtensionTests("123", "5678");

            // Assert
            Assert.Equal("5678", mappedAddress.ZipExtension);
        }

        [Fact]
        public void Address_To_Address_ZipExtensionIsGreaterThan4Digits_BoxNumberIs4Digits_ShouldMapBoxNumberToZipExtension()
        {
            // Arrange & Act
            var mappedAddress = GetMappedAddressForZipExtensionTests("12345", "5678");

            // Assert
            Assert.Equal("5678", mappedAddress.ZipExtension);
        }

        [Fact]
        public void Address_To_Address_ZipExtensionIsEmpty_BoxNumberIs4Digits_ShouldMapBoxNumberToZipExtension()
        {
            // Arrange & Act
            var mappedAddress = GetMappedAddressForZipExtensionTests(string.Empty, "5678");

            // Assert
            Assert.Equal("5678", mappedAddress.ZipExtension);
        }

        [Fact]
        public void Address_To_Address_ZipExtensionIsEmpty_BoxNumberIsLessThan4Digits_ShouldMapNullToZipExtension()
        {
            // Arrange & Act
            var mappedAddress = GetMappedAddressForZipExtensionTests(string.Empty, "567");

            // Assert
            Assert.Null(mappedAddress.ZipExtension);
        }

        [Fact]
        public void Address_To_Address_ZipExtensionIsEmpty_BoxNumberIsGreaterThan4Digits_ShouldMapNullToZipExtension()
        {
            // Arrange & Act
            var mappedAddress = GetMappedAddressForZipExtensionTests(string.Empty, "56789");

            // Assert
            Assert.Null(mappedAddress.ZipExtension);
        }

        [Fact]
        public void Address_To_Address_ZipExtensionIs4Digits_ShouldMapZipExtension()
        {
            // Arrange & Act
            var mappedAddress = GetMappedAddressForZipExtensionTests("1234", null);

            // Assert
            Assert.Equal("1234", mappedAddress.ZipExtension);
        }

        [Fact]
        public void Address_To_Address_ZipExtensionIs4Digits_BoxNumberIs4Digits_ShouldMapZipExtension()
        {
            // Arrange & Act
            var mappedAddress = GetMappedAddressForZipExtensionTests("1234", "5678");

            // Assert
            Assert.Equal("1234", mappedAddress.ZipExtension);
        }

        private Address GetMappedAddressForZipExtensionTests(string zipExtension, string boxNumber)
        {
            // Arrange
            var address = PolicySourceTestData.ActivePolicy.Insureds
                .Single(insured => insured.RelationshipToPrimaryInsured == RelationshipToPrimaryInsured.Self)
                .Participant.Address;

            address.ZipExtension = zipExtension;
            address.BoxNumber = boxNumber;

            // Act
            var mappedAddress = Mapper.Map<Address>(address);

            // Assert
            Assert.NotNull(mappedAddress);

            return mappedAddress;
        }
    }
}