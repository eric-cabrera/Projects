namespace Assurity.AgentPortal.Accessors;

using System.Collections.Generic;
using Assurity.AgentPortal.Accessors.DirectusDTOs;
using Assurity.AgentPortal.Accessors.DirectusQueries;
using Assurity.AgentPortal.Accessors.DirectusQueries.Contracting;
using Assurity.AgentPortal.Contracts.Directus;

public interface IDirectusAccessor
{
    Task<AgentCenterCommissionDatesResponse?> GetAgentCenterCommissionDates();

    Task<ContractsQueryResponse?> GetContracts(ContractsQuery query);

    Task<string?> GetDirectusPage(PageQuery query);

    Task<byte[]?> GetDirectusFile(string fileId);

    Task<TemporaryMessagesQueryResponse?> GetTemporaryMessages();

    Task<List<DirectusContact>?> GetContacts();
}