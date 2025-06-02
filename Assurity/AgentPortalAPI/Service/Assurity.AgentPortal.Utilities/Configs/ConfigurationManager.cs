namespace Assurity.AgentPortal.Utilities.Configs;

using Assurity.Common.Cryptography;
using Microsoft.Extensions.Configuration;

public class ConfigurationManager : IConfigurationManager
{
    private const string SharedSecret = "cwl983kdjnv82jvcnmzk3jg921psaldn12nm38tnvlchdio2o3k48fnaldijf302";

    public ConfigurationManager(
        IConfiguration configuration,
        IAesEncryptor aesEncryptor)
    {
        Configuration = configuration;
        AesEncryptor = aesEncryptor;
    }

    public IConfiguration Configuration { get; }

    public string Environment => Configuration["Environment"];

    public string AzureAdIssuer => Configuration["Authentication:AzureAd:Authority"];

    public string PingOneIssuer => Configuration["Authentication:PingOne:Authority"];

    public string IdentityServerUri => Configuration["Authentication:IdentityServer:BaseUri"];

    public string IdentityServerClientId => Configuration["Authentication:IdentityServer:ClientId"];

    public string IdentityServerClientSecret => Decrypt(Configuration["Authentication:IdentityServer:ClientSecret"]);

    public Uri DocumentVaultBaseUri => Configuration.GetValue<Uri>("DocumentVault:BaseUri");

    public string DocumentQueueHostName => Configuration["DocumentQueue:HostName"];

    public string DocumentQueueUsername => Configuration["DocumentQueue:Username"];

    public string DocumentQueuePassword => Decrypt(Configuration["DocumentQueue:Password"]);

    public string GlobalDataConnectionString => GetConnectionString("GlobalData");

    public string DataStoreConnectionString => GetConnectionString("DataStore");

    public string AssureLinkConnectionString => GetConnectionString("AssureLink");

    public Uri PdfToTiffBaseUri => Configuration.GetValue<Uri>("PdfToTiffBaseUrl");

    public Uri PolicyInfoServiceBaseUri => Configuration.GetValue<Uri>("PolicyInfoServiceBaseUri");

    public Uri CommissionsServiceBaseUri => Configuration.GetValue<Uri>("CommissionsServiceBaseUri");

    public Uri DebtServiceBaseUri => Configuration.GetValue<Uri>("DebtServiceBaseUri");

    public Uri AgentServiceBaseUri => Configuration.GetValue<Uri>("AgentServiceBaseUri");

    public Uri GroupInventoryServiceBaseUri => Configuration.GetValue<Uri>("GroupInventoryServiceBaseUri");

    public Uri ProductionCreditServiceBaseUri => Configuration.GetValue<Uri>("ProductionCreditServiceBaseUri");

    public Uri ListBillServiceBaseUri => Configuration.GetValue<Uri>("ListBillServiceBaseUri");

    public Uri ApplicationTrackerServiceBaseUri => Configuration.GetValue<Uri>("ApplicationTrackerServiceBaseUri");

    public Uri ClaimsServiceBaseUri => Configuration.GetValue<Uri>("ClaimsServiceBaseUri");

    public Uri TaxFormsServiceBaseUri => Configuration.GetValue<Uri>("TaxFormsServiceBaseUri");

    public Uri DocuSignServiceBaseUri => Configuration.GetValue<Uri>("DocuSignServiceBaseUri");

    public string DocumentServiceUrl => Configuration.GetValue<string>("DocumentServiceUrl");

    public string IllustrationProUrl => Configuration.GetValue<string>("IllustrationProUrl");

    public string IllustrationProAgentAccountUrl => Configuration.GetValue<string>("IllustrationProAgentAccountUrl");

    public Uri LifePortraitsUri => Configuration.GetValue<Uri>("LifePortraitsUri");

    public long TakeActionMaximumFileLengthInBytes =>
        GetTakeActionMaximumFileLengthInBytes("TakeAction:MaximumFileLengthInBytes");

    public short TakeActionMaximumFilesPerUpload =>
        GetTakeActionMaximumFilesPerUpload("TakeAction:MaximumFilesPerUpload");

    public string MongoDbConnectionString => Decrypt(Configuration["MongoDb:ConnectionString"]);

    public string MongoDbEventsDatabaseName => Configuration["MongoDb:EventsDatabaseName"];

    public string MongoDbAgentCenterDatabaseName => Configuration["MongoDb:AgentCenterDatabaseName"];

    public string MongoDbBenefitOptionsCollection => Configuration["MongoDb:BenefitOptionsCollection"];

    public string MongoDbPendingSubaccountsCollection => Configuration["MongoDb:PendingSubaccountsCollection"];

    public string MongoDbImpersonationLogCollection => Configuration["MongoDb:ImpersonationLogCollection"];

    public string MongoDbUserSearchCollection => Configuration["MongoDb:UserSearchCollection"];

    public string ExcludedAgentIdsCollection => Configuration["MongoDb:ExcludedAgentIdsCollection"];

    public string MongoDbUseTls => Configuration["MongoDb:UseTls"];

    public string IPipelineConnectionString => Configuration["IPipeline:ConnectionString"];

    public string IPipelineTargetString => Configuration["IPipeline:TargetString"];

    public string IPipelineZanderCompanyId => Configuration["IPipeline:ZanderCompanyId"];

    public string IPipelineAssurityCompanyId => Configuration["IPipeline:AssurityCompanyId"];

    public string IPipelineZanderGroupName => Configuration["IPipeline:ZanderGroupName"];

    public string IPipelineAssurityGroupName => Configuration["IPipeline:AssurityGroupName"];

    public string IPipelineZanderTimeoutUrl => Configuration["IPipeline:ZanderTimeoutUrl"];

    public string IPipelineAssurityTimeoutUrl => Configuration["IPipeline:AssurityTimeoutUrl"];

    public string IPipelineAssuritySamlIssuer => Configuration["IPipeline:AssuritySamlIssuer"];

    public string IPipelineDomain => Configuration["IPipeline:Domain"];

    public string IPipelineZanderAgentId => Configuration["IPipeline:ZanderAgentId"];

    public string IPipelineAudienceString => Configuration["IPipeline:AudienceString"];

    public string SmtpHost => Configuration["SmtpHost"];

    public short SmtpPort => Configuration.GetValue<short>("SmtpPort");

    public string VertaforeUrl => Configuration["Vertafore:Url"];

    public string VertaforeSecret => Decrypt(Configuration["Vertafore:Secret"]);

    public string VertaforeSalt => Decrypt(Configuration["Vertafore:Salt"]);

    public Uri ElasticSearchBaseUri => Configuration.GetValue<Uri>("ElasticSearch:BaseUri");

    public string ElasticSearchApiKey => Decrypt(Configuration["ElasticSearch:ApiKey"]);

    public string ElasticSearchUserSearchIndex => Configuration["ElasticSearch:UserSearchIndex"];

    public string BaseAgentCenterUrl => Configuration["BaseAgentCenterUrl"];

    private IAesEncryptor AesEncryptor { get; }

    private bool IsLocal => Environment.Equals("LOCAL", StringComparison.InvariantCultureIgnoreCase);

    private string Decrypt(string value)
    {
        if (AesEncryptor == null)
        {
            throw new ArgumentNullException(nameof(AesEncryptor));
        }

        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException($"{nameof(value)} cannot be null");
        }

        return IsLocal
            ? value
            : AesEncryptor.DecryptGAC(value, Environment, SharedSecret);
    }

    private string GetConnectionString(string name)
    {
        var connectionString = Configuration.GetConnectionString(name);

        return Decrypt(connectionString);
    }

    private long GetTakeActionMaximumFileLengthInBytes(string configurationKey)
    {
        var takeActionMaximumFileLengthInBytes = Configuration[configurationKey];

        if (long.TryParse(takeActionMaximumFileLengthInBytes, out long parsedTakeActionMaximumFileLengthInBytes))
        {
            return parsedTakeActionMaximumFileLengthInBytes;
        }

        throw new Exception($"Unable to parse {configurationKey} to long. " +
            $"Value: '{takeActionMaximumFileLengthInBytes}'");
    }

    private short GetTakeActionMaximumFilesPerUpload(string configurationKey)
    {
        var takeActionMaximumFilesPerUpload = Configuration[configurationKey];

        if (short.TryParse(takeActionMaximumFilesPerUpload, out short parsedTakeActionMaximumFilesPerUpload))
        {
            return parsedTakeActionMaximumFilesPerUpload;
        }

        throw new Exception($"Unable to parse {configurationKey} to short. " +
            $"Value: '{takeActionMaximumFilesPerUpload}'");
    }
}