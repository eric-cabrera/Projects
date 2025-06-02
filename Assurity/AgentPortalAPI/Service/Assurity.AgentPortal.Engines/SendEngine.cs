namespace Assurity.AgentPortal.Engines;

using System.IO;
using System.Text;
using Assurity.AgentPortal.Accessors.Send;
using Assurity.AgentPortal.Contracts.Send.Enums;
using Assurity.AgentPortal.Engines;
using Assurity.AgentPortal.Utilities.Configs;
using Assurity.AgentPortal.Utilities.TiffCreation;
using Assurity.AgentPortal.Utilities.WordWrap;
using ImageMagick;
using SkiaSharp;
using DTOs = Assurity.AgentPortal.Accessors.DTOs;
using File = Assurity.AgentPortal.Contracts.Send.File;

public class SendEngine : ISendEngine
{
    public SendEngine(
        IConfigurationManager configurationManager,
        IPdfToTiffAccessor pdfToTiffAccessor,
        ITiffCreator tiffCreator)
    {
        ConfigurationManager = configurationManager;
        PdfToTiffAccessor = pdfToTiffAccessor;
        TiffCreator = tiffCreator;
    }

    private IConfigurationManager ConfigurationManager { get; }

    private IPdfToTiffAccessor PdfToTiffAccessor { get; }

    private ITiffCreator TiffCreator { get; }

    public async Task<List<DTOs.File>> CreateImageFiles(List<File> files)
    {
        if (!(files?.Any() ?? false))
        {
            return null;
        }

        var imageFiles = new List<DTOs.File>();

        foreach (var file in files)
        {
            string? tiffImageBase64Content = null;

            // TODO: Rework to support Doc/Docx conversion to Tiff without Microsoft.Office.Interop.Word depencency.
            switch (file.FileType)
            {
                case FileType.Jpeg:
                case FileType.Jpg:
                case FileType.Png:
                    var tiffBytesConvertedFromImage = ConvertImageToTiffBytes(file.Bytes);

                    tiffImageBase64Content = Convert.ToBase64String(tiffBytesConvertedFromImage);

                    break;
                case FileType.Pdf:
                    var tiffBytesConvertedFromPdf = await PdfToTiffAccessor
                        .GetTiffFromPdfAsync(file.Name, file.Bytes);

                    tiffImageBase64Content = Convert.ToBase64String(tiffBytesConvertedFromPdf);

                    break;
                case FileType.Tif:
                case FileType.Tiff:
                    tiffImageBase64Content = Convert.ToBase64String(file.Bytes);

                    break;
                case FileType.Txt:
                    var tiffText = new List<string>
                    {
                        Encoding.UTF8.GetString(file.Bytes)
                    };

                    tiffImageBase64Content = await TiffCreator.CreateMultiPage(tiffText.ToArray());

                    break;
                default:
                    throw new NotImplementedException(
                        $"Tiff conversion for file type {file.FileType} is not supported.");
            }

            if (string.IsNullOrWhiteSpace(tiffImageBase64Content))
            {
                throw new Exception("Failed to convert the image's base64 string to a tiff base64 string " +
                    $"for {nameof(FileType)}: {file.FileType}.");
            }

            imageFiles.Add(new DTOs.File
            {
                Name = file.Name,
                TiffImageBase64Content = tiffImageBase64Content
            });
        }

        return imageFiles;
    }

    public async Task<DTOs.File> CreateMessageFile(string message)
    {
        var wordWrappedMessage = WordWrap.Wrap(message, 100);
        string[] messageArray = new string[] { new string(wordWrappedMessage) };

        string result = await TiffCreator.CreateMultiPage(messageArray);

        return new DTOs.File
        {
            IsMessage = true,
            Name = "messageFile.tiff",
            TiffImageBase64Content = result
        };
    }

    public async Task<DTOs.File> CreateJsonMessageFile(string messageJson)
    {
        string result = await TiffCreator.CreateTiffWithCustomStyling(messageJson);
        return new DTOs.File
        {
            IsMessage = true,
            Name = "messageJsonFile.tiff",
            TiffImageBase64Content = result
        };
    }

    /// <summary>
    /// Converts an image (jpeg, jpg, or png) to Tiff and returns the byte array.
    /// This method now uses SkiaSharp for image decoding and manipulation, making it cross-platform.
    /// The image is decoded from the byte array into an SKBitmap object via SkiaSharp, and then
    /// converted to a PNG format. The resulting PNG is processed with Magick.NET to generate the Tiff.
    /// Since the conversion only uses memory, no physical files are needed on disk.
    /// </summary>
    /// <param name="imageBytes">The byte array of the input image.</param>
    /// <returns>A byte array representing the image in Tiff format.</returns>
    private static byte[] ConvertImageToTiffBytes(byte[] imageBytes)
    {
        using var inputStream = new MemoryStream(imageBytes);
        using var skiaImage = SKBitmap.Decode(inputStream);
        using var pngStream = new MemoryStream();

        using var skImage = SKImage.FromBitmap(skiaImage);
        skImage.Encode(SKEncodedImageFormat.Png, 100).SaveTo(pngStream);

        using var magickImageStream = new MemoryStream(pngStream.ToArray());
        using var magickImage = new MagickImage(magickImageStream);

        using var tiffStream = new MemoryStream();
        magickImage.Format = MagickFormat.Tiff;
        magickImage.Write(tiffStream);

        return tiffStream.ToArray();
    }
}