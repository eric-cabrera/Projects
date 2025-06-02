namespace Assurity.AgentPortal.Accessors.Tests.TestData;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Accessors.DTOs;

[ExcludeFromCodeCoverage]
public static class DocumentAccessorTestData
{
    public static ActionRequest ActionRequest => new()
    {
        AgentId = "AAXB",
        Files = new List<File>
            {
                new File
                {
                    IsMessage = true,
                    TiffImageBase64Content = "Base64ContentCreatedFromText"
                },
                new File
                {
                    Name = "Brochure.pdf",
                    TiffImageBase64Content = "Base64ContentConvertedFromPDF"
                }
            },
        ObjectIdForNewBusinessTransaction = "22192DEV100008B",
        PolicyNumber = "4180075134"
    };
}