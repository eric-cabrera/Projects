namespace Assurity.AgentPortal.Managers.Tests.Mapping.TestData;

using System.Diagnostics.CodeAnalysis;
using Assurity.ListBill.Service.Contracts;

[ExcludeFromCodeCoverage]
public static class ListBillsDestinationData
{
    public static Group Group => new()
    {
        Id = "1234567890",
        Name = "Vault-Tec",
        City = "Lincoln",
        State = "NE"
    };

    public static ListBill ListBill => new()
    {
        Id = "1000000006",
        Date = Convert.ToDateTime("2024-07-24T00:00:00-05:00"),
    };
}
