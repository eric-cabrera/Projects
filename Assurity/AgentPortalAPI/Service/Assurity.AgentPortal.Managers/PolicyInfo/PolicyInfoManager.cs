namespace Assurity.AgentPortal.Managers.PolicyInfo;

using Assurity.AgentPortal.Accessors.MongoDb.Contracts;
using Assurity.AgentPortal.Accessors.PolicyInfo;
using Assurity.AgentPortal.Contracts.PolicyInfo;
using Assurity.AgentPortal.Contracts.PolicyInfo.FileExport;
using Assurity.AgentPortal.Contracts.Shared;
using Assurity.AgentPortal.Engines;
using Assurity.PolicyInfo.Contracts.V1;
using Assurity.PolicyInfo.Contracts.V1.Enums;
using AutoMapper;

public class PolicyInfoManager : IPolicyInfoManager
{
    public PolicyInfoManager(
        IPolicyInfoApiAccessor policyInfoApiAccessor,
        IBenefitOptionsAccessor benefitOptionsAccessor,
        IMapper mapper,
        IFileExportEngine fileExportEngine)
    {
        PolicyInfoApiAccessor = policyInfoApiAccessor;
        BenefitOptionsAccessor = benefitOptionsAccessor;
        Mapper = mapper;
        FileExportEngine = fileExportEngine;
    }

    private IPolicyInfoApiAccessor PolicyInfoApiAccessor { get; }

    private IBenefitOptionsAccessor BenefitOptionsAccessor { get; }

    private IMapper Mapper { get; }

    private IFileExportEngine FileExportEngine { get; }

    public async Task<PolicySummariesResponse?> GetPolicySummaries(
        string agentId,
        Status status,
        string? queryString)
    {
        var policySummaries = await PolicyInfoApiAccessor
            .GetPolicySummaries(agentId, status, queryString);

        return policySummaries == null ? null : Mapper.Map<PolicySummariesResponse>(policySummaries);
    }

    public async Task<FileResponse?> GetPolicySummariesAsExcelDocument(
        string agentId,
        Status status,
        string? queryString)
    {
        var fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var fileName = $"{agentId}_{status}Policies_{DateTime.Now:yyyy'-'MM'-'dd}";
        var fileResponse = new FileResponse(fileName, fileType);

        return status switch
        {
            Status.Pending => await GetPendingPolicySummariesAsExcelDocument(agentId, queryString, fileResponse),
            Status.Active => await GetActivePolicySummariesAsExcelDocument(agentId, queryString, fileResponse),
            Status.Terminated => await GetTerminatedPolicySummariesAsExcelDocument(agentId, queryString, fileResponse),
            _ => fileResponse,
        };
    }

    public async Task<RequirementSummariesResponse?> GetPendingPolicyRequirements(
        string agentId,
        string? queryString)
    {
        var requirementSummariesResponse = await PolicyInfoApiAccessor
            .GetPendingPolicyRequirementSummaries(agentId, queryString);

        return requirementSummariesResponse == null
            ? null
            : Mapper.Map<RequirementSummariesResponse>(requirementSummariesResponse);
    }

    public async Task<PolicyResponse?> GetPolicyInfo(string policyNumber, string agentId)
    {
        var hiddenBenefitOptionsTask = BenefitOptionsAccessor.GetHiddenBenefitOptionsMappings();
        var policyTask = PolicyInfoApiAccessor.GetPolicyInfo(policyNumber, agentId);

        await Task.WhenAll(policyTask, hiddenBenefitOptionsTask);
        if (policyTask.Result == null)
        {
            return null;
        }

        if (hiddenBenefitOptionsTask.Result != null
            && hiddenBenefitOptionsTask.Result.Count != 0
            && policyTask.Result != null)
        {
            foreach (var benefit in policyTask.Result.Benefits
                .Where(benefit => benefit.BenefitOptions != null && benefit.BenefitOptions.Count > 0))
            {
                benefit.BenefitOptions = FilterHiddenBenefitOptions(benefit.BenefitOptions, hiddenBenefitOptionsTask.Result);
            }
        }

        return Mapper.Map<PolicyResponse>(policyTask.Result);
    }

    public async Task<PolicyStatusCountsResponse?> GetPolicyStatusCounts(string agentId)
    {
        var policyStatusCountsResponse = await PolicyInfoApiAccessor.GetPolicyStatusCounts(agentId);
        if (policyStatusCountsResponse == null)
        {
            return null;
        }

        return Mapper.Map<PolicyStatusCountsResponse>(policyStatusCountsResponse);
    }

    public async Task<bool> CheckAgentAccessToPolicy(string agentId, string policyNumber)
    {
        return await PolicyInfoApiAccessor.CheckAgentAccessToPolicy(agentId, policyNumber);
    }

    private static List<BenefitOption> FilterHiddenBenefitOptions(
        List<BenefitOption> benefitOptions,
        List<BenefitOptionsMapping> benefitOptionsToHide)
    {
        foreach (var hiddenOption in benefitOptionsToHide)
        {
            benefitOptions.RemoveAll(
                benefitOption => benefitOption.BenefitOptionName.Equals(hiddenOption.Category)
                    && benefitOption.BenefitOptionValue.Equals(hiddenOption.Option));
        }

        return benefitOptions;
    }

    private async Task<FileResponse?> GetPendingPolicySummariesAsExcelDocument(string agentId, string? queryString, FileResponse fileResponse)
    {
        var requirementSummariesResponse = await PolicyInfoApiAccessor.GetPendingPolicyRequirementSummaries(agentId, queryString);
        if (requirementSummariesResponse == null || requirementSummariesResponse.RequirementSummaries == null)
        {
            return null;
        }

        var headers = FileExportEngine.CreateHeaders<PendingPolicyExport>();
        var mappedPolicies = Mapper.Map<List<PendingPolicyExport>>(requirementSummariesResponse.RequirementSummaries);
        fileResponse.FileData = FileExportEngine.CreateExcelDocument(headers, mappedPolicies, $"Pending Policies");

        return fileResponse;
    }

    private async Task<FileResponse?> GetActivePolicySummariesAsExcelDocument(string agentId, string? queryString, FileResponse fileResponse)
    {
        var policySummariesResponse = await PolicyInfoApiAccessor.GetPolicySummaries(agentId, Status.Active, queryString);
        if (policySummariesResponse == null || policySummariesResponse.Policies == null)
        {
            return null;
        }

        var headers = FileExportEngine.CreateHeaders<ActivePolicyExport>();
        var mappedPolicies = Mapper.Map<List<ActivePolicyExport>>(policySummariesResponse.Policies);
        fileResponse.FileData = FileExportEngine.CreateExcelDocument(headers, mappedPolicies, $"Active Policies");

        return fileResponse;
    }

    private async Task<FileResponse?> GetTerminatedPolicySummariesAsExcelDocument(string agentId, string? queryString, FileResponse fileResponse)
    {
        var policySummariesResponse = await PolicyInfoApiAccessor.GetPolicySummaries(agentId, Status.Terminated, queryString);
        if (policySummariesResponse == null || policySummariesResponse.Policies == null)
        {
            return null;
        }

        var headers = FileExportEngine.CreateHeaders<TerminatedPolicyExport>();
        var mappedPolicies = Mapper.Map<List<TerminatedPolicyExport>>(policySummariesResponse.Policies);
        fileResponse.FileData = FileExportEngine.CreateExcelDocument(headers, mappedPolicies, $"Terminated Policies");

        return fileResponse;
    }
}