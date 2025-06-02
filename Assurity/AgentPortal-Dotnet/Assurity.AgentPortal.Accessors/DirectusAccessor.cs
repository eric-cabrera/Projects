namespace Assurity.AgentPortal.Accessors;

using System.Net.Http.Headers;
using Assurity.AgentPortal.Accessors.DirectusDTOs;
using Assurity.AgentPortal.Accessors.DirectusQueries;
using Assurity.AgentPortal.Accessors.DirectusQueries.Contracting;
using Assurity.AgentPortal.Contracts.Directus;
using Assurity.AgentPortal.Utilities;
using Microsoft.Extensions.Logging;

public class DirectusAccessor : IDirectusAccessor
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILogger<DirectusAccessor> logger;

    public DirectusAccessor(IHttpClientFactory httpClientFactory, ILogger<DirectusAccessor> logger, IConfigurationManager configurationManager)
    {
        this.httpClientFactory = httpClientFactory;
        this.logger = logger;
        ConfigurationManager = configurationManager;
    }

    private IConfigurationManager ConfigurationManager { get; }

    public async Task<AgentCenterCommissionDatesResponse?> GetAgentCenterCommissionDates()
    {
        try
        {
            var httpClient = CreateHttpClient();

            var commissionDatesQuery = new CommissionDatesQuery();
            commissionDatesQuery.AddArgument("status", GetDirectusStatusFilter());

            return await commissionDatesQuery.PostQuery<AgentCenterCommissionDatesResponse>(httpClient, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error encountered while fetching Agent Center Commission Dates from Directus.");
        }

        return null;
    }

    public async Task<ContractsQueryResponse?> GetContracts(ContractsQuery query)
    {
        try
        {
            var httpClient = CreateHttpClient();

            return await query.PostQuery<ContractsQueryResponse>(httpClient, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error encountered while fetching Contracts from Directus");
        }

        return null;
    }

    public async Task<string?> GetDirectusPage(PageQuery query)
    {
        query.AddArgument("statuses", GetDirectusStatusFilter());

        try
        {
            var httpClient = CreateHttpClient();

            return await query.PostQuery(httpClient, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error encountered while fetching Page from Directus");
        }

        return null;
    }

    public async Task<byte[]?> GetDirectusFile(string fileId)
    {
        try
        {
            var httpClient = CreateHttpClient();

            var response = await httpClient.GetAsync($"/assets/{fileId}?download=");

            return await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error encountered while getting file stream from Directus.");
        }

        return null;
    }

    public async Task<TemporaryMessagesQueryResponse?> GetTemporaryMessages()
    {
        try
        {
            var httpClient = CreateHttpClient();

            var tempMessageQuery = new TemporaryMessagesQuery();
            tempMessageQuery.AddArgument("status", GetDirectusStatusFilter());

            return await tempMessageQuery.PostQuery<TemporaryMessagesQueryResponse>(httpClient, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error encountered while fetching temporary messages from Directus.");
        }

        return null;
    }

    public async Task<List<DirectusContact>?> GetContacts()
    {
        try
        {
            var httpClient = CreateHttpClient();
            var contactsQuery = new ContactsQuery();

            return (await contactsQuery.PostQuery<DirectusContactsResponse>(httpClient, logger))?.Data.Contacts;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error encountered while fetching contacts from Directus");
        }

        return null;
    }

    private HttpClient CreateHttpClient()
    {
        var httpClient = httpClientFactory.CreateClient(nameof(DirectusAccessor));
        httpClient.BaseAddress = new Uri(ConfigurationManager.DirectusUrl);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigurationManager.DirectusAccessToken);

        return httpClient;
    }

    private List<string> GetDirectusStatusFilter()
    {
        // Only show draft status in DEV and TEST
        return ConfigurationManager.Environment.ToLower() switch
        {
            "local" or "dev" or "test" => new List<string> { "draft", "published" },
            _ => new List<string> { "published" },
        };
    }
}
