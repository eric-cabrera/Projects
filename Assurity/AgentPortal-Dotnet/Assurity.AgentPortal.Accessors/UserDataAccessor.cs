namespace Assurity.AgentPortal.Accessors;

using System.Net.Http.Headers;
using System.Text.Json;
using Assurity.AgentPortal.Contracts;
using Microsoft.Extensions.Logging;

public class UserDataAccessor : IUserDataAccessor
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILogger logger;

    public UserDataAccessor(IHttpClientFactory httpClientFactory, ILogger<UserDataAccessor> logger)
    {
        this.httpClientFactory = httpClientFactory;
        this.logger = logger;
    }

    public async Task<List<Market>?> GetProductionCreditBusinessTypes(string accessToken, string? agentId)
    {
        var client = httpClientFactory.CreateClient("AgentPortalAPIHttpClient");

        var request = new HttpRequestMessage(HttpMethod.Get, $"{client.BaseAddress}API/UserData/BusinessTypes");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        if (!string.IsNullOrEmpty(agentId))
        {
            logger.LogDebug("Setting agent id header to {agentId}.", agentId);
            request.Headers.Add("agent-id", agentId);
        }

        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            logger.LogDebug("Getting business types was successful. {Status}", response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<string>?>(responseString);

            logger.LogDebug("Business types response body: {Response}", responseString);

            var businessTypes = new List<Market>();
            if (result != null)
            {
                foreach (string businessType in result)
                {
                    businessTypes.Add((Market)Enum.Parse(typeof(Market), businessType));
                }
            }

            return businessTypes;
        }

        var errorResponse = await response.Content.ReadAsStringAsync();
        logger.LogError(
            "Getting business types failed. {Status} | {Response}",
            response.StatusCode,
            errorResponse);

        return null;
    }

    public async Task<MarketCodeInformationResponse?> GetMarketCodeInformation(string accessToken, string agentId, string marketCode)
    {
        var client = httpClientFactory.CreateClient("AgentPortalAPIHttpClient");

        var request = new HttpRequestMessage(HttpMethod.Get, $"{client.BaseAddress}API/UserData/RegionCodeInformation/{marketCode}");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        if (!string.IsNullOrEmpty(agentId))
        {
            logger.LogDebug("Setting agent id header to {agentId}.", agentId);
            request.Headers.Add("agent-id", agentId);
        }

        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<MarketCodeInformationResponse?>(responseString);
        }

        var errorResponse = await response.Content.ReadAsStringAsync();
        logger.LogError(
            "Getting market code information failed. {Status} | {Response}",
            response.StatusCode,
            errorResponse);

        return null;
    }

    public async Task<AdditionalAgentIds> GetAdditionalAgentIds(string accessToken, string? agentId)
    {
        var client = httpClientFactory.CreateClient("AgentPortalAPIHttpClient");

        var request = new HttpRequestMessage(HttpMethod.Get, $"{client.BaseAddress}API/UserData/additionalagentids");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        if (!string.IsNullOrEmpty(agentId))
        {
            logger.LogDebug("Setting agent id header to {agentId}.", agentId);
            request.Headers.Add("agent-id", agentId);
        }

        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AdditionalAgentIds>(responseString);

            return result ?? new AdditionalAgentIds();
        }

        return new AdditionalAgentIds();
    }
}
