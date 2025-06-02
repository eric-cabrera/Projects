namespace Assurity.AgentPortal.Accessors.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using Assurity.AgentPortal.Accessors.Claims;
using Assurity.Claims.Contracts.AssureLink;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

[ExcludeFromCodeCoverage]
public class ClaimsApiAccessorTests
{
    [Fact]
    public async Task GetClaims_Success()
    {
        var mockResponse = GetMockClaimsResponse();

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(mockResponse))
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(httpResponseMessage);

        var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        var mockLogger = new Mock<ILogger<ClaimsApiAccessor>>();

        var claimsApiAccessor = new ClaimsApiAccessor(mockHttpClient, mockLogger.Object);

        var claimsRequest = new ClaimsRequest
        {
            ClaimantName = new Name
            {
                FirstName = "John",
                LastName = "Smith"
            }
        };

        var response = await claimsApiAccessor.GetClaims("aaxb", claimsRequest);

        var expectedUrl = mockHttpClient.BaseAddress + "api/AssureLink/Claims?LoggedInAgentNumber=aaxb&PageNumber=1&PageSize=10&ClaimantName.FirstName=John&ClaimantName.LastName=Smith";

        Assert.NotNull(response);

        Assert.Equal(JsonConvert.SerializeObject(mockResponse), JsonConvert.SerializeObject(response));

        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && expectedUrl.Equals(req.RequestUri.ToString())),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetClaims_Fails_ReturnsNull()
    {
        var mockResponse = GetMockClaimsResponse();

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(httpResponseMessage);

        var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        var mockLogger = new Mock<ILogger<ClaimsApiAccessor>>();

        var claimsApiAccessor = new ClaimsApiAccessor(mockHttpClient, mockLogger.Object);

        var claimRequest = new AssureLinkClaimRequest
        {
            ClaimantName = new Name
            {
                FirstName = "John",
                LastName = "Smith"
            }
        };

        var response = await claimsApiAccessor.GetClaims("aaxb", claimRequest);

        var expectedUrl = mockHttpClient.BaseAddress + "api/AssureLink/Claims?LoggedInAgentNumber=aaxb&PageNumber=1&PageSize=10&ClaimantName.FirstName=John&ClaimantName.LastName=Smith";

        Assert.Null(response);

        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && expectedUrl.Equals(req.RequestUri.ToString())),
            ItExpr.IsAny<CancellationToken>());
    }

    private Mock<HttpMessageHandler> GetMockHttpMessageHandler(HttpResponseMessage httpResponseMessage)
    {
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        mockHttpMessageHandler
           .Protected()
           .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(httpResponseMessage)
           .Verifiable();

        return mockHttpMessageHandler;
    }

    private AssureLinkClaimResponse GetMockClaimsResponse()
    {
        var mockClaimsResponse = new AssureLinkClaimResponse
        {
            Claims = new List<Claim>
            {
                new Claim
                {
                    ClaimNumber = null,
                    Claimant = new Name
                    {
                        FirstName = "John",
                        LastName = "Smith"
                    },
                    DateReported = DateTime.Parse("12/12/2024"),
                    PaymentAmount = 1,
                    PolicyNumber = "4180078103",
                    PolicyType = "Disability Income",
                    Status = ClaimStatus.Received,
                    StatusReason = string.Empty,
                    Details = []
                },
            },
            Page = 1,
            PageSize = 10,
            TotalRecords = 1,
        };

        return mockClaimsResponse;
    }
}