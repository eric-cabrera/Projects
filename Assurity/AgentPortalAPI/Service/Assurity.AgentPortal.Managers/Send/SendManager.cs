namespace Assurity.AgentPortal.Managers.Send;

using System.IO;
using Assurity.AgentPortal.Accessors.Send;
using Assurity.AgentPortal.Engines;
using Newtonsoft.Json.Linq;
using ActionRequest = Assurity.AgentPortal.Contracts.Send.ActionRequest;
using DTOs = Assurity.AgentPortal.Accessors.DTOs;

public class SendManager : ISendManager
{
    public SendManager(
        IDocumentAccessor documentAccessor,
        IGlobalDataAccessor globalDataAccessor,
        ISendEngine sendEngine)
    {
        DocumentAccessor = documentAccessor;
        GlobalDataAccessor = globalDataAccessor;
        SendEngine = sendEngine;
    }

    private IDocumentAccessor DocumentAccessor { get; }

    private IGlobalDataAccessor GlobalDataAccessor { get; }

    private ISendEngine SendEngine { get; }

    public async Task SendMessageAndFilesToGlobal(ActionRequest actionRequest)
    {
        var actionRequestDto = new DTOs.ActionRequest
        {
            AgentId = actionRequest.AgentId,
            Files = new List<DTOs.File>(),
            PolicyNumber = actionRequest.PolicyNumber
        };

        DTOs.File messageFile = null;

        if (!string.IsNullOrWhiteSpace(actionRequest.Message))
        {
            if (IsJson(actionRequest.Message))
            {
                messageFile = await SendEngine.CreateJsonMessageFile(actionRequest.Message);
            }
            else
            {
                messageFile = await SendEngine.CreateMessageFile(actionRequest.Message);
            }

            if (messageFile != null)
            {
                actionRequestDto.Files.Add(messageFile);
            }
        }

        if ((actionRequest.Files?.Count ?? 0) > 0)
        {
            var imageFiles = await SendEngine.CreateImageFiles(actionRequest.Files);

            if (imageFiles?.Any() ?? false)
            {
                actionRequestDto.Files.AddRange(imageFiles);
            }
        }

        if (!actionRequestDto.Files.Any())
        {
            throw new Exception("Expected to create a least one tiff File DTO but created none.");
        }

        var objectIdForNewBusinessTransaction = await GlobalDataAccessor
            .GetObjectIdForNewBusinessTransaction(actionRequest.PolicyNumber);

        if (string.IsNullOrEmpty(objectIdForNewBusinessTransaction))
        {
            throw new Exception("Unable to find ObjectId for NewBusiness Transaction " +
                "(a.k.a. GlobalData.dbo.Attributes.ObjectId) associated with " +
                $"{nameof(actionRequest.PolicyNumber)}: {actionRequest.PolicyNumber}.");
        }

        actionRequestDto.ObjectIdForNewBusinessTransaction = objectIdForNewBusinessTransaction;

        DocumentAccessor.SendDocuments(actionRequestDto);
    }

    private bool IsJson(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        input = input.Trim();

        try
        {
            var json = JObject.Parse(input);
            return json != null;
        }
        catch (Exception)
        {
            return false;
        }
    }
}