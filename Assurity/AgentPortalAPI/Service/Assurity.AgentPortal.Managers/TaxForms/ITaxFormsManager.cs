namespace Assurity.AgentPortal.Managers.TaxForms;

using Assurity.AgentPortal.Contracts.Shared;
using Assurity.AgentPortal.Contracts.TaxForms;

public interface ITaxFormsManager
{
    Task<List<TaxForm>?> GetTaxForms(string agentId, CancellationToken cancellationToken);

    Task<FileResponse?> GetTaxForm(string agentId, string formId, CancellationToken cancellationToken);
}