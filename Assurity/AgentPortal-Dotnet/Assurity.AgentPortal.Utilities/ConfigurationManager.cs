namespace Assurity.AgentPortal.Utilities;

using Assurity.Common.Cryptography;
using Microsoft.Extensions.Configuration;

public class ConfigurationManager : IConfigurationManager
{
    private const string SharedSecret = "MFjCcgV6khyuhTqWabp7Xe4EZnw9jZ6tiM6fdLNG9iMR3MarX6cD2puWJ2sV2v4F";
    private const string AssureLinkSecret = "aa23fcb39a5a37a592c0399c9ec5cc14acc3007c";

    private readonly IConfiguration configuration;

    private readonly IAesEncryptor aesEncryptor;

    public ConfigurationManager(IConfiguration configuration, IAesEncryptor aesEncryptor)
    {
        this.configuration = configuration;
        this.aesEncryptor = aesEncryptor;
    }

    public string Environment => configuration["Environment"]!;

    public string AWPSUrl => configuration["AWPSUrl"]!;

    public string IllustrationProUrl => configuration["IllustrationProUrl"]!;

    public string PingOneAuthority => configuration["OpenId:PingOne:Authority"]!;

    public string PingOneClientId => configuration["OpenId:PingOne:ClientId"]!;

    public string PingOneClientSecret => Decrypt(configuration["OpenId:PingOne:ClientSecret"]!);

    public string PingOneApiScopes => configuration["OpenId:PingOne:ApiScopes"]!;

    public string PingOneUserScopes => configuration["OpenId:PingOne:UserScopes"]!;

    public string PingOneBaseUrl => configuration["OpenId:PingOne:BaseUrl"]!;

    public string PingOnePopulationId => configuration["OpenId:PingOne:PopulationId"]!;

    public string PingOneEnvironmentId => configuration["OpenId:PingOne:EnvironmentId"]!;

    public string PingAdminClientId => configuration["OpenId:PingOne:AdminClientId"]!;

    public string PingAdminClientSecret => Decrypt(configuration["OpenId:PingOne:AdminClientSecret"]!);

    public string AzureAdAuthority => configuration["OpenId:AzureAd:Authority"]!;

    public string AzureAdTokenBaseUrl => configuration["OpenId:AzureAd:TokenBaseUrl"]!;

    public string AzureAdClientId => configuration["OpenId:AzureAd:ClientId"]!;

    public string AzureAdClientSecret => Decrypt(configuration["OpenId:AzureAd:ClientSecret"]!);

    public string AzureAdScopes => configuration["OpenId:AzureAd:Scopes"]!;

    public string AssureLinkUrl => configuration["AssureLinkUrl"]!;

    public string NewRelicInitValue => configuration["NewRelic:BrowserAgent:InitValue"]!;

    public string NewRelicInfoValue => configuration["NewRelic:BrowserAgent:InfoValue"]!;

    public string NewRelicLoaderConfigValue => configuration["NewRelic:BrowserAgent:LoaderConfigValue"]!;

    public string GoogleTagManagerId => configuration["GoogleTagManagerId"]!;

    public string DirectusUrl => configuration["Directus:BaseUrl"]!;

    public string DirectusAccessToken => Decrypt(configuration["Directus:AccessToken"]!);

    public Uri AgentPortalAPIUrl => configuration.GetValue<Uri>("Proxy:Clusters:AgentPortalCluster:Destinations:AgentPortalAPI:Address")!;

    public Dictionary<string, bool> FeatureFlags => configuration.GetSection("FeatureFlags").Get<Dictionary<string, bool>>()!;

    public string IPipelineFormsIndividualFormsCompanyIdentifier => configuration["IPipelineForms:IndividualFormsCompanyIdentifier"]!;

    public string IPipelineFormsWorksiteFormsCompanyIdentifier => configuration["IPipelineForms:WorksiteFormsCompanyIdentifier"]!;

    public string IPipelineFormsUserName => configuration["IPipelineForms:UserName"]!;

    public string IPipelineFormsPassword => configuration["IPipelineForms:Password"]!;

    public string IPipelineFormsTargetString => configuration["IPipelineForms:TargetString"]!;

    public string WPSUrl => configuration["WPSUrl"]!;

    public string Decrypt(string value, bool isValueFromAssureLink = false)
    {
        if (aesEncryptor == null)
        {
            throw new ArgumentNullException(nameof(AesEncryptor));
        }

        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException($"{nameof(value)} cannot be null");
        }

        var environment = Environment.TrimEnd('2');

        var secret = isValueFromAssureLink ? AssureLinkSecret : SharedSecret;

        return environment.Equals("LOCAL", StringComparison.InvariantCultureIgnoreCase)
            ? value
            : aesEncryptor.DecryptGAC(value, environment, secret);
    }

    public string Encrypt(string value)
    {
        if (aesEncryptor == null)
        {
            throw new ArgumentNullException(nameof(AesEncryptor));
        }

        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException($"{nameof(value)} cannot be null");
        }

        var environment = Environment.TrimEnd('2');

        return environment.Equals("LOCAL", StringComparison.InvariantCultureIgnoreCase)
            ? value
            : aesEncryptor.EncryptGAC(value, environment, SharedSecret);
    }
}