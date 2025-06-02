namespace Assurity.Kafka.Managers.DTOs
{
    public record AgentPolicyAccessRecord
    {
        public AgentIdAndCompanyCode AgentIdAndCompanyCode { get; set; }

        public string PolicyNumber { get; set; }

        public AgentPolicyAccessRecord(AgentIdAndCompanyCode idAndCompanyCode, string policyNumber)
        {
            AgentIdAndCompanyCode = idAndCompanyCode;
            PolicyNumber = policyNumber;
        }
    }
}
