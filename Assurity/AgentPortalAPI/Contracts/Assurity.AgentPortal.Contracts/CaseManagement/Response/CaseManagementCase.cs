namespace Assurity.AgentPortal.Contracts.CaseManagement;

using Assurity.ApplicationTracker.Contracts.DataTransferObjects;

public class CaseManagementCase : ApplicationTracker
{
    public string? ProductName { get; set; }

    public new List<CaseManagementEvent> Events
    {
        get
        {
            if (base.Events == null)
            {
                base.Events = new List<AppTrackerEvent>();
            }

            var result = base.Events.Select(e => e as CaseManagementEvent ?? new CaseManagementEvent
            {
                Event = e.Event,
                CreatedDateTime = e.CreatedDateTime,
                EnvelopeId = e.EnvelopeId,
                RecipientId = e.RecipientId,
            }).ToList();

            return result;
        }

        set
        {
            base.Events = value.Cast<AppTrackerEvent>().ToList();
        }
    }

    public new CaseManagementEvent CurrentEvent
    {
        get
        {
            if (base.CurrentEvent == null)
            {
                base.CurrentEvent = new AppTrackerEvent();
            }

            var result = base.CurrentEvent as CaseManagementEvent ?? new CaseManagementEvent
            {
                Event = base.CurrentEvent.Event,
                CreatedDateTime = base.CurrentEvent.CreatedDateTime,
                EnvelopeId = base.CurrentEvent.EnvelopeId,
                RecipientId = base.CurrentEvent.RecipientId,
            };

            return result;
        }

        set
        {
            base.CurrentEvent = value;
        }
    }
}
