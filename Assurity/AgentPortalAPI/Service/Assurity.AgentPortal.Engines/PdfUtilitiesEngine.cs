namespace Assurity.AgentPortal.Engines;

using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

public class PdfUtilitiesEngine : IPdfUtilitiesEngine
{
    public MemoryStream EnsurePortraitOrientation(MemoryStream memoryStream)
    {
        // Load the PDF document
        PdfDocument document = PdfReader.Open(memoryStream, PdfDocumentOpenMode.Modify);

        // Iterate through each page to check and set orientation
        foreach (PdfPage page in document.Pages)
        {
            // Rotate the page to portrait
            page.Rotate = 90;
        }

        document.Save(memoryStream, false);

        // Reset the position of the MemoryStream to the beginning
        memoryStream.Position = 0;

        // Return the MemoryStream
        return memoryStream;
    }
}
