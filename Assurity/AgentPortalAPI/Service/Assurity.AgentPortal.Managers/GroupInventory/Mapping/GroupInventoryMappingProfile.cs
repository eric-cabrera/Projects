namespace Assurity.AgentPortal.Managers.GroupInventory.Mapping;

using Assurity.AgentPortal.Managers.MappingExtensions;
using Assurity.AgentPortal.Utilities.Formatting;
using Assurity.PolicyInfo.Contracts.V1.Enums;
using AutoMapper;
using GroupsAPI = Assurity.Groups.Contracts;
using LocalAgentContracts = Assurity.AgentPortal.Contracts.GroupInventory;
using MongoDb = Assurity.AgentPortal.Accessors.MongoDb.Contracts;

public class GroupInventoryMappingProfile : Profile
{
    private static readonly string GroupInventoryDateFormat = "MM/dd/yyyy";

    public GroupInventoryMappingProfile()
    {
        MapGroupInventorySummary();
        MapGroupInventoryDetails();
    }

    private static string? GetPrimaryOwnerNameAsString(GroupsAPI.Owner owner)
    {
        if (owner.Name.IsBusiness == true)
        {
            return owner.Name.BusinessName;
        }

        return $"{owner.Name.Last}, {owner.Name.First}";
    }

    private static string? GetPrimaryInsuredNameAsString(GroupsAPI.Insured insured)
    {
        if (insured.Name.IsBusiness == true)
        {
            return insured.Name.BusinessName;
        }

        return $"{insured.Name.Last}, {insured.Name.First}";
    }

    private static string? GetCurrentCity(GroupsAPI.Address address)
    {
        return address.City + ", " + address.StateAbbreviation;
    }

    private static string GetCoverageType(List<GroupsAPI.Benefit> benefits)
    {
        var baseBenefitWithCoverageType = benefits
            .Where(benefit => benefit.CoverageType == "Base"
                && benefit.CoverageOptions.Any(option => option.Name == "CoverageType"))
            .FirstOrDefault();

        if (baseBenefitWithCoverageType != null)
        {
            var coverageTypeOption = baseBenefitWithCoverageType.CoverageOptions
                .FirstOrDefault(option => option.Name == "CoverageType");

            if (coverageTypeOption != null)
            {
                var benefitOption = GetMappedBenefitOption(coverageTypeOption);
                if (benefitOption == null)
                {
                    return string.Empty;
                }

                return MappingExtensions.GetEnumDisplayName(benefitOption.Option);
            }
        }

        return string.Empty;
    }

    private static MongoDb.BenefitOptionsMapping? GetMappedBenefitOption(GroupsAPI.CoverageOption coverageOption)
    {
        return Enum.TryParse(coverageOption.Name, out BenefitOptionName category)
            && Enum.TryParse(coverageOption.Value, out BenefitOptionValue option)
            ? new MongoDb.BenefitOptionsMapping { Category = category, Option = option }
            : null;
    }

    private static List<LocalAgentContracts.Response.Benefit> ProcessBenefitCoverageOptions(List<GroupsAPI.Benefit> benefits)
    {
        var processedBenefits = new List<LocalAgentContracts.Response.Benefit>();

        foreach (var benefit in benefits)
        {
            var destinationBenefit = new LocalAgentContracts.Response.Benefit
            {
                CoverageType = benefit.CoverageType,
                Description = benefit.Description,
                Amount = DataFormatter.FormatDecimal(benefit.Amount),
                CoverageOptions = []
            };

            if (string.Equals(benefit.CoverageType, "Base", StringComparison.OrdinalIgnoreCase))
            {
                destinationBenefit.CoverageOptions = MapCoverageOptions(benefit.CoverageOptions, benefit.CoverageType);
            }
            else
            {
                destinationBenefit.CoverageOptions.Clear();
            }

            processedBenefits.Add(destinationBenefit);
        }

        return processedBenefits;
    }

    private static List<string> MapCoverageOptions(List<GroupsAPI.CoverageOption> coverageOptions, string coverageType)
    {
        if (string.IsNullOrEmpty(coverageType) || !coverageType.Equals("base", StringComparison.CurrentCultureIgnoreCase))
        {
            return [];
        }

        var coverageOptionStrings = new List<string>();

        foreach (var coverageOption in coverageOptions)
        {
            var mappedBenefitOption = GetMappedBenefitOption(coverageOption);

            if (mappedBenefitOption != null)
            {
                var formattedOption = $"{MappingExtensions.GetEnumDisplayName(mappedBenefitOption.Category)} - {MappingExtensions.GetEnumDisplayName(mappedBenefitOption.Option)}";

                if (!coverageOptionStrings.Contains(formattedOption))
                {
                    coverageOptionStrings.Add(formattedOption);
                }
            }
        }

        return coverageOptionStrings;
    }

    private void MapGroupInventorySummary()
    {
        CreateMap<LocalAgentContracts.Request.GroupSummaryQueryParameters, GroupsAPI.GroupSummaryQueryParameters>()
            .ForMember(dest => dest.GroupEffectiveDateStartDate, opt => opt.MapFrom(src => src.GroupEffectiveStartDate))
            .ForMember(dest => dest.GroupEffectiveDateEndDate, opt => opt.MapFrom(src => src.GroupEffectiveEndDate));

        CreateMap<GroupsAPI.GroupSummaryResponse, LocalAgentContracts.Response.GroupSummaryResponse>();
        CreateMap<GroupsAPI.GroupSummaryFilters, LocalAgentContracts.Response.GroupSummaryFilters>();
        CreateMap<GroupsAPI.GroupNameAndNumber, LocalAgentContracts.Response.GroupNameAndNumber>()
            .ForMember(dest => dest.DisplayValue, opt => opt.MapFrom(src => src.Number + " - " + src.Name));

        CreateMap<GroupsAPI.GroupSummary, LocalAgentContracts.Response.GroupSummary>()
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.GroupNumber))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GroupName))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.GroupStatus))
            .ForMember(dest => dest.GroupEffectiveDate, opt => opt.MapFrom(src => DataFormatter.FormatDate(src.GroupEffectiveDate, GroupInventoryDateFormat)));

        CreateMap<GroupsAPI.GroupSummary, LocalAgentContracts.FileExport.GroupInventorySummaryExport>()
            .ForMember(dest => dest.GroupEffectiveDate, opt => opt.MapFrom(src => DataFormatter.FormatDate(src.GroupEffectiveDate, GroupInventoryDateFormat)));
    }

    private void MapGroupInventoryDetails()
    {
        CreateMap<LocalAgentContracts.Request.GroupDetailsQueryParameters, GroupsAPI.GroupDetailsQueryParameters>()
            .ForMember(dest => dest.IssueDateStartDate, opt => opt.MapFrom(src => src.IssueStartDate))
            .ForMember(dest => dest.IssueDateEndDate, opt => opt.MapFrom(src => src.IssueEndDate));

        CreateMap<GroupsAPI.GroupDetailResponse, LocalAgentContracts.Response.GroupDetailResponse>();
        CreateMap<GroupsAPI.GroupDetailFilters, LocalAgentContracts.Response.GroupDetailFilters>();

        CreateMap<GroupsAPI.Group, LocalAgentContracts.Response.Group>()
            .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => DataFormatter.FormatDate(src.EffectiveDate, GroupInventoryDateFormat)));

        CreateMap<GroupsAPI.Policy, LocalAgentContracts.Response.Policy>()
            .ForMember(dest => dest.PaidToDate, opt => opt.MapFrom(src => DataFormatter.FormatDate(src.PaidToDate, GroupInventoryDateFormat)))
            .ForMember(dest => dest.AnnualPremium, opt => opt.MapFrom(src => DataFormatter.FormatDecimalAsAMonetaryValue(src.AnnualPremium)))
            .ForMember(dest => dest.ModePremium, opt => opt.MapFrom(src => DataFormatter.FormatDecimalAsAMonetaryValue(src.ModePremium)))
            .ForMember(dest => dest.IssueDate, opt => opt.MapFrom(src => DataFormatter.FormatDate(src.IssueDate, GroupInventoryDateFormat)))
            .ForMember(dest => dest.Benefits, opt => opt.MapFrom(src => ProcessBenefitCoverageOptions(src.Benefits)))
            .ForMember(dest => dest.CoverageType, opt => opt.MapFrom(src => GetCoverageType(src.Benefits)))
            .ForMember(dest => dest.Mode, opt => opt.MapFrom(src => $"{src.BillingMode} {src.PaymentType}"))
            .ForMember(dest => dest.PrimaryOwner, opt => opt.MapFrom(src => GetPrimaryOwnerNameAsString(src.PrimaryOwner)));

        CreateMap<GroupsAPI.Insured, LocalAgentContracts.Response.Insured>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => GetPrimaryInsuredNameAsString(src)))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => DataFormatter.FormatDate(src.DateOfBirth, GroupInventoryDateFormat)))
            .ForMember(dest => dest.CurrentCity, opt => opt.MapFrom(src => GetCurrentCity(src.Address)));

        CreateMap<GroupsAPI.CoverageOption, LocalAgentContracts.Response.CoverageOption>();
        CreateMap<GroupsAPI.Benefit, LocalAgentContracts.Response.Benefit>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => DataFormatter.FormatStringToTitleCase(src.Description)))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => DataFormatter.FormatDecimalAsAMonetaryValue(src.Amount)));

        CreateMap<GroupsAPI.Policy, LocalAgentContracts.FileExport.GroupInventoryDetailsExport>()
            .ForMember(dest => dest.PrimaryOwner, opt => opt.MapFrom(src => GetPrimaryOwnerNameAsString(src.PrimaryOwner)))
            .ForMember(dest => dest.PolicyNumber, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.PolicyStatus, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.AnnualPremium, opt => opt.MapFrom(src => DataFormatter.FormatDecimalAsAMonetaryValue(src.AnnualPremium)))
            .ForMember(dest => dest.ModePremium, opt => opt.MapFrom(src => DataFormatter.FormatDecimalAsAMonetaryValue(src.ModePremium)))
            .ForMember(dest => dest.Mode, opt => opt.MapFrom(src => $"{src.BillingMode} {src.PaymentType}"))
            .ForMember(dest => dest.PrimaryInsured, opt => opt.MapFrom(src => GetPrimaryInsuredNameAsString(src.PrimaryInsured)))
            .ForMember(dest => dest.InsuredDOB, opt => opt.MapFrom(src => src.PrimaryInsured.DateOfBirth != default ? DataFormatter.FormatDate(src.PrimaryInsured.DateOfBirth, GroupInventoryDateFormat) : null))
            .ForMember(dest => dest.CurrentCity, opt => opt.MapFrom(src => GetCurrentCity(src.PrimaryInsured.Address)));
    }
}
