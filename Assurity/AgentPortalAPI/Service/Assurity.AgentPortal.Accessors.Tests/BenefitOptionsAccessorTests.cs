namespace Assurity.AgentPortal.Accessors.Tests;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Accessors.PolicyInfo;
using Assurity.AgentPortal.Accessors.Tests.MongoDb;
using Xunit;

[ExcludeFromCodeCoverage]
public class BenefitOptionsAccessorTests : MongoDbTestContext
{
    [Fact]
    public async Task GetHiddenBenefitOptionsMappings_ReturnsCorrectly()
    {
        var accessor = new BenefitOptionsAccessor(TempConnection);

        var result = await accessor.GetHiddenBenefitOptionsMappings();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }
}
