namespace Assurity.AgentPortal.Contracts.AgentContracts.FileExport;

using System;
using System.ComponentModel;

public class AgentAppointmentExport
{
        [DisplayName("Agent Name")]
        public string? Name { get; set; }

        [DisplayName("Agent ID")]
        public string? AgentNumber { get; set; }

        [DisplayName("State")]
        public string? StateAbbreviation { get; set; }

        [DisplayName("Resident/Non-Resident")]
        public string? IsResident { get; set; }

        [DisplayName("Appointment")]
        public DateTime? GrantedDate { get; set; }

        [DisplayName("License Expiration Date")]
        public DateTime? ExpirationDate { get; set; }
}
