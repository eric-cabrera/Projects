namespace Assurity.AgentPortal.Utilities.FeatureManagement;

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Newtonsoft.Json;

public class DirectusFeatureProvider : IFeatureDefinitionProvider
{
    private const string CacheKey = "agent_center_features";

    private readonly IHttpClientFactory httpClientFactory;

    private readonly IMemoryCache memoryCache;

    private readonly ILogger<DirectusFeatureProvider> logger;

    public DirectusFeatureProvider(
        IHttpClientFactory httpClient,
        IConfigurationManager configurationManager,
        IMemoryCache cache,
        ILogger<DirectusFeatureProvider> logger)
    {
        httpClientFactory = httpClient;
        ConfigurationManager = configurationManager;
        memoryCache = cache;
        this.logger = logger;
    }

    private IConfigurationManager ConfigurationManager { get; }

    public async IAsyncEnumerable<FeatureDefinition> GetAllFeatureDefinitionsAsync()
    {
        var environment = ConfigurationManager.Environment;

        if (!environment.Equals("local", StringComparison.OrdinalIgnoreCase))
        {
            if (!memoryCache.TryGetValue(CacheKey, out DirectusFeatureResponse? features))
            {
                try
                {
                    var httpClient = httpClientFactory.CreateClient(nameof(DirectusFeatureProvider));
                    httpClient.BaseAddress = new Uri(ConfigurationManager.DirectusUrl);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigurationManager.DirectusAccessToken);

                    var response = await httpClient.GetAsync("items/agent_center_features");
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        features = JsonConvert.DeserializeObject<DirectusFeatureResponse>(responseString);

                        memoryCache.Set(CacheKey, features, DateTime.Now.AddMinutes(1));
                    }
                    else
                    {
                        logger.LogWarning("Received {responseStaus} response from Directus.  {response}", response.StatusCode, responseString);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error encountered while fetching features from Directus.");
                }
            }

            if (features != null && features.Data.Any())
            {
                foreach (var feature in features.Data)
                {
                    yield return YieldDirectusFeatures(feature);
                }

                yield break;
            }
        }

        logger.LogInformation("Falling back to locally defined feature flags.");

        var localFeatures = ConfigurationManager.FeatureFlags;

        if (localFeatures == null)
        {
            yield break;
        }

        foreach (var feature in localFeatures)
        {
            if (feature.Value)
            {
                yield return CreateEnabledFeatureDefinition(feature.Key);
            }
            else
            {
                yield return CreateDisabledFeatureDefinition(feature.Key);
            }
        }
    }

    public async Task<FeatureDefinition> GetFeatureDefinitionAsync(string featureName)
    {
        if (featureName == null)
        {
            throw new ArgumentNullException(nameof(featureName));
        }

        await foreach (var feature in GetAllFeatureDefinitionsAsync())
        {
            if (feature.Name.Equals(featureName))
            {
                return feature;
            }
        }

        throw new NotSupportedException("The requested feature is not supported");
    }

    private FeatureDefinition CreateEnabledFeatureDefinition(string featureName)
    {
        return new FeatureDefinition
        {
            Name = featureName,
            EnabledFor = new[]
            {
                new FeatureFilterConfiguration
                {
                    Name = "AlwaysOn",
                }
            }
        };
    }

    private FeatureDefinition CreateDisabledFeatureDefinition(string featureName)
    {
        return new FeatureDefinition
        {
            Name = featureName,
        };
    }

    private FeatureDefinition YieldDirectusFeatures(DirectusFeature feature)
    {
        return ConfigurationManager.Environment.ToLower() switch
        {
            "dev" => feature.Dev
                                ? CreateEnabledFeatureDefinition(feature.FeatureName)
                                : CreateDisabledFeatureDefinition(feature.FeatureName),
            "dev2" => feature.Dev2
                                ? CreateEnabledFeatureDefinition(feature.FeatureName)
                                : CreateDisabledFeatureDefinition(feature.FeatureName),
            "test" => feature.Test
                                ? CreateEnabledFeatureDefinition(feature.FeatureName)
                                : CreateDisabledFeatureDefinition(feature.FeatureName),
            "test2" => feature.Test2
                                ? CreateEnabledFeatureDefinition(feature.FeatureName)
                                : CreateDisabledFeatureDefinition(feature.FeatureName),
            "prod" => feature.Prod
                                ? CreateEnabledFeatureDefinition(feature.FeatureName)
                                : CreateDisabledFeatureDefinition(feature.FeatureName),
            _ => CreateDisabledFeatureDefinition(feature.FeatureName),
        };
    }
}
