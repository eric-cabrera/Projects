namespace Assurity.AgentPortal.Managers.PolicyDelivery;

using System.Threading;
using System.Threading.Tasks;
using Assurity.AgentPortal.Contracts.PolicyDelivery.Request;

public interface IPolicyDeliveryManager
{
    Task<bool?> UpdateAgentPolicyDeliveryOptions(
        string loggedInAgentNumber,
        DocumentConnectOptions documentConnectOptions,
        CancellationToken cancellationToken = default);

    Task<DocumentConnectOptions?> GetPolicyDeliveryOptions(
     string loggedInAgentNumber,
     string viewAsAgentNumber,
     string viewAsMarketCode,
     CancellationToken cancellationToken = default);
}