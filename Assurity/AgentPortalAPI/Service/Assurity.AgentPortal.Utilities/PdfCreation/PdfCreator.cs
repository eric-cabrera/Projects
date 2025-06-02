namespace Assurity.AgentPortal.Utilities.PdfCreation;

using System.Xml.Linq;
using ImageMagick;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

public class PdfCreator : IPdfCreator
{
    private readonly XNamespace ns = "http://schemas.servicestack.net/types";

    public byte[] GetPdfFromPages(IEnumerable<XElement> pages)
    {
        using PdfDocument pdfDocument = new();

        foreach (var page in pages)
        {
            var pdfBytes = Convert.FromBase64String(page.Element(ns + "Data").Value);

            using var src = new MemoryStream(pdfBytes);
            using var srcPdf = PdfReader.Open(src, PdfDocumentOpenMode.Import);

            foreach (var pdfPage in srcPdf.Pages)
            {
                pdfDocument.AddPage(pdfPage);
            }
        }

        using var pdfStream = new MemoryStream();
        pdfDocument.Save(pdfStream);

        return pdfStream.ToArray();
    }

    public byte[] ConvertTiffToPdf(IEnumerable<XElement> pages)
    {
        using PdfDocument pdfDocument = new();

        foreach (var page in pages)
        {
            var tiff = Convert.FromBase64String(page.Element(ns + "Data").Value);
            using var images = new MagickImageCollection(tiff);

            foreach (var image in images)
            {
                image.Format = MagickFormat.Jpeg;

                using var pageStream = new MemoryStream();
                image.Write(pageStream);

                using XImage xImageForPage = XImage.FromStream(pageStream);
                var pdfPage = new PdfPage
                {
                    Width = XUnit.FromPoint(xImageForPage.PointWidth),
                    Height = XUnit.FromPoint(xImageForPage.PointHeight),
                };

                pdfDocument.Pages.Add(pdfPage);
                using XGraphics xgr = XGraphics.FromPdfPage(pdfPage);
                xgr.DrawImage(xImageForPage, 0, 0);
            }
        }

        using var pdfStream = new MemoryStream();
        pdfDocument.Save(pdfStream);

        return pdfStream.ToArray();
    }
}