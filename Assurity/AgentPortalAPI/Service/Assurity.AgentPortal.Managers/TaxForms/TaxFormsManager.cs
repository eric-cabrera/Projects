namespace Assurity.AgentPortal.Managers.TaxForms;

using Assurity.AgentPortal.Accessors.TaxForms;
using Assurity.AgentPortal.Contracts.Shared;
using Assurity.AgentPortal.Contracts.TaxForms;
using AutoMapper;

public class TaxFormsManager : ITaxFormsManager
{
    public TaxFormsManager(
        ITaxFormsApiAccessor taxFormsApiAccessor,
        IMapper mapper)
    {
        TaxFormsApiAccessor = taxFormsApiAccessor;
        Mapper = mapper;
    }

    private ITaxFormsApiAccessor TaxFormsApiAccessor { get; }

    private IMapper Mapper { get; }

    public async Task<List<TaxForm>?> GetTaxForms(
         string agentId,
         CancellationToken cancellationToken)
    {
        var taxFormsResponse = await TaxFormsApiAccessor
            .GetTaxForms(agentId, cancellationToken);

        if (taxFormsResponse == null)
        {
            return null;
        }

        return Mapper.Map<List<TaxForm>>(taxFormsResponse.AgentForms)
                     .OrderByDescending(form => form.DisplayName, new TaxFormsComparer())
                     .ToList();
    }

    public async Task<FileResponse?> GetTaxForm(
      string agentId,
      string formId,
      CancellationToken cancellationToken)
    {
        var responseStream = await TaxFormsApiAccessor.GetTaxForm(
            agentId,
            formId,
            cancellationToken);

        if (responseStream == null)
        {
            return null;
        }

        var fileName = $"{formId}-{DateTime.Now:MMddyyyy}";
        using var memoryStream = new MemoryStream();
        await responseStream.CopyToAsync(memoryStream, cancellationToken);
        var fileData = memoryStream.ToArray();

        var fileResponse = new FileResponse(fileName, "application/pdf")
        {
            FileData = fileData
        };

        return fileResponse;
    }
}