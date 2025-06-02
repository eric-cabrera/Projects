namespace Assurity.AgentPortal.Managers.PolicyDelivery;

using System.Threading;
using System.Threading.Tasks;
using Assurity.AgentPortal.Accessors.Agent;
using Assurity.AgentPortal.Accessors.EConsent;
using Assurity.AgentPortal.Contracts.PolicyDelivery.Request;
using Microsoft.Extensions.Logging;

public class PolicyDeliveryManager : IPolicyDeliveryManager
{
    public PolicyDeliveryManager(
        IPolicyDeliveryApiAccessor eConsentAccessor,
        ILogger<PolicyDeliveryManager> logger,
        IAgentApiAccessor agentApiAccessor)
    {
        EConsentAccessor = eConsentAccessor;
        Logger = logger;
        AgentApiAccessor = agentApiAccessor;
    }

    private IPolicyDeliveryApiAccessor EConsentAccessor { get; }

    private IAgentApiAccessor AgentApiAccessor { get; }

    private ILogger<PolicyDeliveryManager> Logger { get; }

    public async Task<bool?> UpdateAgentPolicyDeliveryOptions(
        string loggedInAgentNumber,
        DocumentConnectOptions documentConnectOptions,
        CancellationToken cancellationToken = default)
    {
        var additionalAgentIds = await AgentApiAccessor.GetAdditionalAgentIds(loggedInAgentNumber, cancellationToken);
        if (additionalAgentIds == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(documentConnectOptions.ViewAsAgentNumber) && !additionalAgentIds.Contains(documentConnectOptions.ViewAsAgentNumber))
        {
            Logger.LogWarning("Agent {loggedInAgentNumber} does not have access to agent {viewAsAgentNumber}", loggedInAgentNumber, documentConnectOptions.ViewAsAgentNumber);
            return null;
        }

        var resultEDelivery = await EConsentAccessor.UpdateAgentPolicyDeliveryOptions(
            documentConnectOptions,
            "EDelivery",
            cancellationToken);

        var resultERequirements = await EConsentAccessor.UpdateAgentPolicyDeliveryOptions(
            documentConnectOptions,
            "ERequirements",
            cancellationToken);

        return resultEDelivery && resultERequirements;
    }

    public async Task<DocumentConnectOptions?> GetPolicyDeliveryOptions(
        string loggedInAgentNumber,
        string viewAsAgentNumber,
        string viewAsMarketCode,
        CancellationToken cancellationToken = default)
    {
        var additionalAgentIds = await AgentApiAccessor.GetAdditionalAgentIds(loggedInAgentNumber, cancellationToken);
        if (additionalAgentIds == null)
        {
            return null;
        }

        if (!additionalAgentIds.Contains(viewAsAgentNumber))
        {
            Logger.LogWarning("Agent {loggedInAgentNumber} does not have access to agent {viewAsAgentNumber}", loggedInAgentNumber, viewAsAgentNumber);
            return null;
        }

        return await EConsentAccessor.GetPolicyDeliveryOptions(
                viewAsAgentNumber,
                viewAsMarketCode,
                cancellationToken);
    }
}
