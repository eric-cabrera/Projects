namespace Assurity.AgentPortal.Contracts.CaseManagement;

using Assurity.ApplicationTracker.Contracts.DataTransferObjects;

public class CaseManagementEvent : AppTrackerEvent
{
    public string? EventName { get; set; }
}
