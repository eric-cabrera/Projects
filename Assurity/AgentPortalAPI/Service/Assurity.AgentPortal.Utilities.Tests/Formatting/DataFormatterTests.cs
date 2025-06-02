namespace Assurity.AgentPortal.Utilities.Tests.Formatting
{
    using System.Diagnostics.CodeAnalysis;
    using Assurity.AgentPortal.Utilities.Formatting;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class DataFormatterTests
    {
        [Theory]
        [ClassData(typeof(FormatDecimalDataGenerator))]
        public void FormatDecimalAsAPercentage_ShouldReturnCorrectString(decimal? inputValue, string expectedOutput)
        {
            // Act
            var output = DataFormatter.FormatDecimalAsAPercentage(inputValue);

            // Assert
            Assert.Equal(output, expectedOutput);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("1234567890", "(123) 456-7890")]
        [InlineData("1234567890123", "")]
        [InlineData("", "")]
        public void FormatPhoneNumber_ShouldReturnCorrectString(string inputValue, string expectedOutput)
        {
            // Act
            var output = DataFormatter.FormatPhoneNumber(inputValue);

            // Assert
            Assert.Equal(output, expectedOutput);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(FakeEnum.First, "First Fake Enum")]
        [InlineData(FakeEnum.Second, "Second Fake Enum")]
        [InlineData(FakeEnum.Third, "Third Fake Enum")]
        public void GetEnumDisplayName_ShouldReturnCorrectString(FakeEnum? inputValue, string expectedOutput)
        {
            // Act
            var output = DataFormatter.GetEnumDisplayName(inputValue);

            // Assert
            Assert.Equal(output, expectedOutput);
        }

        [Theory]
        [ClassData(typeof(FormatDateDataGenerator))]
        public void FormatDate_ShouldReturnCorrectString(DateTime? inputValue, string dateFormat, string expectedOutput)
        {
            // Act
            var output = DataFormatter.FormatDate(inputValue, dateFormat);

            // Assert
            Assert.Equal(output, expectedOutput);
        }
    }
}
