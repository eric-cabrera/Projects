namespace Assurity.AgentPortal.Managers.CommissionsAndDebt.Mapping;

using Assurity.AgentPortal.Accessors.DTOs;
using Assurity.AgentPortal.Contracts.CommissionsDebt.Request;
using AutoMapper;

public class DebtParametersMappingProfile : Profile
{
    public DebtParametersMappingProfile()
    {
        CreateMap<UnsecuredAdvanceParameters, Accessors.DTOs.DebtParameters>();
        CreateMap<SecuredAdvanceParameters, Accessors.DTOs.DebtParameters>();
    }
}
