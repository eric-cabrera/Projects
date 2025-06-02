namespace Assurity.Kafka.Accessors.DataTransferObjects
{
    public record CompanyCodeAndPolicyNumber
    {
        public string CompanyCode { get; set; }

        public string PolicyNumber { get; set; }

        public CompanyCodeAndPolicyNumber()
        {
        }

        public CompanyCodeAndPolicyNumber(string companyCode, string policyNumber)
        {
            CompanyCode = companyCode;
            PolicyNumber = policyNumber;
        }
    }
}