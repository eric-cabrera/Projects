namespace Assurity.AgentPortal.Accessors.DirectusDTOs;

using Newtonsoft.Json;

public class TemporaryMessagesData
{
    [JsonProperty("temp_messages")]
    public List<TemporaryMessage> TemporaryMessages { get; set; }
}
