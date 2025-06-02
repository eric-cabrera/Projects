namespace Assurity.AgentPortal.Utilities.Configs;

public interface IConfigurationManager
{
    string Environment { get; }

    string AzureAdIssuer { get; }

    string PingOneIssuer { get; }

    string IdentityServerUri { get; }

    string IdentityServerClientId { get; }

    string IdentityServerClientSecret { get; }

    Uri DocumentVaultBaseUri { get; }

    string DocumentQueueHostName { get; }

    string DocumentQueueUsername { get; }

    string DocumentQueuePassword { get; }

    string GlobalDataConnectionString { get; }

    string DataStoreConnectionString { get; }

    string AssureLinkConnectionString { get; }

    Uri PdfToTiffBaseUri { get; }

    Uri PolicyInfoServiceBaseUri { get; }

    Uri CommissionsServiceBaseUri { get; }

    Uri AgentServiceBaseUri { get; }

    Uri GroupInventoryServiceBaseUri { get; }

    Uri DebtServiceBaseUri { get; }

    Uri ProductionCreditServiceBaseUri { get; }

    Uri ListBillServiceBaseUri { get; }

    Uri ApplicationTrackerServiceBaseUri { get; }

    Uri ClaimsServiceBaseUri { get; }

    Uri TaxFormsServiceBaseUri { get; }

    Uri DocuSignServiceBaseUri { get; }

    string DocumentServiceUrl { get; }

    string IllustrationProUrl { get; }

    Uri LifePortraitsUri { get; }

    string IllustrationProAgentAccountUrl { get; }

    long TakeActionMaximumFileLengthInBytes { get; }

    short TakeActionMaximumFilesPerUpload { get; }

    string MongoDbConnectionString { get; }

    string MongoDbBenefitOptionsCollection { get; }

    string MongoDbPendingSubaccountsCollection { get; }

    string MongoDbImpersonationLogCollection { get; }

    string MongoDbUserSearchCollection { get; }

    string ExcludedAgentIdsCollection { get; }

    string MongoDbEventsDatabaseName { get; }

    string MongoDbAgentCenterDatabaseName { get; }

    string MongoDbUseTls { get; }

    string IPipelineConnectionString { get; }

    string IPipelineTargetString { get; }

    string IPipelineZanderCompanyId { get; }

    string IPipelineAssurityCompanyId { get; }

    string IPipelineZanderGroupName { get; }

    string IPipelineAssurityGroupName { get; }

    string IPipelineZanderTimeoutUrl { get; }

    string IPipelineAssurityTimeoutUrl { get; }

    string IPipelineAssuritySamlIssuer { get; }

    string IPipelineDomain { get; }

    string IPipelineZanderAgentId { get; }

    string IPipelineAudienceString { get; }

    string SmtpHost { get; }

    short SmtpPort { get; }

    string VertaforeUrl { get; }

    string VertaforeSecret { get; }

    string VertaforeSalt { get; }

    Uri ElasticSearchBaseUri { get; }

    string ElasticSearchApiKey { get; }

    string ElasticSearchUserSearchIndex { get; }

    string BaseAgentCenterUrl { get; }
}