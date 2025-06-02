namespace Assurity.AgentPortal.Contracts;

public class AgentData
{
    public AgentData()
    {
    }

    public AgentData(string username, string agentId)
    {
        Username = username;
        AgentId = agentId;
    }

    public string? Username { get; set; }

    public string? AgentId { get; set; }

    public string? Id { get; set; }

    public string? Email { get; set; }

    public string? Name { get; set; }

    public List<AgentRecord>? AdditionalAgents { get; set; }
}
