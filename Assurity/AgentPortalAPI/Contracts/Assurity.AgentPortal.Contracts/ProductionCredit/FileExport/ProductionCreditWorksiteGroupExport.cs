namespace Assurity.AgentPortal.Contracts.ProductionCredit.FileExport
{
    using System.ComponentModel;

    public class ProductionCreditWorksiteGroupExport
    {
        [DisplayName("Group Name")]
        public string? GroupName { get; set; }

        [DisplayName("Group Number")]
        public string? GroupNumber { get; set; }

        [DisplayName("Writing Agent")]
        public string? WritingAgent { get; set; }

        [DisplayName("Product Name")]
        public string? ProductName { get; set; }

        [DisplayName("Effective Date")]
        public string? EffectiveDate { get; set; }

        [DisplayName("Annualized Premium")]
        public decimal AnnualizedPremium { get; set; }

        [DisplayName("Policy Count")]
        public int PolicyCount { get; set; }
    }
}
