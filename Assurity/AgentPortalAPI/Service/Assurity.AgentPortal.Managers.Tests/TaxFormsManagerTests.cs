namespace Assurity.AgentPortal.Managers.Tests;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Accessors.TaxForms;
using Assurity.AgentPortal.Contracts.Shared;
using Assurity.AgentPortal.Managers.TaxForms;
using Assurity.AgentPortal.Managers.TaxForms.Mapping;
using Assurity.TaxForms.Contracts.V1;
using AutoBogus;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static Assurity.AgentPortal.Managers.TaxForms.TaxFormsManager;

[ExcludeFromCodeCoverage]
public class TaxFormsManagerTests
{
    private readonly Mapper mapper;

    public TaxFormsManagerTests()
    {
        var mapperProfiles = new List<Profile>
        {
            new TaxFormsResponseMappingProfile()
        };

        var mapperConfig = new MapperConfiguration(config => config.AddProfiles(mapperProfiles));
        mapper = new Mapper(mapperConfig);
    }

    [Fact]
    public async Task GetTaxForms_Success()
    {
        // Arrange
        var agentId = "123A";

        var taxFormsResponse = new AutoFaker<GetAgentFormsResponse>().Generate();
        var mockTaxFormsApiAccessor = new Mock<ITaxFormsApiAccessor>();
        mockTaxFormsApiAccessor.Setup(accessor => accessor.GetTaxForms(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(taxFormsResponse)
            .Verifiable();

        var taxFormsManager = new TaxFormsManager(
            mockTaxFormsApiAccessor.Object,
            mapper);

        // Act
        var response = await taxFormsManager.GetTaxForms(
            agentId,
            CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        mockTaxFormsApiAccessor.Verify(
            accessor => accessor.GetTaxForms(
                agentId,
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task GetTaxForm_Success()
    {
        // Arrange
        var agentId = "123A";
        var formId = "TEST293749202089347934";

        var mockTaxFormsApiAccessor = new Mock<ITaxFormsApiAccessor>();
        mockTaxFormsApiAccessor.Setup(accessor => accessor.GetTaxForm(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemoryStream())
            .Verifiable();

        var taxFormsManager = new TaxFormsManager(
            mockTaxFormsApiAccessor.Object,
            mapper);

        // Act
        var response = await taxFormsManager.GetTaxForm(
            agentId,
            formId,
            CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.IsType<FileResponse>(response);
        Assert.Equal($"{formId}-{DateTime.Now:MMddyyyy}", response.FileName);
        Assert.Equal("application/pdf", response.FileType);
        mockTaxFormsApiAccessor.Verify(
            accessor => accessor.GetTaxForm(
                agentId,
                formId,
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task GetTaxForms_NullResponse_ShouldReturn_Null()
    {
        // Arrange
        var agentId = "123A";

        var mockTaxFormsApiAccessor = new Mock<ITaxFormsApiAccessor>();
        mockTaxFormsApiAccessor.Setup(accessor => accessor.GetTaxForms(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null)
            .Verifiable();

        var taxFormsManager = new TaxFormsManager(
            mockTaxFormsApiAccessor.Object,
            mapper);

        // Act
        var response = await taxFormsManager.GetTaxForms(
            agentId,
            CancellationToken.None);

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public async Task GetTaxForm_Success_ShouldReturnFileResponse()
    {
        // Arrange
        var agentId = "123A";
        var formId = "4N913P8C5F202F32650!F195DDF9D2DE921F849";

        var mockTaxFormsApiAccessor = new Mock<ITaxFormsApiAccessor>();
        mockTaxFormsApiAccessor.Setup(accessor => accessor.GetTaxForm(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemoryStream())
            .Verifiable();

        var taxFormsManager = new TaxFormsManager(
            mockTaxFormsApiAccessor.Object,
            mapper);

        var response = await taxFormsManager.GetTaxForm(
            agentId,
            formId,
            CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        mockTaxFormsApiAccessor.Verify(
            accessor => accessor.GetTaxForm(
                agentId, formId, CancellationToken.None),
            Times.Once);
        Assert.IsType<FileResponse>(response);
        Assert.Equal($"{formId}-{DateTime.Now:MMddyyyy}", response.FileName);
        Assert.Equal("application/pdf", response.FileType);
    }

    [Fact]
    public async Task GetTaxForm_NullResponse_ShouldReturn_Null()
    {
        // Arrange
        var agentId = "123A";
        var formId = "4N913P8C5F202F32650!F195DDF9D2DE921F849";

        var mockTaxFormsApiAccessor = new Mock<ITaxFormsApiAccessor>();
        mockTaxFormsApiAccessor.Setup(accessor => accessor.GetTaxForm(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null)
            .Verifiable();

        var taxFormsManager = new TaxFormsManager(
            mockTaxFormsApiAccessor.Object,
            mapper);

        // Act
        var response = await taxFormsManager.GetTaxForm(
            agentId,
            formId,
            CancellationToken.None);

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public void ExtractYear_ValidYear_ReturnsYear()
    {
        // Arrange
        string displayName = "2022";

        // Act
        string result = TaxFormsComparer.ExtractYear(displayName);

        // Assert
        Assert.Equal("2022", result);
    }

    [Fact]
    public void ExtractYear_YearWithSuffix_ReturnsYear()
    {
        // Arrange
        string displayName = "2022 NY";

        // Act
        string result = TaxFormsComparer.ExtractYear(displayName);

        // Assert
        Assert.Equal("2022", result);
    }

    [Fact]
    public void ExtractYear_EmptyString_ReturnsEmptyString()
    {
        // Arrange
        string displayName = string.Empty;

        // Act
        string result = TaxFormsComparer.ExtractYear(displayName);

        // Assert
        Assert.Equal(displayName, result);
    }

    [Fact]
    public void Compare_SameYear_ReturnsOne()
    {
        // Arrange
        var comparer = new TaxFormsComparer();
        string form1 = "2022";
        string form2 = "2022";

        // Act
        int result = comparer.Compare(form1, form2);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void Compare_DifferentYears_ReturnsCorrectOrder()
    {
        // Arrange
        var comparer = new TaxFormsComparer();
        string form1 = "2021";
        string form2 = "2022";

        // Act
        int result = comparer.Compare(form1, form2); // less than 0 means x is less than y

        // Assert
        Assert.True(result < 0);
    }

    [Fact]
    public void Compare_SameYearWithSuffix_ReturnsCorrectOrder()
    {
        // Arrange
        var comparer = new TaxFormsComparer();
        string form1 = "2022";
        string form2 = "2022 NY";

        // Act
        int result = comparer.Compare(form1, form2); // per mockup 2022 comes before 2022 NY

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void Compare_SameYearWithDifferentSuffix_ReturnsCorrectOrder()
    {
        // Arrange
        var comparer = new TaxFormsComparer();
        string form1 = "2022 NY";
        string form2 = "2022 CA";

        // Act
        int result = comparer.Compare(form1, form2);

        // Assert
        Assert.True(result > 0);
    }

    [Fact]
    public void Compare_YearWithSuffixAndYearWithoutSuffix_ReturnsCorrectOrder()
    {
        // Arrange
        var comparer = new TaxFormsComparer();
        string form1 = "2022 NY";
        string form2 = "2022";

        // Act
        int result = comparer.Compare(form1, form2); // per mockup 2022 comes before 2022 NY

        // Assert
        Assert.Equal(-1, result);
    }
}