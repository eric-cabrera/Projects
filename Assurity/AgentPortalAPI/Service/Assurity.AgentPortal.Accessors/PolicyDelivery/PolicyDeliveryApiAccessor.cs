namespace Assurity.AgentPortal.Accessors.EConsent;

using System.Threading.Tasks;
using Assurity.AgentPortal.Accessors.AssureLink.Context;
using Assurity.AgentPortal.Accessors.AssureLink.Entities;
using Assurity.AgentPortal.Contracts.PolicyDelivery.Request;
using Assurity.AgentPortal.Utilities.Configs;
using Assurity.AgentPortal.Utilities.Encryption;
using Microsoft.EntityFrameworkCore;

public class PolicyDeliveryApiAccessor : IPolicyDeliveryApiAccessor
{
    private const string SharedSecret = "aa23fcb39a5a37a592c0399c9ec5cc14acc3007c";

    public PolicyDeliveryApiAccessor(
        IDbContextFactory<AssureLinkContext> assureLinkContextFactory,
        IEncryption encryption,
        IConfigurationManager configuration)
    {
        AssureLinkContextFactory = assureLinkContextFactory;
        Encryption = encryption;
        Config = configuration;
    }

    private IDbContextFactory<AssureLinkContext> AssureLinkContextFactory { get; }

    private IConfigurationManager Config { get; }

    private IEncryption Encryption { get; }

    public async Task<DocumentConnectOptions> GetPolicyDeliveryOptions(
     string agentNumber,
     string marketCode,
     CancellationToken cancellationToken = default)
    {
        using var context = await AssureLinkContextFactory.CreateDbContextAsync(cancellationToken);

        var agentOption = context.AgentOptions
           .Where(x => x.AgentId == agentNumber &&
                       x.MarketCode == marketCode &&
                       x.OptionType == "EDelivery")
           .FirstOrDefault();

        var agentLinkOption = context.AgentLinkOptions
            .Where(x => x.AgentId == agentNumber &&
                        x.MarketCode == marketCode &&
                        x.OptionType == "EDelivery")
            .FirstOrDefault();

        return new DocumentConnectOptions
        {
            ViewAsAgentNumber = agentOption?.AgentId ?? string.Empty,
            ViewAsMarketCode = agentOption?.MarketCode ?? string.Empty,
            OptOutForEDelivery = agentOption?.OptOutForEDelivery ?? false,
            AgentLinkSelected = agentOption?.AgentLinkSelected ?? false,
            IncludeDownline = agentOption?.IncludeDownline ?? false,
            Email = agentLinkOption?.Email ?? string.Empty,
            AccessCode = string.IsNullOrEmpty(agentLinkOption?.AccessCode)
            ? string.Empty
            : Encryption.DecryptGAC(agentLinkOption.AccessCode, Config.Environment, SharedSecret),
            LastSavedDate = agentOption?.DateUpdated
        };
    }

    public async Task<bool> UpdateAgentPolicyDeliveryOptions(
       DocumentConnectOptions documentConnectOptions,
       string optionType,
       CancellationToken cancellationToken = default)
    {
        using var context = await AssureLinkContextFactory.CreateDbContextAsync(cancellationToken);

        await UpdateOrInsertAgentOptions(context, documentConnectOptions, optionType, cancellationToken);
        await UpdateOrInsertAgentLinkOption(context, documentConnectOptions, optionType, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task UpdateOrInsertAgentOptions(
      AssureLinkContext context,
      DocumentConnectOptions documentConnectOptions,
      string optionType,
      CancellationToken cancellationToken)
    {
        if (documentConnectOptions == null)
        {
            return;
        }

        var agentOption = context.AgentOptions
          .Where(x => x.AgentId == documentConnectOptions.ViewAsAgentNumber &&
                      x.MarketCode == documentConnectOptions.ViewAsMarketCode &&
                      x.OptionType == optionType)
          .FirstOrDefault();

        if (agentOption == null)
        {
            agentOption = new AgentOptions
            {
                AgentId = documentConnectOptions.ViewAsAgentNumber,
                MarketCode = documentConnectOptions.ViewAsMarketCode,
                OptionType = optionType,
                OptOutForEDelivery = documentConnectOptions.OptOutForEDelivery,
                AgentLinkSelected = documentConnectOptions.AgentLinkSelected,
                IncludeDownline = documentConnectOptions.IncludeDownline,
                DateUpdated = DateTime.UtcNow
            };
            await context.AgentOptions.AddAsync(agentOption, cancellationToken);
        }
        else
        {
            agentOption.OptOutForEDelivery = documentConnectOptions.OptOutForEDelivery;
            agentOption.AgentLinkSelected = documentConnectOptions.AgentLinkSelected;
            agentOption.IncludeDownline = documentConnectOptions.IncludeDownline;
            agentOption.DateUpdated = DateTime.UtcNow;

            context.AgentOptions.Update(agentOption);
        }
    }

    private async Task UpdateOrInsertAgentLinkOption(
         AssureLinkContext context,
         DocumentConnectOptions documentConnectOptions,
         string optionType,
         CancellationToken cancellationToken)
    {
        if (documentConnectOptions == null)
        {
            return;
        }

        var email = string.IsNullOrEmpty(documentConnectOptions.Email) ? string.Empty : documentConnectOptions.Email;
        var accessCode = string.IsNullOrEmpty(documentConnectOptions.AccessCode)
            ? string.Empty
            : Encryption.EncryptGAC(documentConnectOptions.AccessCode, Config.Environment, SharedSecret);

        var agentLinkOption = context.AgentLinkOptions
           .Where(x => x.AgentId == documentConnectOptions.ViewAsAgentNumber &&
                       x.MarketCode == documentConnectOptions.ViewAsMarketCode &&
                       x.OptionType == optionType)
           .FirstOrDefault();

        if (agentLinkOption == null)
        {
            agentLinkOption = new AgentLinkOptions
            {
                AgentId = documentConnectOptions.ViewAsAgentNumber,
                MarketCode = documentConnectOptions.ViewAsMarketCode,
                OptionType = optionType,
                Email = email,
                AccessCode = accessCode,
                DateUpdated = DateTime.UtcNow
            };
            await context.AgentLinkOptions.AddAsync(agentLinkOption, cancellationToken);
        }
        else
        {
            agentLinkOption.Email = email;
            agentLinkOption.AccessCode = accessCode;
            agentLinkOption.DateUpdated = DateTime.UtcNow;

            context.AgentLinkOptions.Update(agentLinkOption);
        }
    }
}
