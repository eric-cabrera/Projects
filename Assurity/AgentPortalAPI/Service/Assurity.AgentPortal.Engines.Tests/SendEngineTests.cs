namespace Assurity.AgentPortal.Engines.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Assurity.AgentPortal.Accessors.Send;
using Assurity.AgentPortal.Contracts.Send;
using Assurity.AgentPortal.Contracts.Send.Enums;
using Assurity.AgentPortal.Utilities.Configs;
using Assurity.AgentPortal.Utilities.TiffCreation;
using Microsoft.Extensions.Logging;
using Moq;
using SkiaSharp;
using Xunit;

[ExcludeFromCodeCoverage]
public class SendEngineTests
{
    private static byte[] PdfBytes => new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D };

    private static byte[] TifBytes => new byte[] { 0x49, 0x49, 0x2A, 0x00 };

    private static byte[] TiffBytes => new byte[] { 0x4D, 0x4D, 0x00, 0x2A };

    [Fact]
    public async Task CreateImageFiles_NullFiles_ShouldReturnNull()
    {
        // Arrange
        var sendEngine = new SendEngine(null!, null!, null!);

        // Act
        var fileDTOs = await sendEngine.CreateImageFiles(null!);

        // Assert
        Assert.Null(fileDTOs);
    }

    [Fact]
    public async Task CreateImageFiles_EmptyFiles_ShouldReturnNull()
    {
        // Arrange
        var sendEngine = new SendEngine(null!, null!, null!);

        // Act
        var fileDTOs = await sendEngine.CreateImageFiles(new List<File>());

        // Assert
        Assert.Null(fileDTOs);
    }

    [Fact]
    public async Task CreateImageFiles_Jpeg_ShouldReturnFileDTOs()
    {
        // Arrange
        var expectedTiffImageBase64Content = "SUkqAAoAAAD8AA8AAAEDAAEAAAABAAAAAQEDAAEAAAABAAAAAgEDAAEAAAAIAAAAAwEDAAEAAAABAAAABgEDAAEAAAABAAAACgEDAAEAAAABAAAAEQEEAAEAAAAIAAAAEgEDAAEAAAABAAAAFQEDAAEAAAABAAAAFgEDAAEAAAABAAAAFwEEAAEAAAABAAAAHAEDAAEAAAABAAAAKQEDAAIAAAAAAAEAPgEFAAIAAAD0AAAAPwEFAAYAAADEAAAAAAAAAIXrUQAAAIAAw/WoAAAAAALNzEwAAAAAAc3MTAAAAIAAzcxMAAAAAAKPwvUAAAAAEDcaoAAAAAACK4cKAAAAIAA=";

        // Act, and Assert
        await TestCreateImageFiles_Images_ShouldReturnFileDTOs(
            FileType.Jpeg,
            expectedTiffImageBase64Content);
    }

    [Fact]
    public async Task CreateImageFiles_Jpg_ShouldReturnFileDTOs()
    {
        // Arrange
        var expectedTiffImageBase64Content = "SUkqAAoAAAD8AA8AAAEDAAEAAAABAAAAAQEDAAEAAAABAAAAAgEDAAEAAAAIAAAAAwEDAAEAAAABAAAABgEDAAEAAAABAAAACgEDAAEAAAABAAAAEQEEAAEAAAAIAAAAEgEDAAEAAAABAAAAFQEDAAEAAAABAAAAFgEDAAEAAAABAAAAFwEEAAEAAAABAAAAHAEDAAEAAAABAAAAKQEDAAIAAAAAAAEAPgEFAAIAAAD0AAAAPwEFAAYAAADEAAAAAAAAAIXrUQAAAIAAw/WoAAAAAALNzEwAAAAAAc3MTAAAAIAAzcxMAAAAAAKPwvUAAAAAEDcaoAAAAAACK4cKAAAAIAA=";

        // Act, and Assert
        await TestCreateImageFiles_Images_ShouldReturnFileDTOs(
            FileType.Jpg,
            expectedTiffImageBase64Content);
    }

    [Fact]
    public async Task CreateImageFiles_Png_ShouldReturnFileDTOs()
    {
        // Arrange
        var expectedTiffImageBase64Content = "SUkqAAoAAAD8AA8AAAEDAAEAAAABAAAAAQEDAAEAAAABAAAAAgEDAAEAAAAIAAAAAwEDAAEAAAABAAAABgEDAAEAAAABAAAACgEDAAEAAAABAAAAEQEEAAEAAAAIAAAAEgEDAAEAAAABAAAAFQEDAAEAAAABAAAAFgEDAAEAAAABAAAAFwEEAAEAAAABAAAAHAEDAAEAAAABAAAAKQEDAAIAAAAAAAEAPgEFAAIAAAD0AAAAPwEFAAYAAADEAAAAAAAAAIXrUQAAAIAAw/WoAAAAAALNzEwAAAAAAc3MTAAAAIAAzcxMAAAAAAKPwvUAAAAAEDcaoAAAAAACK4cKAAAAIAA=";

        // Act, and Assert
        await TestCreateImageFiles_Images_ShouldReturnFileDTOs(
            FileType.Png,
            expectedTiffImageBase64Content);
    }

    [Fact]
    public async Task CreateImageFiles_Pdf_ShouldReturnFileDTOs()
    {
        // Arrange
        var mockPdfToTiffAccessor = new Mock<IPdfToTiffAccessor>(MockBehavior.Strict);
        mockPdfToTiffAccessor
            .Setup(pdfToTiffAccessor => pdfToTiffAccessor.GetTiffFromPdfAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>()))
            .ReturnsAsync(TiffBytes);

        var sendEngine = new SendEngine(null!, mockPdfToTiffAccessor.Object, null);

        var files = new List<File>
        {
            new File
            {
                Bytes = PdfBytes,
                FileType = FileType.Pdf,
                Name = "TestFile"
            }
        };

        // Act
        var fileDTOs = await sendEngine.CreateImageFiles(files);

        // Assert
        Assert.NotNull(fileDTOs);
        Assert.False(fileDTOs[0].IsMessage);
        Assert.Equal("TestFile", fileDTOs[0].Name);
        Assert.Equal("TU0AKg==", fileDTOs[0].TiffImageBase64Content);
    }

    [Fact]
    public async Task CreateImageFiles_Tif_ShouldReturnFileDTOs()
    {
        await TestCreateImageFiles_TifAndTiff_ShouldReturnFileDTOs(
            FileType.Tif,
            TifBytes,
            "SUkqAA==");
    }

    [Fact]
    public async Task CreateImageFiles_Tiff_ShouldReturnFileDTOs()
    {
        await TestCreateImageFiles_TifAndTiff_ShouldReturnFileDTOs(
            FileType.Tiff,
            TiffBytes,
            "TU0AKg==");
    }

    [Fact]
    public async Task CreateImageFiles_Txt_ShouldReturnFileDTOs()
    {
        // Arrange
        var mockTiffCreator = new Mock<ITiffCreator>(MockBehavior.Strict);

        // Setup the mock to return a Task<string> instead of a plain string
        mockTiffCreator
            .Setup(tiffCreator => tiffCreator.CreateMultiPage(It.IsAny<string[]>()))
            .ReturnsAsync("TiffBase64Content");

        var sendEngine = new SendEngine(null, null, mockTiffCreator.Object);

        var filePath = $"{Assembly.GetExecutingAssembly().Location}TestFiles\\test.txt";
        filePath = filePath.Replace("Assurity.AgentPortal.Engines.Tests.dll", string.Empty);

        var imageBytes = await System.IO.File.ReadAllBytesAsync(filePath);

        var files = new List<File>
    {
        new File
        {
            Bytes = imageBytes,
            FileType = FileType.Txt,
            Name = "TestFile"
        }
    };

        // Act
        var fileDTOs = await sendEngine.CreateImageFiles(files);

        // Assert
        Assert.NotNull(fileDTOs);
        Assert.False(fileDTOs[0].IsMessage);
        Assert.Equal("TestFile", fileDTOs[0].Name);
        Assert.Equal("TiffBase64Content", fileDTOs[0].TiffImageBase64Content);
    }

    [Fact]
    public async Task CreateImageFiles_NullTiffBase64Content_ShouldThrowException()
    {
        // Arrange
        var mockTiffCreator = new Mock<ITiffCreator>(MockBehavior.Strict);
        Task<string?> task = Task.Run(() =>
        {
            return Task.FromResult(null as string);
        });
        mockTiffCreator
            .Setup(tiffCreator => tiffCreator.CreateMultiPage(It.IsAny<string[]>()))
            .Returns(task!);

        var sendEngine = new SendEngine(null!, null!, mockTiffCreator.Object);

        var filePath = $"{Assembly.GetExecutingAssembly().Location}TestFiles\\test.txt";
        filePath = filePath.Replace("Assurity.AgentPortal.Engines.Tests.dll", string.Empty);

        var imageBytes = await System.IO.File.ReadAllBytesAsync(filePath);

        var files = new List<File>
        {
            new File
            {
                Bytes = imageBytes,
                FileType = FileType.Txt,
                Name = "TestFile"
            }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            async () => await sendEngine.CreateImageFiles(files));

        var expectedMessage = "Failed to convert the image's base64 string to a tiff " +
            $"base64 string for {nameof(FileType)}: {FileType.Txt}.";

        Assert.NotNull(exception);
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public async Task CreateImageFiles_EmptyTiffBase64Content_ShouldThrowException()
    {
        // Arrange
        var mockPdfToTiffAccessor = new Mock<IPdfToTiffAccessor>(MockBehavior.Strict);
        mockPdfToTiffAccessor
            .Setup(pdfToTiffAccessor => pdfToTiffAccessor.GetTiffFromPdfAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>()))
            .ReturnsAsync(Array.Empty<byte>());

        var sendEngine = new SendEngine(null, mockPdfToTiffAccessor.Object, null);

        var files = new List<File>
        {
            new File
            {
                Bytes = PdfBytes,
                FileType = FileType.Pdf,
                Name = "TestFile"
            }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            async () => await sendEngine.CreateImageFiles(files));

        var expectedMessage = "Failed to convert the image's base64 string to a tiff " +
            $"base64 string for {nameof(FileType)}: {FileType.Pdf}.";

        Assert.NotNull(exception);
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public async Task CreateJsonMessageFile_ValidJson_ShouldReturnFileDto()
    {
        // Arrange
        var mockConfigManager = new Mock<IConfigurationManager>();
        var mockPdfToTiffAccessor = new Mock<IPdfToTiffAccessor>();
        var mockTiffCreator = new Mock<ITiffCreator>();
        var expectedBase64Content = "base64content";

        mockTiffCreator
            .Setup(tc => tc.CreateTiffWithCustomStyling(It.IsAny<string>()))
            .ReturnsAsync(expectedBase64Content);

        var sendEngine = new SendEngine(mockConfigManager.Object, mockPdfToTiffAccessor.Object, mockTiffCreator.Object);
        var messageJson = "{ \"key\": \"value\" }";

        // Act
        var result = await sendEngine.CreateJsonMessageFile(messageJson);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsMessage);
        Assert.Equal(expectedBase64Content, result.TiffImageBase64Content);
    }

    [Fact]
    public async Task CreateTiffWithCustomStyling_ValidJson_ShouldReturnBase64Tiff()
    {
        // Arrange
        var mockTiffCreator = new Mock<ITiffCreator>();
        var validBase64Content = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/wcAAwAB/8I/AAAAAElFTkSuQmCC"; // Example Base64 content of a small 1x1 pixel PNG image

        mockTiffCreator
            .Setup(tc => tc.CreateTiffWithCustomStyling(It.IsAny<string>()))
            .ReturnsAsync(validBase64Content);

        var tiffCreator = mockTiffCreator.Object;
        var validJson = @"
        {
                'title': 'Test Title',
                'headtitle': 'Test Heading',
                'message': {
                    'value': 'Test Message Content'
                },
                'AdditionalInfo': {
                    'value': 'Value of Additional Info',
                    'subValue': 'Sub-value of Additional Info'
                }
            }";

        // Act
        var result = await tiffCreator.CreateTiffWithCustomStyling(validJson);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result));

        byte[] imageBytes = Convert.FromBase64String(result);
        using (var ms = new MemoryStream(imageBytes))
        {
            using (var skImage = SKImage.FromEncodedData(ms))
            {
                Assert.NotNull(skImage);

                using (var bitmap = SKBitmap.FromImage(skImage))
                {
                    Assert.NotNull(bitmap);
                    Assert.Equal(1, bitmap.Width);
                    Assert.Equal(1, bitmap.Height);
                }
            }
        }
    }

    [Fact]
    public async Task CreateImageFiles_UnsupportedFileType_ShouldThrowNotImplementedException()
    {
        // Arrange
        var sendEngine = new SendEngine(null, null, null);

        var files = new List<File>
        {
            new File
            {
                Bytes = Array.Empty<byte>(),
                FileType = (FileType)4000,
                Name = "TestFile"
            }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotImplementedException>(
            async () => await sendEngine.CreateImageFiles(files));

        var expectedMessage = $"Tiff conversion for file type {files[0].FileType} is not supported.";

        Assert.NotNull(exception);
        Assert.Equal(expectedMessage, exception.Message);
    }

    private static async Task TestCreateImageFiles_Images_ShouldReturnFileDTOs(
        FileType fileType,
        string expectedTiffImageBase64Content)
    {
        // Arrange
        var sendEngine = new SendEngine(null, null, null);

        var filePath = $"{Assembly.GetExecutingAssembly().Location}" +
            $"TestFiles\\test.{fileType.ToString().ToLowerInvariant()}";
        filePath = filePath.Replace("Assurity.AgentPortal.Engines.Tests.dll", string.Empty);

        var imageBytes = await System.IO.File.ReadAllBytesAsync(filePath);

        var imageBase64 = Convert.ToBase64String(imageBytes);

        var files = new List<File>
        {
            new File
            {
                Bytes = imageBytes,
                FileType = fileType,
                Name = "TestFile"
            }
        };

        // Act
        var fileDTOs = await sendEngine.CreateImageFiles(files);

        // Assert
        Assert.NotNull(fileDTOs);
        Assert.False(fileDTOs[0].IsMessage);
        Assert.Equal("TestFile", fileDTOs[0].Name);
        Assert.Equal(expectedTiffImageBase64Content, fileDTOs[0].TiffImageBase64Content);
    }

    private async Task TestCreateImageFiles_TifAndTiff_ShouldReturnFileDTOs(
        FileType fileType,
        byte[] imageBytes,
        string expectedTiffImageBase64Content)
    {
        // Arrange
        var sendEngine = new SendEngine(null, null, null);

        var files = new List<File>
        {
            new File
            {
                Bytes = imageBytes,
                FileType = fileType,
                Name = "TestFile"
            }
        };

        // Act
        var fileDTOs = await sendEngine.CreateImageFiles(files);

        // Assert
        Assert.NotNull(fileDTOs);
        Assert.False(fileDTOs[0].IsMessage);
        Assert.Equal("TestFile", fileDTOs[0].Name);
        Assert.Equal(expectedTiffImageBase64Content, fileDTOs[0].TiffImageBase64Content);
    }
}