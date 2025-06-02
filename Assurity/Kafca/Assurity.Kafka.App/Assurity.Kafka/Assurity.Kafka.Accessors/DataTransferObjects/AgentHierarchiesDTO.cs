namespace Assurity.Kafka.Accessors.DataTransferObjects
{
    public class AgentHierarchiesDTO
    {
        public string CompanyCode { get; set; }

        public string PolicyNumber { get; set; }

        public List<string>? AddedAgents { get; set; }

        public List<string>? RemovedAgents { get; set; }
    }
}