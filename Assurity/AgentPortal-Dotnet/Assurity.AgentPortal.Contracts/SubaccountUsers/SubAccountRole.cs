namespace Assurity.AgentPortal.Contracts.SubaccountUsers;

using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter<SubAccountRole>))]
public enum SubAccountRole
{
    CaseManagement,
    Claims,
    CommissionsAgentDebt, // Commissions/Debt
    Contracting,
    WorksiteParticipation, // GroupInventory
    Hierarchy,
    ListBill,
    PendingActiveTerminated,
    Production,
    TaxForms,
}
