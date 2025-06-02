namespace Assurity.AgentPortal.Accessors.Send;

public interface IPdfToTiffAccessor
{
    Task<byte[]> GetTiffFromPdfAsync(string desiredFileName, byte[] pdfByteArray);
}