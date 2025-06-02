namespace Assurity.AgentPortal.Utilities.Tests.Logging
{
    using System.Diagnostics.CodeAnalysis;
    using Assurity.AgentPortal.Utilities.Logging;
    using Microsoft.AspNetCore.Http;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class HttpRequestMessageValuesProviderTests
    {
        [Fact]
        public void ExtractGuid_NoRequestHeader_ShouldReturnGeneratedGuid()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var requestHeaderValues = new HttpRequestMessageValuesProvider();

            // Act
            var guid = requestHeaderValues
                .ExtractGuid(httpContext.Request.Headers);

            // Assert
            Assert.NotNull(guid);
        }

        [Fact]
        public void ExtractGuid_GuidProvidedInHeaders_ShouldReturnGuidFromRequestHeader()
        {
            // Arrange
            var expectedGuid = Guid.NewGuid();

            var httpContext = new DefaultHttpContext();
            httpContext
                .Request
                .Headers
                .Add("Guid", expectedGuid.ToString());

            var requestHeaderValues = new HttpRequestMessageValuesProvider();

            // Act
            var guid = requestHeaderValues
                .ExtractGuid(httpContext.Request.Headers);

            // Assert
            Assert.Equal(expectedGuid, guid);
        }
    }
}
