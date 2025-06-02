namespace Assurity.Kafka.Managers.DTOs
{
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Utilities.Enums;

    public record BackfillError
    {
        public GetPolicyResult ResultCode { get; set; }

        public string MethodName { get; set; }

        public CompanyCodeAndPolicyNumber CompanyCodeAndPolicyNumber { get; set; }

        /// <summary>
        /// Only populated if GetPolicyResult is <see cref="GetPolicyResult.ExceptionThrown"/>.
        /// </summary>
        public string? Exception { get; set; }

        public BackfillError(GetPolicyResult result, string methodName, CompanyCodeAndPolicyNumber companyCodeAndPolicyNumber, string? exception)
        {
            MethodName = methodName;
            ResultCode = result;
            CompanyCodeAndPolicyNumber = companyCodeAndPolicyNumber;
            Exception = exception;
        }
    }
}
