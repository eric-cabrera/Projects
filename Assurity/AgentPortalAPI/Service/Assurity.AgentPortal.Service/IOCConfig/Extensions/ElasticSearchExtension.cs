namespace Assurity.AgentPortal.Service.IOCConfig.Extensions;

using Assurity.AgentPortal.Utilities.Configs;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

public static class ElasticSearchExtension
{
    public static void AddElasticSearch(
        this IServiceCollection services,
        IConfigurationManager configuration)
    {
        var url = configuration.ElasticSearchBaseUri;
        var settings = new ElasticsearchClientSettings(url)
            .Authentication(new ApiKey(configuration.ElasticSearchApiKey));

        var client = new ElasticsearchClient(settings);

        services.AddSingleton(client);
    }
}
