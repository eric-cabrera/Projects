namespace Assurity.AgentPortal.Accessors.PolicyInfo;

using System.Text;
using System.Xml.Linq;
using Assurity.AgentPortal.Utilities.Configs;
using Assurity.AgentPortal.Utilities.PdfCreation;

public class DocumentServiceAccessor : IDocumentServiceAccessor
{
    public DocumentServiceAccessor(
        IConfigurationManager configurationManager,
        HttpClient httpClient,
        IPdfCreator pdfCreator)
    {
        ConfigurationManager = configurationManager;
        HttpClient = httpClient;
        PdfCreator = pdfCreator;
    }

    private IConfigurationManager ConfigurationManager { get; }

    private HttpClient HttpClient { get; }

    private IPdfCreator PdfCreator { get; }

    public async Task<byte[]> GetImageByIdAsync(string policyNumber, string objectClass)
    {
        var isPdf = objectClass == "DOCPDF01";

        var url = ConfigurationManager.DocumentServiceUrl;
        var action = $"{url}?op=FindDocument";

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
        string xmlBody = $@"<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:typ=""http://schemas.servicestack.net/types"">
                <soap:Header/>
                <soap:Body>
                    <typ:FindDocument>
                        <typ:MetaInformation>
                            <typ:PolicyNumber>{policyNumber}</typ:PolicyNumber>
                        </typ:MetaInformation>
                        <typ:ReturnAllPages>false</typ:ReturnAllPages>
                        <typ:ReturnImageData>true</typ:ReturnImageData>
                        <typ:ReturnPageImageData>true</typ:ReturnPageImageData>
                        <typ:ReturnPolicyPagesOnly>true</typ:ReturnPolicyPagesOnly>
                        <typ:ReturnReadyForPrintOnly>true</typ:ReturnReadyForPrintOnly>
                    </typ:FindDocument>
                </soap:Body>
            </soap:Envelope>";

        request.Content = new StringContent(
            xmlBody,
            Encoding.UTF8,
            "text/xml");
        request.Headers.Add("SOAPAction", action);

        var response = await HttpClient.SendAsync(request);

        XNamespace ns = "http://schemas.servicestack.net/types";
        XDocument xDoc = XDocument.Load(await response.Content.ReadAsStreamAsync());

        var pages = xDoc.Descendants(ns + "Page");

        if (isPdf)
        {
            return PdfCreator.GetPdfFromPages(pages);
        }
        else
        {
            return PdfCreator.ConvertTiffToPdf(pages);
        }
    }
}
