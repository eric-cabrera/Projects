namespace Assurity.AgentPortal.Utilities.PdfCreation
{
    using System.Xml.Linq;

    public interface IPdfCreator
    {
        byte[] GetPdfFromPages(IEnumerable<XElement> pages);

        byte[] ConvertTiffToPdf(IEnumerable<XElement> pages);
    }
}
