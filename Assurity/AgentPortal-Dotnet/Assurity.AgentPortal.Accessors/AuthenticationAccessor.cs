namespace Assurity.AgentPortal.Accessors;

using System.Text.Json;
using Assurity.AgentPortal.Contracts;
using Microsoft.Extensions.Logging;

public class AuthenticationAccessor : IAuthenticationAccessor
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILogger logger;

    public AuthenticationAccessor(IHttpClientFactory httpClientFactory, ILogger<AuthenticationAccessor> logger)
    {
        this.httpClientFactory = httpClientFactory;
        this.logger = logger;
    }

    public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken, string clientId, string clientSecret, string scopes, string url, bool isAzureAd)
    {
        var client = httpClientFactory.CreateClient(nameof(AuthenticationAccessor));

        var body = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("refresh_token", refreshToken),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
        };

        if (isAzureAd)
        {
            body.Add(new KeyValuePair<string, string>("scope", scopes));
        }

        var urlEncodedContent = new FormUrlEncodedContent(body);

        var response = await client.PostAsync($"{url}/token", urlEncodedContent);

        if (response.IsSuccessStatusCode)
        {
            logger.LogDebug("Refresh token response successful. {Status}", response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TokenResponse>(responseString);

            return result;
        }

        var errorResponse = await response.Content.ReadAsStringAsync();
        logger.LogError(
            "Refresh token response failed. {Status} | {Response}",
            response.StatusCode,
            errorResponse);

        return null;
    }
}
