namespace Assurity.AgentPortal.Contracts.ProductionCredit.FileExport;

using System.ComponentModel;

public class ProductionCreditWorksiteDownlineWritingAgentsExport
{
        [DisplayName("Agent")]
        public string? Agent { get; set; }

        [DisplayName("Group Count")]
        public int GroupCount { get; set; }

        [DisplayName("Annualized Premium")]
        public decimal AnnualizedPremium { get; set; }
}
