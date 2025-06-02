namespace Assurity.AgentPortal.Accessors.Send;

using Assurity.PdfToTiffConversion.Client;

public class PdfToTiffAccessor : IPdfToTiffAccessor
{
    public PdfToTiffAccessor(IPdfToTiffClient pdfToTiffClient)
    {
        PdfToTiffClient = pdfToTiffClient;
    }

    private IPdfToTiffClient PdfToTiffClient { get; }

    public async Task<byte[]> GetTiffFromPdfAsync(string fileName, byte[] pdfBytes)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = Guid.NewGuid().ToString();
        }

        var response = await PdfToTiffClient.PostAsync(fileName, pdfBytes);

        if (response.IsSuccessStatusCode)
        {
            return response.Bytes;
        }

        throw new Exception(
            $"Error calling {response.HttpServiceName}, " +
            $"Status Code: {response.HttpStatusCode}, " +
            $"Message: {response.HttpResponseError?.ExceptionMessage}");
    }
}