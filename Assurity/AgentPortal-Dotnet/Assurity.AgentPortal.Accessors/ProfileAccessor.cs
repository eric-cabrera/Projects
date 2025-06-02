namespace Assurity.AgentPortal.Accessors;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using Assurity.AgentPortal.Accessors.Profile;
using Assurity.AgentPortal.Accessors.ProfilePingDTOs;
using Assurity.AgentPortal.Accessors.SubAccount.DTOs;
using Assurity.AgentPortal.Accessors.SubAccount.DTOs.AgentCenterAPIRequests;
using Assurity.AgentPortal.Contracts;
using Assurity.AgentPortal.Utilities;
using Microsoft.Extensions.Logging;

public class ProfileAccessor(
    IHttpClientFactory httpClientFactory,
    IConfigurationManager configuration,
    ILogger<ProfileAccessor> logger) : IProfileAccessor
{
    private readonly ILogger logger = logger;

    internal IConfigurationManager Configuration { get; } = configuration;

    private HttpClient AgentPortalApiHttpClient { get; } = httpClientFactory.CreateClient("AgentPortalAPIHttpClient");

    internal JsonSerializerOptions SerializeOptions { get; } = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    internal JsonSerializerOptions DeserializeOptions { get; } = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    private HttpClient HttpClient { get; } = httpClientFactory.CreateClient(nameof(ProfileAccessor));

    public async Task<PingOneResponse?> ChangePassword(string userId, string currentPassword, string newPassword, string accessToken, CancellationToken cancellationToken = default)
    {
        var endPoint = $"{HttpClient.BaseAddress}users/{userId}/password";

        var changePasswordData = new ChangePassword { CurrentPassword = currentPassword, NewPassword = newPassword };

        var convertedJson = JsonSerializer.Serialize(changePasswordData, SerializeOptions);

        var content = new StringContent(convertedJson, Encoding.UTF8, "application/vnd.pingidentity.password.reset+json");
        var request = new HttpRequestMessage(HttpMethod.Put, endPoint)
        {
            Content = content
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await HttpClient.SendAsync(request, cancellationToken);

        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<PingOneResponse>(responseString, DeserializeOptions);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("{endpoint} failed with a status code of {statusCode}", endPoint, response.StatusCode);
        }

        return result;
    }

    public async Task<string> CreateMFADevice(string userId, MFAType mfaType, string value, string accessToken, CancellationToken cancellationToken = default)
    {
        var endPoint = $"{HttpClient.BaseAddress}users/{userId}/devices";

        var createMFADeviceData = new CreateMFADevice { Type = EnumUtility.GetEnumDescription((MFAType)mfaType) };

        if (mfaType == MFAType.EMAIL)
        {
            createMFADeviceData.Email = value;
        }
        else
        {
            createMFADeviceData.Phone = value;
        }

        var convertedJson = JsonSerializer.Serialize(createMFADeviceData, SerializeOptions);

        var content = new StringContent(convertedJson, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, endPoint)
        {
            Content = content
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await HttpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("{endpoint} failed with a status code of {statusCode}", endPoint, response.StatusCode);
            return string.Empty;
        }

        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<PingOneResponse>(responseString, DeserializeOptions);

        return result == null || result.Id == null ? string.Empty : result.Id;
    }

    public async Task<PingOneResponse?> ActivateMFADevice(string userId, string oneTimePassword, string deviceId, string accessToken, CancellationToken cancellationToken = default)
    {
        var endPoint = $"{HttpClient.BaseAddress}users/{userId}/devices/{deviceId}";

        var contentValues = new Dictionary<string, string>()
        {
            { "otp", oneTimePassword }
        };

        var convertedJson = JsonSerializer.Serialize(contentValues, SerializeOptions);

        var content = new StringContent(convertedJson, Encoding.UTF8, "application/vnd.pingidentity.device.activate+json");
        var request = new HttpRequestMessage(HttpMethod.Post, endPoint)
        {
            Content = content
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await HttpClient.SendAsync(request, cancellationToken);

        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<PingOneResponse>(responseString, DeserializeOptions);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("{endpoint} failed with a status code of {statusCode}", endPoint, response.StatusCode);
        }

        return result;
    }

    public async Task<PingOneResponse?> GetAllPingUserMFADevices(string userId, string accessToken, CancellationToken cancellationToken = default)
    {
        var endPoint = $"{HttpClient.BaseAddress}users/{userId}/devices";

        var request = new HttpRequestMessage(HttpMethod.Get, endPoint);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await HttpClient.SendAsync(request, cancellationToken);

        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<PingOneResponse>(responseString, DeserializeOptions);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("{endpoint} failed with a status code of {statusCode}", endPoint, response.StatusCode);
        }

        return result;
    }

    public async Task<bool> DeleteMFADevice(string userId, string deviceId, string accessToken, CancellationToken cancellationToken = default)
    {
        var endPoint = $"{HttpClient.BaseAddress}users/{userId}/devices/{deviceId}";

        var request = new HttpRequestMessage(HttpMethod.Delete, endPoint);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await HttpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("{endpoint} failed with a status code of {statusCode}", endPoint, response.StatusCode);
        }

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ResendMFACode(string userId, string deviceId, string accessToken, CancellationToken cancellationToken = default)
    {
        var endPoint = $"{HttpClient.BaseAddress}users/{userId}/devices/{deviceId}";

        var content = new StringContent(string.Empty, Encoding.UTF8, "application/vnd.pingidentity.device.sendActivationCode+json");
        var request = new HttpRequestMessage(HttpMethod.Post, endPoint)
        {
            Content = content
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await HttpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("{endpoint} failed with a status code of {statusCode}", endPoint, response.StatusCode);
        }

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateUserContactInfo(string userId, MFAType mfaType, string value, string accessToken, CancellationToken cancellationToken = default)
    {
        var endPoint = $"{HttpClient.BaseAddress}users/{userId}";

        var updateEmailRequest = new Dictionary<string, string>()
        {
            { EnumUtility.GetEnumDescription(mfaType), value }
        };

        var convertedJson = JsonSerializer.Serialize(updateEmailRequest, SerializeOptions);

        var content = new StringContent(convertedJson, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Patch, endPoint)
        {
            Content = content
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await HttpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("{endpoint} failed with a status code of {statusCode}", endPoint, response.StatusCode);
        }

        return response.IsSuccessStatusCode;
    }

    public async Task SendConfirmationEmails(string originalEmailAddress, string newEmailAddress, string accessToken, CancellationToken cancellationToken = default)
    {
        var confirmEmailRequest = new SendConfirmationEmailsRequest
        {
            OriginalEmail = originalEmailAddress,
            NewEmail = newEmailAddress
        };

        var convertedJson = JsonSerializer.Serialize(confirmEmailRequest);

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{AgentPortalApiHttpClient.BaseAddress}API/UserData/SendEmailNotifications")
            {
                Content = new StringContent(convertedJson, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await AgentPortalApiHttpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                logger.LogError("Failed to send confirmation emails.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred while attempting to send confirmation emails.");
            throw;
        }
    }
}
