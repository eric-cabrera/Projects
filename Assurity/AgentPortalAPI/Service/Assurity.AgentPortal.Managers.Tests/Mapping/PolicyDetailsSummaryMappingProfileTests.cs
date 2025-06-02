namespace Assurity.AgentPortal.Managers.Tests.Mapping;

using Assurity.AgentPortal.Contracts.CommissionsDebt.FileExport;
using Assurity.AgentPortal.Contracts.FileExportEngine;
using Assurity.AgentPortal.Managers.CommissionsAndDebt.Mapping;
using Assurity.Commissions.Internal.Contracts.PolicyDetails;
using AutoBogus;
using AutoMapper;
using Xunit;

public class PolicyDetailsSummaryMappingProfileTests
{
    public PolicyDetailsSummaryMappingProfileTests()
    {
        Mapper = new MapperConfiguration(config =>
        {
            config.AddProfile<PolicyDetailsSummaryMappingProfile>();
        }).CreateMapper();
    }

    private IMapper Mapper { get; set; }

    [Fact]
    public void AssertConfigurationIsValid()
    {
        // Assert
        Mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void MapPercentageExcelDataType()
    {
        // Arrange
        var policyDetail = new AutoFaker<PolicyDetail>().Generate();

        // Act
        var result = Mapper.Map<PolicyDetailsExport>(policyDetail);

        // Assert
        Assert.Equal(policyDetail.CommissionRate / 100m, result.CommissionRate.Value);
        Assert.Equal(ExcelFormat.Fraction, result.CommissionRate.Format);
        Assert.Equal(policyDetail.CommissionType.ToString(), result.CommissionType);
        Assert.Equal(policyDetail.LineOfBusiness.ToString(), result.LineOfBusiness);
        Assert.Equal(policyDetail.Mode.ToString(), result.Mode);
    }
}
