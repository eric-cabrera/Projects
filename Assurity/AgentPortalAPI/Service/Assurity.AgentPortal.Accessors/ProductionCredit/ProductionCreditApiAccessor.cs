namespace Assurity.AgentPortal.Accessors.ProductionCredit;

using System.Net.Http;
using System.Threading;
using Assurity.AgentPortal.Contracts.ProductionCredit.Request;
using Assurity.Production.Contracts.V1.Individual;
using Assurity.Production.Contracts.V1.Shared.PolicyDetails;
using Assurity.Production.Contracts.V1.Worksite;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class ProductionCreditApiAccessor : IProductionCreditApiAccessor
{
    public ProductionCreditApiAccessor(HttpClient httpClient, ILogger<ProductionCreditApiAccessor> logger)
    {
        HttpClient = httpClient;
        Logger = logger;
    }

    private HttpClient HttpClient { get; }

    private ILogger<ProductionCreditApiAccessor> Logger { get; }

    public async Task<IndividualProductionReport?> GetIndividualProductionCredit(
        string agentId,
        ProductionCreditParameters parameters,
        CancellationToken cancellationToken)
    {
        var url = "v1/individual/production";
        var optionalQueryParams = parameters
           .GetType()
           .GetProperties()
           .ToDictionary(property => property.Name, property => property.GetValue(parameters)?.ToString());

        return await GetResponse(url, agentId, optionalQueryParams, new IndividualProductionReport(), cancellationToken);
    }

    public async Task<PolicyDetailsReport?> GetIndividualPolicyDetails(
        string agentId,
        ProductionCreditPolicyDetailsParameters parameters,
        CancellationToken cancellationToken)
    {
        var url = "v1/individual/policyDetails";
        var optionalQueryParams = parameters
           .GetType()
           .GetProperties()
           .ToDictionary(property => property.Name, property => property.GetValue(parameters)?.ToString());

        return await GetResponse(url, agentId, optionalQueryParams, new PolicyDetailsReport(), cancellationToken);
    }

    public async Task<WorksiteReport?> GetWorksiteProductionCredit(
        string agentId,
        WorksiteProductionCreditParameters parameters,
        CancellationToken cancellationToken)
    {
        var url = "v1/worksite/production";
        var optionalQueryParams = parameters
           .GetType()
           .GetProperties()
           .ToDictionary(property => property.Name, property => property.GetValue(parameters)?.ToString());

        return await GetResponse(url, agentId, optionalQueryParams, new WorksiteReport(), cancellationToken);
    }

    public async Task<PolicyDetailsReport?> GetWorksitePolicyDetails(
        string agentId,
        ProductionCreditPolicyDetailsParameters parameters,
        CancellationToken cancellationToken)
    {
        var url = "v1/worksite/policyDetails";
        var optionalQueryParams = parameters
           .GetType()
           .GetProperties()
           .ToDictionary(property => property.Name, property => property.GetValue(parameters)?.ToString());

        return await GetResponse(url, agentId, optionalQueryParams, new PolicyDetailsReport(), cancellationToken);
    }

    public async Task<List<string>?> GetProductionCreditMarketcodes(string agentId, CancellationToken cancellationToken)
    {
        var url = "v1/ProductionCredit/AgentMarketCodes";

        var response = await GetResponse(url, agentId, null, new Dictionary<string, List<string>>(), cancellationToken);

        return response?["marketCodes"];
    }

    private async Task<T?> GetResponse<T>(
        string url,
        string agentId,
        Dictionary<string, string?>? optionalParameters,
        T emptyResponse,
        CancellationToken cancellationToken)
    {
        var queryParams = new Dictionary<string, string?>();
        if (optionalParameters != null && optionalParameters.Count > 0)
        {
            queryParams = optionalParameters;
        }

        queryParams.Add("LoggedInAgentId", agentId);

        var newUrl = QueryHelpers.AddQueryString(url, queryParams);

        var response = await HttpClient.GetAsync(newUrl, cancellationToken);

        var stringContent = await response
            .Content
            .ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<T>(stringContent);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return emptyResponse;
    }
}
