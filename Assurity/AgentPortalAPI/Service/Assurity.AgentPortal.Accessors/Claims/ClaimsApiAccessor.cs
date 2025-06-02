namespace Assurity.AgentPortal.Accessors.Claims;

using System.Net.Http;
using Assurity.AgentPortal.Contracts.Claims;
using Assurity.Claims.Contracts;
using Assurity.Claims.Contracts.AssureLink;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AssureLinkClaim = Assurity.Claims.Contracts.AssureLink.Claim;

public class ClaimsApiAccessor : IClaimsApiAccessor
{
    public ClaimsApiAccessor(HttpClient httpClient, ILogger<ClaimsApiAccessor> logger)
    {
        HttpClient = httpClient;
        Logger = logger;
    }

    private HttpClient HttpClient { get; }

    private ILogger<ClaimsApiAccessor> Logger { get; }

    public void UpdateClaimStatus(AssureLinkClaim claim)
    {
        if (claim.Status == ClaimStatus.Pending && claim.PaymentAmount > 0 && (claim.Details == null || claim.Details.Count == 0))
        {
            claim.Status = ClaimStatus.Approved;
        }
    }

    public async Task<AssureLinkClaimResponse?> GetClaims(
        string agentId,
        ClaimsParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var claimsEndpoint = "api/AssureLink/Claims";

        var queryParams = new Dictionary<string, string?>
        {
            { "LoggedInAgentNumber", agentId },
            { "PageNumber", parameters?.PageNumber > 0 ? parameters.PageNumber.ToString() : "1" },
            { "PageSize", parameters?.PageSize > 0 ? parameters.PageSize.ToString() : "10" },
        };

        if (parameters?.ClaimNumber != null)
        {
            queryParams.Add("ClaimNumber", parameters.ClaimNumber);
        }

        if (parameters?.PolicyNumber != null)
        {
            queryParams.Add("PolicyNumber", parameters.PolicyNumber);
        }

        if (parameters?.ClaimantFirstName != null)
        {
            queryParams.Add("ClaimantName.FirstName", parameters.ClaimantFirstName);
        }

        if (parameters?.ClaimantLastName != null)
        {
            queryParams.Add("ClaimantName.LastName", parameters.ClaimantLastName);
        }

        var url = QueryHelpers.AddQueryString(claimsEndpoint, queryParams);

        var response = await HttpClient.GetAsync(url, cancellationToken);

        var stringContent = await response.Content.ReadAsStringAsync(cancellationToken);

        // TODO: When Claims API is updated to check the 404 response content, remove the response checking / exception throwing from this accessor.
        if (response.IsSuccessStatusCode)
        {
            var claimResponse = JsonConvert.DeserializeObject<AssureLinkClaimResponse>(stringContent);
            if (claimResponse?.Claims != null)
            {
                foreach (var claim in claimResponse.Claims)
                {
                    UpdateClaimStatus(claim as AssureLinkClaim);
                    if (claim.Details != null && claim.Details.Count > 0)
                    {
                        claim.Details = [.. claim.Details.OrderByDescending(d => d.PaymentDate)];
                    }
                }
            }

            return claimResponse;
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Logger.LogWarning("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
            return new AssureLinkClaimResponse();
        }

        throw new Exception($"An error occurred while attempting to reach {response.RequestMessage?.RequestUri} | Returned response: {response.StatusCode} | {stringContent}");
    }
}