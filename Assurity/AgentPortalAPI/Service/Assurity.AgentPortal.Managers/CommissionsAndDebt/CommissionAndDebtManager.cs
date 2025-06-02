namespace Assurity.AgentPortal.Managers.CommissionsAndDebt;

using System.Threading;
using Assurity.AgentPortal.Accessors.Agent;
using Assurity.AgentPortal.Accessors.CommissionsAndDebt;
using Assurity.AgentPortal.Contracts.CommissionsDebt;
using Assurity.AgentPortal.Contracts.CommissionsDebt.FileExport;
using Assurity.AgentPortal.Contracts.CommissionsDebt.Request;
using Assurity.AgentPortal.Contracts.CommissionsDebt.Response;
using Assurity.AgentPortal.Contracts.Enums;
using Assurity.AgentPortal.Contracts.Shared;
using Assurity.AgentPortal.Engines;
using AutoMapper;

public class CommissionAndDebtManager : ICommissionAndDebtManager
{
    private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public CommissionAndDebtManager(
        IMapper mapper,
        ICommissionsApiAccessor commissionsApiAccessor,
        IDebtApiAccessor debtApiAccessor,
        IAgentApiAccessor agentApiAccessor,
        IFileExportEngine fileExportEngine)
    {
        Mapper = mapper;
        CommissionsApiAccessor = commissionsApiAccessor;
        DebtApiAccessor = debtApiAccessor;
        AgentApiAccessor = agentApiAccessor;
        FileExportEngine = fileExportEngine;
    }

    private IAgentApiAccessor AgentApiAccessor { get; }

    private ICommissionsApiAccessor CommissionsApiAccessor { get; }

    private IDebtApiAccessor DebtApiAccessor { get; }

    private IFileExportEngine FileExportEngine { get; }

    private IMapper Mapper { get; }

    public async Task<CommissionResponse?> GetCommissionAndSummaryData(
        string agentId,
        PolicyDetailsParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var agentIds = await BuildAgentIdFilter(agentId, parameters.ViewAsAgentId, cancellationToken);
        if (agentIds == null)
        {
            return null;
        }

        var commissionParametersDTO = Mapper.Map<Accessors.DTOs.CommissionParameters>(parameters);

        var cycleTask = CommissionsApiAccessor.GetCommissionsCycle(
            agentIds.FilteredAgentIds,
            parameters.CycleBeginDate,
            parameters.CycleEndDate,
            parameters.WritingAgentIds,
            cancellationToken);
        var policyDetailsTask = CommissionsApiAccessor.GetPolicyDetails(agentIds.FilteredAgentIds, commissionParametersDTO, cancellationToken);
        var commissionSummaryTask = CommissionsApiAccessor.GetCommissionsSummary(agentIds.FilteredAgentIds, cancellationToken);

        await Task.WhenAll(cycleTask, policyDetailsTask, commissionSummaryTask);
        if (cycleTask.Result == null || policyDetailsTask.Result == null || commissionSummaryTask.Result == null)
        {
            return null;
        }

        var commissionResponse = new CommissionResponse
        {
            CommissionCycle = Mapper.Map<CommissionCycle>(cycleTask.Result)
        };

        commissionResponse.CommissionCycle.CycleStartDate = parameters.CycleBeginDate;
        commissionResponse.CommissionCycle.CycleEndDate = parameters.CycleEndDate;

        if (parameters.CycleEndDate > DateTimeOffset.Now)
        {
            commissionResponse.CommissionCycle.Estimated = true;
        }

        commissionResponse.CommissionDetails = Mapper.Map<CommissionDetails>(policyDetailsTask.Result);
        commissionResponse.CommissionSummary = Mapper.Map<CommissionSummary>(commissionSummaryTask.Result);

        return commissionResponse;
    }

    public async Task<FileResponse?> GetPolicyDetailsExcel(
    string agentId,
    PolicyDetailsParameters parameters,
    CancellationToken cancellationToken = default)
    {
        parameters.Page = null;
        parameters.PageSize = null;

        var agentIds = await BuildAgentIdFilter(agentId, parameters.ViewAsAgentId, cancellationToken);
        var mappedParameters = Mapper.Map<Accessors.DTOs.CommissionParameters>(parameters);
        mappedParameters.DisablePagination = true;

        var policyDetailsResponse = await CommissionsApiAccessor.GetPolicyDetails(agentIds.FilteredAgentIds, mappedParameters, cancellationToken);
        if (policyDetailsResponse == null)
        {
            return null;
        }

        var mappedPolicies = Mapper.Map<List<PolicyDetailsExport>>(policyDetailsResponse.PolicyDetails);
        if (mappedPolicies == null)
        {
            return null;
        }

        var fileDownloadResponse = new FileResponse("Commissions_PolicyDetails", ExcelContentType);
        var headers = FileExportEngine.CreateHeaders<PolicyDetailsExport>();
        var document = FileExportEngine.CreateExcelDocument(headers, mappedPolicies, $"Policy Details");

        fileDownloadResponse.FileData = document;

        return fileDownloadResponse;
    }

    public async Task<WritingAgentDetailsResponse?> GetCommissionDataByWritingAgent(
        string agentId,
        WritingAgentParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var agentIds = await BuildAgentIdFilter(agentId, parameters.ViewAsAgentId, cancellationToken);
        if (agentIds == null)
        {
            return null;
        }

        var commissionParametersDTO = Mapper.Map<Accessors.DTOs.CommissionParameters>(parameters);

        var cycleTask = CommissionsApiAccessor.GetCommissionsCycle(
            agentIds.FilteredAgentIds,
            parameters.CycleBeginDate,
            parameters.CycleEndDate,
            parameters.WritingAgentIds,
            cancellationToken);

        var writingAgentDetailsTask = CommissionsApiAccessor.GetWritingAgentDetails(
            agentIds.FilteredAgentIds,
            commissionParametersDTO,
            cancellationToken);

        await Task.WhenAll(cycleTask, writingAgentDetailsTask);
        if (cycleTask.Result == null || writingAgentDetailsTask.Result == null)
        {
            return null;
        }

        var response = Mapper.Map<WritingAgentDetailsResponse>(writingAgentDetailsTask.Result);
        response.CommissionCycle = Mapper.Map<CommissionCycle>(cycleTask.Result);

        return response;
    }

    public async Task<FileResponse?> GetWritingAgentDetailsExcel(
    string agentId,
    WritingAgentParameters parameters,
    CancellationToken cancellationToken = default)
    {
        parameters.Page = null;
        parameters.PageSize = null;

        var agentIds = await BuildAgentIdFilter(agentId, parameters.ViewAsAgentId, cancellationToken);
        var mappedParameters = Mapper.Map<Accessors.DTOs.CommissionParameters>(parameters);
        mappedParameters.DisablePagination = true;

        var writingAgentsResponse = await CommissionsApiAccessor.GetWritingAgentDetails(agentIds.FilteredAgentIds, mappedParameters, cancellationToken);
        if (writingAgentsResponse == null)
        {
            return null;
        }

        var mappedDetails = Mapper.Map<List<WritingAgentDetail>>(writingAgentsResponse.WritingAgentCommissions);
        if (mappedDetails == null)
        {
            return null;
        }

        var fileResponse = new FileResponse("Commissions_WritingAgentDetails", ExcelContentType);
        var headers = FileExportEngine.CreateHeaders<WritingAgentDetail>();
        var document = FileExportEngine.CreateExcelDocument(headers, mappedDetails, $"Writing Agent Details");

        fileResponse.FileData = document;

        return fileResponse;
    }

    public async Task<Stream?> GetAgentStatement(
        string agentId,
        string sessionId,
        string requestAgentId,
        DateTime cycleDate,
        AgentStatementType agentStatementType,
        CancellationToken cancellationToken = default)
    {
        var selectedAgentId = agentId;

        if (!string.IsNullOrEmpty(requestAgentId))
        {
            var agentIds = await BuildAgentIdFilter(agentId, requestAgentId, cancellationToken);
            selectedAgentId = agentIds.FilteredAgentIds.First();
        }

        var fileStream = await CommissionsApiAccessor.GetAgentStatement(
            selectedAgentId,
            cycleDate,
            agentStatementType,
            sessionId,
            cancellationToken);

        return fileStream;
    }

    public async Task<DebtResponse> GetUnsecuredAdvances(
        string agentId,
        UnsecuredAdvanceParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var mappedParameters = Mapper.Map<Accessors.DTOs.DebtParameters>(parameters);

        var unsecuredAdvances = await DebtApiAccessor.GetUnsecuredAdvances(
            agentId, mappedParameters, cancellationToken);

        var debtResponse = Mapper.Map<DebtResponse>(unsecuredAdvances);

        return debtResponse;
    }

    public async Task<FileResponse?> GetUnsecuredAdvancesExcel(
        string agentId,
        UnsecuredAdvanceParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var mappedParameters = Mapper.Map<Accessors.DTOs.DebtParameters>(parameters);

        var advances = await DebtApiAccessor.GetAllUnsecuredAdvances(agentId, mappedParameters, cancellationToken);
        if (advances == null)
        {
            return null;
        }

        var mappedAdvances = advances?.SelectMany(advance => Mapper.Map<List<UnsecuredDebtExport>>(advance)).ToList();
        if (mappedAdvances == null)
        {
            return null;
        }

        var fileResponse = new FileResponse($"UnsecuredAdvances_{agentId}", ExcelContentType);
        var headers = FileExportEngine.CreateHeaders<UnsecuredDebtExport>();
        var document = FileExportEngine.CreateExcelDocument(headers, mappedAdvances, $"Unsecured Advances");

        fileResponse.FileData = document;

        return fileResponse;
    }

    public async Task<DebtResponse> GetSecuredAdvances(
        string agentId,
        SecuredAdvanceParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var mappedParameters = Mapper.Map<Accessors.DTOs.DebtParameters>(parameters);

        var securedAdvances = await DebtApiAccessor.GetSecuredAdvances(
            agentId, mappedParameters, cancellationToken);

        var debtResponse = Mapper.Map<DebtResponse>(securedAdvances);

        return debtResponse;
    }

    public async Task<FileResponse?> GetSecuredAdvancesExcel(
    string agentId,
    SecuredAdvanceParameters parameters,
    CancellationToken cancellationToken = default)
    {
        var mappedParameters = Mapper.Map<Accessors.DTOs.DebtParameters>(parameters);

        var advances = await DebtApiAccessor.GetAllSecuredAdvances(agentId, mappedParameters, cancellationToken);
        if (advances == null)
        {
            return null;
        }

        var mappedAdvances = advances?.SelectMany(advance => Mapper.Map<List<SecuredDebtExport>>(advance)).ToList();
        if (mappedAdvances == null)
        {
            return null;
        }

        var fileResponse = new FileResponse($"SecuredAdvances_{agentId}", ExcelContentType);
        var headers = FileExportEngine.CreateHeaders<SecuredDebtExport>();
        var document = FileExportEngine.CreateExcelDocument(headers, mappedAdvances, $"Secured Advances");

        fileResponse.FileData = document;

        return fileResponse;
    }

    public async Task<AgentStatementOptions?> GetAgentStatementOptions(string agentId, CancellationToken cancellationToken = default)
    {
        var agentIds = await BuildAgentIdFilter(agentId, null, cancellationToken);
        if (agentIds == null)
        {
            return null;
        }

        var agentStatementOptionsResponse = await CommissionsApiAccessor.GetAgentStatementOptions(agentIds.FilteredAgentIds, cancellationToken);

        return Mapper.Map<AgentStatementOptions>(agentStatementOptionsResponse);
    }

    private async Task<AdditionalAgentIds?> BuildAgentIdFilter(
        string agentId,
        string? selectedAgentId,
        CancellationToken cancellationToken)
    {
        var additionalAgentIds = await AgentApiAccessor.GetAdditionalAgentIds(agentId, cancellationToken);
        if (additionalAgentIds == null)
        {
            return null;
        }

        if (additionalAgentIds.Count == 0)
        {
            additionalAgentIds.Add(agentId);
        }

        if (string.IsNullOrEmpty(selectedAgentId))
        {
            return new AdditionalAgentIds(additionalAgentIds);
        }

        return new AdditionalAgentIds(additionalAgentIds, selectedAgentId);
    }
}
