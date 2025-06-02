namespace Assurity.AgentPortal.Accessors.Send;

using Assurity.AgentPortal.Accessors.DTOs;
using Assurity.Document.Client;
using Assurity.Document.Contracts;
using Assurity.Document.Contracts.Enums;

public class DocumentAccessor : IDocumentAccessor
{
    public DocumentAccessor(IDocumentClient documentClient)
    {
        DocumentClient = documentClient;
    }

    private IDocumentClient DocumentClient { get; }

    public void SendDocuments(ActionRequest actionRequest)
    {
        if (actionRequest == null)
        {
            return;
        }

        if (actionRequest.Files == null || actionRequest.Files.Count == 0)
        {
            return;
        }

        List<Document> documents = new();

        foreach (var file in actionRequest.Files)
        {
            documents.Add(new Document
            {
                DocumentOrigin = DocumentOrigin.Electronic,
                DocumentType = file.IsMessage ? "WEB MESSAGE" : "WEB DOCUMENT",
                PolicyNumber = actionRequest.PolicyNumber,
                TiffImageBase64Content = file.TiffImageBase64Content,
                TransactionObjectId = actionRequest.ObjectIdForNewBusinessTransaction,
                WorkItemClassName = WorkItemClassName.DSCAN01,
                WorkstepQueueName = "IW DocImport"
            });
        }

        var documentsJson = new DocumentsJson
        {
            Documents = documents
        };

        var createDocumentsResponse = DocumentClient.CreateDocumentsWithTiffImage(documentsJson);

        if (!(createDocumentsResponse?.Successful ?? false))
        {
            HandleDocumentClientFailure(createDocumentsResponse);
        }
    }

    private void HandleDocumentClientFailure(CreateDocumentsResponse? createDocumentsResponse)
    {
        var errors = createDocumentsResponse?.ModelValidationErrors;
        var errorMessage = string.Empty;

        if (errors != null)
        {
            errorMessage = $" {nameof(createDocumentsResponse.ModelValidationErrors)}: ";

            foreach (var error in errors)
            {
                errorMessage += $"{error.Key}: {string.Join(" ", error.Value)} | ";
            }

            if (errorMessage.EndsWith(" | "))
            {
                errorMessage = errorMessage[..^3];
            }
        }

        throw new Exception($"{nameof(DocumentClient)} failed during " +
            $"{nameof(DocumentClient.CreateDocumentsWithTiffImage)} call.{errorMessage}");
    }
}