namespace Assurity.AgentPortal.Managers.Tests.Mapping.TestData;

using System.Diagnostics.CodeAnalysis;
using Assurity.Claims.Contracts;
using ClaimsAPI = Assurity.Claims.Contracts.AssureLink;

[ExcludeFromCodeCoverage]
public static class ClaimsSourceData
{
    public static ClaimsAPI.AssureLinkClaimResponse ClaimResponse => new()
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
}