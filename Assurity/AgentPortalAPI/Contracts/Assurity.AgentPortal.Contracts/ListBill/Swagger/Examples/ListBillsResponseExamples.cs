namespace Assurity.AgentPortal.Contracts.ListBill.Swagger.Examples;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class ListBillsResponseExamples
{
    public static ListBillsResponse ListBillsResponseExample => new()
    {
        GroupNumber = "1234567890",
        ListBills =
        [
            new()
            {
                Id = "1000000006",
                Date = DateTime.Now.Date,
            },
            new()
            {
                Id = "1000000005",
                Date = DateTime.Now.Date.AddMonths(-1),
            },
            new()
            {
                Id = "1000000004",
                Date = DateTime.Now.Date.AddMonths(-2),
            },
            new()
            {
                Id = "1000000003",
                Date = DateTime.Now.Date.AddMonths(-3),
            },
            new()
            {
                Id = "1000000002",
                Date = DateTime.Now.Date.AddMonths(-4),
            },
            new()
            {
                Id = "1000000001",
                Date = DateTime.Now.Date.AddMonths(-5),
            },
        ]
    };
}
