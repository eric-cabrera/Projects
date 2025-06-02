namespace Assurity.AgentPortal.Accessors.ProductionCredit;

using Assurity.AgentPortal.Contracts.ProductionCredit.Request;
using Assurity.Production.Contracts.V1.Individual;
using Assurity.Production.Contracts.V1.Shared.PolicyDetails;
using Assurity.Production.Contracts.V1.Worksite;

public interface IProductionCreditApiAccessor
{
    Task<IndividualProductionReport?> GetIndividualProductionCredit(string agentId, ProductionCreditParameters parameters, CancellationToken cancellationToken);

    Task<PolicyDetailsReport?> GetIndividualPolicyDetails(string agentId, ProductionCreditPolicyDetailsParameters parameters, CancellationToken cancellationToken);

    Task<WorksiteReport?> GetWorksiteProductionCredit(string agentId, WorksiteProductionCreditParameters parameters, CancellationToken cancellationToken);

    Task<PolicyDetailsReport?> GetWorksitePolicyDetails(string agentId, ProductionCreditPolicyDetailsParameters parameters, CancellationToken cancellationToken);

    Task<List<string>?> GetProductionCreditMarketcodes(string agentId, CancellationToken cancellationToken);
}