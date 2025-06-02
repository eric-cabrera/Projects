namespace Assurity.AgentPortal.Contracts.Directus
{
    public class DirectusPageResponse
    {
        public string PageDataJson { get; set; } = string.Empty;

        public bool HasAccess { get; set; } = true;
    }
}
