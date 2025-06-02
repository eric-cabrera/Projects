namespace Assurity.AgentPortal.Contracts.ListBill.Swagger.Examples;

using System.Diagnostics.CodeAnalysis;
using Assurity.ListBill.Service.Contracts;

[ExcludeFromCodeCoverage]
public class GroupsResponseExamples
{
    public static GroupsResponse GroupsResponseExample => new()
    {
        Page = 1,
        PageSize = 10,
        TotalRecords = 3,
        Groups =
        [
           Group1,
           Group2,
           Group3
        ]
    };

    private static Group Group1 => new()
    {
        Id = "1234567890",
        Name = "Vault-Tec",
        City = "Lincoln",
        State = "NE"
    };

    private static Group Group2 => new()
    {
        Id = "0987654321",
        Name = "Insurance Inc",
        City = "Chicago",
        State = "IL"
    };

    private static Group Group3 => new()
    {
        Id = "2143658709",
        Name = "Listbill LLC",
        City = "New York",
        State = "NY"
    };
}
