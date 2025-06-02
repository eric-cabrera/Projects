namespace Assurity.AgentPortal.Accessors.DirectusDTOs
{
    using Newtonsoft.Json;

    public class DirectusFile
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("filename_download")]
        public string Filename { get; set; }
    }
}
