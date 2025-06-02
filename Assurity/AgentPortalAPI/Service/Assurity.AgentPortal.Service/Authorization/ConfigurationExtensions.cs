namespace Assurity.AgentPortal.Service.Authorization;

using Microsoft.AspNetCore.Authorization;

public static class ConfigurationExtensions
{
    private static string[] AllAuthSchemes => [AuthenticationSchemes.PingOneScheme, AuthenticationSchemes.AzureAdScheme];

    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(AllAuthSchemes)
            .Build())
            .AddPolicy(AuthorizationPolicyConstants.HomeOfficePolicy, policy =>
                policy.RequireAuthenticatedUser()
                .AddAuthenticationSchemes(AuthenticationSchemes.AzureAdScheme))
            .AddPolicy(AuthorizationPolicyConstants.ExcludeSubAccounts, policy =>
                policy.AddAuthenticationSchemes(AllAuthSchemes)
                    .RequireAuthenticatedUser()
                    .Requirements.Add(new ExcludeSubAccountsAuthorizationRequirement()))
            .AddPolicy(AuthorizationPolicyConstants.ExcludeTerminatedAgents, policy =>
                policy.AddAuthenticationSchemes(AllAuthSchemes)
                    .RequireAuthenticatedUser()
                    .Requirements.Add(new ExcludeTerminatedAgentsAuthorizationRequirement(allowHomeOffice: true)))
            .AddPolicy(AuthorizationPolicyConstants.PendingActiveTerminated, policy =>
                policy.RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(AllAuthSchemes)
                    .Requirements.Add(new AssurityAuthorizationRequirement(RoleConstants.PendingActiveTerminatedRole)))
            .AddPolicy(AuthorizationPolicyConstants.CommissionsDebt, policy =>
                policy.AddAuthenticationSchemes(AllAuthSchemes)
                    .RequireAuthenticatedUser()
                    .Requirements.Add(new AssurityAuthorizationRequirement(RoleConstants.CommissionsRole, true)))
            .AddPolicy(AuthorizationPolicyConstants.ProductionCredit, policy =>
                policy.AddAuthenticationSchemes(AllAuthSchemes)
                    .RequireAuthenticatedUser()
                    .Requirements.Add(new AssurityAuthorizationRequirement(RoleConstants.ProductionCreditRole)))
            .AddPolicy(AuthorizationPolicyConstants.ListBill, policy =>
                policy.AddAuthenticationSchemes(AllAuthSchemes)
                    .RequireAuthenticatedUser()
                    .Requirements.Add(new AssurityAuthorizationRequirement(RoleConstants.ListBillRole)))
            .AddPolicy(AuthorizationPolicyConstants.Claims, policy =>
                policy.AddAuthenticationSchemes(AllAuthSchemes)
                    .RequireAuthenticatedUser()
                    .Requirements.Add(new AssurityAuthorizationRequirement(RoleConstants.ClaimsRole)))
            .AddPolicy(AuthorizationPolicyConstants.TaxForms, policy =>
                policy.AddAuthenticationSchemes(AllAuthSchemes)
                    .RequireAuthenticatedUser()
                    .Requirements.Add(new AssurityAuthorizationRequirement(RoleConstants.TaxFormsRole, true)))
            .AddPolicy(AuthorizationPolicyConstants.Contracting, policy =>
                policy.AddAuthenticationSchemes(AllAuthSchemes)
                    .RequireAuthenticatedUser()
                    .Requirements.Add(new AssurityAuthorizationRequirement(RoleConstants.ContractingRole)))
            .AddPolicy(AuthorizationPolicyConstants.Hierarchy, policy =>
                policy.AddAuthenticationSchemes(AllAuthSchemes)
                    .RequireAuthenticatedUser()
                    .Requirements.Add(new AssurityAuthorizationRequirement(RoleConstants.HierarchyRole)))
            .AddPolicy(AuthorizationPolicyConstants.WorksiteParticipation, policy =>
                policy.AddAuthenticationSchemes(AllAuthSchemes)
                    .RequireAuthenticatedUser()
                    .Requirements.Add(new AssurityAuthorizationRequirement(RoleConstants.WorksiteParticipationRole)))
            .AddPolicy(AuthorizationPolicyConstants.CaseManagement, policy =>
                policy.AddAuthenticationSchemes(AllAuthSchemes)
                    .RequireAuthenticatedUser()
                    .Requirements.Add(new AssurityAuthorizationRequirement(RoleConstants.CaseManagementRole)));

        return services;
    }
}
