namespace Assurity.AgentPortal.Contracts.ProductionCredit.FileExport;

using System.ComponentModel;

public class ProductionCreditWorksiteAgentExport
{
        [DisplayName("Line Of Business")]
        public string? Grouping { get; set; }

        [DisplayName("Product Name")]
        public string? ProductName { get; set; }

        [DisplayName("Group Name")]
        public string? GroupName { get; set; }

        [DisplayName("Group Count")]
        public int GroupCount { get; set; }

        [DisplayName("Annualized Premium")]
        public decimal AnnualizedPremium { get; set; }
}
