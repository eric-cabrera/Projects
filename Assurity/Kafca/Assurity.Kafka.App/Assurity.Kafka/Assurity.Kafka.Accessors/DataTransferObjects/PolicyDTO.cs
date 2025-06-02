namespace Assurity.Kafka.Accessors.DataTransferObjects
{
    public class PolicyDTO
    {
        public decimal AnnualPremium { get; set; }

        public int AppReceivedDate { get; set; }

        public int ApplicationDate { get; set; }

        public AnnuityPolicyDTO? AnnuityPolicy { get; set; }

        public string BillingCode { get; set; }

        public int BillingDate { get; set; }

        public string BillingForm { get; set; }

        public short BillingMode { get; set; }

        public string BillingReason { get; set; }

        public string CompanyCode { get; set; }

        public string ContractCode { get; set; }

        public int ContractDate { get; set; }

        public string ContractReason { get; set; }

        public EmployerDTO? Employer { get; set; }

        public string GroupNumber { get; set; }

        public int IssueDate { get; set; }

        public string IssueState { get; set; }

        public string LineOfBusiness { get; set; }

        public decimal ModePremium { get; set; }

        public NewBusinessPendingDTO? NewBusinessPending { get; set; }

        public int PaidToDate { get; set; }

        public string PolcSpecialMode { get; set; }

        public string PolicyNumber { get; set; }

        public short PolicyBillDay { get; set; }

        public string ProductCode { get; set; }

        public string ResidenceState { get; set; }

        public string TaxQualifyCode { get; set; }
    }
}
