namespace Assurity.AgentPortal.Managers.Tests.Mapping.TestData;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Contracts.Claims;

[ExcludeFromCodeCoverage]
public static class ClaimsDestinationData
{
    public static ClaimsResponse ClaimsResponse => new()
    {
        Claims = new List<Claim>
        {
            new Claim
            {
                ClaimNumber = null,
                Claimant = new Name
                {
                    FirstName = "John",
                    LastName = "Smith"
                },
                DateReported = DateTime.Parse("12/12/2024"),
                PaymentAmount = 1,
                PolicyNumber = "4180078103",
                PolicyType = "Disability Income",
                Status = "Received",
                StatusReason = string.Empty,
                Details = [
                    new Contracts.Claims.ClaimDetail
                    {
                        DeliveryMethod = "Check",
                        BenefitDate = DateTime.Parse("12/12/2024"),
                        BenefitDescription = "Cancer",
                        PaymentAmount = 1,
                        PaymentDate = DateTime.Parse("12/12/2024"),
                        PolicyNumber = "4180078103",
                        Status = "Received"
                    }
                ]
            },
        },
        Page = 1,
        PageSize = 10,
        TotalRecords = 1,
    };
}