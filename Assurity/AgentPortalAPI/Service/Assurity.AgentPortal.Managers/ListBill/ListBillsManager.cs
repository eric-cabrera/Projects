namespace Assurity.AgentPortal.Managers.ListBill;

using System.Net.Mime;
using Assurity.AgentPortal.Accessors.ListBill;
using Assurity.AgentPortal.Contracts.ListBill;
using Assurity.AgentPortal.Contracts.Shared;
using Assurity.AgentPortal.Engines;
using AutoMapper;

public class ListBillsManager : IListBillsManager
{
    public ListBillsManager(
        IListBillsApiAccessor listBillsApiAccessor,
        IPdfUtilitiesEngine pdfUtilities,
        IMapper mapper)
    {
        ListBillsApiAccessor = listBillsApiAccessor;
        PdfUtilities = pdfUtilities;
        Mapper = mapper;
    }

    private IListBillsApiAccessor ListBillsApiAccessor { get; }

    private IPdfUtilitiesEngine PdfUtilities { get; }

    private IMapper Mapper { get; }

    public async Task<GroupsResponse?> GetGroups(
        string agentId,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        var groupsResponse = await ListBillsApiAccessor.GetGroups(
            agentId,
            page,
            pageSize,
            cancellationToken);

        if (groupsResponse == null)
        {
            return null;
        }

        return Mapper.Map<GroupsResponse>(groupsResponse);
    }

    public async Task<ListBillsResponse?> GetListBills(
        string groupId,
        CancellationToken cancellationToken)
    {
        var listBillsResponse = await ListBillsApiAccessor.GetListBills(groupId, cancellationToken);

        if (listBillsResponse == null)
        {
            return null;
        }

        return Mapper.Map<ListBillsResponse>(listBillsResponse);
    }

    public async Task<FileResponse?> GetListBillData(
        string listBillId,
        CancellationToken cancellationToken)
    {
        var responseStream = await ListBillsApiAccessor.GetListBillData(
                listBillId,
                cancellationToken);

        if (responseStream == null || responseStream == Stream.Null)
        {
            return null;
        }

        var fileName = $"{listBillId}-{DateTime.Now:MMddyyyy}";

        using var memoryStream = new MemoryStream();
        await responseStream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0; // Reset the position to the beginning

        var portraitStream = PdfUtilities.EnsurePortraitOrientation(memoryStream);

        var fileData = portraitStream.ToArray();

        var fileResponse = new FileResponse(fileName, MediaTypeNames.Application.Pdf)
        {
            FileData = fileData
        };

        return fileResponse;
    }
}
