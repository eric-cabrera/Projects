namespace Assurity.AgentPortal.Accessors.AssureLink.Entities
{
    public class AgentOptions
    {
        public string? AgentId { get; set; }

        public string? MarketCode { get; set; }

        public string? OptionType { get; set; }

        public bool OptOutForEDelivery { get; set; }

        public bool AgentLinkSelected { get; set; }

        public bool IncludeDownline { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
}
