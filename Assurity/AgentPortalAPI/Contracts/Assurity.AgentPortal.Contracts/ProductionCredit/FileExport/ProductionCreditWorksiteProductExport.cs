namespace Assurity.AgentPortal.Contracts.ProductionCredit.FileExport;

using System.ComponentModel;

public class ProductionCreditWorksiteProductExport
{
        [DisplayName("Line Of Business")]
        public string? Grouping { get; set; }

        [DisplayName("Product Type")]
        public string? ProductType { get; set; }

        [DisplayName("Product Name")]
        public string? ProductName { get; set; }

        [DisplayName("Policy Count")]
        public int PolicyCount { get; set; }

        [DisplayName("Annualized Premium")]
        public decimal AnnualizedPremium { get; set; }
}
