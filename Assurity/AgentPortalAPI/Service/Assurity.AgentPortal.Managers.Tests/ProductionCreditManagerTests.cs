namespace Assurity.AgentPortal.Managers.Tests;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Accessors.ProductionCredit;
using Assurity.AgentPortal.Contracts.Enums;
using Assurity.AgentPortal.Contracts.ProductionCredit.FileExport;
using Assurity.AgentPortal.Contracts.ProductionCredit.Request;
using Assurity.AgentPortal.Contracts.ProductionCredit.Response.Worksite;
using Assurity.AgentPortal.Contracts.Shared;
using Assurity.AgentPortal.Engines;
using Assurity.AgentPortal.Managers.ProductionCredit;
using Assurity.Production.Contracts.V1.Individual;
using Assurity.Production.Contracts.V1.Shared.PolicyDetails;
using Assurity.Production.Contracts.V1.Worksite;
using AutoBogus;
using AutoMapper;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class ProductionCreditManagerTests
{
    private readonly Mapper mapper;

    public ProductionCreditManagerTests()
    {
        var mapperProfiles = new List<Profile>
        {
            new ProductionCreditMappingProfile()
        };

        var mapperConfig = new MapperConfiguration(config => config.AddProfiles(mapperProfiles));
        mapper = new Mapper(mapperConfig);
    }

    [Fact]
    public async Task GetIndividualProductionCreditSummary_Success()
    {
        // Arrange
        string agentId = "ABC1";
        var optionalParameters = new AutoFaker<ProductionCreditParameters>().Generate();
        optionalParameters.ViewAsAgentId = null;

        var accessorResponse = new AutoFaker<IndividualProductionReport>().Generate();

        var mockAccessor = new Mock<IProductionCreditApiAccessor>();
        mockAccessor.Setup(accessor => accessor.GetIndividualProductionCredit(
            It.IsAny<string>(),
            It.IsAny<ProductionCreditParameters>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(accessorResponse)
        .Verifiable();

        var mockFileExportEngine = new Mock<IFileExportEngine>();

        var productionCreditManager = new ProductionCreditManager(mapper, mockAccessor.Object, mockFileExportEngine.Object);

        // Act
        var response = await productionCreditManager.GetIndividualProductionCreditSummary(agentId, optionalParameters, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(accessorResponse.ProductionByDownline.First().Grouping, response.ProductionByDownline.First().Grouping);
        Assert.Equal(accessorResponse.ProductionByDownlineCount, response.ProductionByDownlineCount);
        Assert.Equal(accessorResponse.ProductionByProduct.First().Grouping, response.ProductionByProduct.First().Grouping);
        Assert.Equal(accessorResponse.ProductionByProductCount, response.ProductionByProductCount);
        Assert.Equal(accessorResponse.SupplementalReports.First().Name, response.SupplementalReports.First().Name);
        Assert.Equal(accessorResponse.TotalPolicyCountCurrent, response.TotalPolicyCountCurrent);
        Assert.Equal(accessorResponse.TotalPolicyCountPrevious, response.TotalPolicyCountPrevious);
        Assert.Equal(accessorResponse.TotalPolicyCountChangePercent, response.TotalPolicyCountChangePercent);
        Assert.Equal(accessorResponse.TotalPremiumPrevious, response.TotalPremiumPrevious);
        Assert.Equal(accessorResponse.TotalPremiumCurrent, response.TotalPremiumCurrent);
        Assert.Equal(accessorResponse.TotalPremiumChangePercent, response.TotalPremiumChangePercent);
        Assert.Equal(accessorResponse.Filters.Page, response.Filters.Page);
        Assert.Equal(accessorResponse.Filters.PageSize, response.Filters.PageSize);

        mockAccessor.Verify(
            accessor => accessor.GetIndividualProductionCredit(
            agentId,
            optionalParameters,
            CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task GetIndividualPolicyDetailsSummary_Success()
    {
        // Arrange
        string agentId = "ABC1";
        var optionalParameters = new AutoFaker<ProductionCreditPolicyDetailsParameters>().Generate();
        optionalParameters.ViewAsAgentId = null;

        var accessorResponse = new AutoFaker<PolicyDetailsReport>().Generate();

        var mockAccessor = new Mock<IProductionCreditApiAccessor>();
        mockAccessor.Setup(accessor => accessor.GetIndividualPolicyDetails(
            It.IsAny<string>(),
            It.IsAny<ProductionCreditPolicyDetailsParameters>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(accessorResponse)
        .Verifiable();

        var mockFileExportEngine = new Mock<IFileExportEngine>();

        var productionCreditManager = new ProductionCreditManager(mapper, mockAccessor.Object, mockFileExportEngine.Object);

        // Act
        var response = await productionCreditManager.GetIndividualPolicyDetailsSummary(agentId, optionalParameters, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(accessorResponse.PolicyDetails.First().AgentId, response.PolicyDetails.First().AgentId);
        Assert.Equal(accessorResponse.TotalRecordCount, response.TotalRecordCount);
        Assert.Equal(accessorResponse.TotalPolicyCount, response.TotalPolicyCount);
        Assert.Equal(accessorResponse.TotalPremium, response.TotalPremium);
        Assert.Equal(accessorResponse.Filters.Page, response.Filters.Page);
        Assert.Equal(accessorResponse.Filters.PageSize, response.Filters.PageSize);

        mockAccessor.Verify(
            accessor => accessor.GetIndividualPolicyDetails(
            agentId,
            optionalParameters,
            CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task GetWorksiteProductionCreditSummary_Success()
    {
        // Arrange
        string agentId = "ABC1";
        var optionalParameters = new AutoFaker<WorksiteProductionCreditParameters>().Generate();
        optionalParameters.ViewAsAgentId = null;

        var accessorResponse = new AutoFaker<WorksiteReport>().Generate();

        var mockAccessor = new Mock<IProductionCreditApiAccessor>();
        mockAccessor.Setup(accessor => accessor.GetWorksiteProductionCredit(
            It.IsAny<string>(),
            It.IsAny<WorksiteProductionCreditParameters>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(accessorResponse)
        .Verifiable();

        var mockFileExportEngine = new Mock<IFileExportEngine>();

        var productionCreditManager = new ProductionCreditManager(mapper, mockAccessor.Object, mockFileExportEngine.Object);

        // Act
        var response = await productionCreditManager.GetWorksiteProductionCreditSummary(agentId, optionalParameters, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(accessorResponse.ProductionByAgent.First().Grouping, response.ProductionByAgent.First().Grouping);
        Assert.Equal(accessorResponse.ProductionByGroup.First().Grouping, response.ProductionByGroup.First().Grouping);
        Assert.Equal(accessorResponse.ProductionByProduct.First().Grouping, response.ProductionByProduct.First().Grouping);
        Assert.Equal(accessorResponse.TotalGroupCountCurrent, response.TotalGroupCountCurrent);
        Assert.Equal(accessorResponse.TotalGroupCountPrevious, response.TotalGroupCountPrevious);
        Assert.Equal(accessorResponse.TotalGroupCountChangePercent, response.TotalGroupCountChangePercent);
        Assert.Equal(accessorResponse.TotalPolicyCountCurrent, response.TotalPolicyCountCurrent);
        Assert.Equal(accessorResponse.TotalPolicyCountPrevious, response.TotalPolicyCountPrevious);
        Assert.Equal(accessorResponse.TotalPolicyCountChangePercent, response.TotalPolicyCountChangePercent);
        Assert.Equal(accessorResponse.TotalPremiumCurrent, response.TotalPremiumCurrent);
        Assert.Equal(accessorResponse.TotalPremiumPrevious, response.TotalPremiumPrevious);
        Assert.Equal(accessorResponse.TotalPremiumChangePercent, response.TotalPremiumChangePercent);
        Assert.Equal(accessorResponse.ProductionByAgentSupplementalReport.Name, response.ProductionByAgentSupplementalReport.Name);
        Assert.Equal(accessorResponse.ProductionByAgentSupplementalReport.Totals.First().Name, response.ProductionByAgentSupplementalReport.Totals.First().Name);
        Assert.Equal(accessorResponse.Filters.Page, response.Filters.Page);
        Assert.Equal(accessorResponse.Filters.PageSize, response.Filters.PageSize);

        mockAccessor.Verify(
            accessor => accessor.GetWorksiteProductionCredit(
            agentId,
            optionalParameters,
            CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task GetWorksitePolicyDetailsSummary_Success()
    {
        // Arrange
        string agentId = "ABC1";
        var optionalParameters = new AutoFaker<ProductionCreditPolicyDetailsParameters>().Generate();
        optionalParameters.ViewAsAgentId = null;

        var accessorResponse = new AutoFaker<PolicyDetailsReport>().Generate();

        var mockAccessor = new Mock<IProductionCreditApiAccessor>();
        mockAccessor.Setup(accessor => accessor.GetWorksitePolicyDetails(
            It.IsAny<string>(),
            It.IsAny<ProductionCreditPolicyDetailsParameters>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(accessorResponse)
        .Verifiable();

        var mockFileExportEngine = new Mock<IFileExportEngine>();

        var productionCreditManager = new ProductionCreditManager(mapper, mockAccessor.Object, mockFileExportEngine.Object);

        // Act
        var response = await productionCreditManager.GetWorksitePolicyDetailsSummary(agentId, optionalParameters, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(accessorResponse.PolicyDetails.First().AgentId, response.PolicyDetails.First().AgentId);
        Assert.Equal(accessorResponse.TotalRecordCount, response.TotalRecordCount);
        Assert.Equal(accessorResponse.TotalPolicyCount, response.TotalPolicyCount);
        Assert.Equal(accessorResponse.TotalPremium, response.TotalPremium);
        Assert.Equal(accessorResponse.Filters.Page, response.Filters.Page);
        Assert.Equal(accessorResponse.Filters.PageSize, response.Filters.PageSize);

        mockAccessor.Verify(
            accessor => accessor.GetWorksitePolicyDetails(
            agentId,
            optionalParameters,
            CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task GetIndividualProductionCreditByGrouping_Products_RenamesGroupingHeaderToDescription()
    {
        // Arrange
        var agentId = "ABC1";
        var parameters = new AutoFaker<ProductionCreditParameters>().Generate();
        var totals = new List<Total>
    {
        new() { Name = "Product A", PolicyCount = 5, Premium = 1000 }
    };

        var report = new SupplementalReport
        {
            Name = "Products",
            Totals = totals
        };

        var accessorResponse = new IndividualProductionReport
        {
            SupplementalReports = new List<SupplementalReport> { report }
        };

        var mockAccessor = new Mock<IProductionCreditApiAccessor>();
        mockAccessor.Setup(a => a.GetIndividualProductionCredit(
            It.IsAny<string>(),
            It.IsAny<ProductionCreditParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessorResponse);

        var mockFileExportEngine = new Mock<IFileExportEngine>();

        List<string> capturedHeaders = null!;

        mockFileExportEngine.Setup(e => e.CreateHeaders<ProductionCreditGroupedExport>())
            .Returns(new List<string> { "Description", "Policy Count", "Annualized Premium" });

        mockFileExportEngine.Setup(e => e.CreateExcelDocument(
            It.IsAny<List<string>>(),
            It.IsAny<List<ProductionCreditGroupedExport>>(),
            It.IsAny<string>()))
            .Callback<List<string>, List<ProductionCreditGroupedExport>, string>((headers, _, _) =>
            {
                capturedHeaders = headers;
            })
            .Returns(new byte[] { 0x01 });

        var manager = new ProductionCreditManager(mapper, mockAccessor.Object, mockFileExportEngine.Object);

        // Act
        var result = await manager.GetIndividualProductionCreditByGrouping(agentId, parameters, GroupingType.Products, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Individual_PolicyDetails_Products", result.FileName);
        Assert.Contains("Description", capturedHeaders);
        Assert.DoesNotContain("Grouping", capturedHeaders);
    }

    [Fact]
    public async Task GetIndividualProductionCreditByGrouping_LineOfBusiness_KeepsDefaultHeaders()
    {
        // Arrange
        var agentId = "ABC1";
        var parameters = new AutoFaker<ProductionCreditParameters>().Generate();
        var totals = new List<Total>
    {
        new() { Name = "Life", PolicyCount = 10, Premium = 2000 }
    };

        var report = new SupplementalReport
        {
            Name = "Lines of Business",
            Totals = totals
        };

        var accessorResponse = new IndividualProductionReport
        {
            SupplementalReports = new List<SupplementalReport> { report }
        };

        var mockAccessor = new Mock<IProductionCreditApiAccessor>();
        mockAccessor.Setup(a => a.GetIndividualProductionCredit(
            It.IsAny<string>(),
            It.IsAny<ProductionCreditParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessorResponse);

        var mockFileExportEngine = new Mock<IFileExportEngine>();

        List<string> capturedHeaders = null!;
        mockFileExportEngine.Setup(e => e.CreateHeaders<ProductionCreditGroupedExport>())
            .Returns(new List<string> { "Rank", "Grouping", "Policy Count", "Annualized Premium" });

        mockFileExportEngine.Setup(e => e.CreateExcelDocument(
            It.IsAny<List<string>>(),
            It.IsAny<List<ProductionCreditGroupedExport>>(),
            It.IsAny<string>()))
            .Callback<List<string>, List<ProductionCreditGroupedExport>, string>((headers, _, _) =>
            {
                capturedHeaders = headers;
            })
            .Returns(new byte[] { 0x02 });

        var manager = new ProductionCreditManager(mapper, mockAccessor.Object, mockFileExportEngine.Object);

        // Act
        var result = await manager.GetIndividualProductionCreditByGrouping(agentId, parameters, GroupingType.LineOfBusiness, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Individual_PolicyDetails_Lines_of_Business", result.FileName);
        Assert.Contains("Grouping", capturedHeaders);
        Assert.DoesNotContain("Description", capturedHeaders);
    }

    [Fact]
    public async Task GetWorksiteProductionCreditExcelByTaps_ReturnsNull_When_GetWorksiteProductionCreditNestedTablesByTaps_ReturnsNull()
    {
        // Arrange
        string agentId = "testAgentId";
        ProductionCreditViewType tap = ProductionCreditViewType.Products; // Or any tap value
        var parameters = new WorksiteProductionCreditParameters();
        CancellationToken cancellationToken = CancellationToken.None;

        var mockManager = new Mock<ProductionCreditManager>(mapper, Mock.Of<IProductionCreditApiAccessor>(), Mock.Of<IFileExportEngine>());

        mockManager.Setup(m => m.GetWorksiteProductionCreditNestedTablesByTaps(agentId, tap, parameters, cancellationToken))
                   .ReturnsAsync((ProductionCreditWorksiteViewTypeResponse)null); // Explicitly return null

        // Act
        var result = await mockManager.Object.GetWorksiteProductionCreditExcelByTaps(agentId, tap, parameters, cancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetWorksiteProductionCreditExcelByTaps_ReturnsFileResponse_When_GetWorksiteProductionCreditNestedTablesByTaps_ReturnsData()
    {
        // Arrange
        string agentId = "testAgentId";
        ProductionCreditViewType tap = ProductionCreditViewType.Products;
        var parameters = new WorksiteProductionCreditParameters();
        CancellationToken cancellationToken = CancellationToken.None;

        var mockManager = new Mock<ProductionCreditManager>(mapper, Mock.Of<IProductionCreditApiAccessor>(), Mock.Of<IFileExportEngine>()) { CallBase = true }; // Important: CallBase

        var mockResponseData = new ProductionCreditWorksiteViewTypeResponse
        {
            GroupProducts = new List<ProductionCreditWorksiteProductExport>
        {
            new ProductionCreditWorksiteProductExport { Grouping = "LOB1", ProductName = "Product A", PolicyCount = 5, AnnualizedPremium = 1000 }
        }
        };
        mockManager.Setup(m => m.GetWorksiteProductionCreditNestedTablesByTaps(agentId, tap, parameters, cancellationToken))
                   .ReturnsAsync(mockResponseData);

        // Act
        var result = await mockManager.Object.GetWorksiteProductionCreditExcelByTaps(agentId, tap, parameters, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<FileResponse>(result);
        Assert.NotEmpty(result.FileName);
        Assert.NotEmpty(result.FileType);
    }

    [Fact]
    public async Task GetWorksiteProductionCreditExcelByTaps_ReturnsNull_When_GetWorksiteProductionCreditNestedTablesByTaps_ReturnsEmptyData()
    {
        // Arrange
        string agentId = "testAgentId";
        ProductionCreditViewType tap = ProductionCreditViewType.Products;
        var parameters = new WorksiteProductionCreditParameters();
        CancellationToken cancellationToken = CancellationToken.None;

        var mockManager = new Mock<ProductionCreditManager>(mapper, Mock.Of<IProductionCreditApiAccessor>(), Mock.Of<IFileExportEngine>()) { CallBase = true };

        var mockResponseData = new ProductionCreditWorksiteViewTypeResponse
        {
            GroupProducts = new List<ProductionCreditWorksiteProductExport>() // Empty list
        };
        mockManager.Setup(m => m.GetWorksiteProductionCreditNestedTablesByTaps(agentId, tap, parameters, cancellationToken))
                   .ReturnsAsync(mockResponseData);

        // Act
        var result = await mockManager.Object.GetWorksiteProductionCreditExcelByTaps(agentId, tap, parameters, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<FileResponse>(result);
        Assert.NotEmpty(result.FileName);
        Assert.NotEmpty(result.FileType);
    }
}
