namespace Assurity.AgentPortal.Accessors.DirectusDTOs
{
    using Newtonsoft.Json;

    public class ContractsQueryResponse
    {
        [JsonProperty("data")]
        public ContractsQueryData Data { get; set; }
    }
}
