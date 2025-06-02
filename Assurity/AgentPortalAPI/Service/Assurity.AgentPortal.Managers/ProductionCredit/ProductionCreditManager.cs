namespace Assurity.AgentPortal.Managers.ProductionCredit;

using Assurity.AgentPortal.Accessors.ProductionCredit;
using Assurity.AgentPortal.Contracts.Enums;
using Assurity.AgentPortal.Contracts.ProductionCredit.FileExport;
using Assurity.AgentPortal.Contracts.ProductionCredit.Request;
using Assurity.AgentPortal.Contracts.ProductionCredit.Response.Individual;
using Assurity.AgentPortal.Contracts.ProductionCredit.Response.PolicyDetail;
using Assurity.AgentPortal.Contracts.ProductionCredit.Response.Worksite;
using Assurity.AgentPortal.Contracts.Shared;
using Assurity.AgentPortal.Engines;
using Assurity.AgentPortal.Utilities.Formatting;
using Assurity.Production.Contracts.V1.Individual;
using Assurity.Production.Contracts.V1.Worksite;
using AutoMapper;

public class ProductionCreditManager : IProductionCreditManager
{
    private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public ProductionCreditManager(
        IMapper mapper,
        IProductionCreditApiAccessor productionCreditApiAccessor,
        IFileExportEngine fileExportEngine)
    {
        Mapper = mapper;
        ProductionCreditApiAccessor = productionCreditApiAccessor;
        FileExportEngine = fileExportEngine;
    }

    private IMapper Mapper { get; }

    private IProductionCreditApiAccessor ProductionCreditApiAccessor { get; }

    private IFileExportEngine FileExportEngine { get; }

    public async Task<IndividualProductionCreditSummary?> GetIndividualProductionCreditSummary(
        string agentId,
        ProductionCreditParameters parameters,
        CancellationToken cancellationToken)
    {
        var productionCreditResponse = await ProductionCreditApiAccessor.GetIndividualProductionCredit(agentId, parameters, cancellationToken);
        if (productionCreditResponse == null)
        {
            return null;
        }

        return Mapper.Map<IndividualProductionCreditSummary>(productionCreditResponse);
    }

    public async Task<ProductionCreditPolicyDetailsSummary?> GetIndividualPolicyDetailsSummary(
        string agentId,
        ProductionCreditPolicyDetailsParameters parameters,
        CancellationToken cancellationToken)
    {
        var productionCreditResponse = await ProductionCreditApiAccessor.GetIndividualPolicyDetails(agentId, parameters, cancellationToken);
        if (productionCreditResponse == null)
        {
            return null;
        }

        return Mapper.Map<ProductionCreditPolicyDetailsSummary>(productionCreditResponse);
    }

    public async Task<WorksiteProductionCreditSummary?> GetWorksiteProductionCreditSummary(
        string agentId,
        WorksiteProductionCreditParameters parameters,
        CancellationToken cancellationToken)
    {
        var productionCreditResponse = await ProductionCreditApiAccessor.GetWorksiteProductionCredit(agentId, parameters, cancellationToken);
        if (productionCreditResponse == null)
        {
            return null;
        }

        return Mapper.Map<WorksiteProductionCreditSummary>(productionCreditResponse);
    }

    public async Task<ProductionCreditPolicyDetailsSummary?> GetWorksitePolicyDetailsSummary(
        string agentId,
        ProductionCreditPolicyDetailsParameters parameters,
        CancellationToken cancellationToken)
    {
        var productionCreditResponse = await ProductionCreditApiAccessor.GetWorksitePolicyDetails(agentId, parameters, cancellationToken);
        if (productionCreditResponse == null)
        {
            return null;
        }

        return Mapper.Map<ProductionCreditPolicyDetailsSummary>(productionCreditResponse);
    }

    public async Task<FileResponse?> ExportIndividualPolicyDetailsSummary(
        string agentId,
        ProductionCreditPolicyDetailsParameters parameters,
        CancellationToken cancellationToken)
    {
        var productionCreditResponse = await ProductionCreditApiAccessor.GetIndividualPolicyDetails(agentId, parameters, cancellationToken);
        if (productionCreditResponse == null)
        {
            return null;
        }

        var mappedPolicies = Mapper.Map<List<ProductionCreditExport>>(productionCreditResponse.PolicyDetails);
        if (mappedPolicies == null)
        {
            return null;
        }

        var fileDownloadResponse = new FileResponse("Individual_PolicyDetails", ExcelContentType);
        var headers = FileExportEngine.CreateHeaders<ProductionCreditExport>();
        var document = FileExportEngine.CreateExcelDocument(headers, mappedPolicies, $"Individual Policy Details");

        fileDownloadResponse.FileData = document;

        return fileDownloadResponse;
    }

    public async Task<FileResponse?> ExportWorksitePolicyDetailsSummary(
       string agentId,
       ProductionCreditPolicyDetailsParameters parameters,
       CancellationToken cancellationToken)
    {
        var productionCreditResponse = await ProductionCreditApiAccessor.GetWorksitePolicyDetails(agentId, parameters, cancellationToken);
        if (productionCreditResponse == null)
        {
            return null;
        }

        var mappedPolicies = Mapper.Map<List<ProductionCreditWorksiteExport>>(productionCreditResponse.PolicyDetails);
        if (mappedPolicies == null)
        {
            return null;
        }

        var fileDownloadResponse = new FileResponse("Worksite_PolicyDetails", ExcelContentType);
        var headers = FileExportEngine.CreateHeaders<ProductionCreditWorksiteExport>();
        var document = FileExportEngine.CreateExcelDocument(headers, mappedPolicies, $"Worksite Policy Details");

        fileDownloadResponse.FileData = document;

        return fileDownloadResponse;
    }

    public async Task<FileResponse?> GetIndividualProductionCreditByGrouping(
        string agentId,
        ProductionCreditParameters parameters,
        GroupingType groupingType,
        CancellationToken cancellationToken)
    {
        var productionCreditResponse = await ProductionCreditApiAccessor
            .GetIndividualProductionCredit(agentId, parameters, cancellationToken);

        if (productionCreditResponse?.SupplementalReports == null)
        {
            return null;
        }

        string reportName = groupingType switch
        {
            GroupingType.LineOfBusiness => "Lines of Business",
            GroupingType.Products => "Products",
            GroupingType.Downline => "Downline",
            GroupingType.WritingAgents => "Writing Agents",
            _ => throw new ArgumentOutOfRangeException(nameof(groupingType), $"Unsupported grouping type: {groupingType}")
        };

        var supplementalReport = productionCreditResponse.SupplementalReports
            .FirstOrDefault(r => r.Name == reportName);

        if (supplementalReport?.Totals == null)
        {
            return null;
        }

        var groupedData = BuildGroupedData(supplementalReport);
        return CreateExcelFileResponse(groupedData, reportName);
    }

    public async Task<FileResponse?> GetWorksiteProductionCreditExcelByTaps(
        string agentId,
        ProductionCreditViewType tap,
        WorksiteProductionCreditParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var data = await GetWorksiteProductionCreditNestedTablesByTaps(agentId, tap, parameters, cancellationToken);

        if (data == null)
        {
            return null;
        }

        string sheetName = "Worksite Report";
        string reportName = tap.ToString(); // "Products", "Group", or "Agent"
        string fileName = $"Worksite_ProductionCredit_{reportName}_{DateTime.Now:yyyyMMddHHmmss}";

        switch (tap)
        {
            case ProductionCreditViewType.Products:
                return CreateExcelFileResponse(data.GroupProducts, fileName, sheetName);

            case ProductionCreditViewType.Group:
                return CreateExcelFileResponse(data.GroupDetails, fileName, sheetName);

            case ProductionCreditViewType.Agent:
                return CreateExcelFileResponse(data.GroupedAgents, fileName, sheetName);

            case ProductionCreditViewType.DownlineWritingAgents:
                return CreateExcelFileResponse(data.DownlineWritingAgents, fileName, sheetName);

            default:
                throw new ArgumentOutOfRangeException(nameof(tap), $"Unsupported tap value: {tap}");
        }
    }

    public virtual async Task<ProductionCreditWorksiteViewTypeResponse?> GetWorksiteProductionCreditNestedTablesByTaps(
       string agentId,
       ProductionCreditViewType tap,
       WorksiteProductionCreditParameters parameters,
       CancellationToken cancellationToken = default)
    {
        var productionCreditResponse = await ProductionCreditApiAccessor
            .GetWorksiteProductionCredit(agentId, parameters, cancellationToken);

        if (productionCreditResponse == null)
        {
            return null;
        }

        ProductionCreditWorksiteViewTypeResponse productionCreditWorksiteViewTypeResponse = new ProductionCreditWorksiteViewTypeResponse();

        switch (tap)
        {
            case ProductionCreditViewType.DownlineWritingAgents:
                productionCreditWorksiteViewTypeResponse.DownlineWritingAgents = BuildDownlineWritingAgents(productionCreditResponse);
                break;

            case ProductionCreditViewType.Products:
                productionCreditWorksiteViewTypeResponse.GroupProducts = BuildNestedRowsForProducts(productionCreditResponse);
                break;

            case ProductionCreditViewType.Group:
                productionCreditWorksiteViewTypeResponse.GroupDetails = BuildNestedRowsForGroups(productionCreditResponse);
                break;

            case ProductionCreditViewType.Agent:
                productionCreditWorksiteViewTypeResponse.GroupedAgents = BuildNestedRowsForAgents(productionCreditResponse);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(tap), $"Unsupported tap value: {tap}");
        }

        return productionCreditWorksiteViewTypeResponse;
    }

    private List<ProductionCreditGroupedExport> BuildGroupedData(SupplementalReport supplementalReport)
    {
        return supplementalReport.Totals
            .OrderByDescending(t => t.Premium)
            .Select((t, index) => new ProductionCreditGroupedExport
            {
                Grouping = t.Name,
                PolicyCount = (int)t.PolicyCount,
                AnnualizedPremium = t.Premium
            })
            .ToList();
    }

    private FileResponse CreateExcelFileResponse(List<ProductionCreditGroupedExport> groupedData, string reportName)
    {
        var safeReportName = reportName.Replace(" ", "_").Replace("/", "-");
        var fileName = $"Individual_PolicyDetails_{safeReportName}";
        var headers = FileExportEngine.CreateHeaders<ProductionCreditGroupedExport>();

        if (reportName == "Products" && headers.Count > 1)
        {
            headers[0] = "Description";
        }

        if (reportName == "Downline" && headers.Count > 1)
        {
            headers[0] = "Agent Number";
        }

        if (reportName == "Writing Agents" && headers.Count > 1)
        {
            headers[0] = "Agent";
        }

        var document = FileExportEngine.CreateExcelDocument(headers, groupedData, "Individual Policy Details");

        return new FileResponse(fileName, ExcelContentType)
        {
            FileData = document
        };
    }

    private FileResponse CreateExcelFileResponse<T>(List<T> data, string fileName, string sheetName)
    {
        var headers = FileExportEngine.CreateHeaders<T>();
        var document = FileExportEngine.CreateExcelDocument(headers, data, sheetName);

        return new FileResponse(fileName, ExcelContentType)
        {
            FileData = document
        };
    }

    private List<ProductionCreditWorksiteDownlineWritingAgentsExport> BuildDownlineWritingAgents(WorksiteReport data)
    {
        var result = new List<ProductionCreditWorksiteDownlineWritingAgentsExport>();

        var supplementalReport = data?.ProductionByAgentSupplementalReport;
        if (supplementalReport?.Totals == null)
        {
            return result;
        }

        foreach (var total in supplementalReport.Totals.OrderByDescending(t => t.Premium))
        {
            result.Add(new ProductionCreditWorksiteDownlineWritingAgentsExport
            {
                Agent = total.Name ?? string.Empty,
                GroupCount = (int)total.GroupCount,
                AnnualizedPremium = total.Premium
            });
        }

        return result;
    }

    private List<ProductionCreditWorksiteProductExport> BuildNestedRowsForProducts(WorksiteReport data)
    {
        var rows = new List<ProductionCreditWorksiteProductExport>();

        decimal totalPolicyCount = 0;
        decimal totalPremium = 0;

        foreach (var line in data.Filters.LinesOfBusiness)
        {
            var productionLine = data.ProductionByProduct?
                .FirstOrDefault(p => p.Name.Equals(line.Name, StringComparison.OrdinalIgnoreCase));

            if (line.Types != null)
            {
                foreach (var type in line.Types)
                {
                    var productionType = productionLine?.Children?
                        .FirstOrDefault(c => c.Name.Equals(type.Name, StringComparison.OrdinalIgnoreCase));

                    decimal typePolicyCount = productionType?.PolicyCountCurrent ?? 0;
                    decimal typePremium = productionType?.PremiumCurrent ?? 0;

                    totalPolicyCount += typePolicyCount;
                    totalPremium += typePremium;

                    rows.Add(new ProductionCreditWorksiteProductExport
                    {
                        Grouping = line.Name,
                        ProductType = type.Name,
                        ProductName = (type.Descriptions != null && type.Descriptions.Any())
                        ? string.Join(", ", type.Descriptions)
                        : productionType?.Name,
                        AnnualizedPremium = typePremium,
                        PolicyCount = (int)typePolicyCount
                    });
                }
            }
        }

        rows.Add(new ProductionCreditWorksiteProductExport
        {
            Grouping = "Totals",
            ProductType = string.Empty,
            ProductName = string.Empty,
            AnnualizedPremium = totalPremium,
            PolicyCount = (int)totalPolicyCount
        });

        return rows;
    }

    private List<ProductionCreditWorksiteGroupExport> BuildNestedRowsForGroups(WorksiteReport data)
    {
        var rows = new List<ProductionCreditWorksiteGroupExport>();

        if (data?.ProductionByGroup == null)
        {
            return rows;
        }

        foreach (var group in data.ProductionByGroup)
        {
            if (group.Children != null)
            {
                foreach (var product in group.Children)
                {
                    if (product.Children != null && product.Children.Any())
                    {
                        foreach (var agent in product.Children)
                        {
                            rows.Add(new ProductionCreditWorksiteGroupExport
                            {
                                GroupName = group.Name,
                                GroupNumber = group.GroupNumber,
                                WritingAgent = agent.AgentName,
                                ProductName = product.Name,
                                EffectiveDate = DataFormatter.FormatDate(product.EffectiveDate, "MM/dd/yyyy"),
                                AnnualizedPremium = agent.PremiumCurrent,
                                PolicyCount = (int)agent.PolicyCountCurrent
                            });
                        }
                    }
                    else
                    {
                        rows.Add(new ProductionCreditWorksiteGroupExport
                        {
                            GroupName = group.Name,
                            GroupNumber = group.GroupNumber,
                            WritingAgent = null,
                            ProductName = product.Name,
                            EffectiveDate = DataFormatter.FormatDate(product.EffectiveDate, "MM/dd/yyyy"),
                            AnnualizedPremium = product.PremiumCurrent,
                            PolicyCount = (int)product.PolicyCountCurrent
                        });
                    }
                }
            }
        }

        if (rows.Any())
        {
            rows.Add(new ProductionCreditWorksiteGroupExport
            {
                GroupName = "Totals",
                GroupNumber = null,
                WritingAgent = null,
                ProductName = null,
                EffectiveDate = null,
                AnnualizedPremium = rows.Sum(r => r.AnnualizedPremium),
                PolicyCount = rows.Sum(r => r.PolicyCount)
            });
        }

        return rows;
    }

    private List<ProductionCreditWorksiteAgentExport> BuildNestedRowsForAgents(WorksiteReport data)
    {
        var exports = new List<ProductionCreditWorksiteAgentExport>();

        decimal totalPremium = 0m;
        int totalGroupCount = 0;

        if (data?.ProductionByAgent == null)
        {
            return exports;
        }

        foreach (var agent in data.ProductionByAgent)
        {
            string groupName = $"{agent.Name?.Trim() ?? string.Empty} {agent.AgentId?.Trim() ?? string.Empty}".Trim();

            if (agent.Children == null)
            {
                continue;
            }

            foreach (var lineOfBusiness in agent.Children)
            {
                if (string.IsNullOrWhiteSpace(lineOfBusiness.Name) || lineOfBusiness.Children == null)
                {
                    continue;
                }

                foreach (var product in lineOfBusiness.Children)
                {
                    if (string.IsNullOrWhiteSpace(product.Name))
                    {
                        continue;
                    }

                    decimal premium = product.PremiumCurrent;
                    int groupCount = (int)product.PolicyCountCurrent;

                    totalPremium += premium;
                    totalGroupCount += groupCount;

                    exports.Add(new ProductionCreditWorksiteAgentExport
                    {
                        Grouping = lineOfBusiness.Name,
                        ProductName = product.Name,
                        GroupName = groupName,
                        AnnualizedPremium = premium,
                        GroupCount = groupCount
                    });
                }
            }
        }

        exports.Add(new ProductionCreditWorksiteAgentExport
        {
            Grouping = "Totals",
            ProductName = string.Empty,
            GroupName = string.Empty,
            AnnualizedPremium = totalPremium,
            GroupCount = totalGroupCount
        });

        return exports;
    }
}
