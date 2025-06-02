namespace Assurity.AgentPortal.Service.Handlers;

/// <summary>
/// This handler will check for certain response messages that can be returned from the API and throws the appropriate exception depending upon the response.
/// </summary>
public class ErrorResponseHandler : DelegatingHandler
{
    /// <summary>
    /// Override SendAsync to check for certain response messages that can be returned from the API and throw an exception depending upon the response.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The HttpResponseMessage.</returns>
    /// <exception cref="Exception">Either the API is down or returned a response we have not accounted for.</exception>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpResponseMessage = await base.SendAsync(request, cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            return httpResponseMessage;
        }

        var stringContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        // A 404 is returned if the site is unable to find your data or unable to find the endpoint (503 is returned if the API itself is down)
        if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // If the response body is null or empty, that indicates the URL is bad.
            if (string.IsNullOrEmpty(stringContent))
            {
                throw new Exception($"Failed to reach {httpResponseMessage.RequestMessage?.RequestUri} | Status Code: {httpResponseMessage.StatusCode}");
            }

            // If the response body has content, that indicates the API could not find any data for your call. Return the response to generate a default response from the API.
            return httpResponseMessage;
        }

        throw new Exception($"An error occurred while attempting to reach {httpResponseMessage.RequestMessage?.RequestUri} | Returned response: {httpResponseMessage.StatusCode} | {stringContent}");
    }
}
