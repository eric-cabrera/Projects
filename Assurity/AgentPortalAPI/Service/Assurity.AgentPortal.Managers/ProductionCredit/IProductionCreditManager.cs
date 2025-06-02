namespace Assurity.AgentPortal.Managers.ProductionCredit;

using Assurity.AgentPortal.Contracts.Enums;
using Assurity.AgentPortal.Contracts.ProductionCredit.FileExport;
using Assurity.AgentPortal.Contracts.ProductionCredit.Request;
using Assurity.AgentPortal.Contracts.ProductionCredit.Response.Individual;
using Assurity.AgentPortal.Contracts.ProductionCredit.Response.PolicyDetail;
using Assurity.AgentPortal.Contracts.ProductionCredit.Response.Worksite;
using Assurity.AgentPortal.Contracts.Shared;
using static Assurity.AgentPortal.Managers.ProductionCredit.ProductionCreditManager;

public interface IProductionCreditManager
{
    Task<IndividualProductionCreditSummary?> GetIndividualProductionCreditSummary(string agentId, ProductionCreditParameters parameters, CancellationToken cancellationToken = default);

    Task<ProductionCreditPolicyDetailsSummary?> GetIndividualPolicyDetailsSummary(string agentId, ProductionCreditPolicyDetailsParameters parameters, CancellationToken cancellationToken = default);

    Task<WorksiteProductionCreditSummary?> GetWorksiteProductionCreditSummary(string agentId, WorksiteProductionCreditParameters parameters, CancellationToken cancellationToken = default);

    Task<ProductionCreditPolicyDetailsSummary?> GetWorksitePolicyDetailsSummary(string agentId, ProductionCreditPolicyDetailsParameters parameters, CancellationToken cancellationToken = default);

    Task<FileResponse?> ExportIndividualPolicyDetailsSummary(string agentId, ProductionCreditPolicyDetailsParameters parameters, CancellationToken cancellationToken = default);

    Task<FileResponse?> ExportWorksitePolicyDetailsSummary(string agentId, ProductionCreditPolicyDetailsParameters parameters, CancellationToken cancellationToken = default);

    Task<FileResponse?> GetIndividualProductionCreditByGrouping(string agentId, ProductionCreditParameters parameters, GroupingType groupingType, CancellationToken cancellationToken = default);

    Task<FileResponse?> GetWorksiteProductionCreditExcelByTaps(
                string agentId,
                ProductionCreditViewType tap,
                WorksiteProductionCreditParameters parameters,
                CancellationToken cancellationToken = default);
}