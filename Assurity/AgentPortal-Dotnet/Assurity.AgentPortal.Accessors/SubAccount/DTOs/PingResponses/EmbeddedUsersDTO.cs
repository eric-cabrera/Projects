namespace Assurity.AgentPortal.Accessors.SubAccount.DTOs.PingResponses;

using System.Text.Json.Serialization;

public class EmbeddedUsersDTO
{
    [JsonPropertyName("users")]
    public List<UsersResponseDTO>? Users { get; set; }
}
