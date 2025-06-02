namespace Assurity.AgentPortal.Managers.GroupInventory;

using System.Threading;
using System.Threading.Tasks;
using Assurity.AgentPortal.Accessors.Agent;
using Assurity.AgentPortal.Accessors.GroupInventory;
using Assurity.AgentPortal.Accessors.MongoDb.Contracts;
using Assurity.AgentPortal.Accessors.PolicyInfo;
using Assurity.AgentPortal.Contracts.GroupInventory.FileExport;
using Assurity.AgentPortal.Contracts.GroupInventory.Request;
using Assurity.AgentPortal.Contracts.GroupInventory.Response;
using Assurity.AgentPortal.Contracts.Shared;
using Assurity.AgentPortal.Engines;
using Assurity.AgentPortal.Managers.MappingExtensions;
using Assurity.AgentPortal.Utilities.Formatting;
using Assurity.PolicyInfo.Contracts.V1.Enums;
using AutoMapper;
using Microsoft.Extensions.Logging;
using GroupsAPI = Assurity.Groups.Contracts;

public class GroupInventoryManager : IGroupInventoryManager
{
    public GroupInventoryManager(
        IGroupInventoryApiAccessor groupsApiAccessor,
        IBenefitOptionsAccessor benefitOptionsAccessor,
        IAgentApiAccessor agentApiAccessor,
        ILogger<GroupInventoryManager> logger,
        IMapper mapper,
        IFileExportEngine fileExportEngine)
    {
        GroupsApiAccessor = groupsApiAccessor;
        BenefitOptionsAccessor = benefitOptionsAccessor;
        AgentApiAccessor = agentApiAccessor;
        Logger = logger;
        Mapper = mapper;
        FileExportEngine = fileExportEngine;
    }

    private IFileExportEngine FileExportEngine { get; }

    private IGroupInventoryApiAccessor GroupsApiAccessor { get; }

    private IBenefitOptionsAccessor BenefitOptionsAccessor { get; }

    private ILogger<GroupInventoryManager> Logger { get; }

    private IMapper Mapper { get; }

    private IAgentApiAccessor AgentApiAccessor { get; }

    public async Task<GroupSummaryResponse?> GetGroupSummary(
         string loggedInAgentNumber,
         GroupSummaryQueryParameters queryParameters,
         CancellationToken cancellationToken = default)
    {
        var additionalAgentIds = await AgentApiAccessor.GetAdditionalAgentIds(loggedInAgentNumber, cancellationToken);
        if (additionalAgentIds == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(queryParameters.ViewAsAgentId) && !additionalAgentIds.Contains(queryParameters.ViewAsAgentId))
        {
            throw new Exception($"Logged-in agent {loggedInAgentNumber} does not have access to the specified agent. ViewAsAgentId: {queryParameters.ViewAsAgentId}");
        }

        var mappedGroupSummaryQueryParameters = Mapper.Map<GroupsAPI.GroupSummaryQueryParameters>(queryParameters);
        var response = await GroupsApiAccessor.GetGroupSummary(
                loggedInAgentNumber,
                mappedGroupSummaryQueryParameters,
                cancellationToken);

        if (response == null)
        {
            return null;
        }

        var groupSummaryResponse = Mapper.Map<GroupSummaryResponse>(response);
        groupSummaryResponse.Filters ??= new GroupSummaryFilters();
        groupSummaryResponse.Filters.ViewAsAgents = additionalAgentIds;

        return groupSummaryResponse;
    }

    public async Task<GroupDetailResponse?> GetGroupDetail(
        string loggedInAgentNumber,
        GroupDetailsQueryParameters queryParameters,
        CancellationToken cancellationToken = default)
    {
        var mappedGroupDetailsQueryParameters = Mapper.Map<GroupsAPI.GroupDetailsQueryParameters>(queryParameters);
        var response = await GroupsApiAccessor.GetGroupDetail(
                loggedInAgentNumber,
                queryParameters.GroupNumber ?? string.Empty,
                mappedGroupDetailsQueryParameters,
                cancellationToken);

        if (response == null)
        {
            return null;
        }

        if (response.Policies != null)
        {
            await FilterBenefitsWithHiddenCoverageOptions(response);
        }

        return Mapper.Map<GroupDetailResponse>(response);
    }

    public async Task<FileResponse?> GetGroupSummaryExport(string loggedInAgentNumber, GroupSummaryQueryParameters queryParameters, CancellationToken cancellationToken = default)
    {
        var additionalAgentIds = await AgentApiAccessor.GetAdditionalAgentIds(loggedInAgentNumber, cancellationToken);
        if (additionalAgentIds == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(queryParameters.ViewAsAgentId) && !additionalAgentIds.Contains(queryParameters.ViewAsAgentId))
        {
            Logger.LogWarning("Logged-in agent does not have access to the specified agent. ViewAsAgentId: {ViewAsAgentId}", queryParameters.ViewAsAgentId);
            return null;
        }

        var mappedGroupDetailsQueryParameters = Mapper.Map<GroupsAPI.GroupSummaryQueryParameters>(queryParameters);

        var response = await GroupsApiAccessor.GetGroupSummary(
                 loggedInAgentNumber,
                 mappedGroupDetailsQueryParameters,
                 cancellationToken);

        if (response == null)
        {
            return null;
        }

        var fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var fileName = $"{loggedInAgentNumber}_GroupInventoryReport_{DateTime.Now:yyyy-MM-dd}.xlsx";
        var fileResponse = new FileResponse(fileName, fileType);

        if (response.GroupSummaries != null && response.GroupSummaries.Count > 0)
        {
            var headers = FileExportEngine.CreateHeaders<GroupInventorySummaryExport>();
            var mappedGroupsSummary = Mapper.Map<List<GroupInventorySummaryExport>>(response.GroupSummaries);

            fileResponse.FileData = FileExportEngine.CreateExcelDocument(headers, mappedGroupsSummary, "GroupInventorySummary");
        }

        return fileResponse;
    }

    public async Task<FileResponse?> GetGroupDetailsExport(
        string loggedInAgentNumber,
        GroupDetailsQueryParameters queryParameters,
        CancellationToken cancellationToken = default)
    {
        var mappedGroupDetailsQueryParameters = Mapper.Map<GroupsAPI.GroupDetailsQueryParameters>(queryParameters);
        var response = await GroupsApiAccessor.GetGroupDetail(
               loggedInAgentNumber,
               queryParameters.GroupNumber ?? string.Empty,
               mappedGroupDetailsQueryParameters,
               cancellationToken);

        if (response == null || response.Policies == null || response.Policies.Count == 0)
        {
            return null;
        }

        var fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var fileName = $"{loggedInAgentNumber}_GroupInventoryDetailReport_{DateTime.Now:yyyy-MM-dd}.xlsx";
        var fileResponse = new FileResponse(fileName, fileType);

        var headers = FileExportEngine.CreateHeaders<GroupInventoryDetailsExport>();
        var mappedCoverageOptions = new List<GroupInventoryDetailsExport>();

        foreach (var policy in response.Policies)
        {
            if (policy.Benefits != null && policy.Benefits.Count > 0)
            {
                foreach (var benefit in policy.Benefits)
                {
                    if (benefit.CoverageType != "Base")
                    {
                        benefit.CoverageOptions = new List<GroupsAPI.CoverageOption>();
                    }
                    else
                    {
                        benefit.CoverageOptions = FilterDuplicateCoverageOptions(benefit.CoverageOptions);
                    }

                    foreach (var coverageOption in benefit.CoverageOptions)
                    {
                        var mappedExport = Mapper.Map<GroupInventoryDetailsExport>(policy);
                        mappedExport.Benefits = benefit.CoverageType;
                        mappedExport.CoverageType = GetCoverageTypeFromBaseBenefit(coverageOption, benefit.CoverageType);
                        mappedExport.BenefitDescription = DataFormatter.FormatStringToTitleCase(benefit.Description);
                        mappedExport.BenefitAmount = benefit.Amount.ToString("C0");

                        var benefitOption = GetMappedBenefitOption(coverageOption);
                        if (benefitOption != null)
                        {
                            mappedExport.CoverageOptions = $"{MappingExtensions.GetEnumDisplayName(benefitOption.Category)} - {MappingExtensions.GetEnumDisplayName(benefitOption.Option)}";
                            mappedCoverageOptions.Add(mappedExport);
                        }
                    }

                    if (benefit.CoverageType != "Base" && benefit.CoverageOptions.Count == 0)
                    {
                        var mappedExportForRider = Mapper.Map<GroupInventoryDetailsExport>(policy);
                        mappedExportForRider.Benefits = benefit.CoverageType;
                        mappedExportForRider.CoverageType = GetCoverageTypeFromBaseBenefit(null, benefit.CoverageType);
                        mappedExportForRider.BenefitDescription = DataFormatter.FormatStringToTitleCase(benefit.Description);
                        mappedExportForRider.BenefitAmount = benefit.Amount.ToString("C0");
                        mappedExportForRider.CoverageOptions = string.Empty;
                        mappedCoverageOptions.Add(mappedExportForRider);
                    }

                    if (benefit.CoverageType == "Base" && benefit.CoverageOptions.Count == 0)
                    {
                        var mappedExportForRider = Mapper.Map<GroupInventoryDetailsExport>(policy);
                        mappedExportForRider.Benefits = benefit.CoverageType;
                        mappedExportForRider.CoverageType = string.Empty;
                        mappedExportForRider.BenefitDescription = DataFormatter.FormatStringToTitleCase(benefit.Description);
                        mappedExportForRider.BenefitAmount = benefit.Amount.ToString("C0");
                        mappedExportForRider.CoverageOptions = string.Empty;
                        mappedCoverageOptions.Add(mappedExportForRider);
                    }
                }
            }
        }

        fileResponse.FileData = FileExportEngine.CreateExcelDocument(headers, mappedCoverageOptions, "GroupInventoryDetails");
        return fileResponse;
    }

    private static List<GroupsAPI.CoverageOption> FilterDuplicateCoverageOptions(List<GroupsAPI.CoverageOption> benefitCoverageOptions)
    {
        var filteredCoverageOptions = new List<GroupsAPI.CoverageOption>();

        foreach (var newOption in benefitCoverageOptions)
        {
            var duplicateOption = filteredCoverageOptions.Where(existingOption => existingOption.Name == newOption.Name && existingOption.Value == newOption.Value).FirstOrDefault();
            if (duplicateOption == null)
            {
                filteredCoverageOptions.Add(newOption);
            }
        }

        return filteredCoverageOptions;
    }

    private static List<GroupsAPI.CoverageOption> FilterHiddenCoverageOptions(
      List<GroupsAPI.CoverageOption> benefitCoverageOptions,
      List<BenefitOptionsMapping> coverageOptionsToHide)
    {
        foreach (var hiddenOption in coverageOptionsToHide)
        {
            benefitCoverageOptions.RemoveAll(
                coverageOption => coverageOption.Name.Equals(hiddenOption.Category.ToString(), StringComparison.OrdinalIgnoreCase) &&
                                  coverageOption.Value.Equals(hiddenOption.Option.ToString(), StringComparison.OrdinalIgnoreCase));
        }

        return benefitCoverageOptions;
    }

    private async Task FilterBenefitsWithHiddenCoverageOptions(GroupsAPI.GroupDetailResponse groupDetailResponse)
    {
        var hiddenCoverageOptions = await BenefitOptionsAccessor.GetHiddenBenefitOptionsMappings();

        if (hiddenCoverageOptions != null && hiddenCoverageOptions.Count > 0)
        {
            foreach (var policy in groupDetailResponse.Policies)
            {
                foreach (var benefit in policy.Benefits.Where(b => b.CoverageOptions != null && b.CoverageOptions.Count > 0))
                {
                    benefit.CoverageOptions = FilterHiddenCoverageOptions(benefit.CoverageOptions, hiddenCoverageOptions);
                }
            }
        }
    }

    private string GetCoverageTypeFromBaseBenefit(GroupsAPI.CoverageOption coverageOption, string coverageType)
    {
        if (coverageType == "Base" && coverageOption.Name == "CoverageType")
        {
            var benefitOption = GetMappedBenefitOption(coverageOption);

            if (benefitOption != null)
            {
                return MappingExtensions.GetEnumDisplayName(benefitOption.Option);
            }
        }

        return string.Empty;
    }

    private BenefitOptionsMapping? GetMappedBenefitOption(GroupsAPI.CoverageOption coverageOption)
    {
        return Enum.TryParse(coverageOption.Name, out BenefitOptionName category) &&
               Enum.TryParse(coverageOption.Value, out BenefitOptionValue option)
            ? new BenefitOptionsMapping { Category = category, Option = option }
            : null;
    }
}
