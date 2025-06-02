namespace Assurity.AgentPortal.Managers.Tests;

using Assurity.AgentPortal.Accessors;
using Assurity.AgentPortal.Contracts;
using Moq;

public class UserDataManagerTests
{
    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { new List<Market>(), new List<string> { "Individual" } };
        yield return new object[] { new List<Market> { Market.Individual }, new List<string> { "Individual" } };
        yield return new object[] { new List<Market> { Market.Worksite }, new List<string> { "Worksite" } };
        yield return new object[] { new List<Market> { Market.Individual, Market.Worksite }, new List<string> { "Individual", "Worksite" } };
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public async Task GetProductionCreditBusinessTypes_Success(List<Market> accessorResponse, List<string> managerResponse)
    {
        // Arrange
        var fakeAccessToken = "fakeAccessToken";
        var fakeAgentId = "A123";
        var mockAccessor = new Mock<IUserDataAccessor>();
        mockAccessor.Setup(accessor => accessor.GetProductionCreditBusinessTypes(
            It.Is<string>(token => token == fakeAccessToken),
            It.Is<string>(agentId => agentId == fakeAgentId)))
        .ReturnsAsync(accessorResponse)
        .Verifiable();

        var userDataManager = new UserDataManager(mockAccessor.Object);

        // Act
        var response = await userDataManager.GetProductionCreditBusinessTypes(fakeAccessToken, fakeAgentId);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(managerResponse, response);

        mockAccessor.Verify(
            accessor => accessor.GetProductionCreditBusinessTypes(
            fakeAccessToken, fakeAgentId),
            Times.Once);
    }
}
