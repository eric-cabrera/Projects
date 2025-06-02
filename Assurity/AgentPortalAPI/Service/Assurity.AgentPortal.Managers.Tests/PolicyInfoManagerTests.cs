namespace Assurity.AgentPortal.Managers.Tests;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Accessors.MongoDb.Contracts;
using Assurity.AgentPortal.Accessors.PolicyInfo;
using Assurity.AgentPortal.Contracts.PolicyInfo;
using Assurity.AgentPortal.Managers.PolicyInfo;
using Assurity.PolicyInfo.Contracts.V1;
using Assurity.PolicyInfo.Contracts.V1.Enums;
using AutoMapper;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class PolicyInfoManagerTests
{
    [Fact]
    public async Task GetPolicyInfoAsync_ShouldSucceed()
    {
        // Arrange
        var benefitOptionsMapping = new List<BenefitOptionsMapping>
        {
            new BenefitOptionsMapping
            {
                Id = "abc123",
                Category = BenefitOptionName.AccidentBenefitPeriod,
                Option = BenefitOptionValue.Aviation,
                HideBenefitOption = true,
            },
            new BenefitOptionsMapping
            {
                Id = "123456",
                Category = BenefitOptionName.LossRatio,
                Option = BenefitOptionValue.Joint,
                HideBenefitOption = true,
            },
        };
        var policy = Mapping.TestData.PolicySourceTestData.PendingPolicy;
        policy.Benefits.First().BenefitOptions.First().BenefitOptionName = BenefitOptionName.AccidentBenefitPeriod;
        policy.Benefits.First().BenefitOptions.First().BenefitOptionValue = BenefitOptionValue.Aviation;
        var mockBenefitOptionsAccessor = new Mock<IBenefitOptionsAccessor>();
        mockBenefitOptionsAccessor.Setup(accessor => accessor.GetHiddenBenefitOptionsMappings())
            .ReturnsAsync(benefitOptionsMapping);

        var mockPolicyInfoAccessor = new Mock<IPolicyInfoApiAccessor>();
        mockPolicyInfoAccessor.Setup(accessor => accessor.GetPolicyInfo(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(policy);

        var expectedPolicy = Mapping.TestData.PolicyDestinationTestData.PendingPolicy;
        expectedPolicy.Benefits.First().BenefitOptions = new List<BenefitOptionResponse>();

        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(mapper => mapper.Map<PolicyResponse>(It.IsAny<Policy>()))
            .Returns(expectedPolicy)
            .Verifiable();

        var accessor = new PolicyInfoManager(
            mockPolicyInfoAccessor.Object,
            mockBenefitOptionsAccessor.Object,
            mockMapper.Object,
            null);

        // Act
        var result = await accessor.GetPolicyInfo(policy.PolicyNumber, policy.Agents.First().AgentId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Benefits.First().BenefitOptions);
        mockMapper.Verify(
            mock => mock.Map<PolicyResponse>(
                It.Is<Policy>(policy => policy.Benefits.First().BenefitOptions.Count.Equals(0))),
            Times.Once);
    }
}
