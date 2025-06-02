namespace Assurity.Kafka.Managers.DTOs
{
    public record AgentIdAndCompanyCode
    {
        public string AgentId { get; set; }

        public string CompanyCode { get; set; }

        public AgentIdAndCompanyCode(string agentId, string companyCode)
        {
            AgentId = agentId;
            CompanyCode = companyCode;
        }
    }
}
