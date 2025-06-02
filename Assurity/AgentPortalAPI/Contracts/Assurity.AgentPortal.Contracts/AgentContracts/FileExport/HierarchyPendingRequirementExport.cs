namespace Assurity.AgentPortal.Contracts.AgentContracts.FileExport
{
    using System;
    using System.ComponentModel;

    public class HierarchyPendingRequirementExport
    {
        [DisplayName("Agent Name")]
        public string? AgentName { get; set; }

        [DisplayName("Agent ID")]
        public string? AgentId { get; set; }

        [DisplayName("Market Code")]
        public string? MarketCode { get; set; }

        [DisplayName("Agent Level")]
        public string? AgentLevel { get; set; }

        [DisplayName("Email Address")]
        public string? EmailAddress { get; set; }

        [DisplayName("Requirement")]
        public string? Requirement { get; set; }

        [DisplayName("Note")]
        public string? Note { get; set; }
    }
}
