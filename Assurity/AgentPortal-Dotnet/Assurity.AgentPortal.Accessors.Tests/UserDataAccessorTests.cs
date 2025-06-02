namespace Assurity.AgentPortal.Accessors.Tests;

using System.Net;
using Assurity.AgentPortal.Contracts;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

public class UserDataAccessorTests
{
    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { "[]", new List<Market>() };
        yield return new object[] { "[\"Individual\"]", new List<Market> { Market.Individual } };
        yield return new object[] { "[\"Worksite\"]", new List<Market> { Market.Worksite } };
        yield return new object[] { "[\"Individual\",\"Worksite\"]", new List<Market> { Market.Individual, Market.Worksite } };
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public async void GetProductionCreditBusinessTypes(string responseContent, List<Market> expectedOutput)
    {
        var fakeAccessToken = "FakeAccessToken";
        var agentId = "A123";

        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler
            .Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent),
            })
            .Verifiable();

        var httpClient = new HttpClient(mockHttpHandler.Object)
        {
            BaseAddress = new Uri("https://example.com/")
        };

        var mockHttpClientFactory = new Mock<IHttpClientFactory>();
        mockHttpClientFactory.Setup(factory => factory.CreateClient("AgentPortalAPIHttpClient")).Returns(httpClient);
        var mockLogger = new Mock<ILogger<UserDataAccessor>>();

        var userDataAccessor = new UserDataAccessor(mockHttpClientFactory.Object, mockLogger.Object);

        var result = await userDataAccessor.GetProductionCreditBusinessTypes(fakeAccessToken, agentId);

        Assert.NotNull(result);
        Assert.Equal(expectedOutput, result);

        mockHttpHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(
                req => req.Method == HttpMethod.Get
                && req.RequestUri == new Uri("https://example.com/API/UserData/BusinessTypes")),
            ItExpr.IsAny<CancellationToken>());
    }
}
