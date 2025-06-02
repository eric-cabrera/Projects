namespace Assurity.AgentPortal.Contracts.AgentContracts
{
    public class AgentAppointmentResponse
    {
        public List<AgentAppointment>? Appointments { get; set; }

        public List<string>? AppointedStates { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }
    }
}
