namespace Assurity.AgentPortal.Accessors.DirectusDTOs;

using Newtonsoft.Json;

public class TemporaryMessage
{
    public int Id { get; set; }

    public string Heading { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    [JsonProperty("publish_date")]
    public DateTime PublishDate { get; set; }

    [JsonProperty("cta_label")]
    public string CtaLabel { get; set; } = string.Empty;

    [JsonProperty("cta_link")]
    public string CtaLink { get; set; } = string.Empty;
}
