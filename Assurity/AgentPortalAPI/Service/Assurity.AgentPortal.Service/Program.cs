#pragma warning disable SA1200 // Using directives should be placed correctly
using System.Reflection;
using Assurity.AgentPortal.Managers.PolicyInfo.Mapping;
using Assurity.AgentPortal.Service.AppJwtBearerEvents;
using Assurity.AgentPortal.Service.Authorization;
using Assurity.AgentPortal.Service.HealthCheck;
using Assurity.AgentPortal.Service.Helpers;
using Assurity.AgentPortal.Service.IOCConfig;
using Assurity.AgentPortal.Service.IOCConfig.Extensions;
using Assurity.AgentPortal.Service.Middleware;
using Assurity.Common.Cryptography;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Serilog;
using ConfigurationManager = Assurity.AgentPortal.Utilities.Configs.ConfigurationManager;

#pragma warning restore SA1200 // Using directives should be placed correctly

// Configure the logger
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

Log.Debug("Starting Assurity.AgentCenter.API.");

try
{
    JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
        loggerConfiguration
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services));

    var configurationManager = new ConfigurationManager(
        builder.Configuration,
        new AesEncryptor());

    const string readyHealthCheckRoute = "ready";
    builder.Services.AddHealthChecks(configurationManager, readyHealthCheckRoute);

    // Add services to the container.
    builder.Services.AddUtilityDependencies();
    builder.Services.AddDbContextDependencies(configurationManager);
    builder.Services.AddAccessorDependencies(configurationManager);
    builder.Services.AddEngineDependencies();
    builder.Services.AddManagerDependencies();
    builder.Services.AddClientDependencies(configurationManager);
    builder.Services.AddElasticSearch(configurationManager);
    builder.Services.AddMemoryCache();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<AppJwtBearerEvents>();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = AuthenticationSchemes.PingOneScheme;
    })
    .AddJwtBearer(AuthenticationSchemes.PingOneScheme, options =>
     {
         options.Authority = builder.Configuration["Authentication:PingOne:Authority"];
         options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
         {
             NameClaimType = "name",
             RoleClaimType = "role",
             ValidateIssuerSigningKey = true,
             ValidateAudience = true,
             ValidateIssuer = true,
             ValidAudiences = new List<string>
             {
                "AgentPortalAPI"
             },
         };
         options.Audience = builder.Configuration["Authentication:PingOne:Audience"];
         options.EventsType = typeof(AppJwtBearerEvents);
     })
    .AddJwtBearer(AuthenticationSchemes.AzureAdScheme, options =>
    {
        options.Authority = builder.Configuration["Authentication:AzureAd:Authority"];
        options.Audience = builder.Configuration["Authentication:AzureAd:Audience"];
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Authentication:AzureAd:Authority"],
        };
        options.EventsType = typeof(AppJwtBearerEvents);
    });

    builder.Services.AddAuthorizationPolicies();

    builder.Services.AddSwaggerGenNewtonsoftSupport();
    builder.Services.AddControllers()
           .AddNewtonsoftJson(options => { options.SerializerSettings.Converters.Add(new StringEnumConverter()); })
           .ConfigureApiBehaviorOptions(options =>
           {
               // Prevent Invalid ModelState from automatically flagging bad request to catch invalid requests and log appropriately.
               options.SuppressModelStateInvalidFilter = true;
           });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    builder.Services.AddSwaggerGen(
        c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "AgentPortalAPI", Version = "v1.0.0" });

            var securitySchema = new OpenApiSecurityScheme
            {
                Description = "Using the Authorization header with the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            c.AddSecurityDefinition("Bearer", securitySchema);

            c.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                });

            c.CustomSchemaIds(x => x.FullName);
            c.IncludeXmlComments(xmlPath);
            c.SchemaFilter<EnumSchemaFilter>();
        });

    // Brings in all mapping profiles relative to the Manager assembly
    builder.Services.AddAutoMapper(typeof(PolicyMappingProfile).Assembly);

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (builder.Configuration["Environment"] != "PROD")
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgentPortalAPI v1");
            if (builder.Configuration["Environment"] == "LOCAL")
            {
                c.ConfigObject.AdditionalItems.Add("syntaxHighlight", false);
            }
        });
    }

    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
        options.GetLevel = LogHelper.ExcludeCustomPaths;
    });

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseLoggingMiddleware();
    app.MapControllers();

    app.UseHealthChecks("/", new HealthCheckOptions
    {
        Predicate = _ => false // Don't run any health checks.
    });
    app.UseHealthChecks($"/healthcheck/{readyHealthCheckRoute}", new HealthCheckOptions
    {
        Predicate = healthCheck => healthCheck.Tags.Contains(readyHealthCheckRoute),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.Run();

    Log.Debug("Assurity.AgentCenter.API started.");
}
catch (Exception exception)
{
    // catch setup errors
    Log.Error(exception, "Stopped Assurity.AgentCenter.API startup because of an exception.");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    Log.CloseAndFlush();
}