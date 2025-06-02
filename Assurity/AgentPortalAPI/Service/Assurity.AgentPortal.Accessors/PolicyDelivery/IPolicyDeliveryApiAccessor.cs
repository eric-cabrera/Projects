namespace Assurity.AgentPortal.Accessors.EConsent;

using Assurity.AgentPortal.Contracts.PolicyDelivery.Request;

public interface IPolicyDeliveryApiAccessor
{
    Task<bool> UpdateAgentPolicyDeliveryOptions(
         DocumentConnectOptions documentConnectOptions,
         string optionType,
         CancellationToken cancellationToken = default);

    Task<DocumentConnectOptions> GetPolicyDeliveryOptions(
      string agentNumber,
      string marketCode,
      CancellationToken cancellationToken = default);
}