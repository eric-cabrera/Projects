namespace Assurity.AgentPortal.Managers.PolicyInfo
{
    using System.Net.Mime;
    using Assurity.AgentPortal.Accessors.PolicyInfo;
    using Assurity.AgentPortal.Accessors.Send;
    using Assurity.AgentPortal.Contracts.Shared;
    using Microsoft.Extensions.Logging;

    public class Execute360DocumentManager : IExecute360DocumentManager
    {
        public Execute360DocumentManager(
            IGlobalDataAccessor globalDataAccessor,
            IDocumentServiceAccessor documentServiceAccessor,
            ILogger<Execute360DocumentManager> logger)
        {
            GlobalDataAccessor = globalDataAccessor;
            DocumentServiceAccessor = documentServiceAccessor;
            Logger = logger;
        }

        private IGlobalDataAccessor GlobalDataAccessor { get; }

        private IDocumentServiceAccessor DocumentServiceAccessor { get; }

        private ILogger<Execute360DocumentManager> Logger { get; }

        public async Task<FileResponse?> GetApplication(string policyNumber)
        {
            var attributeObjects = await GlobalDataAccessor.GetApplicationData(policyNumber);

            if (attributeObjects == null || attributeObjects.Count != 1)
            {
                Logger.LogError("Application for Policy Number: {PolicyNumber} is not available to download.", policyNumber);
                return null;
            }

            return new FileResponse($"application-{policyNumber}.pdf", MediaTypeNames.Application.Pdf)
            {
                FileData = await DocumentServiceAccessor.GetImageByIdAsync(policyNumber, attributeObjects[0].ObjectClass)
            };
        }
    }
}
