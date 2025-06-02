namespace Assurity.AgentPortal.Accessors.AssureLink.Entities
{
    public class AgentPersonalInformation
    {
        public string? AgentId { get; set; }

        public string? FullName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? ZipCode { get; set; }

        public string? Phone { get; set; }

        public string? Fax { get; set; }

        public string? AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; }

        public string? AddressLine3 { get; set; }

        public string? Prefix { get; set; }

        public string? MiddleName { get; set; }

        public string? Suffix { get; set; }

        public bool DirectDepositIndicator { get; set; }

        public string? Email { get; set; }

        public string? ZipExtension { get; set; }

        public string? HeldCommissions { get; set; }

        public string? HeldAdvances { get; set; }

        public DateTime? AntiMoneyLaunderingDate { get; set; }
    }
}