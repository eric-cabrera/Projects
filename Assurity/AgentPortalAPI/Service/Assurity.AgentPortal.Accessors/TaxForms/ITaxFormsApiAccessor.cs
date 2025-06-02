namespace Assurity.AgentPortal.Accessors.TaxForms;

using Assurity.TaxForms.Contracts.V1;

public interface ITaxFormsApiAccessor
{
    Task<GetAgentFormsResponse?> GetTaxForms(string agentId, CancellationToken cancellationToken);

    Task<Stream?> GetTaxForm(string agentId, string formId, CancellationToken cancellationToken);
}