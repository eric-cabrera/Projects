namespace Assurity.AgentPortal.Accessors.SubAccount.DTOs.PingResponses;

using System.Text.Json.Serialization;

public class SubAccountResponseDTO
{
    [JsonPropertyName("_links")]
    public LinksDTO? Links { get; set; }

    [JsonPropertyName("_embedded")]
    public EmbeddedUsersDTO? Embedded { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }
}