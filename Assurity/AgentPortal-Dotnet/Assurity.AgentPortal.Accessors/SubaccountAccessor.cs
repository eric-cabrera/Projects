namespace Assurity.AgentPortal.Accessors;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Assurity.AgentPortal.Accessors.ProfilePingDTOs;
using Assurity.AgentPortal.Accessors.SubAccount.DTOs;
using Assurity.AgentPortal.Accessors.SubAccount.DTOs.AgentCenterAPIRequests;
using Assurity.AgentPortal.Accessors.SubAccount.DTOs.AgentCenterAPIResponses;
using Assurity.AgentPortal.Accessors.SubAccount.DTOs.PingResponses;
using Assurity.AgentPortal.Contracts.SubaccountUsers;
using Assurity.AgentPortal.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class SubaccountAccessor(
    HttpClient client,
    IHttpClientFactory httpClientFactory,
    IConfigurationManager configurationManager,
    ILogger<SubaccountAccessor> logger,
    IMemoryCache memoryCache) : ISubaccountAccessor
{
    private readonly HttpClient HttpClient = client;

    private readonly ILogger<SubaccountAccessor> Logger = logger;

    private readonly IMemoryCache cache = memoryCache;

    private IConfigurationManager ConfigurationManager { get; } = configurationManager;

    private HttpClient AgentPortalApiHttpClient { get; } = httpClientFactory.CreateClient("AgentPortalAPIHttpClient");

    public async Task<SubAccountResponseDTO?> GetAllPingSubaccountsAsync(
        string? nextPageLink = null,
        CancellationToken cancellationToken = default)
    {
        await GetBearerToken(cancellationToken);

        var baseAddress = ConfigurationManager.PingOneBaseUrl;
        var envId = ConfigurationManager.PingOneEnvironmentId;
        var endpoint = nextPageLink ?? $"{baseAddress}/environments/{envId}/users?filter=agentID eq \"subaccount\"";

        var response = await HttpClient.GetAsync(endpoint, cancellationToken);
        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var result = JsonSerializer.Deserialize<SubAccountResponseDTO>(responseString);

            return result;
        }

        Logger.LogError(
            "{endpoint} return error.  Response body: {responseBody}",
            endpoint,
            responseString);

        return null;
    }

    public async Task<List<PendingSubaccount>?> GetPendingSubaccountsAsync(string accessToken, string? parentAgentId, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{AgentPortalApiHttpClient.BaseAddress}API/Subaccounts");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        if (!string.IsNullOrEmpty(parentAgentId))
        {
            logger.LogDebug("Setting agent id header to {agentId}.", parentAgentId);
            request.Headers.Add("agent-id", parentAgentId);
        }

        var response = await AgentPortalApiHttpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            logger.LogDebug("Getting Subaccounts was successful. {Status}", response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<PendingSubaccount>?>(responseString);

            logger.LogDebug("Subaccounts response body: {Response}", responseString);

            return result;
        }

        var errorResponse = await response.Content.ReadAsStringAsync();
        logger.LogError(
            "Getting Pending subaccounts failed. {Status} | {Response}",
            response.StatusCode,
            errorResponse);

        return new List<PendingSubaccount>();
    }

    public async Task<UsersResponseDTO?> UpdatePingSubaccountAsync(
        string userId,
        string userName,
        string parentAgentId,
        string parentUserName,
        List<SubAccountRole> roles,
        CancellationToken cancellationToken)
    {
        await GetBearerToken(cancellationToken);

        var endpoint = $"{ConfigurationManager.PingOneBaseUrl}/environments/{ConfigurationManager.PingOneEnvironmentId}/users/{userId}";

        var subAccountData = new UsersResponseDTO
        {
            UserName = userName,
            Email = userName,
            SubaccountData = new SubAccountDataDTO
            {
                ParentAgentId = parentAgentId,
                ParentUsername = parentUserName,
                Roles = roles
            }
        };

        var convertedJson = JsonSerializer.Serialize(subAccountData);

        try
        {
            var content = new StringContent(convertedJson, Encoding.UTF8, "application/json");

            var response = await HttpClient.PatchAsync(endpoint, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<UsersResponseDTO>(
                    responseString,
                    options: new(JsonSerializerDefaults.Web));

                return result;
            }

            var errorResponse = await response.Content.ReadAsStringAsync();

            logger.LogError(
                "Error attempting to update Ping Subaccount.  {status} | {responseBody}.",
                response.StatusCode,
                errorResponse);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred while attempting to update subaccount in Ping.");
            throw;
        }

        return null;
    }

    public async Task<bool?> UpdatePendingSubaccountAsync(
        string email,
        List<SubAccountRole> roles,
        string accessToken,
        string parentAgentId,
        CancellationToken cancellationToken)
    {
        var subAccountData = new PendingSubaccountRequest
        {
            Email = email,
            Roles = roles,
        };

        var convertedJson = JsonSerializer.Serialize(subAccountData);

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"{AgentPortalApiHttpClient.BaseAddress}API/Subaccounts")
            {
                Content = new StringContent(convertedJson, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            if (!string.IsNullOrEmpty(parentAgentId))
            {
                logger.LogDebug("Setting agent id header to {agentId}.", parentAgentId);
                request.Headers.Add("agent-id", parentAgentId);
            }

            var response = await AgentPortalApiHttpClient.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            logger.LogError("Failed to update pending subaccount.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred while attempting to update pending subaccount.");
            throw;
        }

        return false;
    }

    public async Task<bool> DoesPendingSubaccountExistAsync(string email, string parentAgentId, string accessToken, CancellationToken cancellationToken)
    {
        var subAccountData = new PendingSubaccountRequest
        {
            Email = email
        };

        var convertedJson = JsonSerializer.Serialize(subAccountData);

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{AgentPortalApiHttpClient.BaseAddress}API/Subaccounts/DoesExist")
            {
                Content = new StringContent(convertedJson, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            if (!string.IsNullOrEmpty(parentAgentId))
            {
                logger.LogDebug("Setting agent id header to {agentId}.", parentAgentId);
                request.Headers.Add("agent-id", parentAgentId);
            }

            var response = await AgentPortalApiHttpClient.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<bool>(
                    responseString,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return result;
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            logger.LogError("Failed while attempting to find existing subaccount.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred while attempting to find existing subaccount.");
            throw;
        }

        return false;
    }

    public async Task<bool> DoesPingSubaccountExistAsync(string email, string parentAgentId, CancellationToken cancellationToken)
    {
        await GetBearerToken(cancellationToken);
        var baseAddress = $"{ConfigurationManager.PingOneBaseUrl}/environments/{ConfigurationManager.PingOneEnvironmentId}";
        var endpoint = baseAddress + "/users?filter=email eq \"" + email + "\"";
        try
        {
            var response = await HttpClient.GetAsync(endpoint, cancellationToken);
            string body = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<SubAccountResponseDTO>(
                    responseString,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (result == null)
                {
                    return false;
                }

                return result.Count > 0;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error encountered while getting subaccounts.");
        }

        return false;
    }

    public async Task<PendingSubaccount> CreatePendingSubaccountAsync(
        string email,
        List<SubAccountRole> roles,
        string accessToken,
        string parentAgentId,
        CancellationToken cancellationToken,
        string? parentUsername = null)
    {
        var subAccountData = new PendingSubaccountRequest
        {
            Email = email,
            Roles = roles
        };

        var convertedJson = JsonSerializer.Serialize(subAccountData);

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{AgentPortalApiHttpClient.BaseAddress}API/Subaccounts/")
            {
                Content = new StringContent(convertedJson, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            if (!string.IsNullOrEmpty(parentAgentId))
            {
                logger.LogDebug("Setting agent id header to {agentId}.", parentAgentId);
                request.Headers.Add("agent-id", parentAgentId);
            }

            if (!string.IsNullOrEmpty(parentUsername))
            {
                Logger.LogDebug("Setting agent username header to {agentUsername}", parentUsername);
                request.Headers.Add("agent-username", parentUsername);
            }

            var response = await AgentPortalApiHttpClient.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PendingSubaccount>(
                    responseString,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return result;
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            logger.LogError("Failed to update pending subaccount.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred while attempting to update pending subaccount.");
            throw;
        }

        return null;
    }

    public async Task<PendingSubaccountActivationResponse?> ConfirmEmailAndGetRolesAsync(string email, string activationId, CancellationToken cancellationToken)
    {
        var verificationRequest = new SubaccountEmailRequest
        {
            Email = email,
        };

        var convertedJson = JsonSerializer.Serialize(verificationRequest);

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{AgentPortalApiHttpClient.BaseAddress}API/Subaccounts/{activationId}")
            {
                Content = new StringContent(convertedJson, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PendingSubaccountActivationResponse>(
                    responseString,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return result;
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            logger.LogError("Failed to verify subaccount.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred while attempting to verify subaccount.");
            throw;
        }

        return null;
    }

    public async Task<bool?> ResendSubaccountActivationLinkAsync(string email, string accessToken, string parentAgentId, CancellationToken cancellationToken)
    {
        var resendVerificationRequest = new SubaccountEmailRequest
        {
            Email = email
        };

        var convertedJson = JsonSerializer.Serialize(resendVerificationRequest);

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{AgentPortalApiHttpClient.BaseAddress}API/Subaccounts/Notify")
            {
                Content = new StringContent(convertedJson, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            if (!string.IsNullOrEmpty(parentAgentId))
            {
                logger.LogDebug("Setting agent id header to {agentId}.", parentAgentId);
                request.Headers.Add("agent-id", parentAgentId);
            }

            var response = await AgentPortalApiHttpClient.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            logger.LogError("Failed to resend subaccount verification link.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred while attempting to resend subaccount verification link.");
            throw;
        }

        return false;
    }

    public async Task<PingOneResponse?> CreatePingSubaccountAsync(
        string email,
        string parentAgentId,
        string parentUserName,
        List<SubAccountRole> roles,
        string passWord,
        CancellationToken cancellationToken)
    {
        await GetBearerToken(cancellationToken);

        var populationId = ConfigurationManager.PingOnePopulationId;

        var body = new UserDTO
        {
            Email = email,
            EmailVerified = true,
            CreatedAt = DateTime.Now.Date,
            AgentID = "subaccount",
            MFAEnabled = true,
            Type = "agent",
            PrimaryPhone = "9999999999",
            Name = new NameDTO
            {
                Given = "Sub",
                Family = "Account"
            },
            Username = email,
            Population = new PopulationDTO
            {
                Id = populationId,
            },
            SubaccountData = new SubAccountDataDTO
            {
                ParentUsername = parentUserName,
                ParentAgentId = parentAgentId,
                Roles = roles
            },
            Password = new PasswordCreationDTO
            {
                Value = passWord,
                ForceChange = false
            }
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new LowercaseNamingPolicy()
        };

        var jsonString = JsonSerializer.Serialize(body, options);
        var baseAddress = ConfigurationManager.PingOneBaseUrl;
        var envId = ConfigurationManager.PingOneEnvironmentId;
        var endpoint = baseAddress + "/environments/" + envId + "/users";

        var content = new StringContent(
            jsonString,
            Encoding.UTF8,
            "application/vnd.pingidentity.user.import+json");

        var response = await HttpClient.PostAsync(
            endpoint,
            content,
            cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<UsersResponseDTO>(responseString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return new PingOneResponse
            {
                Id = result.UserId,
                Status = "OK"
            };
        }
        else
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PingOneResponse>(errorResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            logger.LogError("An exception occurred when attempting to create a new subaccount in Ping. message: {errorResponse}", errorResponse);
            return result;
        }
    }

    public async Task<bool?> CreatePingSubaccountMFADeviceAsync(string userId, string email, CancellationToken cancellationToken)
    {
        logger.LogDebug("Creating MFA for userId:{userId} and email:{email}.", userId, email);

        await GetBearerToken(cancellationToken);

        var populationId = ConfigurationManager.PingOnePopulationId;

        var body = new Dictionary<string, string>()
        {
            { "type", "EMAIL" },
            { "email", email }
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new LowercaseNamingPolicy()
        };

        var jsonString = JsonSerializer.Serialize(body, options);
        var baseAddress = ConfigurationManager.PingOneBaseUrl;
        var envId = ConfigurationManager.PingOneEnvironmentId;
        var endpoint = $"{baseAddress}/environments/{envId}/users/{userId}/devices";

        var content = new StringContent(
            jsonString,
            Encoding.UTF8,
            "application/json");

        var response = await HttpClient.PostAsync(
            endpoint,
            content,
            cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            Logger.LogDebug("MFA device created for subaccount userId:{userId}, email:{email}, response:{responseString}", userId, email, responseString);

            return true;
        }
        else
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PingOneResponse>(errorResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            logger.LogError("An exception occurred when attempting to create MFA for new subaccount in Ping. message: {errorResponse}", errorResponse);
            return false;
        }
    }

    public async Task<bool> DeletePingAccountAsync(string userId, CancellationToken cancellationToken)
    {
        await GetBearerToken(cancellationToken);

        var baseAddress = ConfigurationManager.PingOneBaseUrl;
        var envId = ConfigurationManager.PingOneEnvironmentId;
        var endpoint = $"{baseAddress}/environments/{envId}/users/{userId}";

        var result = await HttpClient.DeleteAsync(endpoint);

        if (result.IsSuccessStatusCode)
        {
            return true;
        }

        var responseString = await result.Content.ReadAsStringAsync();

        Logger.LogError(
            "{Endpoint} returned error.  Response body: {resonseBody}",
            endpoint,
            responseString);

        return false;
    }

    public async Task<bool> DeletePendingSubaccountAsync(string userId, CancellationToken cancellationToken)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{AgentPortalApiHttpClient.BaseAddress}API/Subaccounts/{userId}");

            var response = await AgentPortalApiHttpClient.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            logger.LogError("Failed to delete pending subaccount.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred while attempting to delete pending subaccount.");
            throw;
        }

        return false;
    }

    public async Task<UsersResponseDTO?> GetPingUser(string userId, CancellationToken cancellationToken)
    {
        await GetBearerToken(cancellationToken);

        var baseAddress = ConfigurationManager.PingOneBaseUrl;
        var envId = ConfigurationManager.PingOneEnvironmentId;
        var endpoint = $"{baseAddress}/environments/{envId}/users/{userId}";

        var response = await HttpClient.GetAsync(endpoint, cancellationToken);
        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var result = JsonSerializer.Deserialize<UsersResponseDTO>(responseString);

            return result;
        }

        Logger.LogError(
            "{Endpoint} returned error.  Response body: {resonseBody}",
            endpoint,
            responseString);

        return null;
    }

    private async Task GetBearerToken(CancellationToken cancellationToken)
    {
        if (cache.TryGetValue("PingAuthToken", out ClientCredentialsResponse? token))
        {
            if (token is not null)
            {
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                return;
            }
        }

        var body = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
        };
        var authority = ConfigurationManager.PingOneAuthority;

        var endpoint = authority + "/token";

        var authRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new FormUrlEncodedContent(body)
        };

        var tokenHeaderBytes = System.Text.Encoding.UTF8.GetBytes($"{ConfigurationManager.PingAdminClientId}:{ConfigurationManager.PingAdminClientSecret}");
        var tokenHeaderValue = Convert.ToBase64String(tokenHeaderBytes);

        authRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", tokenHeaderValue);

        var response = await HttpClient.SendAsync(authRequest, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            logger.LogDebug("Refresh token response successful.");

            var result = await response.Content.ReadFromJsonAsync<ClientCredentialsResponse>(cancellationToken);

            if (result is not null)
            {
                cache.Set("PingAuthToken", result, DateTimeOffset.Now.AddSeconds(result.ExpiresIn - 30));

                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            }
        }
        else
        {
            var errorResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Getting Ping access token failed.");
        }
    }

    private class LowercaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name.ToLower();
        }
    }
}