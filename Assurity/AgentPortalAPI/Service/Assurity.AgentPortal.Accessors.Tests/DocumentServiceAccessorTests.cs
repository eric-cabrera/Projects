namespace Assurity.AgentPortal.Accessors.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Xml.Linq;
    using Assurity.AgentPortal.Accessors.PolicyInfo;
    using Assurity.AgentPortal.Utilities.Configs;
    using Assurity.AgentPortal.Utilities.PdfCreation;
    using Moq;
    using Moq.Protected;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class DocumentServiceAccessorTests
    {
        [Fact]
        public async Task GetImageByIdAsync_OkResponse_ShouldReturn_Application()
        {
            // Arrange
            var policyNumber = "4370360984";
            var filePath = new DirectoryInfo("../../../") + @"\XMLData\ResponseData.XML";

            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(File.ReadAllText(filePath)),
            };

            var documentServiceAccessor = GetDocumentServiceAccessor(httpResponseMessage);

            // Act
            var result = await documentServiceAccessor.GetImageByIdAsync(policyNumber, "DSCANO1");

            // Assert
            Assert.NotNull(result);
        }

        private static DocumentServiceAccessor GetDocumentServiceAccessor(HttpResponseMessage httpResponseMessage)
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://ex360ftest/DocumentService/soap12")
            };

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(config => config.DocumentServiceUrl).Returns("http://ex360ftest/DocumentService/soap12");

            var mockPdfCreator = new Mock<IPdfCreator>(MockBehavior.Strict);
            mockPdfCreator.Setup(pdfCreator => pdfCreator.ConvertTiffToPdf(It.IsAny<IEnumerable<XElement>>()))
                .Returns(new byte[100]);
            return new DocumentServiceAccessor(
                mockConfigurationManager.Object,
                mockHttpClient,
                mockPdfCreator.Object);
        }
    }
}
