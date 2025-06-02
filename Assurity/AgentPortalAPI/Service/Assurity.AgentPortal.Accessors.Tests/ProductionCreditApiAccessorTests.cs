namespace Assurity.AgentPortal.Accessors.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using Assurity.AgentPortal.Accessors.ProductionCredit;
using Assurity.AgentPortal.Contracts.ProductionCredit.Request;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;
using ProductionCredit = Assurity.Production.Contracts.V1.Individual;
using ProductionCreditFilters = Assurity.Production.Contracts.V1.Filters.Response.Shared;

[ExcludeFromCodeCoverage]
public class ProductionCreditApiAccessorTests
{
    private const string BaseIndividualProductionCreditUrl = "v1/individual/production";
    private const string BaseIndividualPolicyDetailsUrl = "v1/individual/policyDetails";
    private const string BaseWorksiteProductionCreditUrl = "v1/worksite/production";
    private const string BaseWorksitePolicyDetailsUrl = "v1/worksite/policyDetails";
    private const string BaseAgentMarketCodesUrl = "v1/ProductionCredit/AgentMarketCodes";

    private readonly Uri baseAddress = new Uri("http://localhost");

    [Fact]
    public async Task GetIndividualProductionCredit_NoAdditionalFilters_Success()
    {
        var agentId = "ABC1";
        var mockResponse = GetMockIndividualProductionReport();
        var mockHttpMessageHandler = GetMockHttpMessageHandler(mockResponse);
        var productionCreditAccessor = GetProductionCreditApiAccessor(mockHttpMessageHandler);

        var result = await productionCreditAccessor.GetIndividualProductionCredit(agentId, new ProductionCreditParameters(), CancellationToken.None);

        var expectedUrl = GetExpectedUrl(agentId, BaseIndividualProductionCreditUrl);

        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetIndividualPolicyDetails_NoAdditionalFilters_Success()
    {
        var agentId = "ABC1";
        var mockResponse = GetMockIndividualProductionReport();
        var mockHttpMessageHandler = GetMockHttpMessageHandler(mockResponse);
        var productionCreditAccessor = GetProductionCreditApiAccessor(mockHttpMessageHandler);

        var result = await productionCreditAccessor.GetIndividualPolicyDetails(agentId, new ProductionCreditPolicyDetailsParameters(), CancellationToken.None);

        var expectedUrl = GetExpectedUrl(agentId, BaseIndividualPolicyDetailsUrl);

        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetWorksiteProductionCredit_NoAdditionalFilters_Success()
    {
        var agentId = "ABC1";
        var mockResponse = GetMockIndividualProductionReport();
        var mockHttpMessageHandler = GetMockHttpMessageHandler(mockResponse);
        var productionCreditAccessor = GetProductionCreditApiAccessor(mockHttpMessageHandler);

        var result = await productionCreditAccessor.GetWorksiteProductionCredit(agentId, new WorksiteProductionCreditParameters(), CancellationToken.None);

        var expectedUrl = GetExpectedUrl(agentId, BaseWorksiteProductionCreditUrl);

        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetWorksitePolicyDetails_NoAdditionalFilters_Success()
    {
        var agentId = "ABC1";
        var mockResponse = GetMockIndividualProductionReport();
        var mockHttpMessageHandler = GetMockHttpMessageHandler(mockResponse);
        var productionCreditAccessor = GetProductionCreditApiAccessor(mockHttpMessageHandler);

        var result = await productionCreditAccessor.GetWorksitePolicyDetails(agentId, new ProductionCreditPolicyDetailsParameters(), CancellationToken.None);

        var expectedUrl = GetExpectedUrl(agentId, BaseWorksitePolicyDetailsUrl);

        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    [Theory]
    [InlineData("ABC1", true)]
    [InlineData("XYZ1", false)]
    public async Task GetProductionCreditMarketcodes_Success(string agentId, bool isFound)
    {
        var mockResponse = new Dictionary<string, List<string>> { { "marketCodes", [] } };
        if (isFound)
        {
            mockResponse["marketCodes"] = ["IG"];
        }

        var mockHttpMessageHandler = GetMockHttpMessageHandler(mockResponse);
        var productionCreditAccessor = GetProductionCreditApiAccessor(mockHttpMessageHandler);

        var result = await productionCreditAccessor.GetProductionCreditMarketcodes(agentId, CancellationToken.None);

        var expectedUrl = GetExpectedUrl(agentId, BaseAgentMarketCodesUrl);

        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    private static ProductionCredit.IndividualProductionReport GetMockIndividualProductionReport()
    {
        return new ProductionCredit.IndividualProductionReport
        {
            ProductionByDownline = new List<ProductionCredit.IndividualProduction>
            {
                new ProductionCredit.IndividualProduction
                {
                    Grouping = "Downline",
                    AgentId = "XYZ2",
                    Name = "Bloggs, Joe",
                    PremiumCurrent = 1.00m,
                    PolicyCountCurrent = 1,
                    Children = new List<ProductionCredit.IndividualProduction>
                    {
                        new ProductionCredit.IndividualProduction
                        {
                            Grouping = "Writing Agents",
                            AgentId = "LSO3",
                            Name = "SOME ALLIANCE",
                            PremiumCurrent = 1.00m,
                            PolicyCountCurrent = 1,
                            Children = new List<ProductionCredit.IndividualProduction>
                            {
                                new ProductionCredit.IndividualProduction
                                {
                                    Grouping = "Lines of Business",
                                    Name = "Health",
                                    PremiumCurrent = 1.00m,
                                    PolicyCountCurrent = 1,
                                    Children = new List<ProductionCredit.IndividualProduction>
                                    {
                                        new ProductionCredit.IndividualProduction
                                        {
                                            Grouping = "Products",
                                            Name = "Accidental Death & Dismemberment",
                                            PremiumCurrent = 1.00m,
                                            PolicyCountCurrent = 1,
                                            Children = new List<ProductionCredit.IndividualProduction>
                                            {
                                                new ProductionCredit.IndividualProduction
                                                {
                                                    Grouping = "Product Types",
                                                    Name = "Accident",
                                                    PremiumCurrent = 1.00m,
                                                    PolicyCountCurrent = 1
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            ProductionByDownlineCount = 1,
            ProductionByProduct = new List<ProductionCredit.IndividualProduction>
            {
                new ProductionCredit.IndividualProduction
                {
                    Grouping = "Lines of Business",
                    Name = "Life",
                    PremiumCurrent = 1.00m,
                    PolicyCountCurrent = 1,
                    Children = new List<ProductionCredit.IndividualProduction>
                    {
                        new ProductionCredit.IndividualProduction
                        {
                            Grouping = "Products",
                            Name = "Accidental Death & Dismemberment",
                            PremiumCurrent = 1.00m,
                            PolicyCountCurrent = 1,
                            Children = new List<ProductionCredit.IndividualProduction>
                            {
                                new ProductionCredit.IndividualProduction
                                {
                                    Grouping = "Product Types",
                                    Name = "Accident",
                                    PremiumCurrent = 1.00m,
                                    PolicyCountCurrent = 1,
                                    Children = new List<ProductionCredit.IndividualProduction>
                                    {
                                        new ProductionCredit.IndividualProduction
                                        {
                                            Grouping = "Writing Agents",
                                            AgentId = "XYZ2",
                                            Name = "Bloggs, Joe",
                                            PremiumCurrent = 1.00m,
                                            PolicyCountCurrent = 1,
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            ProductionByProductCount = 1,
            SupplementalReports = new List<ProductionCredit.SupplementalReport>
            {
                new ProductionCredit.SupplementalReport
                {
                    Name = "Downline",
                    Totals = new List<ProductionCredit.Total>
                    {
                        new ProductionCredit.Total
                        {
                            Name = "ABC1",
                            PolicyCount = 1,
                            Premium = 1.00m
                        }
                    }
                },
                new ProductionCredit.SupplementalReport
                {
                    Name = "Products",
                    Totals = new List<ProductionCredit.Total>
                    {
                        new ProductionCredit.Total
                        {
                            Name = "Term Life - 30 Year",
                            PolicyCount = 1,
                            Premium = 1.00m
                        }
                    }
                },
                new ProductionCredit.SupplementalReport
                {
                    Name = "Writing Agents",
                    Totals = new List<ProductionCredit.Total>
                    {
                        new ProductionCredit.Total
                        {
                            Name = "Bloggs, Joe",
                            PolicyCount = 1,
                            Premium = 1.00m
                        }
                    }
                },
                new ProductionCredit.SupplementalReport
                {
                    Name = "Lines Of Business",
                    Totals = new List<ProductionCredit.Total>
                    {
                        new ProductionCredit.Total
                        {
                            Name = "Health",
                            PolicyCount = 1,
                            Premium = 1.00m
                        }
                    }
                }
            },
            TotalPolicyCountCurrent = 1,
            TotalPolicyCountPrevious = 1,
            TotalPolicyCountChangePercent = 0,
            TotalPremiumPrevious = 1.00m,
            TotalPremiumCurrent = 1.00m,
            TotalPremiumChangePercent = 0,
            Filters = new ProductionCreditFilters.FilterValues
            {
                Agents = new List<ProductionCreditFilters.Agent>
                {
                    new ProductionCreditFilters.Agent
                    {
                        Id = "ABC1",
                        Name = "Smith, Max",
                    },
                    new ProductionCreditFilters.Agent
                    {
                        Id = "XYZ2",
                        Name = "Bloggs, Joe"
                    }
                },
                LinesOfBusiness = new List<ProductionCreditFilters.LineOfBusiness>
                {
                    new ProductionCreditFilters.LineOfBusiness
                    {
                        Name = "Health",
                        Types = new List<ProductionCreditFilters.Type>
                        {
                            new ProductionCreditFilters.Type
                            {
                                Name = "Term Life",
                                Descriptions = new List<string>
                                {
                                    "Term Life - 30 Year"
                                }
                            }
                        }
                    }
                },
                MarketCodes = new List<string>
                {
                    "ABXY"
                },
                ViewAsAgentIds = new List<string>
                {
                    "ABC1",
                    "XYZ2"
                },
                Page = 1,
                PageSize = 100
            }
        };
    }

    private static Mock<HttpMessageHandler> GetMockHttpMessageHandler(object mockResponse)
    {
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(mockResponse))
        };

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        mockHttpMessageHandler
           .Protected()
           .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(httpResponseMessage)
           .Verifiable();

        return mockHttpMessageHandler;
    }

    private ProductionCreditApiAccessor GetProductionCreditApiAccessor(Mock<HttpMessageHandler> mockHttpMessageHandler)
    {
        var mockhttpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = baseAddress
        };

        var mockLogger = new Mock<ILogger<ProductionCreditApiAccessor>>(MockBehavior.Loose);

        return new ProductionCreditApiAccessor(mockhttpClient, mockLogger.Object);
    }

    private Uri GetExpectedUrl(string agentId, string path)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "LoggedInAgentId", agentId }
        };

        return new Uri(QueryHelpers.AddQueryString(baseAddress + path, queryParams));
    }
}
