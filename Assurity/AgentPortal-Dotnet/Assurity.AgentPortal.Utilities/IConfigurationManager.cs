namespace Assurity.AgentPortal.Utilities;

public interface IConfigurationManager
{
    string Environment { get; }

    string PingOneAuthority { get; }

    string PingOneClientId { get; }

    string PingOneClientSecret { get; }

    string PingOneApiScopes { get; }

    string PingOneUserScopes { get; }

    string PingOneBaseUrl { get; }

    string PingOneEnvironmentId { get; }

    string PingOnePopulationId { get; }

    string PingAdminClientId { get; }

    string PingAdminClientSecret { get; }

    string AzureAdAuthority { get; }

    string AzureAdTokenBaseUrl { get; }

    string AzureAdClientId { get; }

    string AzureAdClientSecret { get; }

    string AzureAdScopes { get; }

    string AssureLinkUrl { get; }

    string NewRelicInitValue { get; }

    string NewRelicInfoValue { get; }

    string NewRelicLoaderConfigValue { get; }

    string GoogleTagManagerId { get; }

    string DirectusUrl { get; }

    string DirectusAccessToken { get; }

    Uri AgentPortalAPIUrl { get; }

    Dictionary<string, bool> FeatureFlags { get; }

    string IPipelineFormsIndividualFormsCompanyIdentifier { get; }

    string IPipelineFormsWorksiteFormsCompanyIdentifier { get; }

    string IPipelineFormsUserName { get; }

    string IPipelineFormsPassword { get; }

    string IPipelineFormsTargetString { get; }

    string WPSUrl { get; }

    string AWPSUrl { get; }

    string IllustrationProUrl { get; }

    string Decrypt(string value, bool isValueFromAssureLink = false);

    string Encrypt(string value);
}
