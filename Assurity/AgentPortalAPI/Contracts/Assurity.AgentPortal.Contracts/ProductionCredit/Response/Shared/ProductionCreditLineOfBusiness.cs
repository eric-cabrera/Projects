namespace Assurity.AgentPortal.Contracts.ProductionCredit.Response.Shared;

public class ProductionCreditLineOfBusiness
{
    public string Name { get; set; }

    public List<ProductionCreditLineOfBusinessType> Types { get; set; }
}
