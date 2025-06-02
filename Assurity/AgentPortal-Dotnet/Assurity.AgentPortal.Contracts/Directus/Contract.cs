namespace Assurity.AgentPortal.Contracts.Directus
{
    public class Contract(string displayName, string downloadName, string id, string agentLevel)
    {
        public string DisplayName { get; set; } = displayName;

        public string DownloadName { get; set; } = downloadName;

        public string Id { get; set; } = id;

        public string AgentLevel { get; set; } = agentLevel.Trim();

        public double ConvertedAgentLevel
        {
            get
            {
                if (AgentLevel == "2B")
                {
                    return 2.1;
                }
                else
                {
                    return double.Parse(AgentLevel);
                }
            }
        }
    }
}
