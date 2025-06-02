namespace Assurity.AgentPortal.Contracts.CommissionsDebt;

public class AdditionalAgentIds
{
    /// <summary>
    /// Creates new instance of the Additional Agent Ids class without a filtered agent id.
    /// </summary>
    /// <param name="allAgentIds">A list of all available agent ids.</param>
    public AdditionalAgentIds(List<string> allAgentIds)
    {
        AllAgentIds = allAgentIds;
    }

    /// <summary>
    /// Creates new instance of the Additional Agent Ids class with a filtered agent id.
    /// </summary>
    /// <param name="allAgentIds">A list of all available agent ids.</param>
    /// <param name="filteredAgentId">A single agent id on which to filter the AllAgentIds list.</param>
    public AdditionalAgentIds(List<string> allAgentIds, string filteredAgentId)
    {
        AllAgentIds = allAgentIds;
        FilteredAgentId = filteredAgentId;
    }

    /// <summary>
    /// The original list of agent ids.
    /// </summary>
    public List<string> AllAgentIds { get; set; }

    /// <summary>
    /// Returns the AllAgentIds list filtered by the specified FitleredAgentId.
    /// If no agent id was specififed, returns the AllAgentIds list.
    /// </summary>
    public List<string> FilteredAgentIds
    {
        get { return GetFilteredAgentIds(); }
    }

    private string? FilteredAgentId { get; set; }

    private List<string> GetFilteredAgentIds()
    {
        return FilteredAgentId == null ? AllAgentIds : AllAgentIds.Where(agent => agent == FilteredAgentId).ToList();
    }
}
