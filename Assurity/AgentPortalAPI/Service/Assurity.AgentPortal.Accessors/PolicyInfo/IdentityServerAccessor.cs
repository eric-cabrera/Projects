namespace Assurity.AgentPortal.Accessors.PolicyInfo;

using System.Text;
using Assurity.AgentPortal.Accessors.DTOs;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class IdentityServerAccessor : IIdentityServerAccessor
{
    public IdentityServerAccessor(IConfigurationManager configManager, HttpClient httpClient, IMemoryCache cache, ILogger<IdentityServerAccessor> logger)
    {
        ConfigurationManager = configManager;
        HttpClient = httpClient;
        Cache = cache;
        Logger = logger;
    }

    private IConfigurationManager ConfigurationManager { get; }

    private HttpClient HttpClient { get; }

    private IMemoryCache Cache { get; }

    private ILogger<IdentityServerAccessor> Logger { get; }

    public async Task<string?> GetAuthToken(string[] scopes)
    {
        string scopeString = string.Join(",", scopes);

        if (Cache.TryGetValue(scopeString, out string accessToken))
        {
            return accessToken;
        }

        var baseUri = ConfigurationManager.IdentityServerUri;
        var clientId = ConfigurationManager.IdentityServerClientId;
        var clientSecret = ConfigurationManager.IdentityServerClientSecret;

        var body = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("scope", scopeString),
        };

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{baseUri}/connect/token"),
            Content = new FormUrlEncodedContent(body),
        };

        var authHeaderBytes = Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(authHeaderBytes));
        request.Headers.Add("Accept", "application/x-www-form-urlencoded");

        var response = await HttpClient.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var result = JsonConvert.DeserializeObject<TokenResponse>(responseString);

            if (result != null)
            {
                Cache.Set(
                    scopeString,
                    result.AccessToken,
                    TimeSpan.FromSeconds(result.ExpiresIn).Subtract(TimeSpan.FromSeconds(60)));

                return result.AccessToken;
            }
        }

        Logger.LogError("Invalid response from Identity Server. Endpoint: {Endpoint} | Returned response: {StatusCode} | {Content}", response.RequestMessage?.RequestUri, response.StatusCode, responseString);
        return null;
    }
}