namespace Assurity.AgentPortal.Managers.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Assurity.AgentPortal.Accessors.Agent;
    using Assurity.AgentPortal.Accessors.EConsent;
    using Assurity.AgentPortal.Contracts.PolicyDelivery.Request;
    using Assurity.AgentPortal.Managers.PolicyDelivery;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class PolicyDeliveryManagerTests
    {
        private readonly Mock<IPolicyDeliveryApiAccessor> eConsentAccessorMock;
        private readonly Mock<IAgentApiAccessor> agentApiAccessorMock;
        private readonly Mock<ILogger<PolicyDeliveryManager>> loggerMock;
        private readonly PolicyDeliveryManager policyDeliveryManager;

        public PolicyDeliveryManagerTests()
        {
            eConsentAccessorMock = new Mock<IPolicyDeliveryApiAccessor>();
            agentApiAccessorMock = new Mock<IAgentApiAccessor>();
            loggerMock = new Mock<ILogger<PolicyDeliveryManager>>();

            policyDeliveryManager = new PolicyDeliveryManager(
                eConsentAccessorMock.Object,
                loggerMock.Object,
                agentApiAccessorMock.Object);
        }

        [Fact]
        public async Task UpdateAgentPolicyDeliveryOptions_ShouldReturnFalse_WhenEitherOptionUpdateFails()
        {
            var loggedInAgentNumber = "12345";
            var documentConnectOptions = new DocumentConnectOptions
            {
                ViewAsAgentNumber = "67890"
            };

            var additionalAgentIds = new List<string> { "67890", "99999" };

            agentApiAccessorMock.Setup(a => a.GetAdditionalAgentIds(loggedInAgentNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync(additionalAgentIds);

            eConsentAccessorMock.Setup(e => e.UpdateAgentPolicyDeliveryOptions(It.IsAny<DocumentConnectOptions>(), "EDelivery", It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            eConsentAccessorMock.Setup(e => e.UpdateAgentPolicyDeliveryOptions(It.IsAny<DocumentConnectOptions>(), "ERequirements", It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await policyDeliveryManager.UpdateAgentPolicyDeliveryOptions(loggedInAgentNumber, documentConnectOptions);

            Assert.False(result);
            agentApiAccessorMock.Verify(a => a.GetAdditionalAgentIds(loggedInAgentNumber, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAgentPolicyDeliveryOptions_ShouldReturnTrue_WhenBothOptionUpdatesSucceed()
        {
            var loggedInAgentNumber = "12345";
            var documentConnectOptions = new DocumentConnectOptions
            {
                ViewAsAgentNumber = "67890"
            };

            var additionalAgentIds = new List<string> { "67890", "99999" };

            agentApiAccessorMock.Setup(a => a.GetAdditionalAgentIds(loggedInAgentNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync(additionalAgentIds);

            eConsentAccessorMock.Setup(e => e.UpdateAgentPolicyDeliveryOptions(It.IsAny<DocumentConnectOptions>(), "EDelivery", It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            eConsentAccessorMock.Setup(e => e.UpdateAgentPolicyDeliveryOptions(It.IsAny<DocumentConnectOptions>(), "ERequirements", It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await policyDeliveryManager.UpdateAgentPolicyDeliveryOptions(loggedInAgentNumber, documentConnectOptions);

            Assert.True(result);
            agentApiAccessorMock.Verify(a => a.GetAdditionalAgentIds(loggedInAgentNumber, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetdatePolicyDeliveryOptions_ShouldReturnEmpty_WhenAgentHasNoAccessToViewAsAgentNumber()
        {
            // Arrange
            var loggedInAgentNumber = "12345";
            var viewAsAgentNumber = "99999";
            var viewAsMarketCode = "AGTDTC";

            var additionalAgentIds = new List<string> { "67890", "99999" };

            agentApiAccessorMock.Setup(a => a.GetAdditionalAgentIds(loggedInAgentNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync(additionalAgentIds);

            eConsentAccessorMock.Setup(e => e.GetPolicyDeliveryOptions(viewAsAgentNumber, viewAsMarketCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DocumentConnectOptions());

            // Act
            var result = await policyDeliveryManager.GetPolicyDeliveryOptions(
                loggedInAgentNumber,
                viewAsAgentNumber,
                viewAsMarketCode);

            // Assert
            Assert.IsType<DocumentConnectOptions>(result);
            Assert.Null(result.ViewAsAgentNumber);
        }

        [Fact]
        public async Task GetdatePolicyDeliveryOptions_ShouldReturnCorrectData_WhenAgentHasAccessToViewAsAgentNumber()
        {
            // Arrange
            var loggedInAgentNumber = "12345";
            var viewAsAgentNumber = "67890";
            var viewAsMarketCode = "AGTDTC";

            var additionalAgentIds = new List<string> { "67890", "99999" };

            agentApiAccessorMock.Setup(a => a.GetAdditionalAgentIds(loggedInAgentNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync(additionalAgentIds);

            var documentConnectOptions = new DocumentConnectOptions
            {
                ViewAsAgentNumber = "67890"
            };

            eConsentAccessorMock.Setup(e => e.GetPolicyDeliveryOptions(viewAsAgentNumber, viewAsMarketCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(documentConnectOptions);

            // Act
            var result = await policyDeliveryManager.GetPolicyDeliveryOptions(
                loggedInAgentNumber,
                viewAsAgentNumber,
                viewAsMarketCode);

            // Assert
            Assert.Equal(viewAsAgentNumber, result.ViewAsAgentNumber);
        }
    }
}
