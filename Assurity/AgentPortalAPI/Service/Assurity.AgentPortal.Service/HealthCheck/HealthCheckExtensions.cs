namespace Assurity.AgentPortal.Service.HealthCheck;

using Assurity.AgentPortal.Accessors.DataStore.Context;
using Assurity.AgentPortal.Accessors.GlobalData.Context;
using Assurity.AgentPortal.MongoDB;
using Elasticsearch.Net;
using HealthChecks.ApplicationStatus.DependencyInjection;
using RabbitMQ.Client;
using ConfigurationManager = Assurity.AgentPortal.Utilities.Configs.ConfigurationManager;

public static class HealthCheckExtensions
{
    /// <summary>
    /// Adds health checks for this service's direct dependencies.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Logging dependencies are not covered.
    /// </para>
    /// <para>
    /// Assurity.Identity.Server is not covered directly because
    /// it is covered by Assurity.DocumentVault.Service's health check.
    /// </para>
    /// </remarks>
    /// <param name="services"></param>
    /// <param name="configurationManager"></param>
    /// <param name="readyTag">
    /// Tag that will be added to all dependency health checks to
    /// enable health check filtering.
    /// </param>
    /// <returns></returns>
    public static IServiceCollection AddHealthChecks(
        this IServiceCollection services,
        ConfigurationManager configurationManager,
        string readyTag)
    {
        var configuration = configurationManager.Configuration;

        services
            .AddHealthChecks()
            .AddApplicationStatus(
                "Assurity.AgentCenter.Api (Self)",
                tags: new[] { readyTag, "rest" })
            .AddAuthenticationHealthChecks(configuration, readyTag)
            .AddAssurityDatabaseHealthChecks(configurationManager, readyTag)
            .AddAssurityMessageHealthChecks(configurationManager, readyTag)
            .AddAssurityServiceHealthChecks(configuration, readyTag);

        return services;
    }

    private static IHealthChecksBuilder AddAuthenticationHealthChecks(
        this IHealthChecksBuilder healthChecksBuilder,
        IConfiguration configuration,
        string readyTag)
    {
        const string openIdConnectTag = "oidc";

        var authenticationConfigurationSection =
            configuration.GetSection("Authentication");

        var microsoftEntraIdAuthorityUrl =
            authenticationConfigurationSection
                .GetConfigurationValue("AzureAd:Authority");

        healthChecksBuilder
            .AddIdentityServer(
                new Uri($"{microsoftEntraIdAuthorityUrl}/"),
                name: "Microsoft Entra Id (Agent Center)",
                tags: new[] { readyTag, openIdConnectTag });

        var pingOneAuthorityUrl =
            authenticationConfigurationSection
                .GetConfigurationValue("PingOne:Authority");

        healthChecksBuilder
            .AddIdentityServer(
                new Uri($"{pingOneAuthorityUrl}/"),
                name: "PingOne",
                tags: new[] { readyTag, openIdConnectTag });

        return healthChecksBuilder;
    }

    private static IHealthChecksBuilder AddAssurityDatabaseHealthChecks(
        this IHealthChecksBuilder healthChecksBuilder,
        ConfigurationManager configurationManager,
        string readyTag)
    {
        var mongoDbConnection =
            new MongoDBConnection(configurationManager, false);
        var mongoDbEventsDatabaseName = configurationManager.MongoDbEventsDatabaseName;
        var mongoDbAgentCenterDatabaseName = configurationManager.MongoDbAgentCenterDatabaseName;

        var elasticsearchUri = configurationManager.ElasticSearchBaseUri.ToString();
        var elasticsearchApiKey = new ApiKeyAuthenticationCredentials(configurationManager.ElasticSearchApiKey);

        return healthChecksBuilder
            .AddDbContextCheck<GlobalDataContext>(
                $"GlobalData Database",
                tags: new[] { readyTag, "sql" })
            .AddDbContextCheck<DataStoreContext>(
                $"DataStore Database",
                tags: new[] { readyTag, "sql" })
            .AddMongoDb(
                mongoDbConnection.GetMongoClientSettings(),
                mongoDbEventsDatabaseName,
                $"{mongoDbEventsDatabaseName} Database",
                tags: new[] { readyTag, "mongodbEvents" })
            .AddMongoDb(
                mongoDbConnection.GetMongoClientSettings(),
                mongoDbAgentCenterDatabaseName,
                $"{mongoDbAgentCenterDatabaseName} Database",
                tags: new[] { readyTag, "mongodbAgentCenter" })
            .AddElasticsearch(
                elasticsearchOptions =>
                    elasticsearchOptions
                        .UseServer(elasticsearchUri)
                        .UseApiKey(elasticsearchApiKey),
                "elasticsearch",
                tags: [readyTag, "elasticsearch"]);
    }

    private static IHealthChecksBuilder AddAssurityMessageHealthChecks(
        this IHealthChecksBuilder healthChecksBuilder,
        ConfigurationManager configurationManager,
        string readyTag)
    {
        var hostname = configurationManager.DocumentQueueHostName;

        var connectionFactory = new ConnectionFactory
        {
            HostName = hostname,
            UserName = configurationManager.DocumentQueueUsername,
            Password = configurationManager.DocumentQueuePassword,
            AutomaticRecoveryEnabled = true
        };

        if (hostname.ToUpper() != "LOCALHOST")
        {
            connectionFactory.Port = 5671;
            connectionFactory.Ssl = new SslOption
            {
                Enabled = true,
                Version = System.Security.Authentication.SslProtocols.Tls12,
                AcceptablePolicyErrors = System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch,
            };
        }

        return healthChecksBuilder
            .AddRabbitMQ(
                    rabbitMqHealthCheckOptions =>
                        rabbitMqHealthCheckOptions.ConnectionFactory = connectionFactory,
                    $"{hostname}",
                    tags: new[] { readyTag, "rabbitmq" });
    }

    private static IHealthChecksBuilder AddAssurityServiceHealthChecks(
        this IHealthChecksBuilder healthChecksBuilder,
        IConfiguration configuration,
        string readyTag)
    {
        const string restTag = "rest";

        var documentVaultHealthCheckAddress =
            $"{configuration.GetConfigurationValue("DocumentVault:BaseUri")}/api/heartbeat";

        healthChecksBuilder
            .AddUrlGroup(
                new Uri(documentVaultHealthCheckAddress),
                "Assurity.DocumentVault.Service",
                tags: new[] { readyTag, restTag, "Assurity.Identity.Server" });

        var pdfToTiffHealthCheckAddress =
            $"{configuration.GetConfigurationValue("PdfToTiffBaseUrl")}/api/convert/availableactions";

        healthChecksBuilder
            .AddUrlGroup(
                new Uri(pdfToTiffHealthCheckAddress),
                "PdfToTiff",
                tags: new[] { readyTag, restTag });

        var policyInformationHealthCheckAddress =
            $"{configuration.GetConfigurationValue("PolicyInfoServiceBaseUri")}healthcheck/ready";

        healthChecksBuilder
            .AddUrlGroup(
                   new Uri(policyInformationHealthCheckAddress),
                   "Assurity.PolicyInformation.Service",
                   tags: new[] { readyTag, restTag });

        return healthChecksBuilder;
    }

    private static string GetConfigurationValue(this IConfiguration configuration, string key)
    {
        return
            configuration[key]
            ?? throw new InvalidOperationException($"Unable to retrieve configuration value with key: {key}");
    }
}