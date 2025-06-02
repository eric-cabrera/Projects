namespace Assurity.AgentPortal.Accessors.DocuSign;

using System.Net.Http;
using Assurity.AgentPortal.Accessors.ApplicationTracker;
using Microsoft.Extensions.Logging;

public class DocuSignApiAccessor : IDocuSignApiAccessor
{
    public DocuSignApiAccessor(HttpClient httpClient, ILogger<DocuSignApiAccessor> logger)
    {
        HttpClient = httpClient;
        Logger = logger;
    }

    private HttpClient HttpClient { get; }

    private ILogger<DocuSignApiAccessor> Logger { get; }

    public async Task<bool> ResendEmail(
        string envelopeId,
        CancellationToken cancellationToken = default)
    {
        var url = $"v2/Envelope/{envelopeId}/Account/QuickStart/resend";

        var response = await HttpClient.PutAsync(url, null, cancellationToken);

        var stringContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return false;
    }
}
