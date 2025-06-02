namespace Assurity.Kafka.Managers.Tests
{
    using MongoDB.Driver;
    using Moq;

    internal static class MockHelpers
    {
        internal static Mock<IMongoClient> GetMockMongoClient()
        {
            var mockSession = new Mock<IClientSessionHandle>();
            var mockMongoClient = new Mock<IMongoClient>(MockBehavior.Strict);
            mockMongoClient
                .Setup(m => m.StartSessionAsync(null, default))
                .ReturnsAsync(mockSession.Object);

            return mockMongoClient;
        }
    }
}
