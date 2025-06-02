namespace Assurity.AgentPortal.Contracts.ProductionCredit.Response.Shared;

public class ProductionCreditFilterValues
{
    public List<ProductionCreditAgent> Agents { get; set; }

    public List<ProductionCreditLineOfBusiness> LinesOfBusiness { get; set; }

    public List<string> MarketCodes { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public List<string> ViewAsAgentIds { get; set; }
}
