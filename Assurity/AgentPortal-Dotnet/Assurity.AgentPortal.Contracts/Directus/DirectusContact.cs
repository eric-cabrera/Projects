namespace Assurity.AgentPortal.Contracts.Directus;

using Newtonsoft.Json;

public class DirectusContact
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonProperty("last_name")]
    public string LastName { get; set; } = string.Empty;

    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("image")]
    public DirectusFile Image { get; set; } = new();

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("contact_types")]
    public List<DirectusContactTypeXref> ContractTypes { get; set; } = new List<DirectusContactTypeXref>();

    [JsonProperty("region")]
    public DirectusRegion Region { get; set; } = new();
}
