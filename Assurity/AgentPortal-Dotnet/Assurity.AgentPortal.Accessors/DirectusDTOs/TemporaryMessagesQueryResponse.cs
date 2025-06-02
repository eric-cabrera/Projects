namespace Assurity.AgentPortal.Accessors.DirectusDTOs;

using Newtonsoft.Json;

public class TemporaryMessagesQueryResponse
{
    [JsonProperty("data")]
    public TemporaryMessagesData Data { get; set; }
}
