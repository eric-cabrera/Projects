namespace Assurity.AgentPortal.Service.IOCConfig;

using Assurity.AgentPortal.Accessors.Agent;
using Assurity.AgentPortal.Accessors.Alerts;
using Assurity.AgentPortal.Accessors.ApplicationTracker;
using Assurity.AgentPortal.Accessors.AssureLink.Context;
using Assurity.AgentPortal.Accessors.Claims;
using Assurity.AgentPortal.Accessors.CommissionsAndDebt;
using Assurity.AgentPortal.Accessors.DataStore.Context;
using Assurity.AgentPortal.Accessors.DocuSign;
using Assurity.AgentPortal.Accessors.EConsent;
using Assurity.AgentPortal.Accessors.GlobalData.Context;
using Assurity.AgentPortal.Accessors.GroupInventory;
using Assurity.AgentPortal.Accessors.IllustrationPro;
using Assurity.AgentPortal.Accessors.Impersonation;
using Assurity.AgentPortal.Accessors.Integration;
using Assurity.AgentPortal.Accessors.LeadersConference;
using Assurity.AgentPortal.Accessors.LifePortraits;
using Assurity.AgentPortal.Accessors.ListBill;
using Assurity.AgentPortal.Accessors.PolicyInfo;
using Assurity.AgentPortal.Accessors.Polly;
using Assurity.AgentPortal.Accessors.ProductionCredit;
using Assurity.AgentPortal.Accessors.Send;
using Assurity.AgentPortal.Accessors.Subaccounts;
using Assurity.AgentPortal.Accessors.TaxForms;
using Assurity.AgentPortal.Engines;
using Assurity.AgentPortal.Engines.Integration;
using Assurity.AgentPortal.Managers.AgentHierarchy;
using Assurity.AgentPortal.Managers.Alerts;
using Assurity.AgentPortal.Managers.CaseManagement;
using Assurity.AgentPortal.Managers.Claims;
using Assurity.AgentPortal.Managers.CommissionsAndDebt;
using Assurity.AgentPortal.Managers.GroupInventory;
using Assurity.AgentPortal.Managers.Impersonation;
using Assurity.AgentPortal.Managers.Integration;
using Assurity.AgentPortal.Managers.LeadersConference;
using Assurity.AgentPortal.Managers.ListBill;
using Assurity.AgentPortal.Managers.PolicyDelivery;
using Assurity.AgentPortal.Managers.PolicyInfo;
using Assurity.AgentPortal.Managers.ProductionCredit;
using Assurity.AgentPortal.Managers.Send;
using Assurity.AgentPortal.Managers.Subaccounts;
using Assurity.AgentPortal.Managers.TaxForms;
using Assurity.AgentPortal.Managers.UserData;
using Assurity.AgentPortal.MongoDB;
using Assurity.AgentPortal.Service.Authorization;
using Assurity.AgentPortal.Service.Handlers;
using Assurity.AgentPortal.Service.Validation;
using Assurity.AgentPortal.Utilities.Configs;
using Assurity.AgentPortal.Utilities.Emails;
using Assurity.AgentPortal.Utilities.Encryption;
using Assurity.AgentPortal.Utilities.Logging;
using Assurity.AgentPortal.Utilities.PdfCreation;
using Assurity.AgentPortal.Utilities.TiffCreation;
using Assurity.Common.Cryptography;
using Assurity.Document.Client;
using Assurity.Document.Contracts;
using Assurity.DocumentVault.Client;
using Assurity.PdfToTiffConversion.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ConfigurationManager = Assurity.AgentPortal.Utilities.Configs.ConfigurationManager;
using PdfToTiffClientHttp = Assurity.PdfToTiffConversion.Client.Http;

public static class IOCConfigs
{
    public static void AddAccessorDependencies(
        this IServiceCollection services,
        IConfigurationManager configurationManager)
    {
        // This ErrorResponseHandler will automatically process possible responses from the Accessor,
        // throwing exceptions when we dont' get what we expect and creating consistency with how we handle different responses from our underlying APIs
        services.AddTransient<ErrorResponseHandler>();

        services
            .AddHttpClient<IPolicyInfoApiAccessor, PolicyInfoApiAccessor>(
                httpClient => httpClient.BaseAddress = configurationManager.PolicyInfoServiceBaseUri)
            .AddHttpMessageHandler<ErrorResponseHandler>();
        services
            .AddHttpClient<ICommissionsApiAccessor, CommissionsApiAccessor>(
                httpClient => httpClient.BaseAddress = configurationManager.CommissionsServiceBaseUri)
            .AddHttpMessageHandler<ErrorResponseHandler>();
        services
            .AddHttpClient<IDebtApiAccessor, DebtApiAccessor>(
                httpClient => httpClient.BaseAddress = configurationManager.DebtServiceBaseUri)
            .AddHttpMessageHandler<ErrorResponseHandler>();
        services
            .AddHttpClient<IAgentApiAccessor, AgentApiAccessor>(
                httpClient => httpClient.BaseAddress = configurationManager.AgentServiceBaseUri)
            .AddHttpMessageHandler<ErrorResponseHandler>();
        services
            .AddHttpClient<IGroupInventoryApiAccessor, GroupInventoryApiAccessor>(
                httpClient => httpClient.BaseAddress = configurationManager.GroupInventoryServiceBaseUri)
            .AddHttpMessageHandler<ErrorResponseHandler>();
        services
            .AddHttpClient<IProductionCreditApiAccessor, ProductionCreditApiAccessor>(
            httpClient => httpClient.BaseAddress = configurationManager.ProductionCreditServiceBaseUri)
            .AddHttpMessageHandler<ErrorResponseHandler>();
        services
            .AddHttpClient<IListBillsApiAccessor, ListBillsApiAccessor>(
                httpClient => httpClient.BaseAddress = configurationManager.ListBillServiceBaseUri)
            .AddHttpMessageHandler<ErrorResponseHandler>();
        services
            .AddHttpClient<ILifePortraitsApiAccessor, LifePortraitsApiAccessor>(
                httpClient => httpClient.BaseAddress = configurationManager.LifePortraitsUri)
            .AddHttpMessageHandler<ErrorResponseHandler>();
        services
            .AddHttpClient<IApplicationTrackerApiAccessor, ApplicationTrackerApiAccessor>(
                httpClient => httpClient.BaseAddress = configurationManager.ApplicationTrackerServiceBaseUri)
            .AddHttpMessageHandler<ErrorResponseHandler>();

        // TODO: Claims is the only API that does not return a response body or play nice with the 404 error checking.
        // Once Claims has been updated to either return a response body or have consistent headers (currently, they differ between DEV and TEST),
        // add the MessageHandler ErrorResponseHandler to be consistent in our error handling across our API
        services
            .AddHttpClient<IClaimsApiAccessor, ClaimsApiAccessor>(
                httpClient => httpClient.BaseAddress = configurationManager.ClaimsServiceBaseUri);

        services
            .AddHttpClient<ITaxFormsApiAccessor, TaxFormsApiAccessor>(
                httpClient => httpClient.BaseAddress = configurationManager.TaxFormsServiceBaseUri)
            .AddHttpMessageHandler<ErrorResponseHandler>();
        services
            .AddHttpClient<IDocuSignApiAccessor, DocuSignApiAccessor>(
                httpClient => httpClient.BaseAddress = configurationManager.DocuSignServiceBaseUri)
            .AddHttpMessageHandler<ErrorResponseHandler>();
        services.AddHttpClientRetryPipeline();

        // Setting the BaseAddress of HttpClient on these accessors is unnecessary and therefore purposely omitted.
        // These accessors require the use of HttpClient.SendAsync so they can set headers per request, which
        // involves setting up an HttpRequestMessage thereby overriding any such BaseAddress configured here.
        services
            .AddHttpClient<IIdentityServerAccessor, IdentityServerAccessor>()
            .AddHttpMessageHandler<ErrorResponseHandler>();

        services.AddSingleton<PdfToTiffClientHttp.IHttpClient>(
            _ => new PdfToTiffClientHttp.PdfToTiffHttpClient(configurationManager.PdfToTiffBaseUri.AbsoluteUri));
        services.AddTransient<IPdfToTiffClient, PdfToTiffClient>();

        services.AddSingleton<MongoDBConnection>();

        var queueCredentials = new QueueCredentials
        {
            HostName = configurationManager.DocumentQueueHostName,
            Username = configurationManager.DocumentQueueUsername,
            Password = configurationManager.DocumentQueuePassword
        };

        services.AddScoped<IDocumentClient, DocumentClient>(
        serviceProvider => new DocumentClient(queueCredentials));

        services.AddTransient<IAlertsAccessor, AlertsAccessor>();
        services.AddTransient<IIntegrationAccessor, IntegrationAccessor>();
        services.AddTransient<IDocumentVaultAccessor, DocumentVaultAccessor>();
        services.AddTransient<IDocumentAccessor, DocumentAccessor>();
        services.AddTransient<IGlobalDataAccessor, GlobalDataAccessor>();
        services.AddTransient<IPdfToTiffAccessor, PdfToTiffAccessor>();
        services.AddTransient<IBenefitOptionsAccessor, BenefitOptionsAccessor>();
        services.AddTransient<IDocumentServiceAccessor, DocumentServiceAccessor>();
        services.AddTransient<IImpersonationAccessor, ImpersonationAccessor>();
        services.AddTransient<IIPipelineEngine, IPipelineEngine>();
        services.AddTransient<ISubaccountAccessor, SubaccountAccessor>();
        services.AddTransient<IPolicyDeliveryApiAccessor, PolicyDeliveryApiAccessor>();
        services.AddTransient<ILeadersConferenceAccessor, LeadersConferenceAccessor>();
        services.AddTransient<IIllustrationProApiAccessor, IllustrationProApiAccessor>();
        services.AddTransient<ILifePortraitsApiAccessor, LifePortraitsApiAccessor>();
    }

    public static void AddClientDependencies(
        this IServiceCollection services,
        IConfigurationManager configurationManager)
    {
        services.AddSingleton<IAuthorizationHandler, AssurityAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, ExcludeSubAccountsAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, ExcludeTerminatedAgentsAuthorizationHandler>();
        services.AddTransient<IFileValidator, FileValidator>();
        services.AddHttpClient<IDocumentVaultClient, DocumentVaultClient>(httpClient => httpClient.BaseAddress = configurationManager.DocumentVaultBaseUri);
    }

    public static void AddDbContextDependencies(
        this IServiceCollection services,
        IConfigurationManager configurationManager)
    {
        services.AddDbContextFactory<GlobalDataContext>(dbContextOptionsBuilder =>
            dbContextOptionsBuilder.UseSqlServer(
                configurationManager.GlobalDataConnectionString,
                sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));

        services.AddDbContextFactory<DataStoreContext>(dbContextOptionsBuilder =>
            dbContextOptionsBuilder.UseSqlServer(
                configurationManager.DataStoreConnectionString,
                sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));

        services.AddDbContextFactory<AssureLinkContext>(dbContextOptionsBuilder =>
            dbContextOptionsBuilder.UseSqlServer(
            configurationManager.AssureLinkConnectionString,
            sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));
    }

    public static void AddEngineDependencies(this IServiceCollection services)
    {
        services.AddTransient<IFileExportEngine, FileExportEngine>();
        services.AddTransient<ISendEngine, SendEngine>();
        services.AddTransient<IPdfUtilitiesEngine, PdfUtilitiesEngine>();
        services.AddTransient<IIllustrationProEngine, IllustrationProEngine>();
    }

    public static void AddManagerDependencies(this IServiceCollection services)
    {
        services.AddTransient<IPolicyInfoManager, PolicyInfoManager>();
        services.AddTransient<ICommissionAndDebtManager, CommissionAndDebtManager>();
        services.AddTransient<IDocumentVaultManager, DocumentVaultManager>();
        services.AddTransient<ISendManager, SendManager>();
        services.AddTransient<IExecute360DocumentManager, Execute360DocumentManager>();
        services.AddTransient<IProductionCreditManager, ProductionCreditManager>();
        services.AddTransient<IUserDataManager, UserDataManager>();
        services.AddTransient<IListBillsManager, ListBillsManager>();
        services.AddTransient<IClaimsManager, ClaimsManager>();
        services.AddTransient<ICaseManagementManager, CaseManagementManager>();
        services.AddTransient<ITaxFormsManager, TaxFormsManager>();
        services.AddTransient<IAgentHierarchyManager, AgentHierarchyManager>();
        services.AddTransient<IGroupInventoryManager, GroupInventoryManager>();
        services.AddTransient<IIllustrationProManager, IllustrationProManager>();
        services.AddTransient<ILifePortraitsManager, LifePortraitsManager>();
        services.AddTransient<IImpersonationManager, ImpersonationManager>();
        services.AddTransient<IIPipelineManager, IPipelineManager>();
        services.AddTransient<IAlertsManager, AlertsManager>();
        services.AddTransient<ISubaccountManager, SubaccountManager>();
        services.AddTransient<ILeadersConferenceManager, LeadersConferenceManager>();
        services.AddTransient<IPolicyDeliveryManager, PolicyDeliveryManager>();
    }

    public static void AddUtilityDependencies(this IServiceCollection services)
    {
        services.AddTransient<IConfigurationManager, ConfigurationManager>();
        services.AddTransient<IAesEncryptor, AesEncryptor>();
        services.AddTransient<IHttpRequestMessageValuesProvider, HttpRequestMessageValuesProvider>();
        services.AddTransient<ITiffCreator, TiffCreator>();
        services.AddTransient<IPdfCreator, PdfCreator>();
        services.AddTransient<IEncryption, Encryption>();
        services.AddTransient<IEmailUtility, EmailUtility>();
    }
}
