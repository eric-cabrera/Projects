namespace Assurity.AgentPortal.Managers.CaseManagement;

using Assurity.AgentPortal.Accessors.Agent;
using Assurity.AgentPortal.Accessors.ApplicationTracker;
using Assurity.AgentPortal.Contracts.CaseManagement;
using Assurity.ApplicationTracker.Contracts.Enums;
using AutoMapper;

public class CaseManagementManager : ICaseManagementManager
{
    public CaseManagementManager(
        IMapper mapper,
        IApplicationTrackerApiAccessor applicationTrackerApiAccessor,
        IAgentApiAccessor agentApiAccessor,
        IDocuSignApiAccessor docuSignApiAccessor)
    {
        Mapper = mapper;
        ApplicationTrackerApiAccessor = applicationTrackerApiAccessor;
        AgentApiAccessor = agentApiAccessor;
        DocuSignApiAccessor = docuSignApiAccessor;
    }

    private IMapper Mapper { get; }

    private IApplicationTrackerApiAccessor ApplicationTrackerApiAccessor { get; }

    private IAgentApiAccessor AgentApiAccessor { get; }

    private IDocuSignApiAccessor DocuSignApiAccessor { get; }

    public async Task<CaseManagementResponse?> GetCases(
        string agentId,
        CaseManagementParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var response = await ApplicationTrackerApiAccessor.GetCases(agentId, parameters, cancellationToken);

        if (response != null)
        {
            var data = Mapper.Map<CaseManagementResponse>(response);

            if (data?.Cases != null)
            {
                var cases = new List<CaseManagementCase>();

                data.Cases.ForEach(caseRecord =>
                {
                    if (caseRecord.Events?.Any() ?? false)
                    {
                        caseRecord.ProductName = caseRecord.Product switch
                        {
                            "ad" => "Accidental Death",
                            "add" => "Accidental Death & Dismemberment",
                            "ipao" or "ipas" => "Income Protection",
                            "termlife" or "termde" => "Term Life",
                            "ci" => "Critical Illness",
                            "ait" => "Accident Insurance",
                            "fiveyrt" => "5-Year Renewable Term",
                            "cpdi" => "Century+ Individual Disability Income Insurance",
                            _ => caseRecord.ProductName
                        };

                        caseRecord.Events = caseRecord.Events
                            .Select(MapEvent)
                            .Where(e =>
                            {
                                if (string.IsNullOrEmpty(e.EventName))
                                {
                                    return false;
                                }

                                if ((e.Event == EventType.RecipientSent || e.Event == EventType.RecipientCompleted) && e.RecipientId != null)
                                {
                                    return caseRecord.SignedInformation?.Signers?.Any(s => s.RecipientId == e.RecipientId) ?? false;
                                }

                                return true;
                            })
                            .GroupBy(e => e.Event)
                            .SelectMany(g =>
                            {
                                if (g.Key == EventType.RecipientSent || g.Key == EventType.RecipientCompleted)
                                {
                                    return g.OrderByDescending(e => e.CreatedDateTime).ToList();
                                }

                                return new List<CaseManagementEvent> { g.OrderByDescending(e => e.CreatedDateTime).First() };
                            })
                            .OrderByDescending(e => e.CreatedDateTime)
                            .ToList();

                        var lastEvent = caseRecord.Events?.FirstOrDefault();

                        if (lastEvent != null)
                        {
                            caseRecord.CurrentEvent = new CaseManagementEvent
                            {
                                CreatedDateTime = lastEvent.CreatedDateTime,
                                Event = lastEvent.Event,
                                EnvelopeId = lastEvent.EnvelopeId,
                                EventName = lastEvent.EventName,
                                RecipientId = lastEvent.RecipientId
                            };

                            cases.Add(caseRecord);
                        }
                    }
                });

                data.Page = parameters.PageNumber ?? 1;
                data.PageSize = parameters.PageSize ?? 10;
                data.NumberOfPages = response.NumberOfPages;
                data.TotalRecords = response.TotalRecords;
                data.Cases = cases;
            }

            return data;
        }

        return null;
    }

    public async Task<CaseManagementFilters?> GetFilterOptions(
        string agentId,
        CancellationToken cancellationToken = default)
    {
        var filterOptions = await ApplicationTrackerApiAccessor.GetFilterOptions(agentId, cancellationToken);
        if (filterOptions == null)
        {
            return null;
        }

        var agentIds = await AgentApiAccessor.GetAdditionalAgentIds(agentId, cancellationToken);
        if (agentIds == null)
        {
            return null;
        }

        return new CaseManagementFilters
        {
            PrimaryInsuredNames = filterOptions?.PrimaryInsuredName,
            ViewAsAgentIds = agentIds
        };
    }

    public async Task<bool> ResendEmail(
        string envelopeId,
        CancellationToken cancellationToken = default)
    {
        return await DocuSignApiAccessor.ResendEmail(envelopeId, cancellationToken);
    }

    private CaseManagementEvent MapEvent(CaseManagementEvent e)
    {
        switch (e.Event)
        {
            case EventType.InterviewStarted:
                e.EventName = "Interview Started";
                break;
            case EventType.InterviewCompleted:
                e.EventName = "Interview Completed";
                break;
            case EventType.RecipientSent:
                e.EventName = "Signature Requested";
                break;
            case EventType.RecipientDeclined:
                e.EventName = "Signature Declined";
                break;
            case EventType.EnvelopeVoided:
                e.EventName = "Signature Expired";
                break;
            case EventType.RecipientCompleted:
                e.EventName = "Signature Completed";
                break;
            case EventType.ApplicationSubmitted:
                e.EventName = "Application Submitted";
                break;
            case EventType.Expired:
                e.EventName = "Expired";
                break;
            case EventType.CaseStarted:
                e.EventName = "Case Started";
                break;
            case EventType.ReceivedQuote:
                e.EventName = "Quote Sent";
                break;
            default:
                e.EventName = string.Empty;
                break;
        }

        return e;
    }
}
