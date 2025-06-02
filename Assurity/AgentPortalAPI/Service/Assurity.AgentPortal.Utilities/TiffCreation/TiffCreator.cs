namespace Assurity.AgentPortal.Utilities.TiffCreation;

using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using BitMiracle.LibTiff.Classic;
using Newtonsoft.Json.Linq;
using SkiaSharp;

public class TiffCreator : ITiffCreator
{
    public Task<string> CreateMultiPage(string[] text)
    {
        var config = SetTiffConfiguration();
        return CreateMultiPage(config, text);
    }

    public async Task<string> CreateTiffWithCustomStyling(string messageJson)
    {
        var config = SetTiffConfiguration();
        int width = 800;
        int height = 1000;

        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(config.BackgroundColor);

        var headerFont = new SKPaint
        {
            Color = config.TextColor,
            TextSize = 28,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Serif", SKFontStyle.Bold)
        };

        var headingFont = new SKPaint
        {
            Typeface = SKTypeface.FromFamilyName("Serif", SKFontStyle.Bold),
            TextSize = 24,
            Color = config.TextColor
        };

        var normalFont = new SKPaint
        {
            Typeface = SKTypeface.FromFamilyName("Serif", SKFontStyle.Normal),
            TextSize = 24,
            Color = config.TextColor,
            TextAlign = SKTextAlign.Left,
            Style = SKPaintStyle.Fill
        };

        var json = JObject.Parse(messageJson);
        float yOffset = 30;

        string title = json["title"]?.ToString() ?? "Default Title";
        canvas.DrawText(ToTitleCase(title), 10, yOffset, headerFont);
        yOffset += 40;

        var messageObj = json["message"] as JObject;
        string messageValue = messageObj?["value"]?.ToString().Trim() ?? string.Empty;
        messageValue = CleanMessage(messageValue);

        yOffset = await DrawWrappedTextAsync(canvas, messageValue, normalFont, width, yOffset);
        yOffset += 20;

        string headTitle = json["headTitle"]?.ToString() ?? "Default Heading";
        canvas.DrawText(ToTitleCase(headTitle), 10, yOffset, headingFont);
        yOffset += 40;

        foreach (var property in json.Properties())
        {
            if (property.Name == "message" || property.Name == "title" || property.Name == "headTitle")
            {
                continue;
            }

            string heading = ToTitleCase(property.Name);
            var valueObj = property.Value as JObject;
            string value = CleanMessage(valueObj?["value"]?.ToString().Trim() ?? string.Empty);
            string subValue = CleanMessage(valueObj?["subValue"]?.ToString().Trim() ?? string.Empty);

            if (heading == "Requirements/Comments" || heading == "Requirement Applies To" || heading == "Met")
            {
                value = ToTitleCase(value);
            }

            canvas.DrawText($"{heading}: ", 10, yOffset, headingFont);
            yOffset += 5;

            canvas.DrawText(value, 10 + headingFont.MeasureText($"{heading}: "), yOffset - 5, normalFont);
            yOffset += 30;

            if (!string.IsNullOrEmpty(subValue))
            {
                yOffset = DrawStringWithWrapping(canvas, subValue, normalFont, new SKRect(10, yOffset, width - 20, height - yOffset));
                yOffset += 20;
            }
        }

        return await GenerateTiffFromBitmapAsync(bitmap);
    }

    private static Task<string> GenerateTiffFromBitmapAsync(SKBitmap skBitmap)
    {
        using var memoryStream = new MemoryStream();
        using var tiff = Tiff.ClientOpen("InMemory", "w", memoryStream, new TiffStream());

        tiff.SetField(TiffTag.IMAGEWIDTH, 800);
        tiff.SetField(TiffTag.IMAGELENGTH, 1000);
        tiff.SetField(TiffTag.SAMPLESPERPIXEL, 4);
        tiff.SetField(TiffTag.BITSPERSAMPLE, 8);
        tiff.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
        tiff.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);
        tiff.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
        tiff.SetField(TiffTag.ROWSPERSTRIP, 1000);
        tiff.SetField(TiffTag.COMPRESSION, Compression.LZW);

        byte[] pixels = skBitmap.Bytes;
        for (int i = 0; i < 1000; i++)
        {
            tiff.WriteScanline(pixels, i * 800 * 4, i, 0);
        }

        tiff.WriteDirectory();

        byte[] tiffData = memoryStream.ToArray();

        string toBase64String = Convert.ToBase64String(tiffData);

        return Task.FromResult(toBase64String);
    }

    private static float DrawStringWithWrapping(SKCanvas canvas, string text, SKPaint paint, SKRect bounds)
    {
        var lines = WrapText(text, paint, bounds.Width);
        float yOffset = bounds.Top;

        foreach (var line in lines)
        {
            canvas.DrawText(line, bounds.Left, yOffset, paint);
            yOffset += paint.TextSize;
        }

        return yOffset;
    }

    private static List<string> WrapText(string text, SKPaint paint, float maxWidth)
    {
        var lines = new List<string>();
        var words = text.Split(' ');
        var currentLine = new StringBuilder();

        foreach (var word in words)
        {
            var testLine = new StringBuilder(currentLine.ToString()).Append(word).Append(' ');
            if (paint.MeasureText(testLine.ToString()) > maxWidth)
            {
                lines.Add(currentLine.ToString());
                currentLine.Clear().Append(word).Append(' ');
            }
            else
            {
                currentLine.Append(word).Append(' ');
            }
        }

        if (currentLine.Length > 0)
        {
            lines.Add(currentLine.ToString());
        }

        return lines;
    }

    private static string ToTitleCase(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        return textInfo.ToTitleCase(text.ToLower());
    }

    private static TiffConfiguration SetTiffConfiguration()
    {
        return new TiffConfiguration
        {
            XAxisDpi = 200,
            YAxisDpi = 200,
            Font = new FontConfiguration
            {
                FontFamily = "Serif",
                Size = 10
            },
            TextColor = new SKColor(0, 0, 0),
            BackgroundColor = new SKColor(255, 255, 255)
        };
    }

    private static async Task<string> CreateMultiPage(TiffConfiguration tiffConfiguration, string[] messages)
    {
        string message = string.Join(" ", messages);
        message = CleanMessage(message);

        var config = tiffConfiguration;
        int width = 800;

        var normalFont = new SKPaint
        {
            Typeface = SKTypeface.FromFamilyName("Serif", SKFontStyle.Normal),
            TextSize = 22,
            Color = config.TextColor,
            TextAlign = SKTextAlign.Left,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        using var bitmap = new SKBitmap(width, 1000);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(new SKColor(255, 255, 255));

        float initialYOffset = 30;
        await DrawWrappedTextAsync(canvas, message, normalFont, width, initialYOffset);

        string bitmapFilePath = Path.Combine(@"C:\TIFFImages2", "bitmapImage.png");
        return await GenerateTiffFromBitmapAsync(bitmap);
    }

    private static async Task<float> DrawWrappedTextAsync(SKCanvas canvas, string message, SKPaint font, int width, float initialYOffset)
    {
        var wrappedLines = WrapText(message, font, width);
        float yOffset = initialYOffset;

        foreach (var line in wrappedLines)
        {
            canvas.DrawText(line, 10, yOffset, font);
            yOffset += font.TextSize;
            await Task.Yield();
        }

        return yOffset;
    }

    private static string CleanMessage(string message)
    {
        return Regex.Replace(message, @"[^\u0020-\u007E\u00A0-\uFFFF]", string.Empty);
    }
}
