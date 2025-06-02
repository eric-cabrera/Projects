namespace Assurity.AgentPortal.Managers.TaxForms.Mapping;

using Assurity.AgentPortal.Contracts.TaxForms;
using AutoMapper;
using TaxFormsAPI = Assurity.TaxForms.Contracts.V1;

public class TaxFormsResponseMappingProfile : Profile
{
    public TaxFormsResponseMappingProfile()
    {
        CreateMap<TaxFormsAPI.AgentForm, TaxForm>();
    }
}