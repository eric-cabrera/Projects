namespace Assurity.AgentPortal.Accessors.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using Assurity.AgentPortal.Accessors.Claims;
using Assurity.AgentPortal.Contracts.Claims;
using Assurity.AgentPortal.Service.Handlers;
using Assurity.Claims.Contracts;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;
using ClaimsAPI = Assurity.Claims.Contracts.AssureLink;

[ExcludeFromCodeCoverage]
public class ClaimsApiAccessorTests
{
    private readonly Uri baseAddress = new("http://localhost/");

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
        var claimsApiAccessor = GetClaimsApiAccessor(mockHttpMessageHandler);

        var claimsParameters = new ClaimsParameters
        {
            ClaimantFirstName = "John",
            ClaimantLastName = "Smith"
        };

        var response = await claimsApiAccessor.GetClaims("aaxb", claimsParameters);

        var expectedUrl = baseAddress + "api/AssureLink/Claims?LoggedInAgentNumber=aaxb&PageNumber=1&PageSize=10&ClaimantName.FirstName=John&ClaimantName.LastName=Smith";

        Assert.NotNull(response);

        Assert.Equal(JsonConvert.SerializeObject(mockResponse), JsonConvert.SerializeObject(response));

        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && expectedUrl.Equals(req.RequestUri.ToString())),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetClaims_FailsWith404WithResponseBody_ReturnsBlankObject()
    {
        var mockResponse = GetMockClaimsResponse();

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent("couldn't find your claim")
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(httpResponseMessage);
        var claimsApiAccessor = GetClaimsApiAccessor(mockHttpMessageHandler);

        var claimsParameters = new ClaimsParameters
        {
            ClaimantFirstName = "John",
            ClaimantLastName = "Smith"
        };

        var response = await claimsApiAccessor.GetClaims("aaxb", claimsParameters);

        var expectedUrl = baseAddress + "api/AssureLink/Claims?LoggedInAgentNumber=aaxb&PageNumber=1&PageSize=10&ClaimantName.FirstName=John&ClaimantName.LastName=Smith";

        Assert.NotNull(response);
        Assert.Null(response.Claims);

        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && expectedUrl.Equals(req.RequestUri.ToString())),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetClaims_FailsWith404WithoutResponseBody_ThrowsException()
    {
        var mockResponse = GetMockClaimsResponse();

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = null
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(httpResponseMessage);
        var claimsApiAccessor = GetClaimsApiAccessor(mockHttpMessageHandler);

        var claimsParameters = new ClaimsParameters
        {
            ClaimantFirstName = "John",
            ClaimantLastName = "Smith"
        };

        await Assert.ThrowsAsync<Exception>(async () => await claimsApiAccessor.GetClaims("aaxb", claimsParameters));

        var expectedUrl = baseAddress + "api/AssureLink/Claims?LoggedInAgentNumber=aaxb&PageNumber=1&PageSize=10&ClaimantName.FirstName=John&ClaimantName.LastName=Smith";

        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && expectedUrl.Equals(req.RequestUri.ToString())),
            ItExpr.IsAny<CancellationToken>());
    }

    private ClaimsApiAccessor GetClaimsApiAccessor(Mock<HttpMessageHandler> mockHttpMessageHandler)
    {
        var errorResponseHandler = new ErrorResponseHandler()
        {
            InnerHandler = mockHttpMessageHandler.Object
        };

        var mockHttpClient = new HttpClient(errorResponseHandler)
        {
            BaseAddress = baseAddress
        };

        var mockLogger = new Mock<ILogger<ClaimsApiAccessor>>();

        return new ClaimsApiAccessor(mockHttpClient, mockLogger.Object);
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

    private ClaimsAPI.AssureLinkClaimResponse GetMockClaimsResponse()
    {
        var mockClaimsResponse = new ClaimsAPI.AssureLinkClaimResponse
        {
            Claims = new List<ClaimsAPI.Claim>
            {
                new ClaimsAPI.Claim
                {
                    ClaimNumber = null,
                    Claimant = new ClaimsAPI.Name
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
                    Details = [
                        new ClaimsAPI.ClaimDetail
                        {
                            DeliveryMethod = "Check",
                            BenefitDate = DateTime.Parse("12/12/2024"),
                            BenefitDescription = "Cancer",
                            PaymentAmount = 1,
                            PaymentDate = DateTime.Parse("12/12/2024"),
                            PolicyNumber = "4180078103",
                            Status = ClaimStatus.Received
                        }
                    ]
                },
            },
            Page = 1,
            PageSize = 10,
            TotalRecords = 1,
        };

        return mockClaimsResponse;
    }
}