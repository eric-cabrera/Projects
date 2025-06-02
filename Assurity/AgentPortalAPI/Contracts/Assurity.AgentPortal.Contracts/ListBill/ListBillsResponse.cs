namespace Assurity.AgentPortal.Contracts.ListBill;

public class ListBillsResponse
{
    public string GroupNumber { get; set; }

    public List<ListBill> ListBills { get; set; }
}